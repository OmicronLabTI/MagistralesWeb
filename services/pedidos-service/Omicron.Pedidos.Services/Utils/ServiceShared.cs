// <summary>
// <copyright file="ServiceShared.cs" company="Axity">
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
    using System.Net.Http;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Omicron.LeadToCash.Resources.Exceptions;
    using Omicron.Pedidos.Entities.Model;
    using Omicron.Pedidos.Entities.Model.Db;
    using Serilog;

    /// <summary>
    /// The class for the services.
    /// </summary>
    public static class ServiceShared
    {
        /// <summary>
        ///    test.
        /// </summary>
        /// <typeparam name="T">s.</typeparam>
        /// <param name="obj">sfr.</param>
        /// <param name="name">name of param.</param>
        /// <returns>ca.</returns>
        public static T ThrowIfNull<T>(this T obj, string name)
        {
            return obj ?? throw new ArgumentNullException(name);
        }

        /// <summary>
        /// creates the result.
        /// </summary>
        /// <param name="dic">the dictioanry.</param><
        /// <param name="key">the key to search.</param>
        /// <param name="defaultValue">default value.</param>
        /// <returns>the resultModel.</returns>
        public static string GetDictionaryValueString(Dictionary<string, string> dic, string key, string defaultValue)
        {
            return dic.ContainsKey(key) ? dic[key] : defaultValue;
        }

        /// <summary>
        /// Calculate value from validation.
        /// </summary>
        /// <typeparam name="T">the T type.</typeparam>
        /// <param name="validation">Validation.</param>
        /// <param name="value">True value.</param>
        /// <param name="defaultValue">False value.</param>
        /// <returns>the type T..</returns>
        public static T CalculateTernary<T>(bool validation, T value, T defaultValue)
        {
            return validation ? value : defaultValue;
        }

        /// <summary>
        /// Calculate value from validation.
        /// </summary>
        /// <param name="value">True value.</param>
        /// <param name="defaultValue">False value.</param>
        /// <returns>the type T..</returns>
        public static int GetValueFromParamterIntParse(ParametersModel value, int defaultValue)
        {
            return value != null ? int.Parse(value.Value) : defaultValue;
        }

        /// <summary>
        /// get the line products order header.
        /// </summary>
        /// <typeparam name="T">the type.</typeparam>
        /// <param name="value">the value to deserialize.</param>
        /// <param name="defaultList">the default list.</param>
        /// <returns>a line product model.</returns>
        public static List<T> DeserializeObject<T>(string value, List<T> defaultList)
        {
            return !string.IsNullOrEmpty(value) ? JsonConvert.DeserializeObject<List<T>>(value) : defaultList;
        }

        /// <summary>
        /// Calculates the left and right with and AND.
        /// </summary>
        /// <param name="list">List of bools..</param>
        /// <returns>the data.</returns>
        public static bool CalculateAnd(params bool[] list)
        {
            return list.All(element => element);
        }

        /// <summary>
        /// Calculates the left and right with and OR.
        /// </summary>
        /// <param name="list">List of bools..</param>
        /// <returns>the data.</returns>
        public static bool CalculateOr(params bool[] list)
        {
            return list.Any(element => element);
        }

        /// <summary>
        /// Gets the response from a http response.
        /// </summary>
        /// <param name="response">the response.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="error">the error.</param>
        /// <returns>the data.</returns>
        public static async Task<ResultModel> GetResponse(HttpResponseMessage response, ILogger logger, string error)
        {
            var jsonString = await response.Content.ReadAsStringAsync();

            if ((int)response.StatusCode >= 300)
            {
                logger.Information($"{error} {jsonString}");
                throw new CustomServiceException(jsonString, System.Net.HttpStatusCode.NotFound);
            }

            return JsonConvert.DeserializeObject<ResultModel>(await response.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// counts the invices by type and status.
        /// </summary>
        /// <param name="list">the list od data.</param>
        /// <param name="invoiceType">the type.</param>
        /// <param name="status">the status.</param>
        /// <returns>the count.</returns>
        public static int GetInvoiceCount(this List<UserOrderModel> list, string invoiceType, string status)
        {
            return list.Count(x => x.InvoiceType == invoiceType && x.StatusInvoice == status);
        }
    }
}
