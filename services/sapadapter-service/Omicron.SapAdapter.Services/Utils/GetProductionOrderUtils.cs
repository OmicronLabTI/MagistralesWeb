// <summary>
// <copyright file="GetProductionOrderUtils.cs" company="Axity">
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
    using System.Threading.Tasks;
    using Omicron.SapAdapter.DataAccess.DAO.Sap;
    using Omicron.SapAdapter.Entities.Model;
    using Omicron.SapAdapter.Services.Constants;

    /// <summary>
    /// class for the utils for the orders.
    /// </summary>
    public static class GetProductionOrderUtils
    {
        /// <summary>
        /// gets the orders from sap.
        /// </summary>
        /// <param name="parameters">the filter from front.</param>
        /// <param name="dateFilter">the date filter.</param>
        /// <param name="sapDao">The sap dao.</param>
        /// <returns>teh orders.</returns>
        public static async Task<List<OrdenFabricacionModel>> GetSapDbProdOrders(Dictionary<string, string> parameters, Dictionary<string, DateTime> dateFilter, ISapDao sapDao)
        {
            if (parameters.ContainsKey(ServiceConstants.DocNum))
            {
                int.TryParse(parameters[ServiceConstants.DocNum], out int docNum);
                return (await sapDao.GetFabOrderById(new List<int> { docNum })).ToList();
            }

            var filterDate = parameters.ContainsKey(ServiceConstants.FechaInicio);
            var listToReturn = new List<OrdenFabricacionModel>();

            if (filterDate)
            {
                listToReturn.AddRange((await sapDao.GetFabOrderByCreateDate(dateFilter[ServiceConstants.FechaInicio], dateFilter[ServiceConstants.FechaFin])).ToList());
            }

            if (parameters.ContainsKey(ServiceConstants.ItemCode))
            {
                var code = parameters[ServiceConstants.ItemCode];
                listToReturn = filterDate ? listToReturn.Where(x => x.ProductoId.Contains(code)).ToList() : (await sapDao.GetFabOrderByItemCode(code)).ToList();
            }

            return listToReturn.DistinctBy(x => x.OrdenId).ToList();
        }

        /// <summary>
        /// gets the orders from sap.
        /// </summary>
        /// <param name="parameters">the filter from front.</param>
        /// <param name="dateFilter">the date filter.</param>
        /// <param name="orders">The orders.</param>
        /// <returns>teh orders.</returns>
        public static List<OrdenFabricacionModel> GetSapLocalProdOrders(Dictionary<string, string> parameters, Dictionary<string, DateTime> dateFilter, List<OrdenFabricacionModel> orders)
        {
            if (parameters.ContainsKey(ServiceConstants.DocNum))
            {
                int.TryParse(parameters[ServiceConstants.DocNum], out int docNum);
                return orders.Where(x => x.OrdenId == docNum).ToList();
            }

            var filterDate = parameters.ContainsKey(ServiceConstants.FechaInicio);

            if (!filterDate && !parameters.ContainsKey(ServiceConstants.ItemCode))
            {
                return orders;
            }

            var listToReturn = new List<OrdenFabricacionModel>();

            if (filterDate)
            {
                listToReturn.AddRange(orders.Where(x => x.CreatedDate >= dateFilter[ServiceConstants.FechaInicio] && x.CreatedDate <= dateFilter[ServiceConstants.FechaFin]));
            }

            if (parameters.ContainsKey(ServiceConstants.ItemCode))
            {
                var code = parameters[ServiceConstants.ItemCode];
                listToReturn = filterDate ? listToReturn.Where(x => x.ProductoId.Contains(code)).ToList() : orders.Where(x => x.ProductoId.Contains(code)).ToList();
            }

            return listToReturn.DistinctBy(x => x.OrdenId).ToList();
        }
    }
}
