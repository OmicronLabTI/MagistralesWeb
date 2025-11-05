// <summary>
// <copyright file="ResponseUtils.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Almacen.Services.Utils
{
    /// <summary>
    /// Utils for the response.
    /// </summary>
    public static class ResponseUtils
    {
        /// <summary>
        /// Gets the CFDi version from Redis cache or database.
        /// </summary>
        /// <param name="redisService">The redis service.</param>
        /// <param name="catalogService">The catalog service.</param>
        /// <returns>The CFDi version string.</returns>
        public static async Task<string> GetCfdiVersion(IRedisService redisService, ICatalogsService catalogService)
        {
                var cdfiVersion = await redisService.GetRedisKey(ServiceConstants.CdfiKey);

                if (!string.IsNullOrEmpty(cdfiVersion))
                {
                    return cdfiVersion;
                }

                var parameters = await GetParams(new List<string> { ServiceConstants.CfdiVersion }, catalogService);
                var cfdiParameter = parameters?.FirstOrDefault();
                if (cfdiParameter == null || string.IsNullOrEmpty(cfdiParameter.Value))
                {
                    return ServiceConstants.DefaultVersion;
                }

                await redisService.WriteToRedis(ServiceConstants.CdfiKey, cfdiParameter.Value, TimeSpan.FromHours(16));

                return cfdiParameter.Value;
        }

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
        /// Gets the response from a http response.
        /// </summary>
        /// <param name="response">the response.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="error">the error.</param>
        /// <returns>the data.</returns>
        public static async Task<ResultDto> GetResponse(HttpResponseMessage response, Serilog.ILogger logger, string error)
        {
            var jsonString = await response.Content.ReadAsStringAsync();

            if ((int)response.StatusCode >= 300)
            {
                logger.Error($"{error} {jsonString}");
                throw new CustomServiceException(error, System.Net.HttpStatusCode.NotFound);
            }

            return JsonConvert.DeserializeObject<ResultDto>(jsonString);
        }

        /// <summary>
        /// Get Params from catalog.
        /// </summary>
        /// <param name="parameterNames">the parameters.</param>
        /// <param name="catalogService">the service.</param>
        /// <returns>the data.</returns>
        public static async Task<List<ParametersDto>> GetParams(List<string> parameterNames, ICatalogsService catalogService)
        {
            var query = $"{string.Join("=v&", parameterNames)}=v";
            var route = $"{ServiceConstants.GetParams}?{query}";
            var resultModel = await catalogService.GetParams(route);
            return JsonConvert.DeserializeObject<List<ParametersDto>>(resultModel.Response.ToString());
        }
    }
}
