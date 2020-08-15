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
    using System.Linq;
    using System.Collections.Generic;
    using Omicron.Pedidos.Entities.Model;
    using Omicron.Pedidos.Services.Constants;
    using Omicron.Pedidos.Entities.Enums;

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
        /// create users orders.
        /// </summary>
        /// <param name="ordenFabId">the ids to create.</param>
        /// <returns>the list.</returns>
        public static List<UserOrderModel> CreateUserOrder(List<FabricacionOrderModel> ordenFabId)
        {
            var listToReturn = new List<UserOrderModel>();
            ordenFabId.ForEach(x =>
            {
                var userOrder = new UserOrderModel
                {
                    Status = ServiceConstants.Planificado,
                    Salesorderid = x.PedidoId.ToString(),
                    Productionorderid = x.OrdenId.ToString(),
                };

                listToReturn.Add(userOrder);
            });

            ordenFabId.Select(x => x.PedidoId).Distinct().ToList().ForEach(p =>
            {
                listToReturn.Add(new UserOrderModel
                {
                    Status = ServiceConstants.Planificado,
                    Salesorderid = p.ToString(),
                });
            });

            return listToReturn;
        }

        /// <summary>
        /// creates the user model from fabrication.
        /// </summary>
        /// <param name="dataToCreate">the data to create.</param>
        /// <returns>the data.</returns>
        public static List<UserOrderModel> CreateUserModel(List<FabricacionOrderModel> dataToCreate)
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
                listToReturn.Add(order[1]);
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
                                FinishDate = sapOrder.EndDate,
                                PlannedQuantity = sapOrder.PlannedQuantity,
                                ProductionOrderId = sapOrder.ProductionOrderId,
                                StartDate = sapOrder.StartDate,
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
    }
}
