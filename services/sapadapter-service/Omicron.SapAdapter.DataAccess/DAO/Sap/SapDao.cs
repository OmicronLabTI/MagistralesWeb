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
    using Omicron.SapAdapter.Entities.Model.JoinsModels;
    using Omicron.SapAdapter.Entities.Model.DbModels;
    using Omicron.SapAdapter.Resources.Extensions;

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
        public async Task<IEnumerable<CompleteOrderModel>> GetAllOrdersByFechaIni(DateTime initDate, DateTime endDate)
        {
                var query = await (from order in this.databaseContext.OrderModel
                                   join detalle in this.databaseContext.DetallePedido on order.PedidoId equals detalle.PedidoId
                                   join producto in this.databaseContext.ProductoModel on detalle.ProductoId equals producto.ProductoId
                                   join asesor in this.databaseContext.AsesorModel on order.AsesorId equals asesor.AsesorId
                                   where order.FechaInicio >= initDate && order.FechaInicio <= endDate && producto.IsMagistral == "Y"
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
        /// get orders.
        /// </summary>
        /// <returns>the orders.</returns>
        public async Task<IEnumerable<CompleteOrderModel>> GetAllOrdersByFechaFin(DateTime initDate, DateTime endDate)
        {

            var query = await (from order in this.databaseContext.OrderModel
                               join detalle in this.databaseContext.DetallePedido on order.PedidoId equals detalle.PedidoId
                               join producto in this.databaseContext.ProductoModel on detalle.ProductoId equals producto.ProductoId
                               join asesor in this.databaseContext.AsesorModel on order.AsesorId equals asesor.AsesorId
                               where order.FechaFin >= initDate && order.FechaFin <= endDate && producto.IsMagistral == "Y"
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
                                   OrdenFabricacionId = dp.OrdenId,
                                   CodigoProducto = d.ProductoId,
                                   DescripcionProducto = p.LargeDescription,
                                   DescripcionCorta = p.ProductoName,
                                   QtyPlanned = (int)dp.Quantity,
                                   QtyPlannedDetalle = (int)d.Quantity,
                                   FechaOf = dp.PostDate.HasValue ? dp.PostDate.Value.ToString("dd/MM/yyyy") : string.Empty,
                                   FechaOfFin = dp.DueDate.HasValue ? dp.DueDate.Value.ToString("dd/MM/yyyy") : string.Empty,
                                   Status = dp.Status,
                                   IsChecked = false
                               }).ToListAsync();

            return query;
        }

        /// <summary>
        /// Get the orders.
        /// </summary>
        /// <returns>get the orders.</returns>
        public async Task<List<OrderModel>> GetOrdersById(int pedidoID)
        {
            var query = await this.databaseContext.OrderModel.Where(x => x.PedidoId == pedidoID).ToListAsync();

            return query;
        }

        /// <summary>
        /// gets the orders by product and item.
        /// </summary>
        /// <param name="pedidoId">the product id.</param>
        /// <param name="productId">the product id.</param>
        /// <returns>the data.</returns>
        public async Task<OrdenFabricacionModel> GetProdOrderByOrderProduct(int pedidoId, string productId)
        {
            var query = await this.databaseContext.OrdenFabricacionModel.Where(x => x.PedidoId == pedidoId && x.ProductoId == productId && x.DataSource == "O").ToListAsync();
            return query.FirstOrDefault();
        }

        /// <summary>
        /// Get last id of isolated production order created.
        /// </summary>
        /// <param name="productId">the product id.</param>
        /// <param name="uniqueId">the unique record id.</param>
        /// <returns>the data.</returns>
        public async Task<int> GetlLastIsolatedProductionOrderId(string productId, string uniqueId)
        {
            var query = await this.databaseContext.OrdenFabricacionModel
                                    .Where(
                                        x => x.ProductoId.Equals(productId) && 
                                        x.Comments.Equals(uniqueId) &&
                                        string.IsNullOrEmpty(x.CardCode) &&
                                        x.DataSource.Equals("O"))
                                    .MaxAsync(x => x.OrdenId);
            return query;
        }

        /// <summary>
        /// gets the orders by product and item.
        /// </summary>
        /// <param name="listOrders">the product id.</param>        
        /// <returns>the data.</returns>
        public async Task<IEnumerable<OrdenFabricacionModel>> GetFabOrderById(List<int> listOrders)
        {
            var query = await this.databaseContext.OrdenFabricacionModel.Where(x => listOrders.Contains(x.OrdenId)).ToListAsync();
            return query;
        }

        /// <summary>
        /// gets the orders by orderid.
        /// </summary>
        /// <param name="fechaInit">initial date.</param>
        /// <param name="endDate">The end date.</param>
        /// <returns>the data.</returns>
        public async Task<IEnumerable<OrdenFabricacionModel>> GetFabOrderByCreateDate(DateTime fechaInit, DateTime endDate)
        {
            var query = await this.databaseContext.OrdenFabricacionModel.Where(x => x.CreatedDate != null && x.CreatedDate >= fechaInit && x.CreatedDate <= endDate).ToListAsync();
            return query;
        }

        /// <summary>
        /// Gets the prod by itemcode.
        /// </summary>
        /// <param name="itemCode">the item code.</param>
        /// <returns>the data.</returns>
        public async Task<IEnumerable<OrdenFabricacionModel>> GetFabOrderByItemCode(string itemCode)
        {
            return await this.databaseContext.OrdenFabricacionModel.Where(x => x.ProductoId.Contains(itemCode)).ToListAsync();
        }

        /// <summary>
        /// gets the realtion between WOR1, OITM ans OITW.
        /// </summary>
        /// <param name="orderId">the order id.</param>
        /// <returns>the data.</returns>
        public async Task<IEnumerable<CompleteDetalleFormulaModel>> GetDetalleFormula(int orderId)
        {
            var query = await (from w in this.databaseContext.DetalleFormulaModel
                                join i in this.databaseContext.ItemWarehouseModel on
                                new
                                {
                                    w.ItemCode,
                                    Wharehouse = w.Almacen
                                }
                                equals
                                new
                                {
                                    i.ItemCode,
                                    Wharehouse = i.WhsCode
                                }
                                into DetallePedido
                                from dp in DetallePedido
                                join p in this.databaseContext.ProductoModel on w.ItemCode equals p.ProductoId
                                where w.OrderFabId == orderId
                                select new CompleteDetalleFormulaModel
                                {
                                    OrderFabId = w.OrderFabId,
                                    ProductId = w.ItemCode,
                                    Description = p.LargeDescription,
                                    BaseQuantity = w.BaseQuantity,
                                    RequiredQuantity = w.RequiredQty,
                                    Consumed = w.ConsumidoQty,
                                    Available = dp.OnHand - dp.IsCommited + dp.OnOrder,
                                    Unit = w.UnidadCode,
                                    Warehouse = w.Almacen,
                                    PendingQuantity = w.RequiredQty - w.ConsumidoQty,
                                    Stock = p.OnHand,
                                    WarehouseQuantity = dp.OnHand
                                }).ToListAsync();

            return query;
        }

        /// <summary>
        /// gets the sap user.
        /// </summary>
        /// <param name="userId">the user id.</param>
        /// <returns>the data.</returns>
        public async Task<Users> GetSapUserById(int userId)
        {
            var query = await this.databaseContext.Users.FirstOrDefaultAsync(x => x.UserId == userId);
            return query;
        }

        /// <summary>
        /// Gets the value for the item code by filters. 
        /// </summary>
        /// <param name="value">the value to look.</param>
        /// <returns>the value.</returns>
        public async Task<IEnumerable<CompleteDetalleFormulaModel>> GetItemsByContainsItemCode(string value)
        {
            var products = await this.databaseContext.ProductoModel.Where(x => x.ProductoId.ToLower().Contains(value)).ToListAsync();
            return await this.GetComponentes(products);
        }

        /// <summary>
        /// Gets the value for the item code by filters. 
        /// </summary>
        /// <param name="value">the value to look.</param>
        /// <returns>the value.</returns>
        public async Task<IEnumerable<CompleteDetalleFormulaModel>> GetItemsByContainsDescription(string value)
        {
            var products = await this.databaseContext.ProductoModel.Where(x => x.ProductoName.ToLower().Contains(value)).ToListAsync();
            return await this.GetComponentes(products);
        }

        /// <summary>
        /// Gets the pedidos from the Detalle pedido.
        /// </summary>
        /// <param name="pedidoId">the pedido id.</param>
        /// <returns>the data.</returns>
        public async Task<IEnumerable<DetallePedidoModel>> GetPedidoById(int pedidoId)
        {
            return await this.databaseContext.DetallePedido.Where(x => x.PedidoId == pedidoId).ToListAsync();
        }

        /// <summary>
        /// Gets the pedidos from the Detalle pedido.
        /// </summary>
        /// <param name="orderId">the pedido id.</param>
        /// <returns>the data.</returns>
        public async Task<IEnumerable<CompleteDetalleFormulaModel>> GetComponentByBatches(int orderId)
        {
            return await (from c in this.databaseContext.DetalleFormulaModel
                          join p in this.databaseContext.ProductoModel on c.ItemCode equals p.ProductoId
                          where c.OrderFabId == orderId && p.ManagedBatches == "Y"
                          select new CompleteDetalleFormulaModel
                          { 
                              Warehouse = c.Almacen,
                              PendingQuantity = c.RequiredQty,
                              ProductId = c.ItemCode,
                              Description = p.LargeDescription,
                              OrderFabId = c.OrderFabId,
                          }).ToListAsync();
        }

        /// <summary>
        /// Gets the item by code.
        /// </summary>
        /// <param name="itemCode">the item code.</param>
        /// <returns>the data.</returns>
        public async Task<IEnumerable<ProductoModel>> GetProductById(string itemCode)
        {
            return await this.databaseContext.ProductoModel.Where(x => x.ProductoId == itemCode).ToListAsync();
        }

        /// <summary>
        /// gets the valid batches by item.
        /// </summary>
        /// <param name="itemCode">the item code.</param>
        /// <param name="warehouse">the warehouse.</param>
        /// <returns>the data.</returns>
        public async Task<IEnumerable<CompleteBatchesJoinModel>> GetValidBatches(string itemCode, string warehouse)
        {
            var listToReturn = new List<CompleteBatchesJoinModel>();
            var querybatches = (await this.databaseContext.BatchesQuantity.Where(x => x.ItemCode == itemCode && x.WhsCode == warehouse && x.Quantity > 0).ToListAsync()).ToList();

            var validBatches = querybatches.Select(x => x.SysNumber);

            var batches = (await this.databaseContext.Batches.Where(x => x.ItemCode == itemCode && validBatches.Contains(x.SysNumber)).ToListAsync()).ToList();

            querybatches.ForEach(x =>
            {
                var batch = batches.FirstOrDefault(y => x.SysNumber == y.SysNumber);
                batch = batch == null ? new Batches() : batch;
                listToReturn.Add(new CompleteBatchesJoinModel
                {
                    CommitQty = x.CommitQty.HasValue ? x.CommitQty.Value : 0,
                    Quantity = x.Quantity.HasValue ? x.Quantity.Value : 0,
                    DistNumber = batch.DistNumber == null ? string.Empty : batch.DistNumber,
                    SysNumber = x.SysNumber,
                    FechaExp = !batch.ExpDate.HasValue ? null : batch.ExpDate.Value.ToString("dd/MM/yyyy"),
                });
            });

            return listToReturn;
        }

        /// <summary>
        /// Gest the batch transaction by order and item code.
        /// </summary>
        /// <param name="itemCode">the item code.</param>
        /// <param name="orderId">the order id.</param>
        /// <returns>the data.</returns>
        public async Task<IEnumerable<BatchTransacitions>> GetBatchesTransactionByOrderItem(string itemCode, int orderId)
        {
            return await this.databaseContext.BatchTransacitions.Where(x => x.DocNum == orderId && x.ItemCode.Equals(itemCode)).ToListAsync();
        }

        /// <summary>
        /// Gets the record from ITL1 by log entry.
        /// </summary>
        /// <param name="logEntry">the log entry.</param>
        /// <returns>the data.</returns>
        public async Task<IEnumerable<BatchesTransactionQtyModel>> GetBatchTransationsQtyByLogEntry(int logEntry)
        {
            return await this.databaseContext.BatchesTransactionQtyModel.Where(x => x.LogEntry == logEntry).ToListAsync();
        }

        /// <summary>
        /// Get next batch code.
        /// </summary>
        /// <param name="batchCodePattern">Batch code pattern.</param>
        /// <param name="productCode">the product code.</param>
        /// <returns>the data.</returns>
        public async Task<string> GetMaxBatchCode(string batchCodePattern, string productCode)
        { 
            return await (from    batch in this.databaseContext.Batches
                                where   batch.ItemCode.Equals(productCode)
                                &&      EF.Functions.Like(batch.DistNumber, batchCodePattern)
                                orderby batch.DistNumber descending
                                select  batch.DistNumber).Take(1).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Gets the value for the item code by filters. 
        /// </summary>
        /// <param name="criterials">the values to look.</param>
        /// <returns>the value.</returns>
        public async Task<List<ProductoModel>> GetProductsManagmentByBatch(List<string> criterials)
        {
            var results = from product in this.databaseContext.ProductoModel where product.ManagedBatches.Equals("Y") select product;

            foreach (var criterial in criterials)
            {
                results =   from    product in results
                            where   EF.Functions.Like(product.ProductoId, $"%{criterial}%")
                            ||      EF.Functions.Like(product.ProductoName, $"%{criterial}%")
                            orderby product.ProductoId ascending
                            select  product;
            }
             
            return await results.ToListAsync();
        }

        /// <summary>
        /// Gets the componenents from the product.
        /// </summary>
        /// <param name="products">the products.</param>
        /// <returns>the data.</returns>
        private async Task<List<CompleteDetalleFormulaModel>> GetComponentes(List<ProductoModel> products)
        {
            var listToReturn = new List<CompleteDetalleFormulaModel>();

            if (!products.Any())
            {
                return new List<CompleteDetalleFormulaModel>();
            }

            var listIds = products.Select(x => x.ProductoId).ToList();
            var almacen = await this.databaseContext.ItemWarehouseModel.Where(x => listIds.Contains(x.ItemCode) && x.WhsCode == "MN").ToListAsync();

            products.ForEach(p =>
            {
                var datoAlmacen = almacen.FirstOrDefault(y => y.ItemCode == p.ProductoId);
                var datoToAssign = datoAlmacen == null ? new ItemWarehouseModel() : datoAlmacen;
                listToReturn.Add(new CompleteDetalleFormulaModel
                {
                    ProductId = p.ProductoId,
                    Description = p.LargeDescription,
                    Consumed = 0,
                    Available = datoToAssign.OnHand - datoToAssign.IsCommited + datoToAssign.OnOrder,
                    Unit = p.Unit,
                    Warehouse = "MN",
                    Stock = p.OnHand,
                    WarehouseQuantity = datoToAssign.OnHand,
                });
            });

            return listToReturn;
        }
    }
}
