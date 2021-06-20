// <summary>
// <copyright file="PedidosDxpService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Services.Pedidos
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;
    using Omicron.LeadToCash.Resources.Exceptions;
    using Omicron.Pedidos.DataAccess.DAO.Pedidos;
    using Omicron.Pedidos.Entities.Model;
    using Omicron.Pedidos.Resources.Enums;
    using Omicron.Pedidos.Resources.Extensions;
    using Omicron.Pedidos.Services.Broker;
    using Omicron.Pedidos.Services.Constants;
    using Omicron.Pedidos.Services.Redis;
    using Omicron.Pedidos.Services.Reporting;
    using Omicron.Pedidos.Services.SapAdapter;
    using Omicron.Pedidos.Services.SapDiApi;
    using Omicron.Pedidos.Services.SapFile;
    using Omicron.Pedidos.Services.User;
    using Omicron.Pedidos.Services.Utils;

    /// <summary>
    /// the pedidos service.
    /// </summary>
    public class PedidosDxpService : IPedidosDxpService
    {
        private readonly IPedidosDao pedidosDao;

        /// <summary>
        /// Initializes a new instance of the <see cref="PedidosDxpService"/> class.
        /// </summary>
        /// <param name="pedidosDao">pedidos dao.</param>
        public PedidosDxpService(IPedidosDao pedidosDao)
        {
            this.pedidosDao = pedidosDao ?? throw new ArgumentNullException(nameof(pedidosDao));
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetOrdersActive(List<int> ordersid)
        {
            var listIds = ordersid.Select(x => x.ToString()).ToList();
            var orders = (await this.pedidosDao.GetUserOrderBySaleOrder(listIds)).ToList();
            var listToReturn = orders.Select(x => new
            {
                id = x.Id,
                saleOrderId = x.Salesorderid,
                status = x.Status,
                statusAlmacen = x.StatusAlmacen,
                statusInvoice = x.StatusInvoice,
                productionOrder = x.Productionorderid,
                isOrder = x.IsSalesOrder,
                invoiceStoreDate = x.InvoiceStoreDate,
                invoiceId = x.InvoiceId,
            }).ToList();
            return ServiceUtils.CreateResult(true, 200, null, listToReturn, null, null);
        }
    }
}
