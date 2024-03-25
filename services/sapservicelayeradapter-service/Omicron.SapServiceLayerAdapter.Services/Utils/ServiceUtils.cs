// <summary>
// <copyright file="ServiceUtils.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Services.Utils
{
    using Newtonsoft.Json;
    using Omicron.SapServiceLayerAdapter.Common.DTOs.Responses;
    using Omicron.SapServiceLayerAdapter.Model;
    using Serilog;

    /// <summary>
    /// Response utils.
    /// </summary>
    public static class ServiceUtils
    {
        /// <summary>
        /// creates the result.
        /// </summary>
        /// <param name="success">if it was successful.</param>
        /// <param name="code">the code.</param>
        /// <param name="userError">the user error.</param>
        /// <param name="responseObj">the responseobj.</param>
        /// <param name="exceptionMessage">the exception message.</param>
        /// <param name="comments">The comments.</param>
        /// <returns>the resultModel.</returns>
        public static ResultModel CreateResult(bool success, int code, string userError, object responseObj, string exceptionMessage, string comments = null)
        {
            return new ResultModel
            {
                Success = success,
                Response = responseObj,
                UserError = userError,
                ExceptionMessage = exceptionMessage,
                Code = code,
                Comments = comments,
            };
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
        ///  Gets the service layer response.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="requestBody">The request body.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public static async Task<ResultModel> GetServiceLayerResponse(HttpResponseMessage response, ILogger logger, string requestBody = "")
        {
            ResultModel result;
            var jsonString = await response.Content.ReadAsStringAsync();

            var message = $"Omicron.ServiceLayerAdapter Service - {response.RequestMessage.Method} - Service Layer Client" +
              $" - {response.RequestMessage.RequestUri.ToString()}, status: {response.StatusCode}," +
              $" result: {jsonString}, body: {requestBody}";
            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = JsonConvert.DeserializeObject<ServiceLayerErrorResponseDto>(jsonString);
                logger.Error(message);
                return CreateResult(response.IsSuccessStatusCode, (int)response.StatusCode, errorMessage.Error.Message.Value, errorMessage, string.Empty);
            }

            result = new ResultModel
            {
                Code = (int)response.StatusCode,
                Success = response.IsSuccessStatusCode,
                Response = jsonString,
            };
            logger.Information(message);
            return result;
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
        /// Method to cast value to double.
        /// </summary>
        /// <param name="valueToCast">Value to cast.</param>
        /// <param name="defaultValue">Default Value.</param>
        /// <returns>Double value.</returns>
        public static double ToParseDouble(this string valueToCast, double defaultValue)
        {
            if (string.IsNullOrEmpty(valueToCast))
            {
                return defaultValue;
            }

            return int.TryParse(valueToCast, out int result) ? result : defaultValue;
        }
    }
}
