// <summary>
// <copyright file="ProcessPaymentService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Invoice.Services.ProcessPayment
{
    /// <summary>
    /// Class for process payments.
    /// </summary>
    public class ProcessPaymentService : IProcessPaymentService
    {
        /// <summary>
        /// Client Http.
        /// </summary>
        private readonly HttpClient httpClient;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessPaymentService" /> class.
        /// </summary>
        /// <param name="httpClient">Client Http.</param>
        /// <param name="logger">the logger.</param>
        public ProcessPaymentService(HttpClient httpClient, ILogger logger)
        {
            this.httpClient = httpClient;
            this.logger = logger;
        }

        /// <summary>
        /// get orders with the data.
        /// </summary>
        /// <param name="dataToSend">the orders.</param>
        /// <param name="route">the route.</param>
        /// <returns>the return.</returns>
        public async Task<ResultDto> PostProcessPayments(object dataToSend, string route)
        {
            ResultDto result;
            var stringContent = new StringContent(JsonConvert.SerializeObject(dataToSend), UnicodeEncoding.UTF8, "application/json");
            var url = this.httpClient.BaseAddress + route;
            using (var response = await this.httpClient.PostAsync(url, stringContent))
            {
                result = await ResponseUtils.GetResponse(response, this.logger, "Error peticion sapadapter");
            }

            return result;
        }

        /// <summary>
        /// Makes a get to favorites service.
        /// </summary>
        /// <param name="route">the route to send.</param>
        /// <returns>the data.</returns>
        public async Task<ResultDto> GetProcessPayments(string route)
        {
            ResultDto result;
            var url = this.httpClient.BaseAddress + route;

            using (var response = await this.httpClient.GetAsync(url))
            {
                result = await ResponseUtils.GetResponse(response, this.logger, "Error peticion process payments");
            }

            return ServiceUtils.CreateResult(result.Success, result.Code, result.UserError, result.Response, result.ExceptionMessage, result.Comments);
        }
    }
}
