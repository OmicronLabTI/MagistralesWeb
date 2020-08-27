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
        /// <returns>the resultModel.</returns>
        public static ResultModel CreateResult(bool success, int code, string userError, object responseObj, string exceptionMessage)
        {
            return new ResultModel
            {
                Success = success,
                Response = responseObj,
                UserError = userError,
                ExceptionMessage = exceptionMessage,
                Code = code,
            };
        }

        /// <summary>
        /// creates the user model from fabrication.
        /// </summary>
        /// <param name="dataToCreate">the data to create.</param>
        /// <returns>the data.</returns>
        public static List<UserOrderModel> CreateUserModelOrders(List<FabricacionOrderModel> dataToCreate)
        {
            var listToReturn = new List<UserOrderModel>();
            dataToCreate.ForEach(x =>
            {
                listToReturn.Add(new UserOrderModel
                {
                    Productionorderid = x.OrdenId.ToString(),
                    Salesorderid = x.PedidoId.ToString(),
                    Status = ServiceConstants.Planificado,
                });
            });

            return listToReturn;
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
                if (order.Count() > 2)
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
                    .Select(y => y.Productionorderid)
                    .ToList()
                    .ForEach(o =>
                    {
                        int.TryParse(o, out int orderId);
                        var sapOrder = sapOrders.FirstOrDefault(s => s.ProductionOrderId == orderId);

                        if (sapOrder != null)
                        {
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
        /// creates the order detail.
        /// </summary>
        /// <param name="order">the order.</param>
        /// <param name="listToSend">list to send.</param>
        /// <returns>the data.</returns>
        public static OrderWithDetailModel CreateOrderWithDetail(OrderWithDetailModel order, List<CompleteDetailOrderModel> listToSend)
        {
            return new OrderWithDetailModel
            {
                Order = new OrderModel
                {
                    PedidoId = order.Order.PedidoId,
                    FechaInicio = order.Order.FechaInicio,
                    FechaFin = order.Order.FechaFin,
                },
                Detalle = new List<CompleteDetailOrderModel>(listToSend),
            };
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
                return allUsers.Where(x => x.Activo == 1).ToList();
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
        /// Gets the list To update or insert.
        /// </summary>
        /// <param name="pedidosId">the pedidos id.</param>
        /// <param name="dataBaseSaleOrders">the database sale orders.</param>
        /// <returns>the first is the list to insert the second the list to update.</returns>
        public static Tuple<List<UserOrderModel>, List<UserOrderModel>> GetListToUpdateInsert(List<int> pedidosId, List<UserOrderModel> dataBaseSaleOrders)
        {
            var listToInsert = new List<UserOrderModel>();
            var listToUpdate = new List<UserOrderModel>();

            pedidosId.ForEach(p =>
            {
                var insertUserOrdersale = false;
                var saleOrder = dataBaseSaleOrders.FirstOrDefault(x => string.IsNullOrEmpty(x.Productionorderid) && x.Salesorderid.Equals(p.ToString()));

                if (saleOrder == null)
                {
                    saleOrder = new UserOrderModel
                    {
                        Salesorderid = p.ToString(),
                    };

                    insertUserOrdersale = true;
                }

                saleOrder.Status = ServiceConstants.Planificado;

                if (insertUserOrdersale)
                {
                    listToInsert.Add(saleOrder);
                }
                else
                {
                    listToUpdate.Add(saleOrder);
                }
            });

            return new Tuple<List<UserOrderModel>, List<UserOrderModel>>(listToInsert, listToUpdate);
        }
    }
}
