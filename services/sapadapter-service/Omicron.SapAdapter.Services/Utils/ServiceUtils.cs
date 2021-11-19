// <summary>
// <copyright file="ServiceUtils.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Services.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Omicron.SapAdapter.DataAccess.DAO.Sap;
    using Omicron.SapAdapter.Entities.Model;
    using Omicron.SapAdapter.Entities.Model.AlmacenModels;
    using Omicron.SapAdapter.Entities.Model.JoinsModels;
    using Omicron.SapAdapter.Services.Catalog;
    using Omicron.SapAdapter.Services.Constants;
    using Omicron.SapAdapter.Services.Redis;

    /// <summary>
    /// The class for the services.
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
        public static ResultModel CreateResult(bool success, int code, string userError, object responseObj, string exceptionMessage, object comments)
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
        /// gets the date filter for sap.
        /// </summary>
        /// <param name="filter">the dictionary.</param>
        /// <returns>the datetime.</returns>
        public static Dictionary<string, DateTime> GetDateFilter(Dictionary<string, string> filter)
        {
            var dictToReturn = new Dictionary<string, DateTime>();

            if (filter.ContainsKey(ServiceConstants.FechaInicio))
            {
                return GetDictDates(filter[ServiceConstants.FechaInicio]);
            }

            if (filter.ContainsKey(ServiceConstants.FechaFin))
            {
                return GetDictDates(filter[ServiceConstants.FechaFin]);
            }

            return dictToReturn;
        }

        /// <summary>
        /// gets the distinc by.
        /// </summary>
        /// <typeparam name="Tsource">the list source.</typeparam>
        /// <typeparam name="TKey">the key to look.</typeparam>
        /// <param name="source">the sourec.</param>
        /// <param name="keyselector">the key.</param>
        /// <returns>the list distinc.</returns>
        public static IEnumerable<Tsource> DistinctBy<Tsource, TKey>(this IEnumerable<Tsource> source, Func<Tsource, TKey> keyselector)
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
        /// filters the list by the params.
        /// </summary>
        /// <param name="orderModels">the list of data.</param>
        /// <param name="parameters">the params.</param>
        /// <param name="userOrder">the usr orders.</param>
        /// <param name="users">the users.</param>
        /// <returns>the data.</returns>
        public static List<CompleteOrderModel> FilterList(List<CompleteOrderModel> orderModels, Dictionary<string, string> parameters, List<UserOrderModel> userOrder, List<UserModel> users)
        {
            var listToFilter = new List<CompleteOrderModel>();
            orderModels.GroupBy(x => x.DocNum).ToList().ForEach(p =>
            {
                var allPersonalized = p.All(d => d.Detalles != null && !string.IsNullOrEmpty(d.Detalles.Label) && d.Detalles.Label.ToLower() == ServiceConstants.Personalizado.ToLower());
                var allGeneric = p.All(d => d.Detalles != null && !string.IsNullOrEmpty(d.Detalles.Label) && d.Detalles.Label.ToLower() != ServiceConstants.Personalizado.ToLower());

                var typeLabel = allPersonalized ? ServiceConstants.PersonalizadoAbr : ServiceConstants.MixtoAbr;
                typeLabel = allGeneric ? ServiceConstants.GenericoAbr : typeLabel;

                var hasRecipe = p.FirstOrDefault() != null && p.FirstOrDefault().AtcEntry != null;
                var needRecipe = p.Any(d => !string.IsNullOrEmpty(d.Detalles.HasRecipe) && d.Detalles.HasRecipe.ToLower() == ServiceConstants.HasRecipe);

                var recipe = hasRecipe ? ServiceConstants.HasNeedsRecipe : ServiceConstants.DoesntHaveNeedRecipe;
                recipe = needRecipe ? recipe : ServiceConstants.NoNeedRecipe;

                var elementToSave = p.FirstOrDefault();
                elementToSave.LabelType = typeLabel;
                elementToSave.HasRecipte = recipe;

                var order = userOrder.FirstOrDefault(u => u.Salesorderid == elementToSave.DocNum.ToString() && string.IsNullOrEmpty(u.Productionorderid));
                elementToSave.Qfb = order == null ? string.Empty : order.Userid;
                elementToSave.QfbId = elementToSave.Qfb;

                if (elementToSave.PedidoStatus == ServiceConstants.AbiertoSap)
                {
                    elementToSave.PedidoStatus = ServiceConstants.Abierto;
                }

                elementToSave.PedidoStatus = order == null ? elementToSave.PedidoStatus : order.Status;
                elementToSave.FinishedLabel = order == null ? 0 : order.FinishedLabel;
                elementToSave.Detalles = null;
                elementToSave.FechaFin = order != null && order.CloseDate.HasValue ? order.CloseDate.Value.ToString("dd/MM/yyyy") : string.Empty;
                elementToSave.OrderType = !string.IsNullOrEmpty(elementToSave.PedidoMuestra) && elementToSave.PedidoMuestra == ServiceConstants.IsSampleOrder ? ServiceConstants.OrderTypeMU : elementToSave.OrderType;

                var user = users.FirstOrDefault(y => y.Id.Equals(elementToSave.Qfb));
                elementToSave.Qfb = user == null ? string.Empty : $"{user.FirstName} {user.LastName}";
                listToFilter.Add(elementToSave);
            });

            orderModels = listToFilter;

            if (parameters.ContainsKey(ServiceConstants.DocNum))
            {
                var valueSplit = parameters[ServiceConstants.DocNum].Split("-");

                int.TryParse(valueSplit[0], out int docNumInit);
                int.TryParse(valueSplit[1], out int docNumEnd);
                var ordersById = orderModels.Where(x => x.DocNum >= docNumInit && x.DocNum <= docNumEnd).ToList();

                if (!ordersById.Any())
                {
                    return new List<CompleteOrderModel>();
                }

                return ordersById;
            }

            if (parameters.ContainsKey(ServiceConstants.Status))
            {
                orderModels = orderModels.Where(x => x.PedidoStatus == parameters[ServiceConstants.Status]).ToList();
            }

            if (parameters.ContainsKey(ServiceConstants.Qfb))
            {
                orderModels = orderModels.Where(x => !string.IsNullOrEmpty(x.QfbId) && x.QfbId.Equals(parameters[ServiceConstants.Qfb])).ToList();
            }

            if (parameters.ContainsKey(ServiceConstants.Cliente))
            {
                List<string> clientNames = parameters[ServiceConstants.Cliente].Split(",").ToList();
                orderModels = orderModels.Where(x => !string.IsNullOrEmpty(x.Cliente) && clientNames.All(name => x.Cliente.ToLower().Contains(name.ToLower()))).ToList();
            }

            if (parameters.ContainsKey(ServiceConstants.Label))
            {
                orderModels = orderModels.Where(x => x.LabelType == ServiceConstants.MixtoAbr || x.LabelType == parameters[ServiceConstants.Label]).ToList();
            }

            if (parameters.ContainsKey(ServiceConstants.FinishedLabel))
            {
                orderModels = orderModels.Where(x => x.FinishedLabel.ToString() == parameters[ServiceConstants.FinishedLabel]).ToList();
            }

            if (parameters.ContainsKey(ServiceConstants.OrderType))
            {
                orderModels = orderModels.Where(x => x.OrderType == parameters[ServiceConstants.OrderType]).ToList();
            }

            return orderModels;
        }

        /// <summary>
        /// replaces the values for the chips if they have encode values.
        /// </summary>
        /// <param name="chipsValues">the chips.</param>
        /// <returns>the data.</returns>
        public static List<string> UndecodeSpecialCaracters(List<string> chipsValues)
        {
            var listToReturn = new List<string>();

            chipsValues.ForEach(chip =>
            {
                foreach (var key in ServiceConstants.DictUrlEncode)
                {
                    chip = chip.Contains(key.Key) ? chip.Replace(key.Key, key.Value) : chip;
                }

                listToReturn.Add(chip);
            });

            return listToReturn;
        }

        /// <summary>
        /// Prepares the dictionary to a key.
        /// </summary>
        /// <param name="dictToTransform">the dict.</param>
        /// <param name="prefix">the prefix.</param>
        /// <returns>the data.</returns>
        public static string PrepareKeyForRedisFromDic(Dictionary<string, string> dictToTransform, string prefix)
        {
            var keyToReturn = new StringBuilder();
            keyToReturn.Append($"{prefix}");
            foreach (var key in dictToTransform.Keys)
            {
                if (!ServiceConstants.KeysToIgnoreRedis.Contains(key))
                {
                    keyToReturn.Append($"{key.Trim()}-{dictToTransform[key]}-");
                }
            }

            return keyToReturn.ToString();
        }

        /// <summary>
        /// Gets the next id to look.
        /// </summary>
        /// <param name="dict">the dict.</param>
        /// <param name="listIds">the list ids.</param>
        /// <returns>the enxt id.</returns>
        public static int GetKeyToLook(Dictionary<string, string> dict, List<int> listIds)
        {
            var current = dict.ContainsKey(ServiceConstants.Current) ? dict[ServiceConstants.Current] : "0";
            var advance = dict.ContainsKey(ServiceConstants.Advance) ? dict[ServiceConstants.Advance] : "f";
            int.TryParse(current, out int currentInt);
            var index = listIds.IndexOf(currentInt);

            if (!listIds.Any())
            {
                return 0;
            }

            index = advance == "f" ? index + 1 : index - 1;
            index = index == -1 ? listIds.Count - 1 : index;
            index = index == listIds.Count ? 0 : index;
            return index;
        }

        /// <summary>
        /// Gets the local neigbors.
        /// </summary>
        /// <param name="catalogService">the catalog service.</param>
        /// <param name="redis">The redis conection.</param>
        /// <returns>the data.</returns>
        public static async Task<List<string>> GetLocalNeighbors(ICatalogsService catalogService, IRedisService redis)
        {
            if (!redis.IsConnectedRedis())
            {
                var localNeigBorsResponse = await catalogService.GetParams($"{ServiceConstants.GetParams}?{ServiceConstants.LocalNeighborhood}={ServiceConstants.LocalNeighborhood}");
                return JsonConvert.DeserializeObject<List<ParametersModel>>(localNeigBorsResponse.Response.ToString()).Select(x => x.Value).ToList();
            }

            var redisResponse = await redis.GetRedisKey(ServiceConstants.LocalNeighbors);
            var redisProducts = string.IsNullOrEmpty(redisResponse) ? new List<string>() : JsonConvert.DeserializeObject<List<string>>(redisResponse);

            if (redisProducts.Any())
            {
                return redisProducts;
            }

            var localNeigBorsResponses = await catalogService.GetParams($"{ServiceConstants.GetParams}?{ServiceConstants.LocalNeighborhood}={ServiceConstants.LocalNeighborhood}");
            var localNeigbors = JsonConvert.DeserializeObject<List<ParametersModel>>(localNeigBorsResponses.Response.ToString()).Select(x => x.Value).ToList();
            await redis.WriteToRedis(ServiceConstants.LocalNeighbors, JsonConvert.SerializeObject(localNeigbors), new TimeSpan(8, 0, 0));
            return localNeigbors;
        }

        /// <summary>
        /// Calculates if an address is local.
        /// </summary>
        /// <param name="state">the state.</param>
        /// <param name="neigborhood">the municipios.</param>
        /// <param name="address">the address to validta.</param>
        /// <returns>the desition.</returns>
        public static bool CalculateTypeLocal(string state, List<string> neigborhood, string address)
        {
            return address.ToLower().Contains(state.ToLower()) && neigborhood.Any(x => address.ToLower().Contains(x.ToLower()));
        }

        /// <summary>
        /// Get the line products.
        /// </summary>
        /// <param name="sapDao">the sap dao.</param>
        /// <param name="redis">tehr edis.</param>
        /// <returns>the line products.</returns>
        public static async Task<List<string>> GetLineProducts(ISapDao sapDao, IRedisService redis)
        {
            if (!redis.IsConnectedRedis())
            {
                return (await sapDao.GetAllLineProducts()).Select(x => x.ProductoId).ToList();
            }

            var redisResponse = await redis.GetRedisKey(ServiceConstants.AlmacenLineProducts);
            var redisProducts = string.IsNullOrEmpty(redisResponse) ? new List<string>() : JsonConvert.DeserializeObject<List<string>>(redisResponse);

            if (redisProducts.Any())
            {
                return redisProducts;
            }

            var sapProducts = (await sapDao.GetAllLineProducts()).Select(x => x.ProductoId).ToList();
            await redis.WriteToRedis(ServiceConstants.AlmacenLineProducts, JsonConvert.SerializeObject(sapProducts), new TimeSpan(8, 0, 0));
            return sapProducts;
        }

        /// <summary>
        /// gets the dictionary.
        /// </summary>
        /// <param name="dateRange">the date range.</param>
        /// <returns>the data.</returns>
        private static Dictionary<string, DateTime> GetDictDates(string dateRange)
        {
            var dictToReturn = new Dictionary<string, DateTime>();
            var dates = dateRange.Split("-");

            var dateInicioArray = GetDatesAsArray(dates[0]);
            var dateFinArray = GetDatesAsArray(dates[1]);

            var dateInicio = new DateTime(dateInicioArray[2], dateInicioArray[1], dateInicioArray[0]);
            var dateFin = new DateTime(dateFinArray[2], dateFinArray[1], dateFinArray[0]);
            dictToReturn.Add(ServiceConstants.FechaInicio, dateInicio);
            dictToReturn.Add(ServiceConstants.FechaFin, dateFin);
            return dictToReturn;
        }

        /// <summary>
        /// split the dates to int array.
        /// </summary>
        /// <param name="date">the date in string.</param>
        /// <returns>the dates.</returns>
        private static List<int> GetDatesAsArray(string date)
        {
            var dateArrayNum = new List<int>();
            var dateArray = date.Split("/");

            dateArray.ToList().ForEach(x =>
            {
                int.TryParse(x, out int result);
                dateArrayNum.Add(result);
            });

            return dateArrayNum;
        }
    }
}
