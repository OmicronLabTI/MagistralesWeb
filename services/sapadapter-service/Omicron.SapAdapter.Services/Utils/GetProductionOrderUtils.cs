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
    using Omicron.SapAdapter.Entities.Model.BusinessModels;
    using Omicron.SapAdapter.Entities.Model.DbModels;
    using Omicron.SapAdapter.Services.Constants;
    using Serilog;

    /// <summary>
    /// class for the utils for the orders.
    /// </summary>
    public class GetProductionOrderUtils : IGetProductionOrderUtils
    {
        private readonly ISapDao sapDao;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetProductionOrderUtils"/> class.
        /// </summary>
        /// <param name="sapDao">sap dao.</param>
        /// <param name="logger">The logger.</param>
        public GetProductionOrderUtils(ISapDao sapDao, ILogger logger)
        {
            this.sapDao = sapDao;
            this.logger = logger;
        }

        /// <summary>
        /// gets the orders from sap.
        /// </summary>
        /// <param name="parameters">the filter from front.</param>
        /// <param name="dateFilter">the date filter.</param>
        /// <returns>teh orders.</returns>
        public async Task<List<OrdenFabricacionModel>> GetSapDbProdOrders(Dictionary<string, string> parameters, Dictionary<string, DateTime> dateFilter)
        {
            if (parameters.ContainsKey(ServiceConstants.DocNum))
            {
                int.TryParse(parameters[ServiceConstants.DocNum], out int docNum);
                var orders = (await this.sapDao.GetFabOrderById(new List<int> { docNum })).ToList();
                return await this.CompleteOrder(orders);
            }

            var filterDate = parameters.ContainsKey(ServiceConstants.FechaInicio);
            var listToReturn = new List<OrdenFabricacionModel>();

            if (filterDate)
            {
                this.logger.Information("Busqueda por fecha.");
                listToReturn.AddRange((await this.sapDao.GetFabOrderByCreateDate(dateFilter[ServiceConstants.FechaInicio], dateFilter[ServiceConstants.FechaFin])).ToList());
            }

            if (parameters.ContainsKey(ServiceConstants.ItemCode))
            {
                var code = parameters[ServiceConstants.ItemCode].ToLower();
                listToReturn = filterDate ? listToReturn.Where(x => x.ProductoId.ToLower().Contains(code)).ToList() : (await this.sapDao.GetFabOrderByItemCode(code)).ToList();
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
        public List<OrdenFabricacionModel> GetSapLocalProdOrders(Dictionary<string, string> parameters, Dictionary<string, DateTime> dateFilter, List<OrdenFabricacionModel> orders)
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
                var code = parameters[ServiceConstants.ItemCode].ToLower();
                listToReturn = filterDate ? listToReturn.Where(x => x.ProductoId.ToLower().Contains(code)).ToList() : orders.Where(x => x.ProductoId.ToLower().Contains(code)).ToList();
            }

            return listToReturn.DistinctBy(x => x.OrdenId).ToList();
        }

        /// <summary>
        /// Completes the order with the large description.
        /// </summary>
        /// <param name="listOrders">the orders.</param>
        /// <returns>the data.</returns>
        public async Task<List<OrdenFabricacionModel>> CompleteOrder(List<OrdenFabricacionModel> listOrders)
        {
            foreach (var order in listOrders)
            {
                var item = (await this.sapDao.GetProductById(order.ProductoId)).FirstOrDefault();
                order.ProdName = item == null ? order.ProdName : item.LargeDescription;
                order.HasMissingStock = (await this.sapDao.GetDetalleFormula(order.OrdenId)).Any(y => y.Stock == 0);
            }

            return listOrders;
        }

        /// <summary>
        /// Gets if the batches are complete.
        /// </summary>
        /// <param name="ordersId">the orders.</param>
        /// <returns>the data.</returns>
        public async Task<List<string>> GetIncompleteProducts(List<int> ordersId)
        {
            var listToReturn = new List<string>();
            var listTransaction = new List<int>();
            var componentes = (await this.sapDao.GetComponentByBatches(ordersId)).ToList();
            var assignedBatches = (await this.sapDao.GetBatchesTransactionByOrderItem(ordersId)).ToList();

            var componentIds = componentes.Select(x => $"{x.OrderFabId}-{x.ProductId}").ToList();
            assignedBatches.GroupBy(x => new { x.DocNum, x.ItemCode }).ToList().ForEach(x =>
            {
                if (componentIds.Contains($"{x.Key.DocNum}-{x.Key.ItemCode}"))
                {
                    var lastTransaction = x.Any() ? x.OrderBy(y => y.LogEntry).Last(z => z.DocQuantity > 0) : null;

                    if (lastTransaction != null)
                    {
                        listTransaction.Add(lastTransaction.LogEntry);
                    }
                }
            });

            var batchesQty = (await this.sapDao.GetBatchTransationsQtyByLogEntry(listTransaction)).ToList();

            assignedBatches.GroupBy(x => x.DocNum).ToList().ForEach(g =>
            {
                componentes.Where(x => x.OrderFabId == g.Key).ToList().ForEach(i =>
                {
                    var transactions = g.Where(item => item.ItemCode == i.ProductId).ToList();
                    var lastTransaction = transactions.Any() ? transactions.OrderBy(x => x.LogEntry).Last(y => y.DocQuantity > 0) : null;

                    if (lastTransaction == null)
                    {
                        listToReturn.Add($"{g.Key}-{i.ProductId}");
                    }
                    else
                    {
                        var logEntry = lastTransaction.LogEntry;
                        var batches = batchesQty.Where(b => b.ItemCode == i.ProductId && logEntry == b.LogEntry).ToList();
                        var cantidadSeleccionada = batches.Sum(b => b.AllocQty);
                        var totalNecesario = Math.Round(i.PendingQuantity - cantidadSeleccionada, 6);

                        if (totalNecesario > 0)
                        {
                            listToReturn.Add($"{g.Key}-{i.ProductId}");
                        }
                    }
                });
            });

            return listToReturn;
        }
    }
}
