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
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using AutoMapper;
    using Microsoft.IdentityModel.Tokens;
    using Omicron.Catalogos.DataAccess.DAO.Catalog;
    using Omicron.Catalogos.Dtos.User;
    using Omicron.Catalogos.Entities.Model;
    using Omicron.Catalogos.Services.Redis;
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
        private readonly IRedisService redisService;

        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogService"/> class.
        /// </summary>
        /// <param name="catalogDao">the catalog dao.</param>
        /// <param name="configuration"> the configuration service. </param>
        /// <param name="azureService"> the azure service. </param>
        /// <param name="sapAdapter"> the sap service. </param>
        /// <param name="catalogsdxp"> the catalogs dxp. </param>
        /// <param name="redisService"> Redis Service. </param>
        /// <param name="mapper"> Mapper Service. </param>
        public CatalogService(ICatalogDao catalogDao, IConfiguration configuration, IAzureService azureService, ISapAdapterService sapAdapter, ICatalogsDxpService catalogsdxp, IRedisService redisService, IMapper mapper)
        {
            this.catalogDao = catalogDao ?? throw new ArgumentNullException(nameof(catalogDao));
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.azureService = azureService ?? throw new ArgumentNullException(nameof(azureService));
            this.sapAdapter = sapAdapter ?? throw new ArgumentNullException(nameof(sapAdapter));
            this.catalogsdxp = catalogsdxp ?? throw new ArgumentNullException(nameof(catalogsdxp));
            this.redisService = redisService ?? throw new ArgumentNullException(nameof(redisService));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
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
            var textInfo = CultureInfo.CurrentCulture.TextInfo;
            var classifications = (await this.catalogDao.GetActiveClassificationColorsByRoutes([ServiceConstants.Magistrales]))
                .Select(x => new ClassificationMagistralModel
                {
                    Value = x.ClassificationCode,
                    Description = $"{textInfo.ToTitleCase(x.Classification.ToLower())} ({x.ClassificationCode})",
                    Color = ServiceUtils.CalculateTernary(!string.IsNullOrEmpty(x.Color), x.Color, ServiceConstants.DefaultColor),
                }).OrderBy(x => x.Description).ToList();

            return ServiceUtils.CreateResult(true, (int)HttpStatusCode.OK, null, classifications, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetActiveAllClassificationQfb()
        {
            var textInfo = CultureInfo.CurrentCulture.TextInfo;
            var classifications = (await this.catalogDao.GetActiveClassificationColorsByRoutes(ServiceConstants.Routes))
                .Select(x => new ClassificationMagistralModel
                {
                    Value = x.ClassificationCode,
                    Description = $"{textInfo.ToTitleCase(x.Classification.ToLower())} ({x.ClassificationCode})",
                    Color = ServiceUtils.CalculateTernary(!string.IsNullOrEmpty(x.Color), x.Color, ServiceConstants.DefaultColor),
                }).OrderBy(x => x.Description).ToList();

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

            var nomatching = warehousesfile.Except(correctwarehouses).Select(x => x.Name).ToList();
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
                var validWareHouses = warehouseConfigs.Where(x => (GetValidStringList(x.AppliesToProducts).Contains(product.ItemCode.ToUpper()) || GetValidStringList(x.AppliesToManufacturers).Contains(product.FirmName.ToUpper())) && !GetValidStringList(x.Exceptions).Contains(product.ItemCode.ToUpper())).ToList();
                result.Add(new WarehouseDto { WarehouseCodes = validWareHouses.Select(x => x.Name).ToList(), ItemCode = product.ItemCode });
            });

            return ServiceUtils.CreateResult(true, (int)HttpStatusCode.OK, null, result, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetClassifications()
        {
            List<ClassificationDto> byfilter = await this.FilterClasifications();
            List<ColorsDto> colors = await this.ClassificationColors();

            ClassificationGroupDto result = new ()
            {
                Filters = byfilter,
                Colors = colors,
            };

            return ServiceUtils.CreateResult(true, (int)HttpStatusCode.OK, null, result, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> UploadConfigurationRouteFromExcel()
        {
            List<ConfigRoutesModel> valids = new ();
            List<ConfigRoutesModel> invalids = new ();

            var configroute = await this.GetConfigurationRoutesFromExcel();

            ValidConfigRoutes(configroute, valids, invalids);

            await this.ClassificationValidation(valids, invalids);
            await this.ItemCodeValidation(valids, invalids);
            await this.ExceptionValidation(valids, invalids);
            ColorValidation(valids, invalids);
            RouteValidation(valids, invalids);

            await this.InsertConfigRoutes(valids);

            var values = invalids.SelectMany(x => new[] { x.ItemCode, x.Classification, x.Exceptions, x.Route }).Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct().ToList();

            var comments = values.Count > 0 ? string.Format(ServiceConstants.InvalidsSortingRoutes, JsonConvert.SerializeObject(values)) : null;

            return ServiceUtils.CreateResult(true, 200, null, null, comments);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetActiveRouteConfigurationsForProducts()
        {
            var routeConfiguration = await ServiceUtils.DeserializeRedisValue(
                new List<ConfigRoutesModel>(),
                ServiceConstants.ConfigRoutesRedisKey,
                this.redisService);

            if (routeConfiguration.Count == 0)
            {
                routeConfiguration = await this.catalogDao.GetConfigRoutesModel();
                await this.SaveValidsToRedis(routeConfiguration);
            }

            return ServiceUtils.CreateResult(true, 200, null, routeConfiguration.Where(x => x.IsActive).ToList(), null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> UploadProductTypeColorsFromExcel()
        {
            var productTypeColors = await this.GetProductTypeColorsFromExcel();

            foreach (var item in productTypeColors)
            {
                item.TemaId = ServiceUtils.NormalizeComplete(item.TemaId);
            }

            productTypeColors = productTypeColors
                .GroupBy(p => p.TemaId)
                .Select(g => g.First())
                .ToList();

            var temaIds = productTypeColors.Select(x => x.TemaId).ToList();
            var existingTemaIds = await this.catalogDao.GetExistingTemaIds(temaIds);
            var recordsToUpdate = productTypeColors.Where(x => existingTemaIds.Contains(x.TemaId)).ToList();
            var recordsToInsert = productTypeColors.Where(x => !existingTemaIds.Contains(x.TemaId)).ToList();

            if (recordsToUpdate.Any())
            {
                await this.catalogDao.UpdateProductTypecolors(recordsToUpdate);
            }

            if (recordsToInsert.Any())
            {
                await this.catalogDao.InsertProductTypecolors(recordsToInsert);
            }

            await this.redisService.WriteToRedis(
            ServiceConstants.ProductTypeColors,
            JsonConvert.SerializeObject(productTypeColors),
            TimeSpan.FromHours(12));

            return ServiceUtils.CreateResult(true, (int)HttpStatusCode.OK, null, null, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetProductsColors(List<string> themesIds)
        {
            var colors = await ServiceUtils.DeserializeRedisValue(
                new List<ProductTypeColorsModel>(),
                ServiceConstants.ProductTypeColors,
                this.redisService);

            colors = colors.Where(x => x.IsActive).ToList();
            if (colors.Count == 0)
            {
                colors = (await this.catalogDao.GetProductsColors()).ToList();
            }

            var filterColors = colors.ToList().Where(x => themesIds.Any(theme => ServiceUtils.NormalizeComplete(theme) == ServiceUtils.NormalizeComplete(x.TemaId)));
            return ServiceUtils.CreateResult(true, (int)HttpStatusCode.OK, null, this.mapper.Map<List<ProductColorsDto>>(filterColors.ToList()), null);
        }

        private static List<string> GetValidStringList(string value)
        {
            return value.IsNullOrEmpty() ? new List<string>() : value.ToUpper().Split(",").ToList();
        }

        private static string NormalizeAndToUpper(string input)
        {
            return new string(input.Normalize(NormalizationForm.FormD)
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .ToArray())
                .ToUpper();
        }

        private static void ValidateClassificationsFound(HashSet<(string Classification, string Code)> found, List<ConfigRoutesModel> valids, List<ConfigRoutesModel> invalids)
        {
            var cleanedValids = new List<ConfigRoutesModel>();

            var withClassification = valids.Where(x => !string.IsNullOrWhiteSpace(x.Classification)).ToList();
            var withoutClassification = valids.Where(x => string.IsNullOrWhiteSpace(x.Classification)).ToList();

            var grouped = withClassification
                .GroupBy(w => NormalizeAndToUpper(w.Classification))
                .Select(g => g.First())
                .ToList();

            var duplicates = withClassification
                .Except(grouped)
                .ToList();

            invalids.AddRange(duplicates);

            foreach (var item in grouped)
            {
                var match = found.FirstOrDefault(x => x.Classification == item.Classification);

                if (match != default)
                {
                    item.ClassificationCode = match.Code;
                    cleanedValids.Add(item);
                }
                else
                {
                    invalids.Add(item);
                }
            }

            cleanedValids.AddRange(withoutClassification);

            valids.Clear();
            valids.AddRange(cleanedValids);
        }

        private static void ValidateItemCodesFound(HashSet<string> found, List<ConfigRoutesModel> valids, List<ConfigRoutesModel> invalids)
        {
            var seenCodes = new HashSet<string>();
            var cleanedValids = new List<ConfigRoutesModel>();

            var withItemCode = valids.Where(x => !string.IsNullOrWhiteSpace(x.ItemCode)).ToList();
            var withoutItemCode = valids.Where(x => string.IsNullOrWhiteSpace(x.ItemCode)).ToList();

            foreach (var item in withItemCode)
            {
                var itemCodes = item.ItemCode
                    .Split(',')
                    .Select(code => NormalizeAndToUpper(code.Trim()))
                    .ToList();

                if (itemCodes.Exists(code => seenCodes.Contains(code)))
                {
                    invalids.Add(item);
                    continue;
                }

                itemCodes.ForEach(code => seenCodes.Add(code));

                if (itemCodes.TrueForAll(code => found.Contains(code)))
                {
                    cleanedValids.Add(item);
                }
                else
                {
                    invalids.Add(item);
                }
            }

            cleanedValids.AddRange(withoutItemCode);

            valids.Clear();
            valids.AddRange(cleanedValids);
        }

        private static void ValidateExceptionFound(HashSet<string> found, List<ConfigRoutesModel> valids, List<ConfigRoutesModel> invalids)
        {
            var seenExceptions = new HashSet<string>();
            var cleanedValids = new List<ConfigRoutesModel>();

            var withException = valids.Where(x => !string.IsNullOrWhiteSpace(x.Exceptions)).ToList();
            var withoutException = valids.Where(x => string.IsNullOrWhiteSpace(x.Exceptions)).ToList();

            foreach (var item in withException)
            {
                var exceptions = item.Exceptions
                    .Split(',')
                    .Select(code => NormalizeAndToUpper(code.Trim()))
                    .ToList();

                if (exceptions.Exists(code => seenExceptions.Contains(code)))
                {
                    invalids.Add(item);
                    continue;
                }

                exceptions.ForEach(code => seenExceptions.Add(code));

                if (exceptions.TrueForAll(code => found.Contains(code)))
                {
                    cleanedValids.Add(item);
                }
                else
                {
                    invalids.Add(item);
                }
            }

            cleanedValids.AddRange(withoutException);

            valids.Clear();
            valids.AddRange(cleanedValids);
        }

        private static void ValidConfigRoutes(List<ConfigRoutesModel> confingroute, List<ConfigRoutesModel> valids, List<ConfigRoutesModel> invalids)
        {
            if (confingroute == null || confingroute.Count == 0)
            {
                return;
            }

            NormalizeSortingRoutes(confingroute);

            foreach (var route in confingroute)
            {
                if (IsValidRoute(route))
                {
                    valids.Add(route);
                }
                else
                {
                    invalids.Add(route);
                }
            }
        }

        private static void NormalizeSortingRoutes(List<ConfigRoutesModel> sortingRoutes)
        {
            sortingRoutes.ForEach(x =>
            {
                if (!string.IsNullOrEmpty(x.Classification))
                {
                    x.Classification = NormalizeAndToUpper(x.Classification);
                }

                if (!string.IsNullOrEmpty(x.ItemCode))
                {
                    x.ItemCode = NormalizeAndToUpper(x.ItemCode);
                }

                if (!string.IsNullOrEmpty(x.Route))
                {
                    x.Route = NormalizeAndToUpper(x.Route);
                }
            });
        }

        private static bool IsValidRoute(ConfigRoutesModel route)
        {
            return !string.IsNullOrWhiteSpace(route.Classification) ||
                   !string.IsNullOrWhiteSpace(route.ItemCode);
        }

        private static void ColorValidation(List<ConfigRoutesModel> valids, List<ConfigRoutesModel> invalids)
        {
            var hexColorRegex = new Regex(ServiceConstants.HexColor);

            var invalidColorItems = valids
                .Where(x => !string.IsNullOrWhiteSpace(x.Color))
                .Where(x => !hexColorRegex.IsMatch(x.Color))
                .ToList();

            invalids.AddRange(invalidColorItems);

            valids.RemoveAll(x => invalidColorItems.Contains(x));
        }

        private static void RouteValidation(List<ConfigRoutesModel> valids, List<ConfigRoutesModel> invalids)
        {
            var notallowed = valids.Where(x => !ServiceConstants.Routes.Contains(x.Route)).ToList();

            invalids.AddRange(notallowed);
            valids.RemoveAll(x => notallowed.Contains(x));
        }

        private async Task<List<ColorsDto>> ClassificationColors()
        {
            var rules = await ServiceUtils.DeserializeRedisValue(new List<ConfigRoutesModel>(), ServiceConstants.ConfigRoutesRedisKey, this.redisService);

            if (rules.Count == 0)
            {
                rules = await this.catalogDao.GetConfigurationRoute();
            }

            return rules.Select(x => new ColorsDto
            {
                Classification = x.Classification,
                ClassificationCode = x.ClassificationCode,
                ClassificationColor = string.IsNullOrWhiteSpace(x.Color)
                    ? ServiceConstants.DefaultColor
                    : x.Color,
            }).ToList();
        }

        private async Task<List<ClassificationDto>> FilterClasifications()
        {
            var response = await this.catalogsdxp.Get(ServiceConstants.Manufacturers);
            var data = JsonConvert.DeserializeObject<List<ManufacturersDto>>(response.Response.ToString());

            var classifications = data
                .GroupBy(x => new { x.Classification, x.ClassificationCode })
                .Select(g => new ClassificationDto
                {
                    Classification = g.Key.Classification,
                    ClassificationCode = g.Key.ClassificationCode,
                })
                .Where(x => !ServiceConstants.Exlusions.Contains(x.ClassificationCode))
                .ToList();

            return classifications;
        }

        private async Task InsertConfigRoutes(List<ConfigRoutesModel> valids)
        {
            var updates = await this.catalogDao.GetSortingRoutes(valids.Select(x => x.Classification).ToList());
            var dictionary = updates.ToDictionary(u => u.Classification, u => u.Id);

            valids.ForEach(m => m.Id = dictionary.GetValueOrDefault(m.Classification));

            await this.catalogDao.InsertSortingRoute(valids);

            await this.SaveValidsToRedis(valids);
        }

        private async Task SaveValidsToRedis(List<ConfigRoutesModel> valids)
        {
            var serialized = JsonConvert.SerializeObject(valids);
            await this.redisService.WriteToRedis(ServiceConstants.ConfigRoutesRedisKey, serialized, new TimeSpan(8, 0, 0));
        }

        private async Task ClassificationValidation(List<ConfigRoutesModel> valids, List<ConfigRoutesModel> invalids)
        {
            List<string> classifications = valids.Where(c => !string.IsNullOrWhiteSpace(c.Classification))
                .Select(c => c.Classification)
                .Distinct().ToList();

            ResultDto response = await this.sapAdapter.Post(classifications, ServiceConstants.GetClassificationsByDescription);
            List<ClassificationsDto> classificationsfound = JsonConvert.DeserializeObject<List<ClassificationsDto>>(response.Response.ToString());

            HashSet<(string Classification, string Code)> found = classificationsfound.Select(x => (NormalizeAndToUpper(x.Description), NormalizeAndToUpper(x.Value))).ToHashSet();

            ValidateClassificationsFound(found, valids, invalids);
        }

        private async Task ItemCodeValidation(List<ConfigRoutesModel> valids, List<ConfigRoutesModel> invalids)
        {
            var products = valids
                .Where(x => !string.IsNullOrEmpty(x.ItemCode))
                .Select(x => x.ItemCode.Split(',').Select(s => s.Trim()).ToList())
                .ToList();

            var names = products.SelectMany(x => x)
                .Where(name => !string.IsNullOrEmpty(name)).Distinct().ToList();
            if (names.Any())
            {
                var response = await this.catalogsdxp.Post(names, ServiceConstants.Products);
                var data = JsonConvert.DeserializeObject<List<string>>(response.Response.ToString());

                var found = new HashSet<string>(data.Select(NormalizeAndToUpper));

                ValidateItemCodesFound(found, valids, invalids);
            }
        }

        private async Task ExceptionValidation(List<ConfigRoutesModel> valids, List<ConfigRoutesModel> invalids)
        {
            var products = valids
                .Where(x => !string.IsNullOrEmpty(x.Exceptions))
                .Select(x => x.Exceptions.Split(',').Select(s => s.Trim()).ToList())
                .ToList();

            var names = products.SelectMany(x => x)
                .Where(name => !string.IsNullOrEmpty(name)).Distinct().ToList();
            if (names.Any())
            {
                var response = await this.catalogsdxp.Post(names, ServiceConstants.Products);
                var data = JsonConvert.DeserializeObject<List<string>>(response.Response.ToString());

                var found = new HashSet<string>(data.Select(NormalizeAndToUpper));

                ValidateExceptionFound(found, valids, invalids);
            }
        }

        private List<WarehouseModel> CompareProductsExceptions(List<WarehouseModel> exceptions)
        {
            var matchingWarehouses = exceptions
                .Where(warehouse =>
                {
                    var products = NormalizeAndToUpper(warehouse.AppliesToProducts)
                        .Split(',')
                        .Select(item => item.Trim())
                        .Where(item => !string.IsNullOrWhiteSpace(item))
                        .ToList();

                    var exceptionProducts = NormalizeAndToUpper(warehouse.Exceptions)
                        .Split(',')
                        .Select(item => item.Trim())
                        .Where(item => !string.IsNullOrWhiteSpace(item))
                        .ToList();

                    return products.Exists(product => exceptionProducts.Contains(product));
                })
                .ToList();

            var result = exceptions.Except(matchingWarehouses).ToList();

            return result;
        }

        private async Task<List<WarehouseModel>> ProductsAdjustment(List<WarehouseModel> warehouses)
        {
            warehouses.ForEach(x => x.AppliesToProducts = NormalizeAndToUpper(x.AppliesToProducts));

            var products = warehouses
                .Select(x => x.AppliesToProducts.Split(',').Select(s => s.Trim()).ToList())
                .ToList();

            var names = products.SelectMany(x => x)
                .Where(name => !string.IsNullOrEmpty(name)).Distinct().ToList();

            var valids = warehouses
                .Where(warehouse => string.IsNullOrEmpty(warehouse.AppliesToProducts))
                .ToList();

            if (names.Any())
            {
                var response = await this.catalogsdxp.Post(names, ServiceConstants.Products);
                var data = JsonConvert.DeserializeObject<List<string>>(response.Response.ToString());

                var dataNames = new HashSet<string>(data.Select(NormalizeAndToUpper));

                valids.AddRange(warehouses
                    .Where(warehouse => !string.IsNullOrEmpty(warehouse.AppliesToProducts) &&
                        warehouse.AppliesToProducts.Split(',')
                            .Select(m => m.Trim())
                            .All(m => dataNames.Contains(NormalizeAndToUpper(m)))));
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
            warehouses.ForEach(x => x.AppliesToManufacturers = NormalizeAndToUpper(x.AppliesToManufacturers));

            var manufacturers = warehouses
                .Select(x => x.AppliesToManufacturers.Split(',').Select(s => s.Trim()).ToList())
                .ToList();

            var names = manufacturers.SelectMany(x => x)
                .Where(name => !string.IsNullOrEmpty(name)).Distinct().ToList();

            var valids = warehouses
                .Where(warehouse => string.IsNullOrEmpty(warehouse.AppliesToManufacturers))
                .ToList();

            if (names.Any())
            {
                var response = await this.catalogsdxp.Post(names, ServiceConstants.Manufacturers);
                var data = JsonConvert.DeserializeObject<List<string>>(response.Response.ToString());

                var dataNames = new HashSet<string>(data.Select(NormalizeAndToUpper));

                valids.AddRange(warehouses
                    .Where(warehouse => !string.IsNullOrEmpty(warehouse.AppliesToManufacturers) &&
                        warehouse.AppliesToManufacturers.Split(',')
                            .Select(m => m.Trim())
                            .All(m => dataNames.Contains(NormalizeAndToUpper(m)))));
            }

            return valids;
        }

        private async Task<List<ProductTypeColorsModel>> GetProductTypeColorsFromExcel()
        {
            var table = await this.ObtainDataFromExcel(ServiceConstants.ProductTypeColorsFileUrl, 1);

            var columns = table.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToList();

            var temaId = columns[0];
            var backgroundColor = columns[1];
            var labelText = columns[2];
            var textColor = columns[3];
            var isActive = columns[4];

            var typeColors = table.AsEnumerable()
            .Select(row => new ProductTypeColorsModel
            {
                TemaId = row[temaId].ToString().Trim(),
                BackgroundColor = row[backgroundColor].ToString(),
                LabelText = row[labelText].ToString(),
                TextColor = row[textColor].ToString(),
                IsActive = row[isActive].ToString().Trim().Equals(ServiceConstants.IsActiveProduct, StringComparison.OrdinalIgnoreCase),
            }).ToList();

            return typeColors;
        }

        private async Task<List<WarehouseModel>> GetWarehousesFromExcel()
        {
            var table = await this.ObtainDataFromExcel(ServiceConstants.WarehousesFileUrl, 1);

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

        private async Task<List<ConfigRoutesModel>> GetConfigurationRoutesFromExcel()
        {
            var table = await this.ObtainDataFromExcel(ServiceConstants.ManufacturersFileUrl, 2);

            var columns = table.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToList();

            var classification = columns[0];
            var exception = columns[1];
            var itemcode = columns[2];
            var color = columns[3];
            var isactive = columns[4];
            var route = columns[5];

            var sortingroute = table.AsEnumerable()
            .Select(row => new ConfigRoutesModel
            {
                Classification = row[classification].ToString(),
                Exceptions = row[exception].ToString(),
                ItemCode = row[itemcode].ToString(),
                Color = row[color].ToString(),
                Route = row[route].ToString(),
                IsActive = row[isactive].ToString().Equals("1"),
            }).ToList();

            return sortingroute;
        }

        private async Task<DataTable> ObtainDataFromExcel(string url, int sheet)
        {
            var key = this.configuration[ServiceConstants.AzureAccountKey];
            var account = this.configuration[ServiceConstants.AzureAccountName];
            var file = this.configuration[url];

            using var streamWoorkbook = new MemoryStream();

            await this.azureService.GetElementsFromAzure(account, key, file, streamWoorkbook);
            using var workbook = new XLWorkbook(streamWoorkbook);

            DataTable table = ServiceUtils.ReadSheet(workbook, sheet);

            return table;
        }
    }
}
