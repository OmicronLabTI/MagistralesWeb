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
                    Status = ServiceConstants.Planificada,
                    Salesorderid = x.PedidoId.ToString(),
                    Productionorderid = x.OrdenId.ToString(),
                };

                listToReturn.Add(userOrder);
            });

            ordenFabId.Select(x => x.PedidoId).Distinct().ToList().ForEach(p =>
            {
                listToReturn.Add(new UserOrderModel
                {
                    Status = ServiceConstants.Planificada,
                    Salesorderid = p.ToString(),
                });
            });

            return listToReturn;
        }

        /// <summary>
        /// Creates the order logs mode.
        /// </summary>
        /// <param name="user">the user.</param>
        /// <param name="pedidosId">pedidos seleccionados.</param>
        /// <param name="ordenesFabId">ordenes creadas.</param>
        /// <returns>the list to insert.</returns>
        public static List<OrderLogModel> CreateOrderLog(string user, List<int> pedidosId, List<FabricacionOrderModel> ordenesFabId)
        {
            var listToReturn = new List<OrderLogModel>();

            pedidosId.ForEach(x =>
            {
                listToReturn.Add(new OrderLogModel
                {
                    Description = ServiceConstants.OrdenVentaPlan,
                    Logdatetime = DateTime.Now,
                    Noid = x.ToString(),
                    Type = ServiceConstants.OrdenVenta,
                    Userid = user,
                });
            });

            ordenesFabId.ForEach(x =>
            {
                listToReturn.Add(new OrderLogModel
                {
                    Description = ServiceConstants.OrdenFabricacionPlan,
                    Logdatetime = DateTime.Now,
                    Noid = x.OrdenId.ToString(),
                    Type = ServiceConstants.OrdenFab,
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
        public static List<string> GetErrorsWhileInserting(List<string> listWithError)
        {
            var listToReturn = new List<string>();

            listWithError.ForEach(x =>
            {
                var order = x.Split("-");
                listToReturn.Add(order[1]);
            });

            return listToReturn;
        }
    }
}
