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
        /// <param name="orders">The orders.</param>
        /// <returns>teh orders.</returns>
        public async Task<List<OrdenFabricacionModel>> GetSapLocalProdOrders(Dictionary<string, string> parameters, List<OrdenFabricacionModel> orders)
        {
            if (parameters.ContainsKey(ServiceConstants.DocNum))
            {
                var valueSplit = parameters[ServiceConstants.DocNum].Split("-");
                int.TryParse(valueSplit[0], out int docNumInit);
                int.TryParse(valueSplit[1], out int docNumEnd);
                docNumEnd += 1;
                var listIds = Enumerable.Range(docNumInit, docNumEnd - docNumInit).ToList();
                return (await this.sapDao.GetFabOrderById(listIds)).ToList();
            }

            if (parameters.ContainsKey(ServiceConstants.ItemCode))
            {
                var code = parameters[ServiceConstants.ItemCode].ToLower();
                orders = orders.Where(x => x.ProductoId.ToLower().Contains(code)).ToList();
            }

            return orders.DistinctBy(x => x.OrdenId).ToList();
        }

        /// <summary>
        /// Completes the order with the large description.
        /// </summary>
        /// <param name="listOrders">the orders.</param>
        /// <returns>the data.</returns>
        public async Task<List<OrdenFabricacionModel>> CompleteOrder(List<OrdenFabricacionModel> listOrders)
        {
            var items = (await this.sapDao.GetProductByIds(listOrders.Select(x => x.ProductoId).ToList())).ToList();
            var details = (await this.sapDao.GetDetalleFormula(listOrders.Select(x => x.OrdenId).ToList())).ToList();
            foreach (var order in listOrders)
            {
                var item = items.FirstOrDefault(x => order.ProductoId == x.ProductoId);
                order.ProdName = item == null ? order.ProdName : item.LargeDescription;

                var localDetails = details.Where(x => x.OrderFabId == order.OrdenId).ToList();
                order.HasMissingStock = localDetails.Any(y => y.Stock == 0);
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
                        listToReturn.Add($"{g.Key} {i.ProductId}");
                    }
                    else
                    {
                        var logEntry = lastTransaction.LogEntry;
                        var batches = batchesQty.Where(b => b.ItemCode == i.ProductId && logEntry == b.LogEntry).ToList();
                        var cantidadSeleccionada = batches.Sum(b => b.AllocQty);
                        var totalNecesario = Math.Round(i.PendingQuantity - cantidadSeleccionada, 6);

                        if (totalNecesario > 0)
                        {
                            listToReturn.Add($"{g.Key} {i.ProductId}");
                        }
                    }
                });
            });

            return listToReturn;
        }
    }
}
