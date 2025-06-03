// <summary>
// <copyright file="ServiceUtils.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Services.Utils
{
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

        /// <summary>
        /// Calculates the left and right with and AND.
        /// </summary>
        /// <param name="list">list of bools to evaluate.</param>
        /// <returns>the data.</returns>
        public static bool CalculateAnd(params bool[] list)
        {
            return list.All(element => element);
        }

        /// <summary>
        /// Serializes an object of type T with custom property mappings.
        /// </summary>
        /// <typeparam name="T">The type of object to serialize.</typeparam>
        /// <param name="propertyMappings">The property mappings.</param>
        /// <param name="objectToSerialize">The object to serialize.</param>
        /// <returns>The serialized JSON string.</returns>
        public static string SerializeWithCustomProperties<T>(Dictionary<string, string> propertyMappings, T objectToSerialize)
        {
            var converter = new CustomJsonConverter(new Dictionary<Type, Dictionary<string, string>> { { typeof(T), propertyMappings } });
            var settings = new JsonSerializerSettings { Converters = { converter } };
            return JsonConvert.SerializeObject(objectToSerialize, settings);
        }

        /// <summary>
        /// Deserializes a JSON string with custom property mappings into an object of type T.
        /// </summary>
        /// <typeparam name="T">The type of object to deserialize.</typeparam>
        /// <param name="propertyMappings">The property mappings.</param>
        /// <param name="json">The JSON string to deserialize.</param>
        /// <returns>The deserialized object of type T.</returns>
        public static T DeserializeWithCustomProperties<T>(Dictionary<string, string> propertyMappings, string json)
        {
            var converter = new CustomJsonConverter(new Dictionary<Type, Dictionary<string, string>> { { typeof(T), propertyMappings } });
            var settings = new JsonSerializerSettings { Converters = { converter } };
            return JsonConvert.DeserializeObject<T>(json, settings);
        }

        /// <summary>
        /// Gets the response from a http response.
        /// </summary>
        /// <param name="response">the response.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="serviceCalled">Service called name.</param>
        /// <param name="genericError">Generic Error.</param>
        /// <returns>the data.</returns>
        public static async Task<ResultModel> GetResponse(HttpResponseMessage response, ILogger logger, string serviceCalled, string genericError)
        {
            var jsonString = await response.Content.ReadAsStringAsync();

            if ((int)response.StatusCode >= 300)
            {
                logger.Error($"SapServiceLayerdapter-{serviceCalled}-{genericError}-{jsonString}");
                throw new CustomServiceException(jsonString, System.Net.HttpStatusCode.NotFound);
            }

            var result = JsonConvert.DeserializeObject<ResultModel>(await response.Content.ReadAsStringAsync());
            if (!result.Success)
            {
                logger.Error($"SapServiceLayerdapter-{serviceCalled}-{result.UserError}-{result.ExceptionMessage}-{jsonString}");
                throw new CustomServiceException(result.UserError, System.Net.HttpStatusCode.NotFound);
            }

            return result;
        }

        /// <summary>
        /// Calculate value from validation.
        /// </summary>
        /// <typeparam name="T">the type.</typeparam>
        /// <param name="validation">Validation.</param>
        /// <param name="value">True value.</param>
        /// <param name="defaultValue">False value.</param>
        /// <returns>Result.</returns>
        public static T CalculateTernary<T>(bool validation, T value, T defaultValue)
        {
            return validation ? value : defaultValue;
        }

        /// <summary>
        /// AddElementToDictionary.
        /// </summary>
        /// <param name="dictionary">Dictionary.</param>
        /// <param name="key">Key.</param>
        /// <param name="value">value.</param>
        public static void AddElementToDictionary(Dictionary<int, string> dictionary, int key, string value)
        {
            dictionary[key] = dictionary.TryGetValue(key, out string existingMessage)
                ? string.Format(ServiceConstants.DictionaryValueFormat, existingMessage, value)
                : value;
        }

        /// <summary>
        /// Validate if list is null or empty.
        /// </summary>
        /// <typeparam name="T">Type.</typeparam>
        /// <param name="source">Generic list source.</param>
        /// <returns>Validation Result.</returns>
        public static bool ListIsNullOrEmpty<T>(this IEnumerable<T> source)
        {
            return source == null || !source.Any();
        }

    }
}
