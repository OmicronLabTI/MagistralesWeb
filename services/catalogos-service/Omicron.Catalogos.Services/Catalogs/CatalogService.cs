// <summary>
// <copyright file="CatalogService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Catalogos.Services.Catalogs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Omicron.Catalogos.DataAccess.DAO.Catalog;
    using Omicron.Catalogos.Dtos.User;
    using Omicron.Catalogos.Entities.Model;
    using Omicron.Catalogos.Services.Utils;

    /// <summary>
    /// The class for the catalog service.
    /// </summary>
    public class CatalogService : ICatalogService
    {
        private readonly ICatalogDao catalogDao;
        private readonly IAzureService azureService;
        private readonly IConfiguration configuration;
        private readonly ISapAdapterService sapAdapter;
        private readonly ICatalogsDxpService catalogsdxp;

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogService"/> class.
        /// </summary>
        /// <param name="catalogDao">the catalog dao.</param>
        /// <param name="configuration"> the configuration service. </param>
        /// <param name="azureService"> the azure service. </param>
        /// <param name="sapAdapter"> the sap service. </param>
        /// <param name="catalogsdxp"> the catalogs dxp. </param>
        public CatalogService(ICatalogDao catalogDao, IConfiguration configuration, IAzureService azureService, ISapAdapterService sapAdapter, ICatalogsDxpService catalogsdxp)
        {
            this.catalogDao = catalogDao ?? throw new ArgumentNullException(nameof(catalogDao));
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.azureService = azureService ?? throw new ArgumentNullException(nameof(azureService));
            this.sapAdapter = sapAdapter ?? throw new ArgumentNullException(nameof(sapAdapter));
            this.catalogsdxp = catalogsdxp ?? throw new ArgumentNullException(nameof(catalogsdxp));
        }

        /// <summary>
        /// Gets all the roles.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<ResultModel> GetRoles()
        {
            var listRoles = await this.catalogDao.GetAllRoles();

            return ServiceUtils.CreateResult(true, (int)HttpStatusCode.OK, null, listRoles, null);
        }

        /// <summary>
        /// The values in the dictionary.
        /// </summary>
        /// <param name="parameters">the parameters.</param>
        /// <returns>the data.</returns>
        public async Task<ResultModel> GetParamsContains(Dictionary<string, string> parameters)
        {
            var dictKeys = parameters.Keys.ToList();
            var dataParams = (await this.catalogDao.GetParamsByField(dictKeys)).DistinctBy(x => x.Id).ToList();
            return ServiceUtils.CreateResult(true, (int)HttpStatusCode.OK, null, dataParams, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetActiveClassificationQfb()
        {
            var classifications = (await this.catalogDao.GetActiveClassificationQfb()).Select(x => new { x.Value, x.Description }).ToList();
            return ServiceUtils.CreateResult(true, (int)HttpStatusCode.OK, null, classifications, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> UploadWarehouseFromExcel()
        {
            var warehousesfile = await this.GetWarehousesFromExcel();

            warehousesfile.ForEach(x => x.Name = NormalizeAndToUpper(x.Name));

            warehousesfile = warehousesfile
                .GroupBy(w => new { w.Name })
                .Select(g => g.First())
                .ToList();

            var warehouses = await this.WarehouseAdjustment(warehousesfile);
            var manufacturers = await this.ManufacturersAdjustment(warehouses);
            var products = await this.ProductsAdjustment(manufacturers);
            var exceptions = await this.ProductsAdjustment(products);

            var correctwarehouses = this.CompareProductsExceptions(exceptions);

            await this.catalogDao.InsertWarehouses(correctwarehouses);

            var nomatching = warehousesfile.Except(correctwarehouses).ToList();
            var comments = nomatching.Count > 0 ? string.Format(ServiceConstants.NoMatching, JsonConvert.SerializeObject(nomatching)) : null;

            return ServiceUtils.CreateResult(true, 200, null, null, comments);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetActivesWarehouses(List<ActiveWarehouseDto> products)
        {
            var warehouseConfigs = await this.catalogDao.GetActiveWarehouses();

            var result = new List<WarehouseDto>();
            products.ForEach(product =>
            {
                var validWareHouses = warehouseConfigs.Where(x => x.AppliesToProducts.ToUpper().Split(",").Contains(product.ItemCode.ToUpper()) || x.AppliesToManufacturers.ToUpper().Split(",").Contains(product.FirmName.ToUpper())).ToList();
                result.Add(new WarehouseDto { WarehouseCodes = validWareHouses.Select(x => x.Name).ToList(), ItemCode = product.ItemCode });
            });

            return ServiceUtils.CreateResult(true, (int)HttpStatusCode.OK, null, result, null);
        }

        private static string NormalizeAndToUpper(string input)
        {
            return new string(input.Normalize(NormalizationForm.FormD)
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .ToArray())
                .ToUpper();
        }

        private List<WarehouseModel> CompareProductsExceptions(List<WarehouseModel> exceptions)
        {
            var matchingWarehouses = exceptions
                .Where(warehouse =>
                {
                    var products = NormalizeAndToUpper(warehouse.AppliesToProducts).Split(',').Select(item => item.Trim()).ToList();
                    var exceptionProducts = NormalizeAndToUpper(warehouse.Exceptions).Split(',').Select(item => item.Trim()).ToList();

                    return products.Exists(product => exceptionProducts.Contains(product));
                })
                .ToList();

            var result = exceptions.Except(matchingWarehouses).ToList();

            return result;
        }

        private async Task<List<WarehouseModel>> ProductsAdjustment(List<WarehouseModel> warehouses)
        {
            var valids = new List<WarehouseModel>();

            warehouses.ForEach(x => x.AppliesToProducts = NormalizeAndToUpper(x.AppliesToProducts));

            var products = warehouses
                .Select(x => x.AppliesToProducts.Split(',').Select(s => s.Trim()).ToList())
                .ToList();

            var names = products.SelectMany(x => x)
                .Where(name => !string.IsNullOrEmpty(name)).Distinct().ToList();

            if (names.Any())
            {
                var response = await this.catalogsdxp.Post(names, ServiceConstants.Products);
                var data = JsonConvert.DeserializeObject<List<string>>(response.Response.ToString());

                var dataNames = new HashSet<string>(data.Select(NormalizeAndToUpper));

                valids = warehouses
                    .Where(warehouse => warehouse.AppliesToProducts.Split(',').Select(m => m.Trim())
                    .All(m => dataNames.Contains(NormalizeAndToUpper(m))))
                    .ToList();
            }

            return valids;
        }

        private async Task<List<WarehouseModel>> WarehouseAdjustment(List<WarehouseModel> warehousesfile)
        {
            var names = warehousesfile.Select(x => x.Name).ToList();

            var response = await this.sapAdapter.Post(names, ServiceConstants.Warehouses);
            var data = JsonConvert.DeserializeObject<List<WarehousesDto>>(response.Response.ToString());

            var dataNames = new HashSet<string>(data.Select(y => NormalizeAndToUpper(y.WarehouseCode)));
            var warehouses = warehousesfile.Where(x => dataNames.Contains(x.Name)).ToList();

            var updates = await this.catalogDao.GetWarehouses(warehouses.Select(x => x.Name).ToList());
            var dict = updates.ToDictionary(u => u.Name, u => u.Id);

            warehouses.ForEach(m => m.Id = dict.GetValueOrDefault(m.Name));

            return warehouses;
        }

        private async Task<List<WarehouseModel>> ManufacturersAdjustment(List<WarehouseModel> warehouses)
        {
            var valids = new List<WarehouseModel>();

            warehouses.ForEach(x => x.AppliesToManufacturers = NormalizeAndToUpper(x.AppliesToManufacturers));

            var manufacturers = warehouses
                .Select(x => x.AppliesToManufacturers.Split(',').Select(s => s.Trim()).ToList())
                .ToList();

            var names = manufacturers.SelectMany(x => x)
                .Where(name => !string.IsNullOrEmpty(name)).Distinct().ToList();

            if (names.Any())
            {
                var response = await this.catalogsdxp.Post(names, ServiceConstants.Manufacturers);
                var data = JsonConvert.DeserializeObject<List<string>>(response.Response.ToString());

                var dataNames = new HashSet<string>(data.Select(NormalizeAndToUpper));

                valids = warehouses
                    .Where(warehouse => warehouse.AppliesToManufacturers.Split(',')
                    .Select(m => m.Trim()).All(m => dataNames.Contains(NormalizeAndToUpper(m)))).ToList();
            }

            return valids;
        }

        private async Task<List<WarehouseModel>> GetWarehousesFromExcel()
        {
            var table = await this.ObtainDataFromExcel(ServiceConstants.WarehousesFileUrl);

            var columns = table.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToList();

            var name = columns[0];
            var manufacturers = columns[1];
            var products = columns[2];
            var isactive = columns[3];
            var excepetions = columns[4];

            var warehouses = table.AsEnumerable()
            .Select(row => new WarehouseModel
            {
                Name = row[name].ToString().Trim(),
                IsActive = row[isactive].ToString().Trim().Equals(ServiceConstants.IsActive, StringComparison.OrdinalIgnoreCase),
                AppliesToProducts = row[products].ToString(),
                AppliesToManufacturers = row[manufacturers].ToString(),
                Exceptions = row[excepetions].ToString(),
            }).ToList();

            return warehouses;
        }

        private async Task<DataTable> ObtainDataFromExcel(string url)
        {
            var key = this.configuration[ServiceConstants.AzureAccountKey];
            var account = this.configuration[ServiceConstants.AzureAccountName];
            var file = this.configuration[url];

            using var streamWoorkbook = new MemoryStream();

            await this.azureService.GetElementsFromAzure(account, key, file, streamWoorkbook);
            using var workbook = new XLWorkbook(streamWoorkbook);

            DataTable table = ServiceUtils.ReadSheet(workbook, 1);

            return table;
        }
    }
}
