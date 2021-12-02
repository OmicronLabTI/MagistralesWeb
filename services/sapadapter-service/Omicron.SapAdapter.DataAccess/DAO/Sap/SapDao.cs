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
    using Omicron.SapAdapter.Entities.Model.BusinessModels;
    using Serilog;

    /// <summary>
    /// Class for the dao.
    /// </summary>
    public class SapDao : ISapDao
    {
        private readonly IDatabaseContext databaseContext;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SapDao"/> class.
        /// </summary>
        /// <param name="databaseContext">the context.</param>
        /// <param name="logger">the logger.</param>
        public SapDao(IDatabaseContext databaseContext, ILogger logger)
        {
            this.databaseContext = databaseContext ?? throw new ArgumentNullException(nameof(databaseContext));
            this.logger = logger;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<CompleteOrderModel>> GetAllOrdersByFechaIni(DateTime initDate, DateTime endDate)
        {
                var query = (from order in this.databaseContext.OrderModel
                                   join detalle in this.databaseContext.DetallePedido on order.PedidoId equals detalle.PedidoId
                                   into DetalleOrden
                                   from dp in DetalleOrden.DefaultIfEmpty()
                                   join producto in this.databaseContext.ProductoModel on dp.ProductoId equals producto.ProductoId
                                   join asesor in this.databaseContext.AsesorModel on order.AsesorId equals asesor.AsesorId
                                   join doctor in this.databaseContext.ClientCatalogModel on order.Codigo equals doctor.ClientId
                                   join doctordet in this.databaseContext.DoctorInfoModel.Where(x => x.AdressType == "S") on
                                    new
                                    {
                                        DoctorId = order.Codigo,
                                        Address = order.ShippingAddressName
                                    }
                                    equals
                                    new
                                    {
                                        DoctorId = doctordet.CardCode,
                                        Address = doctordet.NickName
                                    }
                                    into detalleDireccion
                                   from dop in detalleDireccion.DefaultIfEmpty()
                                   where order.FechaInicio >= initDate && order.FechaInicio <= endDate && producto.IsMagistral == "Y"
                                   select new CompleteOrderModel
                                   {
                                       DocNum = order.DocNum,
                                       Cliente = doctor.AliasName,
                                       Codigo = order.Codigo,
                                       Medico = dop.Address2 ?? doctor.AliasName,
                                       AsesorName = asesor.AsesorName,
                                       FechaInicio = order.FechaInicio.ToString("dd/MM/yyyy"),
                                       FechaFin = order.FechaFin.ToString("dd/MM/yyyy"),
                                       PedidoStatus = order.PedidoStatus,
                                       AtcEntry = order.AtcEntry,
                                       IsChecked = false,
                                       Detalles = dp,
                                       OrderType = order.OrderType,
                                       Canceled = order.Canceled,
                                       PedidoMuestra = order.PedidoMuestra,
                                       DocNumDxp = order.DocNumDxp,
                                   });

            return await this.RetryQuery<CompleteOrderModel>(query);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<CompleteOrderModel>> GetAllOrdersByIds(List<int> ids)
        {
            var query = (from order in this.databaseContext.OrderModel.Where(x => ids.Contains(x.DocNum))
                               join detalle in this.databaseContext.DetallePedido on order.PedidoId equals detalle.PedidoId
                               into DetalleOrden
                               from dp in DetalleOrden.DefaultIfEmpty()
                               join producto in this.databaseContext.ProductoModel on dp.ProductoId equals producto.ProductoId
                               join asesor in this.databaseContext.AsesorModel on order.AsesorId equals asesor.AsesorId
                               join doctor in this.databaseContext.ClientCatalogModel on order.Codigo equals doctor.ClientId
                               join doctordet in this.databaseContext.DoctorInfoModel.Where(x => x.AdressType == "S") on
                               new
                               {
                                   DoctorId = order.Codigo,
                                   Address = order.ShippingAddressName
                               }
                               equals
                               new
                               {
                                   DoctorId = doctordet.CardCode,
                                   Address = doctordet.NickName
                               }
                               into detalleDireccion
                               from dop in detalleDireccion.DefaultIfEmpty()
                               where producto.IsMagistral == "Y"
                               select new CompleteOrderModel
                               {
                                   DocNum = order.DocNum,
                                   Cliente = doctor.AliasName,
                                   Codigo = order.Codigo,
                                   Medico = dop.Address2 ?? doctor.AliasName,
                                   AsesorName = asesor.AsesorName,
                                   FechaInicio = order.FechaInicio.ToString("dd/MM/yyyy"),
                                   FechaFin = order.FechaFin.ToString("dd/MM/yyyy"),
                                   PedidoStatus = order.PedidoStatus,
                                   AtcEntry = order.AtcEntry,
                                   IsChecked = false,
                                   Detalles = dp,
                                   OrderType = order.OrderType,
                                   Canceled = order.Canceled,
                                   PedidoMuestra = order.PedidoMuestra,
                                   DocNumDxp = order.DocNumDxp,
                               });

            return await this.RetryQuery<CompleteOrderModel>(query);
        }

        /// <summary>
        /// Get the orders.
        /// </summary>
        /// <returns>get the orders.</returns>
        public async Task<IEnumerable<CompleteOrderModel>> GetAllOrdersById(int init, int end)
        {
            var query = (from order in this.databaseContext.OrderModel
                              join detalle in this.databaseContext.DetallePedido on order.PedidoId equals detalle.PedidoId
                              join producto in this.databaseContext.ProductoModel on detalle.ProductoId equals producto.ProductoId
                              join asesor in this.databaseContext.AsesorModel on order.AsesorId equals asesor.AsesorId
                              join doctor in this.databaseContext.ClientCatalogModel on order.Codigo equals doctor.ClientId
                              join doctordet in this.databaseContext.DoctorInfoModel.Where(x => x.AdressType == "S") on
                              new
                              {
                                  DoctorId = order.Codigo,
                                  Address = order.ShippingAddressName
                              }
                              equals
                              new
                              {
                                  DoctorId = doctordet.CardCode,
                                  Address = doctordet.NickName
                              }
                              into detalleDireccion
                              from dp in detalleDireccion.DefaultIfEmpty()
                              where order.PedidoId >= init && order.PedidoId <= end && producto.IsMagistral == "Y"
                              select new CompleteOrderModel
                              {
                                  DocNum = order.DocNum,
                                  Cliente = doctor.AliasName,
                                  Codigo = order.Codigo,
                                  Medico = dp.Address2 ?? doctor.AliasName,
                                  AsesorName = asesor.AsesorName,
                                  FechaInicio = order.FechaInicio.ToString("dd/MM/yyyy"),
                                  FechaFin = order.FechaFin.ToString("dd/MM/yyyy"),
                                  PedidoStatus = order.PedidoStatus,
                                  AtcEntry = order.AtcEntry,
                                  IsChecked = false,
                                  Detalles = detalle,
                                  OrderType = order.OrderType,
                                  Canceled = order.Canceled,
                                  PedidoMuestra = order.PedidoMuestra,
                                  DocNumDxp = order.DocNumDxp,
                              });

            return await this.RetryQuery<CompleteOrderModel>(query);
        }

        /// <inheritdoc/>
        public async Task<List<CompleteOrderModel>> GetAllOrdersByDocNumDxp(string docNumDxp)
        {
            var query = (from order in this.databaseContext.OrderModel
                         join detalle in this.databaseContext.DetallePedido on order.PedidoId equals detalle.PedidoId
                         join producto in this.databaseContext.ProductoModel on detalle.ProductoId equals producto.ProductoId
                         join asesor in this.databaseContext.AsesorModel on order.AsesorId equals asesor.AsesorId
                         join doctor in this.databaseContext.ClientCatalogModel on order.Codigo equals doctor.ClientId
                         join doctordet in this.databaseContext.DoctorInfoModel.Where(x => x.AdressType == "S") on
                              new
                              {
                                  DoctorId = order.Codigo,
                                  Address = order.ShippingAddressName
                              }
                              equals
                              new
                              {
                                  DoctorId = doctordet.CardCode,
                                  Address = doctordet.NickName
                              }
                              into detalleDireccion
                         from dp in detalleDireccion.DefaultIfEmpty()
                         where order.DocNumDxp == docNumDxp && producto.IsMagistral == "Y"
                         select new CompleteOrderModel
                         {
                             DocNum = order.DocNum,
                             Cliente = doctor.AliasName,
                             Codigo = order.Codigo,
                             Medico = dp.Address2 ?? doctor.AliasName,
                             AsesorName = asesor.AsesorName,
                             FechaInicio = order.FechaInicio.ToString("dd/MM/yyyy"),
                             FechaFin = order.FechaFin.ToString("dd/MM/yyyy"),
                             PedidoStatus = order.PedidoStatus,
                             AtcEntry = order.AtcEntry,
                             IsChecked = false,
                             Detalles = detalle,
                             OrderType = order.OrderType,
                             Canceled = order.Canceled,
                             PedidoMuestra = order.PedidoMuestra,
                             DocNumDxp = order.DocNumDxp,
                         });

            return (await this.RetryQuery<CompleteOrderModel>(query)).ToList();
        }

        /// <summary>
        /// gets the asesors by salesOrderId.
        /// </summary>
        /// <param name="docsEntry">the list of salesOrderId.</param>        
        /// <returns>the data.</returns>
        public async Task<IEnumerable<SalesAsesorModel>> GetAsesorWithEmailByIds(List<int> docsEntry)
        {
             var query = (from order in this.databaseContext.OrderModel.Where(x => docsEntry.Contains(x.PedidoId)) 
                          join salesPerson in this.databaseContext.SalesPersonModel on order.AsesorId equals salesPerson.AsesorId
                          select new SalesAsesorModel
                          {                              
                              Email = string.IsNullOrEmpty(salesPerson.Email) ? string.Empty : salesPerson.Email, 
                              OrderId = order.PedidoId,
                              Cliente = order.Medico
                          });

            return await this.RetryQuery<SalesAsesorModel>(query);
        }

        /// <summary>
        /// gets the asesors by salesOrderId.
        /// </summary>
        /// <param name="docsEntry">the list of salesOrderId.</param>        
        /// <returns>the data.</returns>
        public async Task<IEnumerable<SalesPersonModel>> GetAsesorWithEmailByIdsFromTheAsesor(List<int> salesPrsonId)
        {
            return await this.RetryQuery<SalesPersonModel>(this.databaseContext.SalesPersonModel.Where(x => salesPrsonId.Contains(x.AsesorId)));
    
        }

        /// <summary>
        /// gets the details.
        /// </summary>
        /// <param name="pedidoId">Pedido id.</param>
        /// <returns>the details.</returns>
        public async Task<IEnumerable<CompleteDetailOrderModel>> GetAllDetails(List<int?> pedidoId)
        {
            var query = (from d in this.databaseContext.DetallePedido.Where(x => pedidoId.Contains(x.PedidoId))
                         join o in this.databaseContext.OrdenFabricacionModel on
                         new
                         {
                             Pedido = d.PedidoId,
                             ItemCode = d.ProductoId
                         }
                         equals
                         new
                         {
                             Pedido = o.PedidoId,
                             ItemCode = o.ProductoId
                         }
                         into DetallePedido
                         from dp in DetallePedido.DefaultIfEmpty()
                         join p in this.databaseContext.ProductoModel on d.ProductoId equals p.ProductoId
                         join ped in this.databaseContext.OrderModel on d.PedidoId equals ped.PedidoId
                         where p.IsMagistral == "Y"
                         select new CompleteDetailOrderModel
                         {
                             OrdenFabricacionId = dp.OrdenId,
                             CodigoProducto = d.ProductoId,
                             DescripcionProducto = p.LargeDescription,
                             DescripcionCorta = p.ProductoName,
                             QtyPlanned = dp.Quantity,
                             QtyPlannedDetalle = (int)d.Quantity,
                             FechaOf = dp.PostDate.HasValue ? dp.PostDate.Value.ToString("dd/MM/yyyy") : string.Empty,
                             FechaOfFin = dp.DueDate.HasValue ? dp.DueDate.Value.ToString("dd/MM/yyyy") : string.Empty,
                             Status = dp.Status,
                             IsChecked = false,
                             CreatedDate = dp.CreatedDate,
                             Label = d.Label,
                             NeedsCooling = p.NeedsCooling,
                             Container = d.Container,
                             PatientName = ped.Patient ?? string.Empty,
                             PedidoId = d.PedidoId ?? 0,
                         });

            return await this.RetryQuery<CompleteDetailOrderModel>(query);
        }

        /// <inheritdoc/>
        public async Task<List<OrderModel>> GetOrdersById(int pedidoID)
        {            
            return (await this.RetryQuery<OrderModel>(this.databaseContext.OrderModel.Where(x => x.PedidoId == pedidoID))).ToList();
        }

        /// <inheritdoc/>
        public async Task<List<OrderModel>> GetOrdersById(List<int> pedidoID)
        {
            return (await this.RetryQuery<OrderModel>(this.databaseContext.OrderModel.Where(x => pedidoID.Contains(x.PedidoId)))).ToList();
        }

        /// <inheritdoc/>
        public async Task<List<OrderModel>> GetOrdersByIdJoinDoctor(List<int> pedidoID)
        {
            var query = from order in this.databaseContext.OrderModel.Where(x => pedidoID.Contains(x.PedidoId))
                        join doctor in this.databaseContext.ClientCatalogModel on order.Codigo equals doctor.ClientId
                        select new OrderModel
                        {
                            Address = order.Address,
                            AsesorId = order.AsesorId,
                            AtcEntry = order.AtcEntry,
                            Canceled = order.Canceled,
                            Codigo = order.Codigo,
                            Comments = order.Comments,
                            DocNum = order.DocNum,
                            DocNumDxp = order.DocNumDxp,
                            FechaFin = order.FechaFin,
                            FechaInicio = order.FechaInicio,
                            OrderType = order.OrderType,
                            Patient = order.Patient,
                            PedidoMuestra= order.PedidoMuestra,
                            PedidoId = order.PedidoId,
                            PedidoStatus = order.PedidoStatus,
                            ShippingAddressName = order.ShippingAddressName,
                            ShippingCost = order.ShippingCost,
                            Medico = doctor.AliasName,
                        };

            return (await this.RetryQuery<OrderModel>(query)).ToList();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<OrdenFabricacionModel>> GetProdOrderByOrderProduct(int pedidoId, string productId)
        {
            return await this.RetryQuery<OrdenFabricacionModel>(this.databaseContext.OrdenFabricacionModel.Where(x => x.PedidoId == pedidoId && x.ProductoId == productId && x.DataSource == "O"));
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public async Task<IEnumerable<OrdenFabricacionModel>> GetFabOrderById(List<int> pedidoId)
        {
            return await this.RetryQuery(this.databaseContext.OrdenFabricacionModel.Where(x => pedidoId.Contains(x.OrdenId)));            
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<OrdenFabricacionModel>> GetFabOrderBySalesOrderId(List<int> salesOrderIds)
        {
            var query = await this.databaseContext.OrdenFabricacionModel.Where(x => salesOrderIds.Contains(x.PedidoId.Value)).ToListAsync();
            return query;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<OrdenFabricacionModel>> GetFabOrderByCreateDate(DateTime fechaInit, DateTime endDate)
        {
            return await this.RetryQuery<OrdenFabricacionModel>(this.databaseContext.OrdenFabricacionModel.Where(x => x.CreatedDate != null && x.CreatedDate >= fechaInit && x.CreatedDate <= endDate));
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<OrdenFabricacionModel>> GetFabOrderByItemCode(string itemCode)
        {
            return await this.RetryQuery<OrdenFabricacionModel>(this.databaseContext.OrdenFabricacionModel.Where(x => x.ProductoId.ToLower().Contains(itemCode)));            
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<CompleteDetalleFormulaModel>> GetDetalleFormula(int orderId)
        {
            var query = (from w in this.databaseContext.DetalleFormulaModel
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
                                });

            return await this.RetryQuery<CompleteDetalleFormulaModel>(query);
        }

        /// <summary>
        /// gets the realtion between WOR1, OITM ans OITW.
        /// </summary>
        /// <param name="orderId">the order id.</param>
        /// <returns>the data.</returns>
        public async Task<IEnumerable<CompleteDetalleFormulaModel>> GetDetalleFormula(List<int> orderId)
        {
            var query = (from w in this.databaseContext.DetalleFormulaModel.Where(d => orderId.Contains(d.OrderFabId))
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
                         });

            return await this.RetryQuery<CompleteDetalleFormulaModel>(query);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<DetalleFormulaModel>> GetDetalleFormulaByProdOrdId(List<int> ordersId)
        {
            return await this.RetryQuery<DetalleFormulaModel>(this.databaseContext.DetalleFormulaModel.Where(x => ordersId.Contains(x.OrderFabId)));
        }

        /// <inheritdoc/>
        public async Task<Users> GetSapUserById(int userId)
        {
            var query = await this.databaseContext.Users.FirstOrDefaultAsync(x => x.UserId == userId);
            return query;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<CompleteDetalleFormulaModel>> GetItemsByContainsItemCode(string value, string warehouse)
        {
            var products = await this.RetryQuery<ProductoModel>(this.databaseContext.ProductoModel.Where(x => x.ProductoId.ToLower().Contains(value)));
            return await this.GetComponentes(products.ToList(), warehouse);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<CompleteDetalleFormulaModel>> GetItemsByContainsItemCode(List<string> value, string warehouse)
        {
            var products = await this.RetryQuery<ProductoModel>(this.databaseContext.ProductoModel.Where(x => value.Contains(x.ProductoId.ToLower())));
            return await this.GetComponentes(products.ToList(), warehouse);
        }

        /// <summary>
        /// Gets the value for the item code by filters. 
        /// </summary>
        /// <param name="value">the value to look.</param>
        /// <returns>the value.</returns>
        public async Task<IEnumerable<CompleteDetalleFormulaModel>> GetItemsByContainsDescription(string value, string warehouse)
        {
            var products = await this.RetryQuery<ProductoModel>(this.databaseContext.ProductoModel.Where(x => x.ProductoName.ToLower().Contains(value)));
            return await this.GetComponentes(products.ToList(), warehouse);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<DetallePedidoModel>> GetPedidoByIdJoinProduct(int pedidoId)
        {
            var query = from saleDetail in this.databaseContext.DetallePedido.Where(x => x.PedidoId == pedidoId)
                        join product in this.databaseContext.ProductoModel on saleDetail.ProductoId equals product.ProductoId
                        where product.IsWorkableProduct == "Y"
                        select new DetallePedidoModel
                        {
                            Container = saleDetail.Container,
                            Description = saleDetail.Description,
                            DestinyAddress = saleDetail.DestinyAddress,
                            DetalleId = saleDetail.DetalleId,
                            DocDate = saleDetail.DocDate,
                            HasRecipe = saleDetail.HasRecipe,
                            Label = saleDetail.Label,
                            LineStatus = saleDetail.LineStatus,
                            PedidoId = saleDetail.PedidoId,
                            ProductoId = saleDetail.ProductoId,
                            Quantity = saleDetail.Quantity,
                        };
            return await this.RetryQuery<DetallePedidoModel>(query);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<CompleteDetalleFormulaModel>> GetComponentByBatches(int orderId)
        {
            var query = (from c in this.databaseContext.DetalleFormulaModel
                          join p in this.databaseContext.ProductoModel on c.ItemCode equals p.ProductoId
                          where c.OrderFabId == orderId && p.ManagedBatches == "Y"
                          select new CompleteDetalleFormulaModel
                          { 
                              Warehouse = c.Almacen,
                              PendingQuantity = c.RequiredQty,
                              ProductId = c.ItemCode,
                              Description = p.LargeDescription,
                              OrderFabId = c.OrderFabId,
                          });

            return await this.RetryQuery<CompleteDetalleFormulaModel>(query);
        }

        /// <summary>
        /// Gets the pedidos from the Detalle pedido.
        /// </summary>
        /// <param name="orderId">the pedido id.</param>
        /// <returns>the data.</returns>
        public async Task<IEnumerable<CompleteDetalleFormulaModel>> GetComponentByBatches(List<int> orderId)
        {
            var query = (from c in this.databaseContext.DetalleFormulaModel.Where(d => orderId.Contains(d.OrderFabId))
                         join p in this.databaseContext.ProductoModel on c.ItemCode equals p.ProductoId
                         where p.ManagedBatches == "Y"
                         select new CompleteDetalleFormulaModel
                         {
                             Warehouse = c.Almacen,
                             PendingQuantity = c.RequiredQty,
                             ProductId = c.ItemCode,
                             Description = p.LargeDescription,
                             OrderFabId = c.OrderFabId,
                         });

            return await this.RetryQuery<CompleteDetalleFormulaModel>(query);
        }

        /// <summary>
        /// Gets the item by code.
        /// </summary>
        /// <param name="itemCode">the item code.</param>
        /// <returns>the data.</returns>
        public async Task<IEnumerable<ProductoModel>> GetProductById(string itemCode)
        {
            return await this.RetryQuery<ProductoModel>(this.databaseContext.ProductoModel.Where(x => x.ProductoId == itemCode));
        }

        /// <summary>
        /// gets the valid batches by item.
        /// </summary>
        /// <param name="itemCode">the item code.</param>
        /// <param name="warehouse">the warehouse.</param>
        /// <returns>the data.</returns>
        public async Task<IEnumerable<CompleteBatchesJoinModel>> GetValidBatches(List<CompleteDetalleFormulaModel> components)
        {
            var listToReturn = new List<CompleteBatchesJoinModel>();
            var listItems = components.Select(x => x.ProductId).ToList();

            var querybatches = (await this.databaseContext.BatchesQuantity.Where(x => listItems.Contains(x.ItemCode)).ToListAsync()).ToList();
            querybatches = querybatches.Where(x => components.Any(y => y.ProductId == x.ItemCode && y.Warehouse == x.WhsCode)).ToList();

            var validBatches = querybatches.Select(x => x.SysNumber);

            var batches = (await this.databaseContext.Batches.Where(x => listItems.Contains(x.ItemCode)).ToListAsync()).ToList();
            batches = batches.Where(x => validBatches.Contains(x.SysNumber)).ToList();

            querybatches.ForEach(x =>
            {
                var batch = batches.FirstOrDefault(y => x.SysNumber == y.SysNumber && x.ItemCode == y.ItemCode);
                batch ??= new Batches();
                listToReturn.Add(new CompleteBatchesJoinModel
                {
                    CommitQty = x.CommitQty ?? 0,
                    Quantity = x.Quantity ?? 0,
                    DistNumber = batch.DistNumber ?? string.Empty,
                    SysNumber = x.SysNumber,
                    FechaExp = !batch.ExpDate.HasValue ? null : batch.ExpDate.Value.ToString("dd/MM/yyyy"),
                    ItemCode = x.ItemCode,
                });
            });

            return listToReturn;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<BatchTransacitions>> GetBatchesTransactionByOrderItem(string itemCode, int orderId)
        {
            return await this.RetryQuery<BatchTransacitions>(this.databaseContext.BatchTransacitions.Where(x => x.DocNum == orderId && x.ItemCode.Equals(itemCode)));
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<BatchTransacitions>> GetBatchesTransactionByOrderItem(List<int> orderId)
        {
            return await this.RetryQuery<BatchTransacitions>(this.databaseContext.BatchTransacitions.Where(x => orderId.Contains(x.DocNum)));
        }

        /// <summary>
        /// Gets the record from ITL1 by log entry.
        /// </summary>
        /// <param name="logEntry">the log entry.</param>
        /// <returns>the data.</returns>
        public async Task<IEnumerable<BatchesTransactionQtyModel>> GetBatchTransationsQtyByLogEntry(List<int> logEntry)
        {
            return await this.RetryQuery<BatchesTransactionQtyModel>(this.databaseContext.BatchesTransactionQtyModel.Where(x => logEntry.Contains(x.LogEntry)));
        }

        /// <inheritdoc/>
        public async Task<string> GetMaxBatchCode(string batchCodePattern, string productCode)
        { 
            return await (from    batch in this.databaseContext.Batches
                                where   batch.ItemCode.Equals(productCode)
                                &&      EF.Functions.Like(batch.DistNumber, batchCodePattern)
                                orderby batch.DistNumber descending
                                select  batch.DistNumber).Take(1).FirstOrDefaultAsync();
        }

        /// <inheritdoc/>
        public async Task<string> GetBatchCode(string productCode, string batchCode)
        {
            return await (from batch in this.databaseContext.Batches
                            where batch.ItemCode.Equals(productCode)
                                && batch.DistNumber.Equals(batchCode)
                            select batch.DistNumber).Take(1).FirstOrDefaultAsync();
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public async Task<List<AttachmentModel>> GetAttachmentsById(List<int> ids)
        {
            return await this.databaseContext.AttachmentModel.Where(x => ids.Contains(x.AbsEntry)).ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<CompleteAlmacenOrderModel>> GetAllOrdersForAlmacen(DateTime initDate)
        {
            var query = (from order in this.databaseContext.OrderModel
                         join detalle in this.databaseContext.DetallePedido on order.PedidoId equals detalle.PedidoId
                         into DetalleOrden
                         from dp in DetalleOrden.DefaultIfEmpty()
                         join product in this.databaseContext.ProductoModel on dp.ProductoId equals product.ProductoId
                         join doctor in this.databaseContext.ClientCatalogModel on order.Codigo equals doctor.ClientId
                         join doctordet in this.databaseContext.DoctorInfoModel.Where(x => x.AdressType == "S") on
                         new
                         {
                             DoctorId = order.Codigo,
                             Address = order.ShippingAddressName
                         }
                         equals
                         new
                         {
                             DoctorId = doctordet.CardCode,
                             Address = doctordet.NickName
                         }
                         into detalleDireccion
                         from dop in detalleDireccion.DefaultIfEmpty()
                         where order.FechaInicio >= initDate && (order.PedidoStatus == "O" || order.Canceled == "Y") && product.IsWorkableProduct == "Y"
                         select new CompleteAlmacenOrderModel
                         {
                             DocNum = order.DocNum,
                             Cliente = dop.Address2 ?? string.Empty,
                             Medico = doctor.AliasName,                             
                             FechaInicio = order.FechaInicio,
                             Detalles = dp,
                             Address = order.Address,
                             TypeOrder = order.OrderType,
                             PedidoMuestra = order.PedidoMuestra,
                             Comments = order.Comments,
                             IsLine = product.IsLine,
                             IsMagistral = product.IsMagistral,
                             Canceled = order.Canceled,
                         });

            return await this.RetryQuery<CompleteAlmacenOrderModel>(query);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<CompleteAlmacenOrderModel>> GetAllOrdersForAlmacenById(int saleOrderId)
        {
            var query = from order in this.databaseContext.OrderModel.Where(x => x.DocNum == saleOrderId)
                        join detail in this.databaseContext.DetallePedido on order.DocNum equals detail.PedidoId
                        join product in this.databaseContext.ProductoModel on detail.ProductoId equals product.ProductoId
                        join doctor in this.databaseContext.ClientCatalogModel on order.Codigo equals doctor.ClientId
                        join doctordet in this.databaseContext.DoctorInfoModel.Where(x => x.AdressType == "S") on
                         new
                         {
                             DoctorId = order.Codigo,
                             Address = order.ShippingAddressName
                         }
                         equals
                         new
                         {
                             DoctorId = doctordet.CardCode,
                             Address = doctordet.NickName
                         }
                         into detalleDireccion
                        from dop in detalleDireccion.DefaultIfEmpty()
                        where product.IsWorkableProduct == "Y"
                        select new CompleteAlmacenOrderModel
                        {
                            Cliente = dop.Address2 ?? string.Empty,
                            DocNum = order.DocNum,
                            Detalles = detail,
                            FechaInicio = order.FechaInicio,
                            Medico = doctor.AliasName,
                            Address = order.Address,
                            DocNumDxp = order.DocNumDxp,
                            ShippingCost = order.ShippingCost,
                            ClientId = doctor.ClientId,
                        };
            return await this.RetryQuery<CompleteAlmacenOrderModel>(query);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<CompleteAlmacenOrderModel>> GetAllOrdersForAlmacenByTypeOrder(string typeOrder, List<int> orderToLook)
        {
            var query = (from order in this.databaseContext.OrderModel.Where(x => orderToLook.Contains(x.DocNum))
                         join detalle in this.databaseContext.DetallePedido on order.PedidoId equals detalle.PedidoId
                         into DetalleOrden
                         from dp in DetalleOrden.DefaultIfEmpty()
                         join product in this.databaseContext.ProductoModel on dp.ProductoId equals product.ProductoId
                         join doctor in this.databaseContext.ClientCatalogModel on order.Codigo equals doctor.ClientId
                         join doctordet in this.databaseContext.DoctorInfoModel.Where(x => x.AdressType == "S") on
                         new
                         {
                             DoctorId = order.Codigo,
                             Address = order.ShippingAddressName
                         }
                         equals
                         new
                         {
                             DoctorId = doctordet.CardCode,
                             Address = doctordet.NickName
                         }
                         into detalleDireccion
                         from dop in detalleDireccion.DefaultIfEmpty()
                         where order.OrderType == typeOrder && order.PedidoStatus == "O" && product.IsWorkableProduct == "Y"
                         select new CompleteAlmacenOrderModel
                         {
                             DocNum = order.DocNum,
                             Cliente = dop.Address2 ?? string.Empty,
                             Medico = doctor.AliasName,
                             FechaInicio = order.FechaInicio,
                             Detalles = dp,
                             Address = order.Address,
                             TypeOrder = order.OrderType,
                             Canceled = order.Canceled,
                         });

            return await this.RetryQuery<CompleteAlmacenOrderModel>(query);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<DeliveryDetailModel>> GetDeliveryDetailBySaleOrder(List<int> ordersId)
        {            
            return (await this.RetryQuery<DeliveryDetailModel>(this.databaseContext.DeliveryDetailModel.Where(x => ordersId.Contains(x.BaseEntry))));
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<DeliveryDetailModel>> GetDeliveryDetailBySaleOrderJoinProduct(List<int> ordersId)
        {
            var query = from deliveryDet in this.databaseContext.DeliveryDetailModel.Where(x => ordersId.Contains(x.BaseEntry))
                        join product in this.databaseContext.ProductoModel on deliveryDet.ProductoId equals product.ProductoId
                        where product.IsWorkableProduct == "Y"
                        select new DeliveryDetailModel
                        {
                            BaseEntry = deliveryDet.BaseEntry,
                            Container = deliveryDet.Container,
                            DeliveryId = deliveryDet.DeliveryId,
                            Description = deliveryDet.Description,
                            DocDate = deliveryDet.DocDate,
                            InvoiceId = deliveryDet.InvoiceId,
                            LineNum = deliveryDet.LineNum,
                            LineStatus = deliveryDet.LineStatus,
                            ProductoId = deliveryDet.ProductoId,
                            Quantity = deliveryDet.Quantity,
                        };
            return (await this.RetryQuery<DeliveryDetailModel>(query));
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<ProductoModel>> GetProductByCodeBar(string codeBar)
        {
            return await this.databaseContext.ProductoModel.Where(x => x.BarCode.Equals(codeBar)).ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<DeliverModel>> GetDeliveryModelByDocNum(List<int> docuNums)
        {
            return await this.RetryQuery<DeliverModel>(this.databaseContext.DeliverModel.Where(x => docuNums.Contains(x.DocNum)));
        }

        /// <inheritdoc/>
        public async Task<List<DeliverModel>> GetDeliveryModelByDocNumJoinDoctor(List<int> docuNums)
        {
            var query = (from delivery in this.databaseContext.DeliverModel.Where(x => docuNums.Contains(x.DocNum))
                         join doctor in this.databaseContext.ClientCatalogModel on delivery.CardCode equals doctor.ClientId
                         join doctordet in this.databaseContext.DoctorInfoModel.Where(x => x.AdressType == "S") on
                         new
                         {
                            DoctorId = delivery.CardCode,
                            Address = delivery.ShippingAddressName
                         }
                         equals
                         new
                         {
                            DoctorId = doctordet.CardCode,
                            Address = doctordet.NickName
                         }
                        into detalleDireccion
                         from dop in detalleDireccion.DefaultIfEmpty()
                         select new DeliverModel
                         {
                             Address = delivery.Address,
                             Canceled = delivery.Canceled,
                             CardCode = delivery.CardCode,
                             Cliente = dop.Address2 ?? string.Empty,
                             DeliveryStatus = delivery.DeliveryStatus,
                             DocNum = delivery.DocNum,
                             FechaInicio = delivery.FechaInicio,
                             Medico = doctor.AliasName,
                             PedidoId = delivery.PedidoId,
                             TypeOrder = delivery.TypeOrder,
                         });

            return (await this.RetryQuery<DeliverModel>(query)).ToList();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<DetallePedidoModel>> GetDetailByDocNum(List<int> docuNums)
        {
            return await this.RetryQuery<DetallePedidoModel>(this.databaseContext.DetallePedido.Where(x => docuNums.Contains(x.PedidoId.Value)));
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<InvoiceHeaderModel>> GetInvoiceHeaderByInvoiceId(List<int> docNums)
        {
            return await this.RetryQuery<InvoiceHeaderModel>(this.databaseContext.InvoiceHeaderModel.Where(x => docNums.Contains(x.InvoiceId)));
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<InvoiceHeaderModel>> GetInvoiceHeaderByInvoiceIdJoinDoctor(List<int> docNums)
        {
            var query = (from invoice in this.databaseContext.InvoiceHeaderModel.Where(x => docNums.Contains(x.InvoiceId))
                         join doctor in this.databaseContext.ClientCatalogModel on invoice.CardCode equals doctor.ClientId
                         join doctordet in this.databaseContext.DoctorInfoModel.Where(x => x.AdressType == "S") on
                         new
                         {
                             DoctorId = invoice.CardCode,
                             Address = invoice.ShippingAddressName
                         }
                         equals
                         new
                         {
                             DoctorId = doctordet.CardCode,
                             Address = doctordet.NickName
                         }
                        into detalleDireccion
                        from dop in detalleDireccion.DefaultIfEmpty()
                         select new InvoiceHeaderModel
                         {
                             Address = invoice.Address,
                             Canceled = invoice.Canceled,
                             CardCode = invoice.CardCode,
                             Cliente = dop.Address2 ?? string.Empty,
                             Comments = invoice.Comments,
                             CommentsInvoice = invoice.CommentsInvoice,
                             DocNum = invoice.DocNum,
                             FechaInicio = invoice.FechaInicio,
                             InvoiceId = invoice.InvoiceId,
                             InvoiceStatus = invoice.InvoiceStatus,
                             Medico = doctor.AliasName,
                             Refactura = invoice.Refactura,
                             TrackingNumber = invoice.TrackingNumber,
                             TransportCode = invoice.TransportCode,
                             SalesPrsonId = invoice.SalesPrsonId,
                             TypeOrder = invoice.TypeOrder,
                             UpdateDate = invoice.UpdateDate,
                         });

            return (await this.RetryQuery<InvoiceHeaderModel>(query)).ToList();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<InvoiceDetailModel>> GetInvoiceDetailByDocEntry(List<int> docEntry)
        {
            return await this.RetryQuery<InvoiceDetailModel>(this.databaseContext.InvoiceDetailModel.Where(x => docEntry.Contains(x.InvoiceId)));
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<InvoiceDetailModel>> GetInvoiceDetailByDocEntryJoinProduct(List<int> docEntry)
        {
            var query = from invoiceDetail in this.databaseContext.InvoiceDetailModel.Where(x => docEntry.Contains(x.InvoiceId))
                        join product in this.databaseContext.ProductoModel on invoiceDetail.ProductoId equals product.ProductoId
                        where product.IsWorkableProduct == "Y"
                        select new InvoiceDetailModel
                        {
                            BaseEntry = invoiceDetail.BaseEntry,
                            Container = invoiceDetail.Container,
                            Description = invoiceDetail.Description,
                            DocDate = invoiceDetail.DocDate,
                            InvoiceId = invoiceDetail.InvoiceId,
                            LineNum = invoiceDetail.LineNum,
                            ProductoId = invoiceDetail.ProductoId,
                            Quantity = invoiceDetail.Quantity,
                        };
            return await this.RetryQuery<InvoiceDetailModel>(query);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<InvoiceHeaderModel>> GetInvoiceHeadersByDocNum(List<int> docNum)
        {
            return await this.RetryQuery<InvoiceHeaderModel>(this.databaseContext.InvoiceHeaderModel.Where(x => docNum.Contains(x.DocNum)));
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<InvoiceHeaderModel>> GetInvoiceHeadersByDocNumJoinDoctor(List<int> docNum)
        {
            var query = (from invoice in this.databaseContext.InvoiceHeaderModel.Where(x => docNum.Contains(x.DocNum))
                         join doctor in this.databaseContext.ClientCatalogModel on invoice.CardCode equals doctor.ClientId
                         join doctordet in this.databaseContext.DoctorInfoModel.Where(x => x.AdressType == "S") on
                         new
                         {
                             DoctorId = invoice.CardCode,
                             Address = invoice.ShippingAddressName
                         }
                         equals
                         new
                         {
                             DoctorId = doctordet.CardCode,
                             Address = doctordet.NickName
                         }
                         into detalleDireccion
                         from dop in detalleDireccion.DefaultIfEmpty()
                         select new InvoiceHeaderModel
                         {
                             Address = invoice.Address,
                             Canceled = invoice.Canceled,
                             CardCode = invoice.CardCode,
                             Cliente = dop.Address2 ?? string.Empty,
                             Comments = invoice.Comments,
                             CommentsInvoice = invoice.CommentsInvoice,
                             DocNum = invoice.DocNum,
                             FechaInicio = invoice.FechaInicio,
                             InvoiceId = invoice.InvoiceId,
                             InvoiceStatus = invoice.InvoiceStatus,
                             Medico = doctor.AliasName,
                             Refactura = invoice.Refactura,
                             TrackingNumber = invoice.TrackingNumber,
                             TransportCode = invoice.TransportCode,
                             SalesPrsonId = invoice.SalesPrsonId,
                             TypeOrder = invoice.TypeOrder,
                             UpdateDate = invoice.UpdateDate,
                         });

            return (await this.RetryQuery<InvoiceHeaderModel>(query)).ToList();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<DeliveryDetailModel>> GetDeliveryDetailByDocEntry(List<int> ordersId)
        {
            return await this.RetryQuery<DeliveryDetailModel>(this.databaseContext.DeliveryDetailModel.Where(x => ordersId.Contains(x.DeliveryId)).Select(x => new DeliveryDetailModel
            {
                BaseEntry = x.BaseEntry != null ? x.BaseEntry : 0,
                Container = x.Container,
                DeliveryId = x.DeliveryId,
                Description = x.Description,
                DocDate = x.DocDate,
                InvoiceId = x.InvoiceId,
                LineNum = x.LineNum,
                LineStatus = x.LineStatus,
                ProductoId = x.ProductoId,
                Quantity = x.Quantity,
            }));
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<DeliveryDetailModel>> GetDeliveryDetailByDocEntryJoinProduct(List<int> ordersId)
        {
            var query = from deliveryDet in this.databaseContext.DeliveryDetailModel.Where(x => ordersId.Contains(x.DeliveryId))
                        join product in this.databaseContext.ProductoModel on deliveryDet.ProductoId equals product.ProductoId
                        where product.IsWorkableProduct == "Y"
                        select new DeliveryDetailModel
                        {
                            BaseEntry = deliveryDet.BaseEntry,
                            Container = deliveryDet.Container,
                            DeliveryId = deliveryDet.DeliveryId,
                            Description = deliveryDet.Description,
                            DocDate = deliveryDet.DocDate,
                            InvoiceId = deliveryDet.InvoiceId,
                            LineNum = deliveryDet.LineNum,
                            LineStatus = deliveryDet.LineStatus,
                            ProductoId = deliveryDet.ProductoId,
                            Quantity = deliveryDet.Quantity,
                        };
            return await this.RetryQuery<DeliveryDetailModel>(query);
        }

        /// <inheritdoc/>
        public async Task<List<Batches>> GetBatchByProductDistNumber(List<string> productCode, List<string> batchCode)
        {
            var batches = await this.RetryQuery<Batches>(this.databaseContext.Batches.Where(x => batchCode.Contains(x.DistNumber)));

            return batches.Where(x => productCode.Contains(x.ItemCode)).ToList();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<ClientCatalogModel>> GetClientsById(List<string> clientId)
        {
            return await this.RetryQuery<ClientCatalogModel>(this.databaseContext.ClientCatalogModel.Where(x => clientId.Contains(x.ClientId)));
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Repartidores>> GetDeliveryCompanyById(List<short> ids)
        {
            return await this.RetryQuery<Repartidores>(this.databaseContext.Repartidores.Where(x => ids.Contains(x.TrnspCode)));
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<DeliveyJoinOrderModel>> GetDeliveryByInvoiceId(List<int?> invoices)
        {
            var query = from deliveryDetail in this.databaseContext.DeliveryDetailModel.Where(x => invoices.Contains(x.InvoiceId))
                        join order in this.databaseContext.OrderModel on deliveryDetail.BaseEntry equals order.PedidoId
                        select new DeliveyJoinOrderModel
                        {
                            DeliveryId = deliveryDetail.DeliveryId,
                            PedidoDxpId = order.DocNumDxp,
                            InvoiceId = deliveryDetail.InvoiceId,
                            PedidoId = order.PedidoId,
                        };

            return (await this.RetryQuery<DeliveyJoinOrderModel>(query)).ToList();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<ProductoModel>> GetAllLineProducts()
        {
            return await this.RetryQuery<ProductoModel>(this.databaseContext.ProductoModel.Where(x => x.IsMagistral == "N" && x.IsLine == "Y"));
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<DetallePedidoModel>> GetDetailsbyDocDate(DateTime initDate, DateTime endDate)
        {
            return await this.RetryQuery<DetallePedidoModel>(this.databaseContext.DetallePedido.Where(x => x.DocDate >= initDate && x.DocDate <= endDate));
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<ProductoModel>> GetProductByIds(List<string> itemCode)
        {
            var query = (from p in this.databaseContext.ProductoModel.Where(x => itemCode.Contains(x.ProductoId))
                         join g in this.databaseContext.CatalogProductModel on p.ProductGroupId equals g.ProductGroupId
                         select new ProductoModel
                         {
                             BarCode = p.BarCode,
                             IsLine = p.IsLine,
                             IsMagistral = p.IsMagistral,
                             LargeDescription = p.LargeDescription,
                             ManagedBatches = p.ManagedBatches,
                             NeedsCooling = p.NeedsCooling,
                             OnHand = p.OnHand,
                             ProductGroupId = p.ProductGroupId,
                             ProductoId = p.ProductoId,
                             ProductoName = p.ProductoName,
                             Unit = p.Unit,
                             Groupname = g.CatalogName,
                         });

            return await this.RetryQuery<ProductoModel>(query);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Repartidores>> GetDeliveryCompanies()
        {
            return await this.RetryQuery<Repartidores>(this.databaseContext.Repartidores.Where(x => !string.IsNullOrEmpty(x.TrnspName)));
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Batches>> GetBatchesByProdcuts(List<string> productsIds)
        {
            return await this.RetryQuery<Batches>(this.databaseContext.Batches.Where(x => productsIds.Contains(x.ItemCode)));
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<InvoiceHeaderModel>> GetInvoiceByDocDate(DateTime date)
        {
            return await this.RetryQuery<InvoiceHeaderModel>(this.databaseContext.InvoiceHeaderModel.Where(x => x.FechaInicio >= date));
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<DeliverModel>> GetDeliveryByDocDateJoinDoctor(DateTime initDate, DateTime endDate)
        {
            var query = (from delivery in this.databaseContext.DeliverModel.Where(x => x.FechaInicio >= initDate && x.FechaInicio <= endDate)
                         join doctor in this.databaseContext.ClientCatalogModel on delivery.CardCode equals doctor.ClientId
                         join doctordet in this.databaseContext.DoctorInfoModel.Where(x => x.AdressType == "S") on
                         new
                         {
                             DoctorId = delivery.CardCode,
                             Address = delivery.ShippingAddressName
                         }
                         equals
                         new
                         {
                             DoctorId = doctordet.CardCode,
                             Address = doctordet.NickName
                         }
                         into detalleDireccion
                         from dop in detalleDireccion.DefaultIfEmpty()
                         select new DeliverModel
                         {
                             Address = delivery.Address,
                             Canceled = delivery.Canceled,
                             CardCode = delivery.CardCode,
                             Cliente = dop.Address2 ?? string.Empty,
                             DeliveryStatus = delivery.DeliveryStatus,
                             DocNum = delivery.DocNum,
                             FechaInicio = delivery.FechaInicio,
                             Medico = doctor.AliasName,
                             PedidoId = delivery.PedidoId,
                             TypeOrder = delivery.TypeOrder,
                         });

            return (await this.RetryQuery<DeliverModel>(query)).ToList();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<InvoiceHeaderModel>> GetInvoiceHeadersByDocDateJoinDoctor(DateTime initDate, DateTime endDate)
        {
            var query = (from invoice in this.databaseContext.InvoiceHeaderModel.Where(x => x.FechaInicio >= initDate && x.FechaInicio <= endDate)
                         join doctor in this.databaseContext.ClientCatalogModel on invoice.CardCode equals doctor.ClientId
                         join doctordet in this.databaseContext.DoctorInfoModel.Where(x => x.AdressType == "S") on
                         new
                         {
                             DoctorId = invoice.CardCode,
                             Address = invoice.ShippingAddressName
                         }
                         equals
                         new
                         {
                             DoctorId = doctordet.CardCode,
                             Address = doctordet.NickName
                         }
                         into detalleDireccion
                         from dop in detalleDireccion.DefaultIfEmpty()
                         select new InvoiceHeaderModel
                         {
                             Address = invoice.Address,
                             Canceled = invoice.Canceled,
                             CardCode = invoice.CardCode,
                             Cliente = dop.Address2 ?? string.Empty,
                             Comments = invoice.Comments,
                             CommentsInvoice = invoice.CommentsInvoice,
                             DocNum = invoice.DocNum,
                             FechaInicio = invoice.FechaInicio,
                             InvoiceId = invoice.InvoiceId,
                             InvoiceStatus = invoice.InvoiceStatus,
                             Medico = doctor.AliasName,
                             Refactura = invoice.Refactura,
                             TrackingNumber = invoice.TrackingNumber,
                             TransportCode = invoice.TransportCode,
                             SalesPrsonId = invoice.SalesPrsonId,
                             TypeOrder = invoice.TypeOrder,
                             UpdateDate = invoice.UpdateDate,
                         });

            return (await this.RetryQuery<InvoiceHeaderModel>(query)).ToList();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<InvoiceDetailModel>> GetInvoiceDetailByBaseEntry(List<int> baseEntry)
        {
            return await this.RetryQuery<InvoiceDetailModel>(this.databaseContext.InvoiceDetailModel.Where(x => x.BaseEntry != null && baseEntry.Contains(x.BaseEntry.Value)));
        }

        public async Task<IEnumerable<InvoiceDetailModel>> GetInvoiceDetailByBaseEntryJoinProduct(List<int> baseEntry)
        {
            var query = from invoicedet in this.databaseContext.InvoiceDetailModel.Where(x => x.BaseEntry != null && baseEntry.Contains(x.BaseEntry.Value))
                        join product in this.databaseContext.ProductoModel on invoicedet.ProductoId equals product.ProductoId
                        where product.IsWorkableProduct == "Y"
                        select new InvoiceDetailModel
                        {
                            BaseEntry = invoicedet.BaseEntry,
                            Container = invoicedet.Container,
                            Description = invoicedet.Description,
                            DocDate = invoicedet.DocDate,
                            InvoiceId = invoicedet.InvoiceId,
                            LineNum = invoicedet.LineNum,
                            ProductoId = invoicedet.ProductoId,
                            Quantity = invoicedet.Quantity,
                        };

            return await this.RetryQuery<InvoiceDetailModel>(query);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<CompleteOrderModel>> GetAllOrdersWIthDetailByIds(List<int> ids)
        {
            var query = (from order in this.databaseContext.OrderModel.Where(x => ids.Contains(x.DocNum))
                         join detalle in this.databaseContext.DetallePedido on order.PedidoId equals detalle.PedidoId
                         into DetalleOrden from dp in DetalleOrden.DefaultIfEmpty()
                         join doctor in this.databaseContext.ClientCatalogModel on order.Codigo equals doctor.ClientId
                         join doctordet in this.databaseContext.DoctorInfoModel.Where(x => x.AdressType == "S") on
                         new
                         {
                             DoctorId = order.Codigo,
                             Address = order.ShippingAddressName
                         }
                         equals
                         new
                         {
                             DoctorId = doctordet.CardCode,
                             Address = doctordet.NickName
                         }
                         into detalleDireccion
                         from dop in detalleDireccion.DefaultIfEmpty()
                         select new CompleteOrderModel
                         {
                             DocNum = order.DocNum,
                             Cliente = dop.Address2 ?? string.Empty,
                             Codigo = order.Codigo,
                             Medico = doctor.AliasName,
                             FechaInicio = order.FechaInicio.ToString("dd/MM/yyyy"),
                             FechaFin = order.FechaFin.ToString("dd/MM/yyyy"),
                             PedidoStatus = order.PedidoStatus,
                             IsChecked = false,
                             Detalles = dp,
                             OrderType = order.OrderType,
                             Address = order.Address,
                             PedidoMuestra = order.PedidoMuestra,
                             Comments = order.Comments,
                             AsesorId = order.AsesorId,
                         });

            return await this.RetryQuery<CompleteOrderModel>(query);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<OrderModel>> GetOrderModelByDocDateJoinDoctor(DateTime initDate, DateTime endDate)
        {
            var query = (from order in this.databaseContext.OrderModel.Where(x => x.FechaInicio >= initDate && x.FechaInicio <= endDate)
                         join doctor in this.databaseContext.ClientCatalogModel on order.Codigo equals doctor.ClientId
                         join doctordet in this.databaseContext.DoctorInfoModel.Where(x => x.AdressType == "S") on
                         new
                         {
                             DoctorId = order.Codigo,
                             Address = order.ShippingAddressName
                         }
                         equals
                         new
                         {
                             DoctorId = doctordet.CardCode,
                             Address = doctordet.NickName
                         }
                         into detalleDireccion
                         from dop in detalleDireccion.DefaultIfEmpty()
                         select new OrderModel
                         {
                             Address = order.Address,
                             Canceled = order.Canceled,
                             Codigo = order.Codigo,
                             Comments = order.Comments,
                             DocNum = order.DocNum,
                             FechaInicio = order.FechaInicio,
                             PedidoId = order.PedidoId,
                             Medico = doctor.AliasName,
                             OrderType = order.OrderType,
                             PedidoMuestra = order.PedidoMuestra,
                             Cliente = dop.Address2 ?? string.Empty,
                         });

            return await this.RetryQuery<OrderModel>(query);
        }

        public async Task<List<CompleteRecepcionPedidoDetailModel>> GetSapOrderDetailForAlmacenRecepcionById(List<int> orderIds)
        {
            var query = (from order in this.databaseContext.OrderModel.Where(x => orderIds.Contains(x.DocNum))
                         join detalle in this.databaseContext.DetallePedido on order.PedidoId equals detalle.PedidoId
                         into DetalleOrden
                         from dp in DetalleOrden.DefaultIfEmpty()
                         join doctor in this.databaseContext.ClientCatalogModel on order.Codigo equals doctor.ClientId
                         join doctordet in this.databaseContext.DoctorInfoModel.Where(x => x.AdressType == "S") on
                         new
                         {
                             DoctorId = order.Codigo,
                             Address = order.ShippingAddressName
                         }
                         equals
                         new
                         {
                             DoctorId = doctordet.CardCode,
                             Address = doctordet.NickName
                         }
                         into detalleDireccion
                         from dop in detalleDireccion.DefaultIfEmpty()
                         join o in this.databaseContext.OrdenFabricacionModel on
                         new
                         {
                             Pedido = dp.PedidoId,
                             ItemCode = dp.ProductoId
                         }
                         equals
                         new
                         {
                             Pedido = o.PedidoId,
                             ItemCode = o.ProductoId
                         }
                         into DetallePedido
                         from dpf in DetallePedido.DefaultIfEmpty()
                         join p in this.databaseContext.ProductoModel on dp.ProductoId equals p.ProductoId
                         where p.IsWorkableProduct ==  "Y"
                         select new CompleteRecepcionPedidoDetailModel
                         {
                             DocNum = order.DocNum,
                             Cliente = dop.Address2 ?? string.Empty,
                             Medico = doctor.AliasName,
                             FechaInicio = order.FechaInicio,
                             Detalles = dp,
                             TypeOrder = order.OrderType,
                             Address = order.Address,
                             PedidoMuestra = order.PedidoMuestra,
                             Comments = order.Comments,
                             Producto = p,
                             FabricationOrder = dpf != null ? dpf.OrdenId.ToString() : string.Empty,
                             Canceled = order.Canceled,
                         });

            return (await this.RetryQuery<CompleteRecepcionPedidoDetailModel>(query)).ToList();
        }

        /// <summary>
        /// Gets the retry.
        /// </summary>
        /// <typeparam name="T">the type.</typeparam>
        /// <param name="query">the query.</param>
        /// <returns>the data.</returns>
        private async Task<IEnumerable<T>> RetryQuery<T>(IQueryable<T> query)
        {
            try
            {                
                return await query.ToListAsync();
            }
            catch(Exception ex)
            {
                this.logger.Error($"Error al correr query {ex.Message} --- {ex.StackTrace}");
                return await query.ToListAsync();
            }
        }

        private async Task<List<CompleteDetalleFormulaModel>> GetComponentes(List<ProductoModel> products, string warehouse)
        {
            var listToReturn = new List<CompleteDetalleFormulaModel>();

            if (!products.Any())
            {
                return new List<CompleteDetalleFormulaModel>();
            }

            var listIds = products.Select(x => x.ProductoId).ToList();
            var almacen = await this.databaseContext.ItemWarehouseModel.Where(x => listIds.Contains(x.ItemCode) && x.WhsCode == warehouse).ToListAsync();

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
                    Warehouse = warehouse,
                    Stock = p.OnHand,
                    WarehouseQuantity = datoToAssign.OnHand,
                });
            });

            return listToReturn;
        }
    }
}
