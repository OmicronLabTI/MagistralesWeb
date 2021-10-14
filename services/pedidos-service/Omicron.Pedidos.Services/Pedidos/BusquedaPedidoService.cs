// <summary>
// <copyright file="BusquedaPedidoService.cs" company="Axity">
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
    using System.Threading.Tasks;
    using Omicron.Pedidos.DataAccess.DAO.Pedidos;
    using Omicron.Pedidos.Entities.Model;
    using Omicron.Pedidos.Services.Constants;
    using Omicron.Pedidos.Services.Utils;

    /// <summary>
    /// class for look pedidos.
    /// </summary>
    public class BusquedaPedidoService : IBusquedaPedidoService
    {
        private readonly IPedidosDao pedidosDao;

        /// <summary>
        /// Initializes a new instance of the <see cref="BusquedaPedidoService"/> class.
        /// </summary>
        /// <param name="pedidosDao">pedidos dao.</param>
        public BusquedaPedidoService(IPedidosDao pedidosDao)
        {
            this.pedidosDao = pedidosDao ?? throw new ArgumentNullException(nameof(pedidosDao));
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetOrders(Dictionary<string, string> parameters)
        {
            var dates = ServiceUtils.GetDateFilter(parameters);
            var listOrders = new List<UserOrderModel>();
            if (parameters.ContainsKey(ServiceConstants.FechaFin))
            {
                listOrders = (await this.pedidosDao.GetUserOrderByFechaClose(dates[ServiceConstants.FechaInicio], dates[ServiceConstants.FechaFin])).ToList();
            }

            return ServiceUtils.CreateResult(true, 200, null, listOrders, null, null);
        }
    }
}
