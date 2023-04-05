// <summary>
// <copyright file="CommonCall.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Reporting.Services.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Omicron.Reporting.Resources.Exceptions;
    using Omicron.Reporting.Entities.Model;
    using Serilog;

    /// <summary>
    /// common call services.
    /// </summary>
    public static class CommonCall
    {
        /// <summary>
        /// Calculate value from validation.
        /// </summary>
        /// <typeparam name="T">The Type T.</typeparam>
        /// <param name="validation">Validation.</param>
        /// <param name="value">True value.</param>
        /// <param name="defaultValue">False value.</param>
        /// <returns>Result.</returns>
        public static T CalculateTernary<T>(bool validation, T value, T defaultValue)
        {
            return validation ? value : defaultValue;
        }

        /// <summary>
        /// Create Smtp Config Model.
        /// </summary>
        /// <param name="parameters">Parameters.</param>
        /// <returns>SmtpConfigModel.</returns>
        public static SmtpConfigModel CreateSmtpConfigModel(List<ParametersModel> parameters)
        {
            return new SmtpConfigModel
            {
                SmtpServer = parameters.FirstOrDefault(x => x.Field.Equals("SmtpServer")).Value,
                SmtpPort = int.Parse(parameters.FirstOrDefault(x => x.Field.Equals("SmtpPort")).Value),
                SmtpDefaultPassword = parameters.FirstOrDefault(x => x.Field.Equals("EmailMiddlewarePassword")).Value,
                SmtpDefaultUser = parameters.FirstOrDefault(x => x.Field.Equals("EmailMiddleware")).Value,
                EmailCCDelivery = parameters.FirstOrDefault(x => x.Field.Equals("EmailCCDelivery")).Value,
                EmailMiddlewareUser = parameters.FirstOrDefault(x => x.Field.Equals("EmailMiddlewareUser")).Value,
            };
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
    }
}
