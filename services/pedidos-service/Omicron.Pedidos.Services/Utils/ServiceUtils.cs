// <summary>
// <copyright file="ServiceUtils.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Services.Utils
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Omicron.Pedidos.Entities.Enums;
    using Omicron.Pedidos.Entities.Model;
    using Omicron.Pedidos.Services.Constants;
    using Omicron.Pedidos.Services.SapAdapter;
    using Omicron.Pedidos.Services.User;

    /// <summary>
    /// the class for utils.
    /// </summary>
    public static class ServiceUtils
    {
        /// <summary>
        /// creates the result.
        /// </summary>
        /// <param name="success">if it was successful.</param>
        /// <param name="code">the code.</param>
        /// <param name="userError">the user error.</param>
        /// <param name="responseObj">the responseobj.</param>
        /// <param name="exceptionMessage">the exception message.</param>
        /// <param name="comments">The comments.</param>
        /// <returns>the resultModel.</returns>
        public static ResultModel CreateResult(bool success, int code, string userError, object responseObj, string exceptionMessage, string comments = null)
        {
            return new ResultModel
            {
                Success = success,
                Response = responseObj,
                UserError = userError,
                ExceptionMessage = exceptionMessage,
                Code = code,
                Comments = comments,
            };
        }

        /// <summary>
        /// Creates the order logs mode.
        /// </summary>
        /// <param name="user">the user.</param>
        /// <param name="pedidosId">pedidos seleccionados.</param>
        /// <param name="description">the description.</param>
        /// <param name="type">The type.</param>
        /// <returns>the list to insert.</returns>
        public static List<OrderLogModel> CreateOrderLog(string user, List<int> pedidosId, string description, string type)
        {
            var listToReturn = new List<OrderLogModel>();

            pedidosId.ForEach(x =>
            {
                listToReturn.Add(new OrderLogModel
                {
                    Description = description,
                    Logdatetime = DateTime.Now,
                    Noid = x.ToString(),
                    Type = type,
                    Userid = user,
                });
            });

            return listToReturn;
        }

        /// <summary>
        /// Gets the list of keys by a value.
        /// </summary>
        /// <param name="dictResult">the dict.</param>
        /// <param name="correctValue">the correct value.</param>
        /// <returns>the list.</returns>
        public static List<string> GetValuesByExactValue(Dictionary<string, string> dictResult, string correctValue)
        {
            var listToReturn = new List<string>();
            foreach (var k in dictResult.Keys)
            {
                if (dictResult[k].Equals(correctValue))
                {
                    listToReturn.Add(k);
                }
            }

            return listToReturn;
        }

        /// <summary>
        /// Gets the list of keys by a value.
        /// </summary>
        /// <param name="dictResult">the dict.</param>
        /// <param name="correctValue">the correct value.</param>
        /// <returns>the list.</returns>
        public static List<string> GetValuesByContainsKeyValue(Dictionary<string, string> dictResult, string correctValue)
        {
            var listToReturn = new List<string>();
            foreach (var k in dictResult.Keys)
            {
                if (k.Contains(correctValue))
                {
                    listToReturn.Add(dictResult[k]);
                }
            }

            return listToReturn;
        }

        /// <summary>
        /// Gets the list of keys by a value.
        /// </summary>
        /// <param name="dictResult">the dict.</param>
        /// <param name="correctValue">the correct value.</param>
        /// <returns>the list.</returns>
        public static List<string> GetValuesContains(Dictionary<string, string> dictResult, string correctValue)
        {
            var listToReturn = new List<string>();
            foreach (var k in dictResult.Keys)
            {
                if (dictResult[k].Contains(correctValue))
                {
                    listToReturn.Add(k);
                }
            }

            return listToReturn;
        }

        /// <summary>
        /// gets the products Id to return.
        /// </summary>
        /// <param name="listWithError">the error list.</param>
        /// <returns>the list of values.</returns>
        public static List<string> GetErrorsFromSapDiDic(List<string> listWithError)
        {
            var listToReturn = new List<string>();

            listWithError.ForEach(x =>
            {
                var order = x.Split("-");
                if (order.Length > 2)
                {
                    listToReturn.Add($"{order[1]}-{order[2]}");
                }
                else
                {
                    listToReturn.Add(order[1]);
                }
            });

            return listToReturn;
        }

        /// <summary>
        /// Groups the data for the front by status.
        /// </summary>
        /// <param name="sapOrders">the sap ordrs.</param>
        /// <param name="userOrders">the user ordes.</param>
        /// <returns>the data froupted.</returns>
        public static QfbOrderModel GroupUserOrder(List<CompleteFormulaWithDetalle> sapOrders, List<UserOrderModel> userOrders)
        {
            var result = new QfbOrderModel
            {
                Status = new List<QfbOrderDetail>(),
            };

            foreach (var status in Enum.GetValues(typeof(ServiceEnums.Status)))
            {
                var statusId = (int)Enum.Parse(typeof(ServiceEnums.Status), status.ToString());
                var orders = new QfbOrderDetail
                {
                    StatusName = statusId == (int)ServiceEnums.Status.Proceso ? ServiceConstants.ProcesoStatus : status.ToString(),
                    StatusId = statusId,
                    Orders = new List<FabOrderDetail>(),
                };

                var ordersDetail = new List<FabOrderDetail>();

                userOrders
                    .Where(x => x.Status.Equals(status.ToString()))
                    .ToList()
                    .ForEach(o =>
                    {
                        int.TryParse(o.Productionorderid, out int orderId);
                        var sapOrder = sapOrders.FirstOrDefault(s => s.ProductionOrderId == orderId);

                        if (sapOrder != null)
                        {
                            var destiny = sapOrder.DestinyAddress.Split(",");

                            var order = new FabOrderDetail
                            {
                                BaseDocument = sapOrder.BaseDocument,
                                Container = sapOrder.Container,
                                Tag = sapOrder.ProductLabel,
                                DescriptionProduct = sapOrder.ProductDescription,
                                FinishDate = sapOrder.DueDate,
                                PlannedQuantity = sapOrder.PlannedQuantity,
                                ProductionOrderId = sapOrder.ProductionOrderId,
                                StartDate = sapOrder.FabDate,
                                ItemCode = sapOrder.Code,
                                HasMissingStock = sapOrder.HasMissingStock,
                                Destiny = destiny.Count() < 3 || destiny[destiny.Count() - 3].Contains(ServiceConstants.NuevoLeon) ? ServiceConstants.Local : ServiceConstants.Foraneo,
                                FinishedLabel = o.FinishedLabel,
                            };

                            ordersDetail.Add(order);
                        }
                    });

                orders.Orders = ordersDetail;
                result.Status.Add(orders);
            }

            return result;
        }

        /// <summary>
        /// gets the user by role.
        /// </summary>
        /// <param name="userService">the user service.</param>
        /// <param name="role">the role.</param>
        /// <param name="active">if the return data is by active or all.</param>
        /// <returns>the users.</returns>
        public static async Task<List<UserModel>> GetUsersByRole(IUsersService userService, string role, bool active)
        {
            var resultUsers = await userService.SimpleGetUsers(string.Format(ServiceConstants.GetUsersByRole, role));
            var allUsers = JsonConvert.DeserializeObject<List<UserModel>>(JsonConvert.SerializeObject(resultUsers.Response));

            if (active)
            {
                return allUsers.Where(x => x.Activo == 1 && x.Asignable == 1).ToList();
            }

            return allUsers;
        }

        /// <summary>
        /// Gets the orders to update.
        /// </summary>
        /// <param name="docEntry">the list of pedidos.</param>
        /// <param name="sapAdapter">the sap adapter.</param>
        /// <returns>the list to update.</returns>
        public static async Task<List<UpdateFabOrderModel>> GetOrdersToAssign(List<int> docEntry, ISapAdapter sapAdapter)
        {
            var orders = new List<CompleteDetailOrderModel>();
            var listToUpdate = new List<UpdateFabOrderModel>();
            foreach (var de in docEntry)
            {
                var sapResponse = await sapAdapter.GetSapAdapter(string.Format(ServiceConstants.GetFabOrdersByPedidoId, de));
                orders.AddRange(JsonConvert.DeserializeObject<List<CompleteDetailOrderModel>>(sapResponse.Response.ToString()));
            }

            orders.Where(x => x.Status.Equals(ServiceConstants.Planificado)).ToList().ForEach(o =>
            {
                listToUpdate.Add(new UpdateFabOrderModel
                {
                    OrderFabId = o.OrdenFabricacionId,
                    Status = ServiceConstants.StatusSapLiberado,
                });
            });

            return listToUpdate;
        }

        /// <summary>
        /// gets the updatefaborder model from the list of orders.
        /// </summary>
        /// <param name="ordersWithDetail">the details.</param>
        /// <returns>the data.</returns>
        public static List<UpdateFabOrderModel> GetOrdersToAssign(List<OrderWithDetailModel> ordersWithDetail)
        {
            var orderToSend = new List<UpdateFabOrderModel>();

            ordersWithDetail.ForEach(order =>
            {
                order.Detalle
                .Where(d => d.Status.Equals("P"))
                .ToList()
                .ForEach(x =>
                {
                    orderToSend.Add(new UpdateFabOrderModel
                    {
                        OrderFabId = x.OrdenFabricacionId,
                        Status = ServiceConstants.StatusSapLiberado,
                    });
                });
            });

            return orderToSend;
        }

        /// <summary>
        /// Create a cancellation order fail object.
        /// </summary>
        /// <param name="cancellationModel">Model with data.</param>
        /// <param name="reason">Fail reason.</param>
        /// <returns>Formated object.</returns>
        public static object CreateCancellationFail(OrderIdModel cancellationModel, string reason)
        {
            return new
            {
                cancellationModel.OrderId,
                cancellationModel.UserId,
                reason,
            };
        }

        /// <summary>
        /// gets the distinc by.
        /// </summary>
        /// <typeparam name="Tsource">the list source.</typeparam>
        /// <typeparam name="TKey">the key to look.</typeparam>
        /// <param name="source">the sourec.</param>
        /// <param name="keyselector">the key.</param>
        /// <returns>the list distinc.</returns>
        public static IEnumerable<Tsource> DistinctBy<Tsource, TKey>(this IEnumerable<Tsource> source, Func<Tsource, TKey> keyselector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (Tsource element in source)
            {
                if (seenKeys.Add(keyselector(element)))
                {
                    yield return element;
                }
            }
        }

        /// <summary>
        /// Gets a list divided in sublists.
        /// </summary>
        /// <typeparam name="Tsource">the original list.</typeparam>
        /// <param name="listToSplit">the original list to split.</param>
        /// <param name="maxCount">the max count per group.</param>
        /// <returns>the list of list.</returns>
        public static List<List<Tsource>> GetGroupsOfList<Tsource>(List<Tsource> listToSplit, int maxCount)
        {
            var listToReturn = new List<List<Tsource>>();
            var offset = 0;

            while (offset < listToSplit.Count)
            {
                var sublist = new List<Tsource>();
                sublist.AddRange(listToSplit.Skip(offset).Take(maxCount).ToList());
                listToReturn.Add(sublist);
                offset += maxCount;
            }

            return listToReturn;
        }

        /// <summary>
        /// gets the date filter for sap.
        /// </summary>
        /// <param name="filter">the dictionary.</param>
        /// <returns>the datetime.</returns>
        public static Dictionary<string, DateTime> GetDateFilter(Dictionary<string, string> filter)
        {
            if (filter.ContainsKey(ServiceConstants.FechaFin))
            {
                return GetDictDates(filter[ServiceConstants.FechaFin]);
            }

            return new Dictionary<string, DateTime>();
        }

        /// <summary>
        /// Gets the orders with their details.
        /// </summary>
        /// <param name="sapAdapter">the sapAdapter.</param>
        /// <param name="salesOrdersId">the "Pedido" id.</param>
        /// <returns>the data.</returns>
        public static async Task<List<OrderWithDetailModel>> GetOrdersWithFabOrders(ISapAdapter sapAdapter, List<int> salesOrdersId)
        {
            var sapResponse = await sapAdapter.PostSapAdapter(salesOrdersId, ServiceConstants.GetOrderWithDetail);
            return JsonConvert.DeserializeObject<List<OrderWithDetailModel>>(JsonConvert.SerializeObject(sapResponse.Response));
        }

        /// <summary>
        /// Get sales order from SAP.
        /// </summary>
        /// <param name="salesOrder">Sales order in local db.</param>
        /// <param name="sapAdapter">The sap adapter.</param>
        /// <returns>Preproduction orders.</returns>
        public static async Task<List<CompleteDetailOrderModel>> GetPreProductionOrdersFromSap(UserOrderModel salesOrder, ISapAdapter sapAdapter)
        {
            var sapResults = await GetSalesOrdersFromSap(int.Parse(salesOrder.Salesorderid), sapAdapter);
            return sapResults.PreProductionOrders;
        }

        /// <summary>
        /// check if the folder exist and created is if not.
        /// </summary>
        /// <param name="route">the route.</param>
        public static void VerifyIfFolderExist(string route)
        {
            if (!Directory.Exists(route))
            {
                Directory.CreateDirectory(route);
            }
        }

        /// <summary>
        /// gets the dictionary.
        /// </summary>
        /// <param name="dateRange">the date range.</param>
        /// <returns>the data.</returns>
        public static Dictionary<string, DateTime> GetDictDates(string dateRange)
        {
            var dictToReturn = new Dictionary<string, DateTime>();
            var dates = dateRange.Split("-");

            var dateInicioArray = GetDatesAsArray(dates[0]);
            var dateFinArray = GetDatesAsArray(dates[1]);

            var dateInicio = new DateTime(dateInicioArray[2], dateInicioArray[1], dateInicioArray[0]);
            var dateFin = new DateTime(dateFinArray[2], dateFinArray[1], dateFinArray[0]);
            dictToReturn.Add(ServiceConstants.FechaInicio, dateInicio);
            dictToReturn.Add(ServiceConstants.FechaFin, dateFin);
            return dictToReturn;
        }

        /// <summary>
        /// Get sales order from SAP.
        /// </summary>
        /// <param name="salesOrderId">Sales order id.</param>
        /// <returns>Sales order.</returns>
        private static async Task<(OrderWithDetailModel SapOrder, List<CompleteDetailOrderModel> ProductionOrders, List<CompleteDetailOrderModel> PreProductionOrders)> GetSalesOrdersFromSap(int salesOrderId, ISapAdapter sapAdapter)
        {
            var orders = await sapAdapter.PostSapAdapter(new List<int> { salesOrderId }, ServiceConstants.GetOrderWithDetail);
            var sapOrders = JsonConvert.DeserializeObject<List<OrderWithDetailModel>>(JsonConvert.SerializeObject(orders.Response));
            var sapOrder = sapOrders.FirstOrDefault();
            var preProductionOrders = sapOrder.Detalle.Where(x => string.IsNullOrEmpty(x.Status));
            var productionOrders = sapOrder.Detalle.Where(x => !string.IsNullOrEmpty(x.Status));
            return (sapOrder, productionOrders.ToList(), preProductionOrders.ToList());
        }

        /// <summary>
        /// split the dates to int array.
        /// </summary>
        /// <param name="date">the date in string.</param>
        /// <returns>the dates.</returns>
        private static List<int> GetDatesAsArray(string date)
        {
            var dateArrayNum = new List<int>();
            var dateArray = date.Split("/");

            dateArray.ToList().ForEach(x =>
            {
                int.TryParse(x, out int result);
                dateArrayNum.Add(result);
            });

            return dateArrayNum;
        }
    }
}
