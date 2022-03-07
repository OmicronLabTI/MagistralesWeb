// <summary>
// <copyright file="DoctorService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Services.Doctors
{
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Omicron.SapAdapter.Dtos.Models;
    using Omicron.SapAdapter.Services.Utils;
    using Serilog;

    /// <summary>
    /// clase de ProccessPayments Service.
    /// </summary>
    public class DoctorService : IDoctorService
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
        /// Initializes a new instance of the <see cref="DoctorService" /> class.
        /// </summary>
        /// <param name="httpClient">Client Http.</param>
        /// <param name="logger">the logger.</param>
        public DoctorService(HttpClient httpClient, ILogger logger)
        {
            this.httpClient = httpClient;
            this.logger = logger;
        }

        /// <inheritdoc/>
        public async Task<ResultDto> PostDoctors(object objectToSend, string route)
        {
            ResultDto result;
            var stringContent = new StringContent(JsonConvert.SerializeObject(objectToSend), UnicodeEncoding.UTF8, "application/json");

            var url = $"{this.httpClient.BaseAddress}{route}";
            using (var response = await this.httpClient.PostAsync(url, stringContent))
            {
                result = await ServiceUtils.GetResponse(response, this.logger, "Error peticion proccess payments");
            }

            return result;
        }
    }
}
