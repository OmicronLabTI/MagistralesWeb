// <summary>
// <copyright file="TestExtensions.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Test.Extensions
{
    /// <summary>
    /// Class TestExtensions.
    /// </summary>
    public static class TestExtensions
    {
        /// <summary>
        /// Mthod to load json.
        /// </summary>
        /// <typeparam name="T">TypeJson.</typeparam>
        /// <param name="fileNameJson">File name csv in setup.</param>
        /// <returns>Instance of <see cref="T"/>.</returns>
        public static T CreateInstanceFromJsonFile<T>(string fileNameJson)
        where T : class
        {
            var pathFile = GetPathFromFiletCsv(fileNameJson);
            using StreamReader r = new StreamReader(pathFile);
            string json = r.ReadToEnd();
            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// Method to set data from file csv.
        /// </summary>
        /// <typeparam name="TEntity">Type generic.</typeparam>
        /// <param name="dbset">DBSet class model.</param>
        /// <param name="fileNameCsv">File name csv in setup.</param>
        /// <exception cref="Exception">exception.</exception>
        public static void AddToFileCsv<TEntity>(this DbSet<TEntity> dbset, string fileNameCsv)
            where TEntity : class
        {
            var pathFile = GetPathFromFiletCsv(fileNameCsv);

            IEnumerable<string[]> values = File.ReadLines(pathFile)
                .Select(row => Regex.Split(row, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)"));
            string[] csvheaders = values.First();

            List<Dictionary<string, object>> dataBinding = values.Skip(1)
                .Select(s => BindToHeaders(s, csvheaders))
                .ToList();

            var properties = typeof(TEntity).GetProperties();

            var requiredHeaders = properties.Select(w => new
            {
                PropertyName = w.Name,
                ColumnName = w.GetCustomAttributes<ColumnAttribute>(false).SingleOrDefault()?.Name,
            }).Where(s => !string.IsNullOrEmpty(s.ColumnName))
                .ToDictionary(k => k.ColumnName, v => v.PropertyName);

            if (csvheaders.Length < requiredHeaders.Count)
            {
                throw new Exception($"requiredHeader count '{requiredHeaders.Count}' is bigger then csv header count '{csvheaders.Length}' ");
            }

            foreach (var requiredHeader in requiredHeaders)
            {
                if (!csvheaders.Contains(requiredHeader.Key))
                {
                    throw new Exception($"does not contain required header '{requiredHeader}'");
                }
            }

            var entityData = dataBinding.Select(row =>
            {
                var instance = Activator.CreateInstance<TEntity>();
                foreach (var header in requiredHeaders)
                {
                    object value = row.GetValueOrDefault(header.Key);
                    PropertyInfo property = properties.First(f => f.Name.Equals(header.Value));
                    object safeValue = ChangeTypeSafe(value, property);
                    property.SetValue(instance, safeValue, null);
                }

                return instance;
            });

            dbset.AddRange(entityData);
        }

        private static object ChangeTypeSafe(object value, PropertyInfo property)
        {
            Type typeNullable = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

            object safeValue =
                value == null ?
                value :
                typeNullable.ToString() switch
                {
                    "System.Boolean" => value.Equals("1"),
                    _ => Convert.ChangeType(value, typeNullable)
                };

            return safeValue;
        }

        /// <summary>
        /// Method to get path from filet .csv.
        /// </summary>
        /// <param name="fileName">File name.</param>
        /// <returns>Path from filet .csv.</returns>
        /// <exception cref="Exception">Exception.</exception>
        private static string GetPathFromFiletCsv(string fileName)
        {
            string rootDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.Replace("bin\\Debug\\net6.0", string.Empty));
            string csvFile = Path.Combine(rootDirectory, "Setup", $"{fileName}");

            if (!File.Exists(csvFile))
            {
                throw new Exception($"ERROR: FILE NOT FOUND {fileName}");
            }

            return csvFile;
        }

        /// <summary>
        /// Method to get values from file.
        /// </summary>
        /// <param name="rows">Path from file .csv.</param>
        /// <param name="headers">Headers.</param>
        /// <returns><see cref="List{string[]}"/>.</returns>
        private static Dictionary<string, object> BindToHeaders(string[] rows, string[] headers)
        {
            return rows.Select((data, index) => new
            {
                Header = headers[index],
                Value = data.Equals("NULL") ? null : (object)data,
            }).ToDictionary(k => k.Header, v => v.Value);
        }
    }
}
