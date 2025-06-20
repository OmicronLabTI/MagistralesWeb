// <summary>
// <copyright file="ServiceUtils.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Catalogos.Services.Utils
{
    /// <summary>
    /// The static class for service utils.
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
        /// <returns>the resultModel.</returns>
        public static ResultModel CreateResult(bool success, int code, string userError, object responseObj, string exceptionMessage)
        {
            return new ResultModel
            {
                Success = success,
                Response = responseObj,
                UserError = userError,
                ExceptionMessage = exceptionMessage,
                Code = code,
            };
        }

        /// <summary>
        /// gets the distinc by.
        /// </summary>
        /// <typeparam name="Tsource">the list source.</typeparam>
        /// <typeparam name="TKey">the key to look.</typeparam>
        /// <param name="source">the sourec.</param>
        /// <param name="keyselector">the key.</param>
        /// <returns>the list distinc.</returns>
        public static IEnumerable<Tsource> ServiceDistinctBy<Tsource, TKey>(this IEnumerable<Tsource> source, Func<Tsource, TKey> keyselector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (Tsource element in source)
            {
                if (seenKeys.Add(keyselector(element)))
                {
                    yield return element;
                }
            }
        }

        /// <summary>
        /// Retrieves the content of a specific sheet from an Excel workbook as a DataTable.
        /// </summary>
        /// <param name="workbook"> The Excel workbook from which the sheet will be read. </param>
        /// <param name="sheetNumber"> The index (1-based) of the sheet to be retrieved. </param>
        /// <returns> A DataTable containing the content of the specified sheet. </returns>
        public static DataTable ReadSheet(XLWorkbook workbook, int sheetNumber)
        {
            var datatable = new DataTable();
            var firstrow = true;
            string readRange = "1:1";
            var worksheet = workbook.Worksheet(sheetNumber);

            foreach (var row in worksheet.RowsUsed())
            {
                if (firstrow)
                {
                    readRange = string.Format("{0}:{1}", 1, row.LastCellUsed().Address.ColumnNumber);
                    foreach (var cell in row.Cells(readRange))
                    {
                        datatable.Columns.Add(cell.Value.ToString());
                    }

                    firstrow = false;
                    continue;
                }

                datatable.Rows.Add();
                int cellIndex = 0;

                foreach (var cell in row.Cells(readRange))
                {
                    datatable.Rows[datatable.Rows.Count - 1][cellIndex] = cell.Value.ToString();
                    cellIndex++;
                }
            }

            return datatable;
        }

        /// <summary>
        /// Retrieves data from an HTTP response, logging any issues and handling errors during the process.
        /// </summary>
        /// <param name="response"> The HTTP response containing status, headers, and body data. </param>
        /// <param name="logger"> The logger used for capturing execution details. </param>
        /// <param name="error"> An output parameter for any errors encountered. </param>
        /// <returns> the data extracted from the response body. </returns>
        public static async Task<ResultDto> GetResponse(HttpResponseMessage response, ILogger logger, string error)
        {
            var jsonString = await response.Content.ReadAsStringAsync();

            if ((int)response.StatusCode >= 300)
            {
                logger.Information($"{error} {jsonString}");
                throw new CustomServiceException(jsonString, System.Net.HttpStatusCode.NotFound);
            }

            return JsonConvert.DeserializeObject<ResultDto>(await response.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// Deserilize redis info.
        /// </summary>
        /// <typeparam name="T">List.</typeparam>
        /// <param name="list">Default list.</param>
        /// <param name="redisKey">Redis key.</param>
        /// <param name="redisService">Redis service interface.</param>
        /// <returns>Deserialized object.</returns>
        public static async Task<List<T>> DeserializeRedisValue<T>(List<T> list, string redisKey, IRedisService redisService)
        {
            if (redisService.IsConnectedRedis())
            {
                var redisValues = await redisService.GetRedisKey(redisKey);
                return !string.IsNullOrEmpty(redisValues) ? JsonConvert.DeserializeObject<List<T>>(redisValues) : list;
            }

            return list;
        }

        /// <summary>
        /// Calculate string value.
        /// </summary>
        /// <typeparam name="T">The value.</typeparam>
        /// <param name="validation">Validation.</param>
        /// <param name="firstValue">First Value.</param>
        /// <param name="defaultValue">Default Value.</param>
        /// <returns>result.</returns>
        public static T CalculateTernary<T>(bool validation, T firstValue, T defaultValue)
        {
            return validation ? firstValue : defaultValue;
        }
    }
}
