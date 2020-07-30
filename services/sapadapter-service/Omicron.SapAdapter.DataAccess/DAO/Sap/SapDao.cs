// <summary>
// <copyright file="SapDao.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.DataAccess.DAO.Sap
{
    using Microsoft.EntityFrameworkCore;
    using Omicron.SapAdapter.Entities.Context;
    using Omicron.SapAdapter.Entities.Model;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Class for the dao.
    /// </summary>
    public class SapDao : ISapDao
    {
        private readonly IDatabaseContext databaseContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="SapDao"/> class.
        /// </summary>
        /// <param name="databaseContext">the context.</param>
        public SapDao(IDatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext ?? throw new ArgumentNullException(nameof(databaseContext));
        }

        /// <summary>
        /// get orders.
        /// </summary>
        /// <returns>the orders.</returns>
        public async Task<IEnumerable<CompleteOrder>> GetAllOrders(DateTime date)
        {
            
                var query = await (from order in this.databaseContext.OrderModel
                                   join detalle in this.databaseContext.DetallePedido on order.PedidoId equals detalle.PedidoId
                                   join producto in this.databaseContext.ProductoModel on detalle.ProductoId equals producto.ProductoId
                                   join asesor in this.databaseContext.AsesorModel on order.AsesorId equals asesor.AsesorId
                                   where order.FechaInicio >= date && producto.IsMagistral == "N"
                                   select new CompleteOrder
                                   {
                                       DocNum = order.DocNum,
                                       Cliente = order.Cliente,
                                       Codigo = order.Codigo,
                                       Medico = order.Medico,
                                       AsesorName = asesor.AsesorName,
                                       FechaInicio = order.FechaInicio,
                                       FechaFin = order.FechaFin,
                                       PedidoStatus = order.PedidoStatus,
                                       IsChecked = false
                                   }).ToListAsync();

                return query;
        }
    }
}
