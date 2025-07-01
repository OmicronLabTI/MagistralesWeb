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
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Omicron.SapAdapter.Dtos.DxpModels;
    using Omicron.SapAdapter.Dtos.Models;
    using Omicron.SapAdapter.Entities.Model.AlmacenModels;
    using Omicron.SapAdapter.Entities.Model.BusinessModels;
    using Omicron.SapAdapter.Entities.Model.JoinsModels;
    using Omicron.SapAdapter.Services.Constants;
    using Omicron.SapAdapter.Services.Doctors;
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
            return userOrders.FirstOrDefault(x => CalculateAnd(x.Salesorderid == saleOrderId, string.IsNullOrEmpty(x.Productionorderid)));
        }

        /// <summary>
        /// get the line products order header.
        /// </summary>
        /// <param name="lineProducts">the lineProducts.</param>
        /// <param name="saleOrderId">the saleorderid to look for.</param>
        /// <returns>a line product model.</returns>
        public static LineProductsModel GetLineProductOrderHeader(this List<LineProductsModel> lineProducts, int saleOrderId)
        {
            return lineProducts.FirstOrDefault(x => CalculateAnd(x.SaleOrderId == saleOrderId, string.IsNullOrEmpty(x.ItemCode)));
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
        /// Calculates the short shop transaction.
        /// </summary>
        /// <param name="transactionId">Transaction id.</param>
        /// <returns>Short shop transaction.</returns>
        public static string GetShortShopTransaction(this string transactionId)
        {
            if (string.IsNullOrEmpty(transactionId))
            {
                return string.Empty;
            }

            return transactionId[^ServiceConstants.DigitsForShortShopTransaction..];
        }

        /// <summary>
        /// Validate shop transaction.
        /// </summary>
        /// <param name="transactionId">Transaction id.</param>
        /// <param name="transactionIdToLook">Transaction id to look.</param>
        /// <returns>the data.</returns>
        public static bool ValidateShopTransaction(string transactionId, string transactionIdToLook)
        {
            return CalculateOr(transactionId.Equals(transactionIdToLook), transactionId.GetShortShopTransaction().Equals(transactionIdToLook));
        }

        /// <summary>
        /// Gets the response from a http response.
        /// </summary>
        /// <param name="proccessPayments">the proccess payments.</param>
        /// <param name="transactionsIds">th transactions ids.</param>
        /// <returns>the data.</returns>+
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
            var payment = payments.FirstOrDefault(p => p.TransactionId == docNumDxp);
            payment ??= new PaymentsDto { ShippingCostAccepted = ServiceConstants.ShippingCostAccepted, DeliveryComments = string.Empty, DeliverySuggestedTime = string.Empty };
            return payment;
        }

        /// <summary>
        /// get payments from a list.
        /// </summary>
        /// <param name="deliveryAddresses">the deliveryAddresses.</param>
        /// <param name="cardcode">the card code to look up.</param>
        /// <param name="addreesName">the addres id to look up.</param>
        /// <returns>data.</returns>
        public static DoctorDeliveryAddressModel GetSpecificDeliveryAddress(this List<DoctorDeliveryAddressModel> deliveryAddresses, string cardcode, string addreesName)
        {
            var deliveryAddress = deliveryAddresses.FirstOrDefault(y => CalculateAnd(y.DoctorId == cardcode, y.AddressId == addreesName));
            deliveryAddress ??= new DoctorDeliveryAddressModel();
            return deliveryAddress;
        }

        /// <summary>
        /// get batch by dist number.
        /// </summary>
        /// <param name="batchName">the batch list.</param>
        /// <param name="distNumber">the dist number.</param>
        /// <returns>a batch.</returns>
        public static AlmacenBatchModel GetBatch(this List<AlmacenBatchModel> batchName, string distNumber)
        {
            return batchName.FirstOrDefault(a => a.BatchNumber == distNumber) ?? new AlmacenBatchModel() { BatchQty = 0 };
        }

        /// <summary>
        /// Gets the doctors precriptionData.
        /// </summary>
        /// <param name="doctorService">the doctor service.</param>
        /// <param name="cardCodes">the cardcodes.</param>
        /// <returns>the dta.</returns>
        public static async Task<List<DoctorPrescriptionInfoModel>> GetDoctors(IDoctorService doctorService, List<string> cardCodes)
        {
            var doctorsResponse = await doctorService.PostDoctors(cardCodes, ServiceConstants.GetResponsibleDoctors);
            return JsonConvert.DeserializeObject<List<DoctorPrescriptionInfoModel>>(doctorsResponse.Response.ToString());
        }

        /// <summary>
        /// Cast parameters to universal datetime.
        /// </summary>
        /// <param name="date">Date.</param>
        /// <returns>Cast Date.</returns>
        public static DateTime ToUniversalDateTime(this string date)
        {
            DateTime fechaLocal = DateTime.ParseExact(date, ServiceConstants.DateTimeFormatddMMyyyy, CultureInfo.InvariantCulture);
            return DateTime.SpecifyKind(fechaLocal, DateTimeKind.Local).ToUniversalTime();
        }

        /// <summary>
        /// Normalizes input string by removing accents, converting to uppercase.
        /// </summary>
        /// <param name="input">Input string to normalize.</param>
        /// <returns>Normalized string with only letters, numbers and hyphens in uppercase format.</returns>
        public static string NormalizeComplete(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            string normalized = new string(input.Normalize(NormalizationForm.FormD)
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .ToArray())
                .ToUpper();

            return Regex.Replace(normalized, @"[^A-Z0-9\-]+", string.Empty).Trim('-');
        }

        /// <summary>
        /// Normalizes input string by removing accents, converting to uppercase.
        /// </summary>
        /// <param name="theme">The theme.</param>
        /// <param name="themes">The themes list.</param>
        /// <returns>Normalized string with only letters, numbers and hyphens in uppercase format.</returns>
        public static ProductColorsDto GetSelectedTheme(string theme, List<ProductColorsDto> themes)
        {
            return themes.Where(x => NormalizeComplete(x.TemaId) == NormalizeComplete(theme)).FirstOrDefault() ?? new ProductColorsDto() { BackgroundColor = "#FBC115", LabelText = "Tipo de producto NA", TextColor = "#FFFFFF" };
        }
    }
}
