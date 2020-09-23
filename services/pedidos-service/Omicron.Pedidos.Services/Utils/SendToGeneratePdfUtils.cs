// <summary>
// <copyright file="SendToGeneratePdfUtils.cs" company="Axity">
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
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Omicron.Pedidos.DataAccess.DAO.Pedidos;
    using Omicron.Pedidos.Entities.Model;
    using Omicron.Pedidos.Entities.Model.Db;
    using Omicron.Pedidos.Services.Constants;
    using Omicron.Pedidos.Services.SapAdapter;
    using Omicron.Pedidos.Services.SapFile;
    using Omicron.Pedidos.Services.User;

    /// <summary>
    /// Class to generate pdfs.
    /// </summary>
    public static class SendToGeneratePdfUtils
    {
        /// <summary>
        /// Generates the model to send to create the pdfs.
        /// </summary>
        /// <param name="ordersToGenerate">the data.</param>
        /// <param name="pedidosDao">the pedido dao.</param>
        /// <param name="sapAdapter">the sap adapter.</param>
        /// <param name="sapFileService">Sap file service.</param>
        /// <param name="usersService">The user service.</param>
        /// <returns>nothing.</returns>
        public static async Task CreateModelGeneratePdf(List<FinalizaGeneratePdfModel> ordersToGenerate, IPedidosDao pedidosDao, ISapAdapter sapAdapter, ISapFileService sapFileService, IUsersService usersService)
        {
            var listOrder = ordersToGenerate.Where(x => x.OrderId != 0).Select(x => x.OrderId).ToList();
            var orderFab = ordersToGenerate.Where(x => x.FabOrderId != 0).Select(x => x.FabOrderId).ToList();
            var userOrders = ordersToGenerate.Where(x => x.UserOrderId != 0).Select(x => x.UserOrderId).ToList();
            var userIds = ordersToGenerate.Where(x => !string.IsNullOrEmpty(x.QfbName)).Select(x => x.QfbName).ToList();

            var recipes = await GetRecipes(listOrder, sapAdapter);
            var listOrderFab = await GetFabOrders(orderFab, sapAdapter);
            var orderSignature = await pedidosDao.GetSignaturesByUserOrderId(userOrders);
            var users = await GetUsers(userIds, usersService);

            ordersToGenerate.ForEach(o =>
            {
                var recipe = recipes.FirstOrDefault(x => x.Order == o.OrderId);
                var orderFab = listOrderFab.FirstOrDefault(x => x.OrdenId == o.FabOrderId);
                var signature = orderSignature.FirstOrDefault(x => x.UserOrderId == o.UserOrderId);
                var user = users.FirstOrDefault(x => x.Id.Equals(o.QfbName));

                o.RecipeRoute = recipe == null ? string.Empty : recipe.Recipe;
                o.CreateDate = orderFab == null ? string.Empty : orderFab.CreatedDate.ToString("dd/MM/yyyy");
                o.QfbSignature = signature == null ? new byte[0] : signature.QfbSignature;
                o.TechnicalSignature = signature == null ? new byte[0] : signature.TechnicalSignature;
                o.QfbName = user == null ? string.Empty : $"{user.FirstName} {user.LastName}";
            });

            await sapFileService.PostSimple(ordersToGenerate, ServiceConstants.CreatePdf);
        }

        /// <summary>
        /// Gets The recipes.
        /// </summary>
        /// <param name="orderIds">the orders id.</param>
        /// <param name="sapAdapter">the sap adapter.</param>
        /// <returns>the data.</returns>
        private static async Task<List<OrderRecipeModel>> GetRecipes(List<int> orderIds, ISapAdapter sapAdapter)
        {
            var sapResponse = await sapAdapter.PostSapAdapter(orderIds, ServiceConstants.GetRecipes);
            return JsonConvert.DeserializeObject<List<OrderRecipeModel>>(sapResponse.Response.ToString());
        }

        /// <summary>
        /// Gets the order fabs.
        /// </summary>
        /// <param name="fabOrdersId">the orders fab.</param>
        /// <param name="sapAdapter">the sap adapter.</param>
        /// <returns>the data.</returns>
        private static async Task<List<FabricacionOrderModel>> GetFabOrders(List<int> fabOrdersId, ISapAdapter sapAdapter)
        {
            var sapResponse = await sapAdapter.PostSapAdapter(fabOrdersId, ServiceConstants.GetUsersByOrdersById);
            return JsonConvert.DeserializeObject<List<FabricacionOrderModel>>(sapResponse.Response.ToString());
        }

        private static async Task<List<UserModel>> GetUsers(List<string> userIds, IUsersService usersService)
        {
            var userResponse = await usersService.PostSimpleUsers(userIds, ServiceConstants.GetUsersById);
            return JsonConvert.DeserializeObject<List<UserModel>>(userResponse.Response.ToString());
        }
    }
}
