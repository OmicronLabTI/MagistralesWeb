// <summary>
// <copyright file="CommonFunctions.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// </summary>

namespace Omicron.OrderDivisionProcess.Batch.Utils
{
    public static class CommonFunctions
    {
        /// <summary>
        /// Gets the response from a http response.
        /// </summary>
        /// <param name="response">the response.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="error">the error.</param>
        /// <returns>the data.</returns>
        public static async Task<ResultDto> GetResponse(
            HttpResponseMessage response,
            ILogger logger,
            string error)
        {
            var jsonString = await response.Content.ReadAsStringAsync();

            if ((int)response.StatusCode >= 300)
            {
                logger.Error($"{error} {jsonString}");
                throw new Exception(error);
            }

            return JsonConvert.DeserializeObject<ResultDto>(jsonString);
        }

        /// <summary>
        /// Gets a list divided in sublists.
        /// </summary>
        /// <typeparam name="Tsource">the original list.</typeparam>
        /// <param name="listToSplit">the original list to split.</param>
        /// <param name="maxCount">the max count per group.</param>
        /// <returns>the list of list.</returns>
        public static List<List<Tsource>> GetGroupsOfList<Tsource>(List<Tsource> listToSplit, int maxCount)
        {
            var listToReturn = new List<List<Tsource>>();
            var offset = 0;
            while (offset < listToSplit.Count)
            {
                var sublist = new List<Tsource>();
                sublist.AddRange(listToSplit.Skip(offset).Take(maxCount).ToList());
                listToReturn.Add(sublist);
                offset += maxCount;
            }

            return listToReturn;
        }

        /// <summary>
        /// GenerateSkipTakeBatches.
        /// </summary>
        /// <param name="total">Total.</param>
        /// <param name="batchSize">Batch Size.</param>
        /// <returns>Batch Enumerable.</returns>
        public static IEnumerable<BatchRangeModel> GenerateSkipTakeBatches(int total, int batchSize)
        {
            return Enumerable
            .Range(0, (int)Math.Ceiling((double)total / batchSize))
            .Select(i => new BatchRangeModel
            {
                Offset = i * batchSize,
                Limit = Math.Min(batchSize, total - (i * batchSize)),
            });
        }
    }
}
