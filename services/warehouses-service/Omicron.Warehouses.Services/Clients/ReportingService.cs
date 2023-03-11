// <summary>
// <copyright file="ReportingService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Warehouses.Services.Clients
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Omicron.Warehouses.Entities.Model;
    using Omicron.Warehouses.Services.Constants;

    /// <summary>
    /// Class reporting service.
    /// </summary>
    public class ReportingService : BaseClientService, IReportingService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReportingService" /> class.
        /// </summary>
        /// <param name="httpClient">Client Http.</param>
        public ReportingService(HttpClient httpClient)
            : base(httpClient)
        {
        }

        /// <summary>
        /// Method for submit request.
        /// </summary>
        /// <param name="request">Request to submit.</param>
        /// <param name="numberOfAttemps">Numbers of attemps.</param>
        /// <returns>User list.</returns>
        public async Task<(bool, string)> SubmitRequest(RawMaterialRequestModel request, int numberOfAttemps = 3)
        {
            var result = false;
            var message = string.Empty;
            for (int attempNumber = 0; attempNumber < numberOfAttemps; attempNumber++)
            {
                try
                {
                    var resultModel = await this.PostAsync(request, EndPointConstants.SubmitRawMaterialRequest);
                    result = bool.Parse(resultModel.Response.ToString());
                    message = resultModel.UserError;
                }
                catch (Exception)
                {
                    result = false;
                }

                if (result)
                {
                    break;
                }
            }

            return (result, message);
        }
    }
}
