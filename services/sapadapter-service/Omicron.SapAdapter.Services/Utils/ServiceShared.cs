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
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Omicron.SapAdapter.Dtos.DxpModels;
    using Omicron.SapAdapter.Entities.Model.AlmacenModels;
    using Omicron.SapAdapter.Entities.Model.JoinsModels;
    using Omicron.SapAdapter.Services.Constants;
    using Omicron.SapAdapter.Services.ProccessPayments;

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
        /// Calculates the "and's" conditions.
        /// </summary>
        /// <param name="list">list of bools to evaluate.</param>
        /// <returns>the data.</returns>
        public static bool CalculateAnd(params bool[] list)
        {
            return list.All(element => element);
        }

        /// <summary>
        /// Calculates the "or´s" conditions.
        /// </summary>
        /// <param name="list">list of bools to evaluate.</param>
        /// <returns>the data.</returns>
        public static bool CalculateOr(params bool[] list)
        {
            return list.Any(element => element);
        }

        /// <summary>
        /// Applies the offset and limit.
        /// </summary>
        /// <typeparam name="T">the list.</typeparam>
        /// <param name="listToApply">the list to apply.</param>
        /// <param name="parameters">the data.</param>
        /// <returns>the values.</returns>
        public static List<T> GetOffsetLimit<T>(List<T> listToApply, Dictionary<string, string> parameters)
        {
            var offset = GetDictionaryValueString(parameters, ServiceConstants.Offset, "0");
            var limit = GetDictionaryValueString(parameters, ServiceConstants.Limit, "1");

            int.TryParse(offset, out int offsetNumber);
            int.TryParse(limit, out int limitNumber);
            return listToApply.Skip(offsetNumber).Take(limitNumber).ToList();
        }

        /// <summary>
        /// Calculates the "or´s" conditions.
        /// </summary>
        /// <param name="transactionId">the transactionid.</param>
        /// <returns>the data.</returns>
        public static string GetSubtransaction(this string transactionId)
        {
            return transactionId.Substring(transactionId.Length - 6, 6);
        }

        /// <summary>
        /// Gets the response from a http response.
        /// </summary>
        /// <param name="proccessPayments">the proccess payments.</param>
        /// <param name="transactionsIds">th transactions ids.</param>
        /// <returns>the data.</returns>
        public static async Task<List<PaymentsDto>> GetPaymentsByTransactionsIds(IProccessPayments proccessPayments, List<string> transactionsIds)
        {
            var paymentsResponse = await proccessPayments.PostProccessPayments(transactionsIds, ServiceConstants.EndPointToGetPayments);
            return JsonConvert.DeserializeObject<List<PaymentsDto>>(paymentsResponse.Response.ToString());
        }

        /// <summary>
        /// get payments from a list.
        /// </summary>
        /// <param name="payments">the payments.</param>
        /// <param name="docNumDxp">the docnumdxp.</param>
        /// <returns>data.</returns>
        public static PaymentsDto GetPaymentBydocNumDxp(this List<PaymentsDto> payments, string docNumDxp)
        {
            var payment = payments.FirstOrDefault(p => p.TransactionId.GetSubtransaction() == docNumDxp);
            payment ??= new PaymentsDto { ShippingCostAccepted = ServiceConstants.ShippingCostAccepted };
            return payment;
        }
    }
}
