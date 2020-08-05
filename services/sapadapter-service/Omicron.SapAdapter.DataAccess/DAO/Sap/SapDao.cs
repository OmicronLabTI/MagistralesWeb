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
        public async Task<IEnumerable<CompleteOrderModel>> GetAllOrders(DateTime initDate, DateTime endDate)
        {
            
                var query = await (from order in this.databaseContext.OrderModel
                                   join detalle in this.databaseContext.DetallePedido on order.PedidoId equals detalle.PedidoId
                                   join producto in this.databaseContext.ProductoModel on detalle.ProductoId equals producto.ProductoId
                                   join asesor in this.databaseContext.AsesorModel on order.AsesorId equals asesor.AsesorId
                                   where order.FechaInicio >= initDate && order.FechaFin <= endDate && producto.IsMagistral == "Y"
                                   select new CompleteOrderModel
                                   {
                                       DocNum = order.DocNum,
                                       Cliente = order.Cliente,
                                       Codigo = order.Codigo,
                                       Medico = order.Medico,
                                       AsesorName = asesor.AsesorName,
                                       FechaInicio = order.FechaInicio.ToString("dd/MM/yyyy"),
                                       FechaFin = order.FechaFin.ToString("dd/MM/yyyy"),
                                       PedidoStatus = order.PedidoStatus,
                                       IsChecked = false
                                   }).ToListAsync();

                return query;
        }

        /// <summary>
        /// Get the orders.
        /// </summary>
        /// <returns>get the orders.</returns>
        public async Task<IEnumerable<CompleteOrderModel>> GetAllOrdersById(int id)
        {
            var query = await(from order in this.databaseContext.OrderModel
                              join detalle in this.databaseContext.DetallePedido on order.PedidoId equals detalle.PedidoId
                              join producto in this.databaseContext.ProductoModel on detalle.ProductoId equals producto.ProductoId
                              join asesor in this.databaseContext.AsesorModel on order.AsesorId equals asesor.AsesorId
                              where order.PedidoId == id && producto.IsMagistral == "Y"
                              select new CompleteOrderModel
                              {
                                  DocNum = order.DocNum,
                                  Cliente = order.Cliente,
                                  Codigo = order.Codigo,
                                  Medico = order.Medico,
                                  AsesorName = asesor.AsesorName,
                                  FechaInicio = order.FechaInicio.ToString("dd/MM/yyyy"),
                                  FechaFin = order.FechaFin.ToString("dd/MM/yyyy"),
                                  PedidoStatus = order.PedidoStatus,
                                  IsChecked = false,
                              }).ToListAsync();

            return query;
        }    

        /// <summary>
        /// gets the details.
        /// </summary>
        /// <param name="pedidoId">Pedido id.</param>
        /// <returns>the details.</returns>
        public async Task<IEnumerable<CompleteDetailOrderModel>> GetAllDetails(int pedidoId)
        {
            var query = await (from d in this.databaseContext.DetallePedido
                         join o in this.databaseContext.OrdenFabricacionModel on
                         new
                         {
                             Pedido = d.PedidoId, ItemCode = d.ProductoId
                         }
                         equals
                         new
                         {
                             Pedido = o.PedidoId, ItemCode = o.ProductoId
                         }
                         into DetallePedido                         
                         from dp in DetallePedido.DefaultIfEmpty()
                         join p in this.databaseContext.ProductoModel on d.ProductoId equals p.ProductoId
                         where d.PedidoId == pedidoId && p.IsMagistral == "Y"
                               select new CompleteDetailOrderModel
                         {
                             OrdenFabricacionId = d.PedidoId,
                             CodigoProducto = d.ProductoId,
                             DescripcionProducto = d.Description,
                             QtyPlanned = (int)dp.Quantity,
                             FechaOf = dp.PostDate.ToString("dd/MM/yyyy"),
                             FechaOfFin = null,
                             Status = dp.Status,
                             IsChecked = false
                         }).ToListAsync();

            return query;
        }
    }
}
