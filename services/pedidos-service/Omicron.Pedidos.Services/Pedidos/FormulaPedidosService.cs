// <summary>
// <copyright file="FormulaPedidosService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Services.Pedidos
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore.Internal;
    using Omicron.Pedidos.DataAccess.DAO.Pedidos;
    using Omicron.Pedidos.Entities.Model;
    using Omicron.Pedidos.Entities.Model.Db;
    using Omicron.Pedidos.Resources.Extensions;
    using Omicron.Pedidos.Services.Constants;
    using Omicron.Pedidos.Services.Utils;

    /// <summary>
    /// Implementation for formulas.
    /// </summary>
    public class FormulaPedidosService : IFormulaPedidosService
    {
        private readonly IPedidosDao pedidosDao;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormulaPedidosService"/> class.
        /// </summary>
        /// <param name="pedidosDao">pedidos dao.</param>
        public FormulaPedidosService(IPedidosDao pedidosDao)
        {
            this.pedidosDao = pedidosDao ?? throw new ArgumentNullException(nameof(pedidosDao));
        }

        /// <summary>
        /// Create custom component list.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="customList">The custom list.</param>
        /// <returns>New custom list.</returns>
        public async Task<ResultModel> CreateCustomComponentList(string userId, CustomComponentListModel customList)
        {
            if (await this.Exists(customList))
            {
                var msg = string.Format(ServiceConstants.ReasonCustomListAlreadyExists, customList.Name, customList.ProductId);
                return ServiceUtils.CreateResult(true, 200, msg, 0, null);
            }

            customList.Id = 0;
            customList.Components.ForEach(x => x.Id = 0);

            customList.CreationDate = DateTime.Now.FormatedLargeDate();
            customList.CreationUserId = userId;

            if (await this.pedidosDao.InsertCustomComponentList(customList))
            {
                customList.Components.ForEach(x => x.CustomListId = customList.Id);
                if (await this.pedidosDao.InsertComponentsOfCustomList(customList.Components))
                {
                    return ServiceUtils.CreateResult(true, 200, null, customList.Id, null);
                }
            }

            return ServiceUtils.CreateResult(true, 200, ServiceConstants.ReasonUnexpectedError, 0, null);
        }

        /// <summary>
        /// Get custom components list by product id.
        /// </summary>
        /// <param name="productId">The product id.</param>
        /// <returns>Custom lists.</returns>
        public async Task<ResultModel> GetCustomComponentListByProductId(string productId)
        {
            var customLists = await this.pedidosDao.GetCustomComponentListByProduct(productId);
            var allDetails = await this.pedidosDao.GetComponentsByCustomListId(customLists.Select(x => x.Id).ToList());

            foreach (var list in customLists)
            {
                list.Components = allDetails.Where(x => x.CustomListId.Equals(list.Id)).ToList();
            }

            return ServiceUtils.CreateResult(true, 200, null, customLists, null);
        }

        /// <summary>
        /// Delete custom component list.
        /// </summary>
        /// <param name="parameters">The user id.</param>
        /// <returns>New custom list.</returns>
        public async Task<ResultModel> DeleteCustomComponentList(Dictionary<string, string> parameters)
        {
            var name = parameters.ContainsKey(ServiceConstants.Name) ? parameters[ServiceConstants.Name] : string.Empty;
            var productId = parameters.ContainsKey(ServiceConstants.ProductId) ? parameters[ServiceConstants.ProductId] : string.Empty;

            var customComponList = await this.GetCustomComponentListByProductAndName(productId, name);
            if (customComponList.Id != 0)
            {
                var listTocustom = new List<int> { customComponList.Id };
                var customComponListComponent = await this.pedidosDao.GetComponentsByCustomListId(listTocustom);

                await this.pedidosDao.DeleteComponentsOfCustomList(customComponListComponent);
                await this.pedidosDao.DeleteCustomComponentList(customComponList);
                return ServiceUtils.CreateResult(true, 200, null, customComponList.Id, null);
            }

            return ServiceUtils.CreateResult(false, 300, ServiceConstants.ReasonUnexpectedError, 0, null);
        }

        /// <summary>
        /// Validate if custom list exist.
        /// </summary>
        /// <param name="customList">Custom list to validate.</param>
        /// <returns>Flag result.</returns>urns>
        private async Task<bool> Exists(CustomComponentListModel customList)
        {
            var customLists = await this.pedidosDao.GetCustomComponentListByProduct(customList.ProductId);
            return customLists.Any(x => x.Name.ToLower().Equals(customList.Name.ToLower()));
        }

        /// <summary>
        /// Validate if custom list exist for with productId and name expecific.
        /// </summary>
        /// <param name="productId">Custom list to validate.</param>
        /// <param name="name">Custom name to validate.</param>
        /// <returns>Flag result.</returns>urns>
        private async Task<CustomComponentListModel> GetCustomComponentListByProductAndName(string productId, string name)
        {
            var customLists = await this.pedidosDao.GetCustomComponentListByProductAndName(productId, name);
            if (customLists.Any())
            {
               return customLists.FirstOrDefault();
            }

            return new CustomComponentListModel
            {
                Id = 0,
                Name = string.Empty,
                ProductId = string.Empty,
                Components = null,
                CreationUserId = string.Empty,
                CreationDate = string.Empty,
            };
        }
    }
}
