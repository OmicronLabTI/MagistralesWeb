// <summary>
// <copyright file="ServiceShared.cs" company="Axity">
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
    using Omicron.SapAdapter.Entities.Model.AlmacenModels;
    using Omicron.SapAdapter.Entities.Model.JoinsModels;
    using Omicron.SapAdapter.Services.Constants;

    /// <summary>
    /// The class for the services.
    /// </summary>
    public static class ServiceShared
    {
        /// <summary>
        /// creates the result.
        /// </summary>
        /// <param name="word">the word to split.</param>
        /// <returns>the resultModel.</returns>
        public static string ValidateNull(this string word)
        {
            return string.IsNullOrEmpty(word) ? string.Empty : word;
        }

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
        /// get a date value or default value.
        /// </summary>
        /// <param name="date">the date to check.</param>
        /// <param name="defaultDate">tehd efault date.</param>
        /// <returns>a date in string format.</returns>
        public static string GetDateValueOrDefault(DateTime? date, string defaultDate)
        {
            return date.HasValue ? date.Value.ToString("dd/MM/yyyy") : defaultDate;
        }

        /// <summary>
        /// get a parse exact date time.
        /// </summary>
        /// <param name="date">the date.</param>
        /// <param name="defaultDate">the default value.</param>
        /// <returns>string value.</returns>
        public static DateTime ParseExactDateOrDefault(string date, DateTime defaultDate)
        {
            return !string.IsNullOrEmpty(date) ? DateTime.ParseExact(date, "dd/MM/yyyy", null) : defaultDate;
        }

        /// <summary>
        /// get item code with fabrication order.
        /// </summary>
        /// <param name="productoId">the productoId.</param>
        /// <param name="orderId">the order id.</param>
        /// <returns>complete itemcode.</returns>
        public static string GetItemcode(string productoId, string orderId)
        {
            return !string.IsNullOrEmpty(orderId) ? $"{productoId} - {orderId}" : productoId;
        }

        /// <summary>
        /// get if is valid filter by type shipping.
        /// </summary>
        /// <param name="parameters">the params.</param>
        /// <returns>a bool.</returns>
        public static bool IsValidFilterByTypeShipping(Dictionary<string, string> parameters)
        {
            return parameters.ContainsKey(ServiceConstants.Shipping) && parameters[ServiceConstants.Shipping].Split(",").Count() == 1;
        }

        /// <summary>
        /// get the userorder header.
        /// </summary>
        /// <param name="userOrders">the userorders.</param>
        /// <param name="saleOrderId">the saleorderid to look for.</param>
        /// <returns>a user order model.</returns>
        public static UserOrderModel GetSaleOrderHeader(this List<UserOrderModel> userOrders, string saleOrderId)
        {
            return userOrders.FirstOrDefault(x => x.Salesorderid == saleOrderId && string.IsNullOrEmpty(x.Productionorderid));
        }

        /// <summary>
        /// get the line products order header.
        /// </summary>
        /// <param name="lineProducts">the lineProducts.</param>
        /// <param name="saleOrderId">the saleorderid to look for.</param>
        /// <returns>a line product model.</returns>
        public static LineProductsModel GetLineProductOrderHeader(this List<LineProductsModel> lineProducts, int saleOrderId)
        {
            return lineProducts.FirstOrDefault(x => x.SaleOrderId == saleOrderId && string.IsNullOrEmpty(x.ItemCode));
        }
    }
}
