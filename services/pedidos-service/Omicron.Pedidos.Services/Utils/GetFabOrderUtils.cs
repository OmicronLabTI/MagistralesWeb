// <summary>
// <copyright file="GetFabOrderUtils.cs" company="Axity">
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
    using Omicron.Pedidos.DataAccess.DAO.Pedidos;
    using Omicron.Pedidos.Entities.Model;
    using Omicron.Pedidos.Services.Constants;

    /// <summary>
    /// The class for the get orders.
    /// </summary>
    public static class GetFabOrderUtils
    {
        /// <summary>
        /// gets the data by the filters of status, qfb, end fate.
        /// </summary>
        /// <param name="parameters">the parameters.</param>
        /// <param name="pedidosDao">the pedidos dao.</param>
        /// <returns>the data.</returns>
        public static async Task<List<UserOrderModel>> GetOrdersByFilter(Dictionary<string, string> parameters, IPedidosDao pedidosDao)
        {
            if (parameters.ContainsKey(ServiceConstants.DocNum))
            {
                return (await pedidosDao.GetUserOrderByProducionOrder(new List<string> { parameters[ServiceConstants.DocNum] })).ToList();
            }

            var listOrders = new List<UserOrderModel>();
            var filterQfb = parameters.ContainsKey(ServiceConstants.Qfb);
            var filterFechaFin = parameters.ContainsKey(ServiceConstants.FechaFin);
            var filterStatus = parameters.ContainsKey(ServiceConstants.Status);

            if (filterQfb)
            {
                listOrders.AddRange((await pedidosDao.GetUserOrderByUserId(new List<string> { parameters[ServiceConstants.Qfb] })).ToList());
            }

            if (filterStatus)
            {
                var listStatus = filterQfb ? listOrders.Where(x => x.Status.Equals(parameters[ServiceConstants.Status])).ToList() : (await pedidosDao.GetUserOrderByStatus(new List<string> { parameters[ServiceConstants.Status] })).ToList();
                listOrders.AddRange(listStatus);
            }

            if (filterFechaFin)
            {
                listOrders = await GetOrdersFilteredByDate(parameters, filterQfb || filterStatus, listOrders, pedidosDao);
            }

            return listOrders.DistinctBy(x => x.Id).ToList();
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
        /// Creates the model to return.
        /// </summary>
        /// <param name="fabOrderModel">the order.</param>
        /// <param name="userOrders">the user order.</param>
        /// <param name="users">the user.</param>
        /// <returns>the data.</returns>
        public static List<CompleteOrderModel> CreateModels(List<FabricacionOrderModel> fabOrderModel, List<UserOrderModel> userOrders, List<UserModel> users)
        {
            var listToReturn = new List<CompleteOrderModel>();

            fabOrderModel.ForEach(x =>
            {
                var userOrder = userOrders.FirstOrDefault(y => y.Productionorderid.Equals(x.OrdenId.ToString()));
                userOrder = userOrder == null ? new UserOrderModel() : userOrder;
                var status = userOrder.Status == null ? ServiceConstants.Abierto : userOrder.Status;

                var user = users.FirstOrDefault(y => y.Id.Equals(userOrder.Userid));

                var fabOrder = new CompleteOrderModel
                {
                    DocNum = x.PedidoId,
                    FabOrderId = x.OrdenId,
                    ItemCode = x.ProductoId,
                    Description = x.ProdName,
                    Quantity = x.Quantity,
                    CreateDate = x.CreatedDate.ToString("dd/MM/yyyy"),
                    FinishDate = userOrder.FinishDate == null ? string.Empty : userOrder.FinishDate,
                    Status = status.Equals(ServiceConstants.Proceso) ? ServiceConstants.ProcesoStatus : status,
                    Qfb = user == null ? string.Empty : $"{user.FirstName} {user.LastName}",
                };

                listToReturn.Add(fabOrder);
            });

            return listToReturn;
        }

        /// <summary>
        /// gets the dictionary.
        /// </summary>
        /// <param name="dateRange">the date range.</param>
        /// <returns>the data.</returns>
        private static Dictionary<string, DateTime> GetDictDates(string dateRange)
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

        /// <summary>
        /// Get the data filtered by date.
        /// </summary>
        /// <param name="parameters">the original dict.</param>
        /// <param name="dataFiltered">if there are other filtesr.</param>
        /// <param name="listOrders">the data already filtered.</param>
        /// <param name="pedidosDao">the dao.</param>
        /// <returns>the data.</returns>
        private static async Task<List<UserOrderModel>> GetOrdersFilteredByDate(Dictionary<string, string> parameters, bool dataFiltered, List<UserOrderModel> listOrders, IPedidosDao pedidosDao)
        {
            var dateFilter = GetDateFilter(parameters);

            if (dataFiltered)
            {
                var listToReturn = new List<UserOrderModel>();
                listOrders.ForEach(x =>
                {
                    DateTime.TryParse(x.FinishDate, out var date);

                    if (date >= dateFilter[ServiceConstants.FechaInicio] && date <= dateFilter[ServiceConstants.FechaFin])
                    {
                        listToReturn.Add(x);
                    }
                });

                return listToReturn;
            }
            else
            {
                return (await pedidosDao.GetUserOrderByFechaFin(dateFilter[ServiceConstants.FechaInicio], dateFilter[ServiceConstants.FechaFin])).ToList();
            }
        }
    }
}
