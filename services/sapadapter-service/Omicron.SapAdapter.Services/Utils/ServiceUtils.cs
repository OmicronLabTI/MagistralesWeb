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
        /// gets the date filter for sap.
        /// </summary>
        /// <param name="filter">the dictionary.</param>
        /// <returns>the datetime.</returns>
        public static DateTime GetDateFilter(Dictionary<string, string> filter)
        {
            if (!filter.ContainsKey(ServiceConstants.FilterDate) || filter[ServiceConstants.FilterDate].Equals(ServiceConstants.Today))
            {
                return DateTime.Today;
            }

            if (filter[ServiceConstants.FilterDate].Equals(ServiceConstants.TwoWeeks))
            {
                return DateTime.Today.AddDays(-14);
            }

            if (filter[ServiceConstants.FilterDate].Equals(ServiceConstants.Month))
            {
                return new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            }

            if (filter[ServiceConstants.FilterDate].Equals(ServiceConstants.Week))
            {
                return DateTime.Today.AddDays(-7);
            }

            return DateTime.Today;
        }
    }
}
