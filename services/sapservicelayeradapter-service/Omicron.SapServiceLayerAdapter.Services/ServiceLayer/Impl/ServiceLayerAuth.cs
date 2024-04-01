// <summary>
// <copyright file="ServiceLayerAuth.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

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
        private string authToken;

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
            if (string.IsNullOrEmpty(this.authToken))
            {
                await this.Login();
            }

            return this.authToken;
        }

        /// <inheritdoc/>
        public async Task<string> RefreshSession()
        {
            await this.Login();
            return this.authToken;
        }

        private async Task Login()
        {
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
                        var authResponseContent = await authResponse.Content.ReadAsStringAsync();
                        var authResponseObj = System.Text.Json.JsonSerializer.Deserialize<ServiceLayerAuthResponseDto>(authResponseContent);
                        this.authToken = authResponseObj.SessionId;
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
