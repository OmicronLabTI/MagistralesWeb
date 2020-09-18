// <summary>
// <copyright file="SapAdapterService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Warehouses.Services.Clients
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Omicron.Warehouses.Entities.Model;
    using Omicron.Warehouses.Services.Constants;

    /// <summary>
    /// Class sap dapter Service.
    /// </summary>
    public class SapAdapterService : BaseClientService, ISapAdapterService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SapAdapterService" /> class.
        /// </summary>
        /// <param name="httpClient">Client Http.</param>
        public SapAdapterService(HttpClient httpClient)
            : base(httpClient)
        {
        }

        /// <summary>
        /// Method for get production orders by criterial.
        /// </summary>
        /// <param name="salesOrderIds">Sales orders.</param>
        /// <param name="productionOrderIds">ProductionOrders.</param>
        /// <returns>Production orders.</returns>
        public async Task<List<ProductionOrderModel>> GetProductionOrdersByCriterial(List<int> salesOrderIds, List<int> productionOrderIds)
        {
            var route = $"{EndPointConstants.SapAdapterGetProductionOrders}?salesOrders={string.Join(",", salesOrderIds)}&productionOrders={string.Join(",", productionOrderIds)}";
            var resultModel = await this.GetAsync(route);
            return JsonConvert.DeserializeObject<List<ProductionOrderModel>>(resultModel.Response.ToString());
        }
    }
}
