// <summary>
// <copyright file="ComponentsService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Services.Sap
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Omicron.SapAdapter.DataAccess.DAO.Sap;
    using Omicron.SapAdapter.Entities.Model;
    using Omicron.SapAdapter.Entities.Model.BusinessModels;
    using Omicron.SapAdapter.Entities.Model.JoinsModels;
    using Omicron.SapAdapter.Services.Constants;
    using Omicron.SapAdapter.Services.Redis;
    using Omicron.SapAdapter.Services.Utils;

    /// <summary>
    /// class for components.
    /// </summary>
    public class ComponentsService : IComponentsService
    {
        private readonly ISapDao sapDao;

        private readonly IRedisService redisService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentsService"/> class.
        /// </summary>
        /// <param name="sapDao">sap dao.</param>
        /// <param name="redisService">The reddis service.</param>
        public ComponentsService(ISapDao sapDao, IRedisService redisService)
        {
            this.sapDao = sapDao.ThrowIfNull(nameof(sapDao));
            this.redisService = redisService.ThrowIfNull(nameof(redisService));
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetMostCommonComponents(Dictionary<string, string> parameters)
        {
            var listToReturn = new List<CompleteDetalleFormulaModel>();
            var redisValue = await this.redisService.GetRedisKey(ServiceConstants.RedisComponents);
            var redisComponents = !string.IsNullOrEmpty(redisValue) ? JsonConvert.DeserializeObject<List<ComponentsRedisModel>>(redisValue) : new List<ComponentsRedisModel>();

            if (!redisComponents.Any())
            {
                return ServiceUtils.CreateResult(true, 200, null, new List<CompleteDetalleFormulaModel>(), null, null);
            }

            redisComponents = redisComponents.OrderByDescending(x => x.Total).ToList();
            var ids = redisComponents.Skip(0).Take(10).Select(x => x.ItemCode.ToLower()).ToList();

            var warehouse = parameters.ContainsKey(ServiceConstants.CatalogGroup) ? parameters[ServiceConstants.CatalogGroup] : ServiceConstants.MagistralWareHouse;
            var listComponents = await this.sapDao.GetItemsByContainsItemCode(ids, warehouse);

            ids.ForEach(x =>
            {
                var component = listComponents.FirstOrDefault(y => y.ProductId.ToLower() == x);
                component ??= new CompleteDetalleFormulaModel { ProductId = string.Empty };
                listToReturn.Add(component);
            });

            listToReturn = listToReturn.Where(x => !string.IsNullOrEmpty(x.ProductId)).ToList();
            return ServiceUtils.CreateResult(true, 200, null, listToReturn, null, null);
        }
    }
}
