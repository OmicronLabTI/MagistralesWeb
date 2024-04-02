// <summary>
// <copyright file="ServiceLayerAuth.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

using System.Linq;
using Azure;

namespace Omicron.SapServiceLayerAdapter.Services.ServiceLayer.Impl
{
    /// <summary>
    /// Class for providing authentication-related functionality to service layer clients.
    /// </summary>
    public class ServiceLayerAuth : IServiceLayerAuth
    {
        private readonly HttpClient httpClient;
        private readonly IConfiguration configuration;
        private readonly ILogger logger;
        private string cookiesString;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceLayerAuth"/> class.
        /// </summary>
        /// <param name="httpClient">The httpClient.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="logger">Te logger.</param>
        public ServiceLayerAuth(HttpClient httpClient, IConfiguration configuration, ILogger logger)
        {
            this.httpClient = httpClient;
            this.logger = logger;
            this.configuration = configuration;
        }

        /// <inheritdoc/>
        public async Task<string> GetSessionIdAsync()
        {
            if (string.IsNullOrEmpty(this.cookiesString))
            {
                await this.Login();
            }

            return this.cookiesString;
        }

        /// <inheritdoc/>
        public async Task<string> RefreshSession()
        {
            await this.Login();
            return this.cookiesString;
        }

        private async Task Login()
        {
            this.cookiesString = string.Empty;
            string authUrl = "Login";
            var authData = new
            {
                UserName = this.configuration[ServiceConstants.SAPServiceLayerUserEnvName],
                Password = this.configuration[ServiceConstants.SAPServiceLayerPwEnvName],
                CompanyDB = this.configuration[ServiceConstants.SAPServiceLayerDatabaseName],
                Language = 23,
            };

            var authJson = System.Text.Json.JsonSerializer.Serialize(authData);
            using (var authContent = new StringContent(authJson, Encoding.UTF8, "application/json"))
            {
                using (var authResponse = await this.httpClient.PostAsync(authUrl, authContent))
                {
                    if (authResponse.IsSuccessStatusCode)
                    {
                        var cookies = authResponse.Headers.GetValues("Set-Cookie");
                        this.cookiesString = string.Join("; ", cookies.ToList());
                    }
                    else
                    {
                        this.logger.Error($"Omicron.SapServiceLayerAdapter Service. POST - Service Layer Auth {authResponse.Content.ToString()}, StatusCode: {authResponse.StatusCode}");
                        throw new CustomServiceException($"Service Layer Auth Error: {authResponse.StatusCode}", authResponse.StatusCode);
                    }
                }
            }
        }
    }
}
