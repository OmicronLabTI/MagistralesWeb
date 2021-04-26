// <summary>
// <copyright file="PedidosUtils.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Services.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Omicron.Pedidos.Entities.Model;
    using Omicron.Pedidos.Services.Constants;
    using Omicron.Pedidos.Services.Redis;

    /// <summary>
    /// Class for pedidos utils.
    /// </summary>
    public class PedidosUtils
    {
        private readonly IRedisService redisService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PedidosUtils"/> class.
        /// </summary>
        /// <param name="redisService">The redis Service.</param>
        public PedidosUtils(IRedisService redisService)
        {
            this.redisService = redisService ?? throw new ArgumentNullException(nameof(redisService));
        }

        /// <summary>
        /// set the components and how many times.
        /// </summary>
        /// <param name="components">the components.</param>
        /// <returns>the data.</returns>
        public async Task UpdateMostUsedComponents(List<string> components)
        {
            if (!components.Any())
            {
                return;
            }

            var redisValue = await this.redisService.GetRedisKey(ServiceConstants.RedisComponents);
            var redisComponents = !string.IsNullOrEmpty(redisValue) ? JsonConvert.DeserializeObject<List<ComponentsRedisModel>>(redisValue) : new List<ComponentsRedisModel>();
            redisComponents ??= new List<ComponentsRedisModel>();

            var listToUpdate = new List<ComponentsRedisModel>();
            foreach (var c in components)
            {
                var component = redisComponents.FirstOrDefault(y => y.ItemCode == c);

                if (component == null)
                {
                    listToUpdate.Add(new ComponentsRedisModel { ItemCode = c, Total = 1 });
                    continue;
                }

                listToUpdate.Add(new ComponentsRedisModel { ItemCode = c, Total = component.Total + 1 });
            }

            var listItemsToInsert = listToUpdate.Select(x => x.ItemCode).ToList();
            var missing = redisComponents.Where(x => !listItemsToInsert.Contains(x.ItemCode));
            listToUpdate.AddRange(missing);
            await this.redisService.WriteToRedis(ServiceConstants.RedisComponents, JsonConvert.SerializeObject(listToUpdate));
        }
    }
}
