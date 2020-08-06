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
    using System.Linq;
    using System.Collections.Generic;
    using Omicron.SapAdapter.Entities.Model;
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
