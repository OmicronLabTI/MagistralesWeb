// <summary>
// <copyright file="ServiceUtils.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Invoice.Services.Utils
{
    /// <summary>
    /// The class for the Service Utils.
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
        /// <param name="comments">the comments.</param>
        /// <returns>the resultModel.</returns>
        public static ResultDto CreateResult(bool success, int code, string userError, object responseObj, string exceptionMessage, object comments)
        {
            return new ResultDto
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
        /// <typeparam name="T">the generic object.</typeparam>
        /// <param name="obj">the object.</param>
        /// <param name="name">name of param.</param>
        /// <returns>ca.</returns>
        public static T ThrowIfNull<T>(this T obj, string name)
        {
            return obj ?? throw new ArgumentNullException(name);
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
        /// Normalizes input string by removing accents, converting to uppercase.
        /// </summary>
        /// <param name="input">Input string to normalize.</param>
        /// <returns>Normalized string with only letters, numbers and hyphens in uppercase format.</returns>
        public static string NormalizeComplete(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            string normalized = new string(input.Normalize(NormalizationForm.FormD)
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .ToArray())
                .ToUpper();

            return Regex.Replace(normalized, @"[^A-Z0-9\-]+", string.Empty).Trim('-');
        }

        /// <summary>
        /// creates the result.
        /// </summary>
        /// <param name="dic">the dictioanry.</param>
        /// <param name="key">the key to search.</param>
        /// <param name="defaultValue">default value.</param>
        /// <returns>the resultModel.</returns>
        public static string GetDictionaryValueString(Dictionary<string, string> dic, string key, string defaultValue)
        {
            return dic.ContainsKey(key) ? dic[key] : defaultValue;
        }

        /// <summary>
        /// GetDateRangeFromParameters.
        /// </summary>
        /// <param name="parameters">parameters.</param>
        /// <returns>dates.</returns>
        public static (DateTime fechaInicio, DateTime fechaFin) GetDateRangeFromParameters(Dictionary<string, string> parameters)
        {
            parameters.TryGetValue(ServiceConstants.Date, out string dateValue);
            var parts = dateValue.Split('-', StringSplitOptions.RemoveEmptyEntries);
            var fechaInicio = DateTime.ParseExact(parts[0].Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture).Date;
            var fechaFin = DateTime.ParseExact(parts[1].Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture).Date.AddDays(1).AddMilliseconds(-1);
            return (fechaInicio, fechaFin);
        }

        /// <summary>
        /// GetDateRangeFromParameters.
        /// </summary>
        /// <param name="value">value.</param>
        /// <returns>dates.</returns>
        public static List<string> SplitStringList(string value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? new List<string>()
                : value.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => s.Trim())
                        .ToList();
        }

        /// <summary>
        /// GetDateRangeFromParameters.
        /// </summary>
        /// <param name="value">value.</param>
        /// <returns>dates.</returns>
        public static List<int> SplitIntList(string value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? new List<int>()
                : value.Split(',', StringSplitOptions.RemoveEmptyEntries)
                       .Select(s => int.Parse(s.Trim()))
                       .ToList();
        }

    }
}
