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
        /// Creates the models to send.
        /// </summary>
        /// <param name="ordersId">the orders id.</param>
        /// <param name="fabOrdersId">the fab orders id.</param>
        /// <param name="sapAdapter">the sap adapter.</param>
        /// <param name="pedidosDao">the pedidos dao.</param>
        /// <param name="sapFileService">the sap file service.</param>
        /// <param name="usersService">the user service.</param>
        /// <param name="onlyFinalized">if only applies to finalized.</param>
        /// <returns>the data.</returns>
        public static async Task<Task<ResultModel>> CreateModelGeneratePdf(List<int> ordersId, List<int> fabOrdersId, ISapAdapter sapAdapter, IPedidosDao pedidosDao, ISapFileService sapFileService, IUsersService usersService, bool onlyFinalized)
        {
            var listOrdersWithDetail = new List<OrderWithDetailModel>();
            var listFabOrders = new List<FabricacionOrderModel>();
            var recipes = new List<OrderRecipeModel>();
            var listToSend = new List<FinalizaGeneratePdfModel>();
            var listUserOrders = new List<UserOrderModel>();

            if (ordersId.Any())
            {
                listOrdersWithDetail = await GetDetails(ordersId, sapAdapter, ServiceConstants.GetOrderWithDetail);
                var listIdString = ordersId.Select(x => x.ToString()).ToList();
                var userSaleOrders = (await pedidosDao.GetUserOrderBySaleOrder(listIdString)).Where(x => x.Status != ServiceConstants.Cancelled).ToList();
                userSaleOrders = onlyFinalized ? userSaleOrders.Where(x => x.Status == ServiceConstants.Finalizado).ToList() : userSaleOrders;
                listUserOrders.AddRange(userSaleOrders);
                recipes = await GetRecipes(ordersId, sapAdapter, ServiceConstants.GetRecipes);
            }

            if (fabOrdersId.Any())
            {
                listFabOrders = await GetFabOrders(fabOrdersId, sapAdapter);
                var listFabOrderIdString = fabOrdersId.Select(x => x.ToString()).ToList();
                var userOrders = (await pedidosDao.GetUserOrderByProducionOrder(listFabOrderIdString)).Where(x => x.Status != ServiceConstants.Cancelled).ToList();
                listUserOrders.AddRange(userOrders);
            }

            var userIds = listUserOrders.Where(x => !string.IsNullOrEmpty(x.Userid)).Select(x => x.Userid).DistinctBy(x => x).ToList();
            var userOrdersId = listUserOrders.Select(x => x.Id).ToList();
            var users = await GetUsers(userIds, usersService);
            var orderSignature = (await pedidosDao.GetSignaturesByUserOrderId(userOrdersId)).ToList();

            listToSend.AddRange(GetModelsByOrders(listOrdersWithDetail, recipes, users, orderSignature, listUserOrders));
            listToSend.AddRange(GetModelBySaleOrder(listFabOrders, users, orderSignature, listUserOrders));

            return sapFileService.PostSimple(listToSend, ServiceConstants.CreatePdf);
        }

        /// <summary>
        /// Creates the models by sale Order.
        /// </summary>
        /// <param name="ordersWithDetail">the orders with detail.</param>
        /// <param name="recipes">the recipes.</param>
        /// <param name="users">the users.</param>
        /// <param name="signatures">the signatures.</param>
        /// <param name="userOrders">the userOrders.</param>
        /// <returns>the data.</returns>
        private static List<FinalizaGeneratePdfModel> GetModelsByOrders(List<OrderWithDetailModel> ordersWithDetail, List<OrderRecipeModel> recipes, List<UserModel> users, List<UserOrderSignatureModel> signatures, List<UserOrderModel> userOrders)
        {
            var listToReturn = new List<FinalizaGeneratePdfModel>();
            foreach (var order in ordersWithDetail)
            {
                var recipe = recipes.FirstOrDefault(r => r.Order == order.Order.PedidoId);

                if (!order.Detalle.Any(d => d.OrdenFabricacionId != 0))
                {
                    var modelOrder = new FinalizaGeneratePdfModel
                    {
                        OrderId = order.Order.PedidoId,
                        SaleOrderCreateDate = order.Order.FechaInicio.ToString("dd/MM/yyyy"),
                        MedicName = NormalizeMedicName(order.Order.Medico),
                        RecipeRoute = recipe == null ? string.Empty : recipe.Recipe,
                    };

                    listToReturn.Add(modelOrder);
                    continue;
                }

                foreach (var detail in order.Detalle.Where(x => x.OrdenFabricacionId != 0).ToList())
                {
                    var userOrder = userOrders.Where(y => !string.IsNullOrEmpty(y.Productionorderid)).FirstOrDefault(x => x.Productionorderid.Equals(detail.OrdenFabricacionId.ToString()));
                    userOrder = userOrder == null ? new UserOrderModel { Id = -1, Userid = "NoUser" } : userOrder;

                    if (userOrder.Id == -1)
                    {
                        continue;
                    }

                    var signaturesByOrder = signatures.FirstOrDefault(x => x.UserOrderId == userOrder.Id);
                    var user = users.FirstOrDefault(x => x.Id.Equals(userOrder.Userid));

                    var model = new FinalizaGeneratePdfModel
                    {
                        CreateDate = detail.CreatedDate.HasValue ? detail.CreatedDate.Value.ToString("dd/MM/yyyy") : DateTime.Now.ToString("dd/MM/yyyy"),
                        FabOrderId = detail.OrdenFabricacionId,
                        ItemCode = detail.CodigoProducto,
                        MedicName = NormalizeMedicName(order.Order.Medico),
                        OrderId = order.Order.PedidoId,
                        QfbName = user == null ? string.Empty : $"{user.FirstName} {user.LastName}",
                        QfbSignature = signaturesByOrder == null ? new byte[0] : signaturesByOrder.QfbSignature,
                        RecipeRoute = recipe == null ? string.Empty : recipe.Recipe,
                        SaleOrderCreateDate = order.Order.FechaInicio.ToString("dd/MM/yyyy"),
                        TechnicalSignature = signaturesByOrder == null ? new byte[0] : signaturesByOrder.TechnicalSignature,
                        UserOrderId = userOrder.Id,
                    };

                    listToReturn.Add(model);
                }
            }

            return listToReturn;
        }

        /// <summary>
        /// Creates the model for fab orders.
        /// </summary>
        /// <param name="orders">the fab orders.</param>
        /// <param name="users">the user.</param>
        /// <param name="signatures">the signatures.</param>
        /// <param name="userOrders">the userordees.</param>
        /// <returns>the data.</returns>
        private static List<FinalizaGeneratePdfModel> GetModelBySaleOrder(List<FabricacionOrderModel> orders, List<UserModel> users, List<UserOrderSignatureModel> signatures, List<UserOrderModel> userOrders)
        {
            var listToReturn = new List<FinalizaGeneratePdfModel>();

            foreach (var order in orders)
            {
                var userOrder = userOrders.FirstOrDefault(x => x.Productionorderid.Equals(order.OrdenId.ToString()));
                userOrder = userOrder == null ? new UserOrderModel { Id = -1, Userid = "NoUser" } : userOrder;

                if (userOrder.Id == -1)
                {
                    continue;
                }

                var signaturesByOrder = signatures.FirstOrDefault(x => x.UserOrderId == userOrder.Id);
                var user = users.FirstOrDefault(x => x.Id.Equals(userOrder.Userid));

                var model = new FinalizaGeneratePdfModel
                {
                    CreateDate = order.CreatedDate.ToString("dd/MM/yyyy"),
                    FabOrderId = order.OrdenId,
                    ItemCode = order.ProductoId,
                    OrderId = 0,
                    QfbName = user == null ? string.Empty : $"{user.FirstName} {user.LastName}",
                    QfbSignature = signaturesByOrder == null ? new byte[0] : signaturesByOrder.QfbSignature,
                    RecipeRoute = string.Empty,
                    SaleOrderCreateDate = string.Empty,
                    TechnicalSignature = signaturesByOrder == null ? new byte[0] : signaturesByOrder.TechnicalSignature,
                    UserOrderId = userOrder.Id,
                };

                listToReturn.Add(model);
            }

            return listToReturn;
        }

        /// <summary>
        /// Normalize the medic name.
        /// </summary>
        /// <param name="medicName">the medic.</param>
        /// <returns>the data.</returns>
        private static string NormalizeMedicName(string medicName)
        {
            medicName = medicName.Replace("*", string.Empty);
            medicName = medicName.Replace(":", string.Empty);
            medicName = medicName.Replace("/", string.Empty);
            medicName = medicName.Replace(@"\", string.Empty);
            return medicName;
        }

        /// <summary>
        /// Gets The recipes.
        /// </summary>
        /// <param name="orderIds">the orders id.</param>
        /// <param name="sapAdapter">the sap adapter.</param>
        /// <returns>the data.</returns>
        private static async Task<List<OrderRecipeModel>> GetRecipes(List<int> orderIds, ISapAdapter sapAdapter, string route)
        {
            var sapResponse = await sapAdapter.PostSapAdapter(orderIds, route);
            return JsonConvert.DeserializeObject<List<OrderRecipeModel>>(JsonConvert.SerializeObject(sapResponse.Response));
        }

        /// <summary>
        /// Gets The recipes.
        /// </summary>
        /// <param name="orderIds">the orders id.</param>
        /// <param name="sapAdapter">the sap adapter.</param>
        /// <returns>the data.</returns>
        private static async Task<List<OrderWithDetailModel>> GetDetails(List<int> orderIds, ISapAdapter sapAdapter, string route)
        {
            var sapResponse = await sapAdapter.PostSapAdapter(orderIds, route);
            return JsonConvert.DeserializeObject<List<OrderWithDetailModel>>(JsonConvert.SerializeObject(sapResponse.Response));
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
            return JsonConvert.DeserializeObject<List<FabricacionOrderModel>>(JsonConvert.SerializeObject(sapResponse.Response));
        }

        /// <summary>
        /// Gets the users.
        /// </summary>
        /// <param name="userIds">the user id.</param>
        /// <param name="usersService">the service.</param>
        /// <returns>the data.</returns>
        private static async Task<List<UserModel>> GetUsers(List<string> userIds, IUsersService usersService)
        {
            var userResponse = await usersService.PostSimpleUsers(userIds, ServiceConstants.GetUsersById);
            return JsonConvert.DeserializeObject<List<UserModel>>(userResponse.Response.ToString());
        }
    }
}
