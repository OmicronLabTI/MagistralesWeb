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
                var dateArrayNum = new List<int>();
                var dateArray = filter[ServiceConstants.FechaInicio].Split("/");
                dateArray.ToList().ForEach(x =>
                {
                    int.TryParse(x, out int result);
                    dateArrayNum.Add(result);
                });

                var date = new DateTime(dateArrayNum[2], dateArrayNum[1], dateArrayNum[0]);
                dictToReturn.Add(ServiceConstants.FechaInicio, date);
            }
            else
            {
                dictToReturn.Add(ServiceConstants.FechaInicio, DateTime.Today.AddDays(-30));
            }

            if (filter.ContainsKey(ServiceConstants.FechaFin))
            {
                var dateArrayNum = new List<int>();
                var dateArray = filter[ServiceConstants.FechaFin].Split("/");
                dateArray.ToList().ForEach(x =>
                {
                    int.TryParse(x, out int result);
                    dateArrayNum.Add(result);
                });

                var date = new DateTime(dateArrayNum[2], dateArrayNum[1], dateArrayNum[0]);
                dictToReturn.Add(ServiceConstants.FechaFin, date);
            }
            else
            {
                dictToReturn.Add(ServiceConstants.FechaFin, DateTime.Today);
            }

            return dictToReturn;
        }
    }
}
