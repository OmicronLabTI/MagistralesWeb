// <summary>
// <copyright file="ServiceUtils.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Services.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Omicron.SapAdapter.Entities.Model;
    using Omicron.SapAdapter.Entities.Model.JoinsModels;
    using Omicron.SapAdapter.Services.Constants;

    /// <summary>
    /// The class for the services.
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
        /// <param name="comments">the comments.</param>
        /// <returns>the resultModel.</returns>
        public static ResultModel CreateResult(bool success, int code, string userError, object responseObj, string exceptionMessage, object comments)
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
        /// gets the date filter for sap.
        /// </summary>
        /// <param name="filter">the dictionary.</param>
        /// <returns>the datetime.</returns>
        public static Dictionary<string, DateTime> GetDateFilter(Dictionary<string, string> filter)
        {
            var dictToReturn = new Dictionary<string, DateTime>();

            if (filter.ContainsKey(ServiceConstants.FechaInicio))
            {
                return GetDictDates(filter[ServiceConstants.FechaInicio]);
            }

            if (filter.ContainsKey(ServiceConstants.FechaFin))
            {
                return GetDictDates(filter[ServiceConstants.FechaFin]);
            }

            return dictToReturn;
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
        /// filters the list by the params.
        /// </summary>
        /// <param name="orderModels">the list of data.</param>
        /// <param name="parameters">the params.</param>
        /// <param name="userOrder">the usr orders.</param>
        /// <param name="users">the users.</param>
        /// <returns>the data.</returns>
        public static List<CompleteOrderModel> FilterList(List<CompleteOrderModel> orderModels, Dictionary<string, string> parameters, List<UserOrderModel> userOrder, List<UserModel> users)
        {
            orderModels.ForEach(x =>
            {
                var order = userOrder.FirstOrDefault(u => u.Salesorderid == x.DocNum.ToString() && string.IsNullOrEmpty(u.Productionorderid));
                x.Qfb = order == null ? string.Empty : order.Userid;

                if (x.PedidoStatus == "O")
                {
                    x.PedidoStatus = ServiceConstants.Abierto;
                }

                x.PedidoStatus = order == null ? x.PedidoStatus : order.Status;
            });

            if (parameters.ContainsKey(ServiceConstants.DocNum))
            {
                int.TryParse(parameters[ServiceConstants.DocNum], out int docId);
                var ordersById = orderModels.FirstOrDefault(x => x.DocNum == docId);

                var user = users.FirstOrDefault(y => y.Id.Equals(ordersById.Qfb));
                ordersById.Qfb = user == null ? string.Empty : $"{user.FirstName} {user.LastName}";

                return new List<CompleteOrderModel> { ordersById };
            }

            if (parameters.ContainsKey(ServiceConstants.Status))
            {
                orderModels = orderModels.Where(x => x.PedidoStatus == parameters[ServiceConstants.Status]).ToList();
            }

            if (parameters.ContainsKey(ServiceConstants.Qfb))
            {
                orderModels = orderModels.Where(x => !string.IsNullOrEmpty(x.Qfb) && x.Qfb.Equals(parameters[ServiceConstants.Qfb])).ToList();
            }

            orderModels.ForEach(x =>
            {
                var user = users.FirstOrDefault(y => y.Id.Equals(x.Qfb));
                x.Qfb = user == null ? string.Empty : $"{user.FirstName} {user.LastName}";
            });

            return orderModels;
        }

        /// <summary>
        /// replaces the values for the chips if they have encode values.
        /// </summary>
        /// <param name="chipsValues">the chips.</param>
        /// <returns>the data.</returns>
        public static List<string> UndecodeSpecialCaracters(List<string> chipsValues)
        {
            var listToReturn = new List<string>();

            chipsValues.ForEach(chip =>
            {
                foreach (var key in ServiceConstants.DictUrlEncode)
                {
                    chip = chip.Contains(key.Key) ? chip.Replace(key.Key, key.Value) : chip;
                }

                listToReturn.Add(chip);
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
    }
}
