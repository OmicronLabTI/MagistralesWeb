// <summary>
// <copyright file="SapDao.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.DataAccess.DAO.Sap
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Omicron.SapAdapter.DataAccess.Extensions;
    using Omicron.SapAdapter.Entities.Context;
    using Omicron.SapAdapter.Entities.Model;
    using Omicron.SapAdapter.Entities.Model.BusinessModels;
    using Omicron.SapAdapter.Entities.Model.DbModels;
    using Omicron.SapAdapter.Entities.Model.JoinsModels;
    using Omicron.SapAdapter.Entities.Model.Wraps;
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
            var query = this.GetCompleteOrderyJoinDoctorQueryWrap()
                .Where(s => s.OrderModel.FechaInicio >= initDate && s.OrderModel.FechaInicio <= endDate)
                .GetCompleteOrdery();
            return await this.RetryQuery(query);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<CompleteOrderModel>> GetAllOrdersByIds(List<int> ids)
        {
            var query = this.GetCompleteOrderyJoinDoctorQueryWrap()
                .Where(x => ids.Contains(x.OrderModel.DocNum))
                .AsNoTracking()
                .GetCompleteOrdery();
            return await this.RetryQuery(query);
        }

        /// <summary>
        /// Get the orders.
        /// </summary>
        /// <returns>get the orders.</returns>
        public async Task<IEnumerable<CompleteOrderModel>> GetAllOrdersById(int init, int end)
        {
            var query = this.GetCompleteOrderyJoinDoctorQueryWrap()
                .Where(s => s.OrderModel.DocNum >= init && s.OrderModel.DocNum <= end)
                .AsNoTracking()
                .GetCompleteOrdery();
            return await this.RetryQuery(query);
        }

        /// <inheritdoc/>
        public async Task<List<CompleteOrderModel>> GetAllOrdersByDocNumDxp(string docNumDxp)
        {
            var query = this.GetCompleteOrderyJoinDoctorQueryWrap()
                .Where(s => s.OrderModel.DocNumDxp == docNumDxp || s.OrderModel.DocNumDxp.Contains(docNumDxp))
                .AsNoTracking()
                .GetCompleteOrdery();
            return (await this.RetryQuery(query)).ToList();
        }

        /// <inheritdoc/>
        public async Task<List<OrderModel>> GetOrdersByDocNumDxp(List<string> docNumDxp)
        {
            return await this.databaseContext.OrderModel.Where(x => docNumDxp.Contains(x.DocNumDxp)).AsNoTracking().ToListAsync();
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

            return await this.RetryQuery(query);
        }

        /// <summary>
        /// gets the asesors by salesOrderId.
        /// </summary>
        /// <param name="docsEntry">the list of salesOrderId.</param>        
        /// <returns>the data.</returns>
        public async Task<IEnumerable<SalesPersonModel>> GetAsesorWithEmailByIdsFromTheAsesor(List<int> salesPrsonId)
        {
            return await this.RetryQuery(this.databaseContext.SalesPersonModel.Where(x => salesPrsonId.Contains(x.AsesorId)).AsNoTracking());

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
                         join firm in this.databaseContext.ProductFirmModel on p.ProductFirmCode equals firm.ProductFirmCode
                         into productFirm
                         from fm in productFirm.DefaultIfEmpty()
                         join ped in this.databaseContext.OrderModel on d.PedidoId equals ped.PedidoId
                         join g in this.databaseContext.CatalogProductModel on p.ProductGroupId equals g.ProductGroupId
                         where p.IsMagistral == "Y"
                         select new CompleteDetailOrderModel
                         {
                             OrdenFabricacionId = dp == default ? 0 : dp.OrdenId,
                             CodigoProducto = d.ProductoId,
                             DescripcionProducto = p.LargeDescription,
                             DescripcionCorta = p.ProductoName,
                             QtyPlanned = dp == default ? 0 : dp.Quantity,
                             QtyPlannedDetalle = (int)d.Quantity,
                             FechaOf = dp.PostDate.HasValue ? dp.PostDate.Value.ToString("dd/MM/yyyy") : string.Empty,
                             FechaOfFin = dp.DueDate.HasValue ? dp.DueDate.Value.ToString("dd/MM/yyyy") : string.Empty,
                             Status = dp == default ? string.Empty : dp.Status,
                             IsChecked = false,
                             CreatedDate = dp.CreatedDate.HasValue ? dp.CreatedDate.Value : null,
                             Label = d.Label,
                             NeedsCooling = p.NeedsCooling,
                             Container = d.Container,
                             PatientName = ped.Patient ?? string.Empty,
                             PedidoId = d.PedidoId ?? 0,
                             CatalogGroup = g.CatalogName,
                             IsOmigenomics = g.CatalogName.ToLower() == "omigenomics",
                             ProductFirmName = fm == default ? string.Empty : fm.ProductFirmName,
                         }).AsNoTracking();

            return await this.RetryQuery(query);
        }

        /// <inheritdoc/>
        public async Task<List<OrderModel>> GetOrdersById(int pedidoID)
        {
            return (await this.RetryQuery(this.databaseContext.OrderModel.Where(x => x.PedidoId == pedidoID))).ToList();
        }

        /// <inheritdoc/>
        public async Task<List<OrderModel>> GetOrdersById(List<int> pedidoID)
        {
            return (await this.RetryQuery(this.databaseContext.OrderModel.Where(x => pedidoID.Contains(x.PedidoId)).AsNoTracking())).ToList();
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
                            PedidoMuestra = order.PedidoMuestra,
                            PedidoId = order.PedidoId,
                            PedidoStatus = order.PedidoStatus,
                            ShippingAddressName = order.ShippingAddressName,
                            ShippingCost = order.ShippingCost,
                            Medico = doctor.AliasName,
                            IsPackage = order.IsPackage,
                            IsOmigenomics = order.IsOmigenomics,
                            IsSecondary = order.IsSecondary,
                            ProffesionalLicense = doctor.ProffesionalLicense,
                        };

            return (await this.RetryQuery(query)).ToList();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<OrdenFabricacionModel>> GetProdOrderByOrderProduct(int pedidoId, string productId, List<string> datasources)
        {
            return (await (from order in this.databaseContext.OrdenFabricacionModel.Where(x => x.PedidoId == pedidoId && x.ProductoId == productId && datasources.Contains(x.DataSource))
                           join prod in this.databaseContext.ProductoModel on order.ProductoId equals prod.ProductoId
                           join catalog in this.databaseContext.CatalogProductModel on prod.ProductGroupId equals catalog.ProductGroupId
                           select new { order, catalog })
            .ToListAsync())
            .Select(x =>
            {
                x.order.IsOmigenomics = x.catalog.CatalogName.ToLower() == "omigenomics";
                return x.order;
            });
        }
        /// <inheritdoc/>
        public async Task<int> GetlLastIsolatedProductionOrderId(string productId, string uniqueId, List<string> datasources)
        {
            var query = await this.databaseContext.OrdenFabricacionModel
                                    .Where(
                                        x => x.ProductoId.Equals(productId) &&
                                        x.Comments.Equals(uniqueId) &&
                                        string.IsNullOrEmpty(x.CardCode) &&
                                        datasources.Contains(x.DataSource))
                                    .MaxAsync(x => x.OrdenId);
            return query;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<OrdenFabricacionModel>> GetFabOrderById(List<int> pedidoId)
        {
            return (await (from order in this.databaseContext.OrdenFabricacionModel.Where(x => pedidoId.Contains(x.OrdenId))
                           join prod in this.databaseContext.ProductoModel on order.ProductoId equals prod.ProductoId
                           join catalog in this.databaseContext.CatalogProductModel on prod.ProductGroupId equals catalog.ProductGroupId
                           select new { order, catalog })
            .ToListAsync())
            .Select(x =>
            {
                x.order.IsOmigenomics = x.catalog.CatalogName.ToLower() == "omigenomics";
                return x.order;
            });
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<OrdenFabricacionModel>> GetFabOrderBySalesOrderId(List<int> salesOrderIds)
        {
            var query = await this.databaseContext.OrdenFabricacionModel.Where(x => salesOrderIds.Contains(x.PedidoId.Value)).AsNoTracking().ToListAsync();
            return query;
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

            return await this.RetryQuery(query);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<DetalleFormulaModel>> GetDetalleFormulaByProdOrdId(List<int> ordersId)
        {
            return await this.RetryQuery(this.databaseContext.DetalleFormulaModel.Where(x => ordersId.Contains(x.OrderFabId)).AsNoTracking());
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
            var products = await this.RetryQuery(this.databaseContext.ProductoModel.Where(x => x.ProductoId.ToLower().Contains(value)).AsNoTracking());
            return await this.GetComponentes(products.ToList(), warehouse);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<CompleteDetalleFormulaModel>> GetItemsByContainsItemCode(List<string> value, string warehouse)
        {
            var products = await this.RetryQuery(this.databaseContext.ProductoModel.Where(x => value.Contains(x.ProductoId.ToLower())).AsNoTracking());
            return await this.GetComponentes(products.ToList(), warehouse);
        }

        /// <summary>
        /// Gets the value for the item code by filters. 
        /// </summary>
        /// <param name="value">the value to look.</param>
        /// <returns>the value.</returns>
        public async Task<IEnumerable<CompleteDetalleFormulaModel>> GetItemsByContainsDescription(string value, string warehouse)
        {
            var products = await this.RetryQuery(this.databaseContext.ProductoModel.Where(x => x.ProductoName.ToLower().Contains(value)).AsNoTracking());
            return await this.GetComponentes(products.ToList(), warehouse);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<DetallePedidoModel>> GetPedidoByIdJoinProduct(int pedidoId)
        {
            var query = from saleOrder in this.databaseContext.OrderModel.Where(x => x.PedidoId == pedidoId)
                        join saleDetail in this.databaseContext.DetallePedido on saleOrder.PedidoId equals saleDetail.PedidoId
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
                            CanceledOrder = saleOrder.Canceled
                        };
            return await this.RetryQuery(query);
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

            return await this.RetryQuery(query);
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

            return await this.RetryQuery(query);
        }

        /// <summary>
        /// Gets the item by code.
        /// </summary>
        /// <param name="itemCode">the item code.</param>
        /// <returns>the data.</returns>
        public async Task<IEnumerable<ProductoModel>> GetProductById(string itemCode)
        {
            return await this.RetryQuery(this.databaseContext.ProductoModel.Where(x => x.ProductoId == itemCode).AsNoTracking());
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
            var productIds = components.Select(c => c.ProductId).Distinct();
            var warehouseIds = components.Select(c => c.Warehouse).Distinct();

            var querybatches = await this.databaseContext.BatchesQuantity
                .Where(b => productIds.Contains(b.ItemCode) && warehouseIds.Contains(b.WhsCode) && b.Quantity > 0)
                .AsNoTracking()
                .ToListAsync();


            var validBatches = querybatches.Select(x => x.SysNumber);

            var batches = await this.databaseContext.Batches
                .Where(x => productIds.Contains(x.ItemCode)
                        && validBatches.Contains(x.SysNumber))
                .AsNoTracking()
                .ToListAsync();

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
                    FechaExpDateTime = batch.ExpDate,
                    ItemCode = x.ItemCode,
                });
            });

            return listToReturn;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<BatchTransacitions>> GetBatchesTransactionByOrderItem(string itemCode, int orderId)
        {
            return await this.RetryQuery(this.databaseContext.BatchTransacitions.Where(x => x.DocNum == orderId && x.ItemCode.Equals(itemCode)).AsNoTracking());
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<BatchTransacitions>> GetBatchesTransactionByOrderItem(List<int> orderId)
        {
            return await this.RetryQuery(this.databaseContext.BatchTransacitions.Where(x => orderId.Contains(x.DocNum)).AsNoTracking());
        }

        /// <summary>
        /// Gets the record from ITL1 by log entry.
        /// </summary>
        /// <param name="logEntry">the log entry.</param>
        /// <returns>the data.</returns>
        public async Task<IEnumerable<BatchesTransactionQtyModel>> GetBatchTransationsQtyByLogEntry(List<int> logEntry)
        {
            return await this.RetryQuery(this.databaseContext.BatchesTransactionQtyModel.Where(x => logEntry.Contains(x.LogEntry)).AsNoTracking());
        }

        /// <inheritdoc/>
        public async Task<string> GetMaxBatchCode(string batchCodePattern, string productCode)
        {
            return await (from batch in this.databaseContext.Batches
                          where batch.ItemCode.Equals(productCode)
                          && EF.Functions.Like(batch.DistNumber, batchCodePattern)
                          orderby batch.DistNumber descending
                          select batch.DistNumber).Take(1).FirstOrDefaultAsync();
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
                results = from product in results
                          where EF.Functions.Like(product.ProductoId, $"%{criterial}%")
                          || EF.Functions.Like(product.ProductoName, $"%{criterial}%")
                          orderby product.ProductoId ascending
                          select product;
            }

            return await results.AsNoTracking().ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<List<AttachmentModel>> GetAttachmentsById(List<int> ids)
        {
            return await this.databaseContext.AttachmentModel.Where(x => ids.Contains(x.AbsEntry)).AsNoTracking().ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<CompleteAlmacenOrderModel>> GetAllOrdersForAlmacen(DateTime startDate, DateTime endDate)
        {
            var query = this.GetAllOrdersForAlmacenQuery(startDate, endDate);
            return await this.RetryQuery(query);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<CompleteAlmacenOrderModel>> GetAllOrdersForAlmacenDxp(DateTime startDate, DateTime endDate)
        {
            var query = this.GetAllOrdersForAlmacenQuery(startDate, endDate).Where(x => !string.IsNullOrEmpty(x.DocNumDxp));
            return await this.RetryQuery(query);
        }

        public async Task<IEnumerable<CompleteAlmacenOrderModel>> GetAllOrdersForAlmacenByListIds(List<int> saleordersIds)
        {
            var query = (from order in this.databaseContext.OrderModel.Where(x => saleordersIds.Contains(x.DocNum))
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
                         where (order.PedidoStatus == "O" || order.Canceled == "Y") && product.IsWorkableProduct == "Y"
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
                             IsPackage = order.IsPackage,
                             DocNumDxp = order.DocNumDxp,
                             IsOmigenomics = order.IsOmigenomics,
                         });

            return await this.RetryQuery(query);
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
                            IsPackage = order.IsPackage,
                            IsOmigenomics = order.IsOmigenomics,
                            IsSecondary = order.IsSecondary,
                        };
            return await this.RetryQuery(query);
        }


        /// <inheritdoc/>
        public async Task<IEnumerable<CompleteAlmacenOrderModel>> GetCountDxpOrdersByIds(List<string> dxpIds)
        {
            var query = (from order in this.databaseContext.OrderModel
                         join detail in this.databaseContext.DetallePedido on order.DocNum equals detail.PedidoId
                         join product in this.databaseContext.ProductoModel on detail.ProductoId equals product.ProductoId
                         where dxpIds.Contains(order.DocNumDxp)
                         select new CompleteAlmacenOrderModel
                         {
                             DocNumDxp = order.DocNumDxp,
                             DocNum = order.DocNum,
                             Detalles = detail,
                             IsWorkableProduct = product.IsWorkableProduct
                         });

            return await this.RetryQuery(query);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<CompleteAlmacenOrderModel>> GetAllOrdersForAlmacenByTypeOrder(string typeOrder, List<int> orderToLook, bool needOnlyDxp)
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
                             IsLine = product.IsLine,
                             IsMagistral = product.IsMagistral,
                             IsPackage = order.IsPackage,
                             DocNumDxp = order.DocNumDxp,
                             IsOmigenomics = order.IsOmigenomics,
                         });

            if (needOnlyDxp)
            {
                query.Where(x => !string.IsNullOrEmpty(x.DocNumDxp));
            }

            return await this.RetryQuery(query);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<DeliveryDetailModel>> GetDeliveryDetailBySaleOrder(List<int> ordersId)
        {
            return (await this.RetryQuery(this.databaseContext.DeliveryDetailModel.Where(x => x.BaseEntry.HasValue && ordersId.Contains(x.BaseEntry.Value)).AsNoTracking()));
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<DeliveryDetailModel>> GetCompleteDeliveryWithDetailBySaleOrder(List<int> ordersId)
        {
            var query = from deliveryDet in this.databaseContext.DeliveryDetailModel.Where(x => x.BaseEntry.HasValue && ordersId.Contains(x.BaseEntry.Value))
                        join deliverMod in this.databaseContext.DeliverModel on deliveryDet.DeliveryId equals deliverMod.DocNum
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
                            DocNumDxp = deliverMod.DocNumDxp,
                        };
            return (await this.RetryQuery(query));
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<DeliveryDetailModel>> GetDeliveryDetailBySaleOrderJoinProduct(List<int> ordersId)
        {
            var query = from deliveryDet in this.databaseContext.DeliveryDetailModel.Where(x => x.BaseEntry.HasValue && ordersId.Contains(x.BaseEntry.Value))
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
            return (await this.RetryQuery(query));
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<ProductoModel>> GetProductByCodeBar(string codeBar)
        {
            return await this.databaseContext.ProductoModel.Where(x => x.BarCode.Equals(codeBar)).AsNoTracking().ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<DeliverModel>> GetDeliveryModelByDocNum(List<int> docuNums)
        {
            return await this.RetryQuery(this.databaseContext.DeliverModel.Where(x => docuNums.Contains(x.DocNum)).AsNoTracking());
        }

        /// <inheritdoc/>
        public async Task<List<DeliverModel>> GetDeliveryModelByDocNumJoinDoctor(List<int> docuNums)
        {
            var query = this.GetDeliveryJoinDoctorQuery().Where(x => docuNums.Contains(x.DocNum));
            return (await this.RetryQuery(query)).ToList();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<DetallePedidoModel>> GetDetailByDocNum(List<int> docuNums)
        {
            return await this.RetryQuery(this.databaseContext.DetallePedido.Where(x => docuNums.Contains(x.PedidoId.Value)).AsNoTracking());
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<InvoiceHeaderModel>> GetInvoiceHeaderByInvoiceId(List<int> docNums)
        {
            return await this.RetryQuery(this.databaseContext.InvoiceHeaderModel.Where(x => docNums.Contains(x.InvoiceId)).AsNoTracking());
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
                             IsPackage = invoice.IsPackage,
                             DocNumDxp = invoice.DocNumDxp,
                             ShippingAddressName = invoice.ShippingAddressName,
                             IsOmigenomics = invoice.IsOmigenomics,
                             IsSecondary = invoice.IsSecondary,
                             IsDeliveredInOffice = invoice.IsDeliveredInOffice,
                         });

            return (await this.RetryQuery(query)).ToList();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<CompleteInvoiceDetailModel>> GetInvoiceHeaderDetailByInvoiceIdJoinDoctor(List<int> docNums)
        {
            var query = (from invoice in this.databaseContext.InvoiceHeaderModel.Where(x => docNums.Contains(x.DocNum))
                         join detail in this.databaseContext.InvoiceDetailModel on invoice.InvoiceId equals detail.InvoiceId
                         join doctor in this.databaseContext.ClientCatalogModel on invoice.CardCode equals doctor.ClientId
                         join product in this.databaseContext.ProductoModel on detail.ProductoId equals product.ProductoId
                         where product.IsWorkableProduct == "Y"
                         select new CompleteInvoiceDetailModel
                         {
                             InvoiceHeader = invoice,
                             Detail = detail,
                             Cliente = doctor.AliasName ?? string.Empty,
                             Medico = doctor.AliasName ?? string.Empty,
                         });

            return (await this.RetryQuery(query)).ToList();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<InvoiceDetailModel>> GetInvoiceDetailByDocEntry(List<int> docEntry)
        {
            return await this.RetryQuery(this.databaseContext.InvoiceDetailModel.Where(x => docEntry.Contains(x.InvoiceId)).AsNoTracking());
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
            return await this.RetryQuery(query);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<InvoiceHeaderModel>> GetInvoiceHeadersByDocNum(List<int> docNum)
        {
            return await this.RetryQuery(this.databaseContext.InvoiceHeaderModel.Where(x => docNum.Contains(x.DocNum)).AsNoTracking());
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
                             Cliente = doctor.AliasName,
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
                             ClientEmail = doctor.Email,
                             ClientPhone = doctor.Phone,
                             IsPackage = invoice.IsPackage,
                             DocNumDxp = invoice.DocNumDxp,
                             DoctorPhoneNumber = dop.GlblLocNum,
                             ShippingAddressName = invoice.ShippingAddressName,
                             IsOmigenomics = invoice.IsOmigenomics,
                             IsSecondary = invoice.IsSecondary,
                             IsDeliveredInOffice = invoice.IsDeliveredInOffice,
                         });

            return (await this.RetryQuery(query)).ToList();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<DeliveryDetailModel>> GetDeliveryDetailByDocEntry(List<int> ordersId)
        {
            return await this.RetryQuery(this.databaseContext.DeliveryDetailModel.Where(x => ordersId.Contains(x.DeliveryId)).Select(x => new DeliveryDetailModel
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
                            Producto = product
                        };
            return await this.RetryQuery(query);
        }

        /// <inheritdoc/>
        public async Task<List<Batches>> GetBatchByProductDistNumber(List<string> productCode, List<string> batchCode)
        {
            var batches = await this.RetryQuery(this.databaseContext.Batches.Where(x => batchCode.Contains(x.DistNumber)).AsNoTracking());

            return batches.Where(x => productCode.Contains(x.ItemCode)).ToList();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Repartidores>> GetDeliveryCompanyById(List<short> ids)
        {
            return await this.RetryQuery(this.databaseContext.Repartidores.Where(x => ids.Contains(x.TrnspCode)).AsNoTracking());
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

            return (await this.RetryQuery(query)).ToList();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<ProductoModel>> GetAllLineProducts()
        {
            return await this.RetryQuery(this.databaseContext.ProductoModel.Where(x => x.IsMagistral == "N" && x.IsLine == "Y").AsNoTracking());
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<DetallePedidoModel>> GetDetailsbyDocDate(DateTime initDate, DateTime endDate)
        {
            return await this.RetryQuery(this.databaseContext.DetallePedido.Where(x => x.DocDate >= initDate && x.DocDate <= endDate).AsNoTracking());
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
                             IsWorkableProduct = p.IsWorkableProduct,
                             IsPackage = p.IsPackage,
                         });

            return await this.RetryQuery(query);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Repartidores>> GetDeliveryCompanies()
        {
            return await this.RetryQuery(this.databaseContext.Repartidores.Where(x => !string.IsNullOrEmpty(x.TrnspName)).AsNoTracking());
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Batches>> GetBatchesByProdcuts(List<string> productsIds)
        {
            return await this.RetryQuery(this.databaseContext.Batches.Where(x => productsIds.Contains(x.ItemCode)).AsNoTracking());
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<InvoiceHeaderModel>> GetInvoiceByDocDate(DateTime date)
        {
            return await this.RetryQuery(this.databaseContext.InvoiceHeaderModel.Where(x => x.FechaInicio >= date).AsNoTracking());
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<DeliverModel>> GetDeliveryByDocDateJoinDoctor(DateTime initDate, DateTime endDate)
        {
            var query = this.GetDeliveryJoinDoctorQuery()
                            .Where(x => x.FechaInicio >= initDate && x.FechaInicio <= endDate);

            return (await this.RetryQuery(query)).ToList();
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

            return (await this.RetryQuery(query)).ToList();
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

            return await this.RetryQuery(query);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<CompleteOrderModel>> GetAllOrdersWIthDetailByIds(List<int> ids)
        {
            var query = (from order in this.databaseContext.OrderModel.Where(x => ids.Contains(x.DocNum))
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

            return await this.RetryQuery(query);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<CompleteOrderModel>> GetAllOrdersWIthDetailByIdsJoinProduct(List<int> ids)
        {
            var query = this.GetAllOrdersWithDetailQuery().Where(x => ids.Contains(x.DocNum));
            return await this.RetryQuery(query);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<CompleteOrderModel>> GetAllOrdersWIthDetailByDocNumDxpJoinProduct(string DocNumDxp)
        {
            var query = this.GetAllOrdersWithDetailQuery().Where(o => o.DocNumDxp.Contains(DocNumDxp) || o.DocNumDxp == DocNumDxp);
            return await this.RetryQuery(query);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<CompleteOrderModel>> GetCountOrdersWIthDetailByDocNumDxpJoinProduct(string DocNumDxp)
        {
            return await this.RetryQuery(this.GetAllOrdersWithDetailQuery().AsNoTracking().Where(x => x.DocNumDxp == DocNumDxp));
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
                             IsPackage = order.IsPackage,
                         });

            return await this.RetryQuery(query);
        }

        /// <inheritdoc/>
        public async Task<List<CompleteRecepcionPedidoDetailModel>> GetSapOrderDetailForAlmacenRecepcionById(List<int> orderIds)
        {
            var query = (from order in this.databaseContext.OrderModel.Where(x => orderIds.Contains(x.DocNum))
                         join detalle in this.databaseContext.DetallePedido on order.PedidoId equals detalle.PedidoId
                         into DetalleOrden
                         from dp in DetalleOrden.DefaultIfEmpty()
                         join doctor in this.databaseContext.ClientCatalogModel on order.Codigo equals doctor.ClientId
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
                         where p.IsWorkableProduct == "Y"
                         select new CompleteRecepcionPedidoDetailModel
                         {
                             DocNum = order.DocNum,
                             Cliente = doctor.AliasName,
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
                             IsPackage = order.IsPackage,
                             DocNumDxp = order.DocNumDxp,
                             CardCode = order.Codigo,
                             DeliveryAddressId = order.ShippingAddressName,
                             IsOmigenomics = order.IsOmigenomics,
                         });

            return (await this.RetryQuery(query)).ToList();
        }

        /// <inheritdoc/>
        public async Task<List<CompleteDeliveryDetailModel>> GetDeliveryDetailForDeliveryById(List<int> delveryId)
        {
            var query = (from order in this.databaseContext.DeliverModel.Where(x => delveryId.Contains(x.DocNum))
                         join detalle in this.databaseContext.DeliveryDetailModel on order.PedidoId equals detalle.DeliveryId
                         into DetalleOrden
                         from dp in DetalleOrden.DefaultIfEmpty()
                         join doctor in this.databaseContext.ClientCatalogModel on order.CardCode equals doctor.ClientId
                         join p in this.databaseContext.ProductoModel on dp.ProductoId equals p.ProductoId
                         where p.IsWorkableProduct == "Y"
                         select new CompleteDeliveryDetailModel
                         {
                             DocNum = order.DocNum,
                             Cliente = doctor.AliasName,
                             Medico = doctor.AliasName,
                             FechaInicio = order.FechaInicio,
                             Detalles = dp,
                             TypeOrder = order.TypeOrder,
                             Address = order.Address,
                             Producto = p,
                             IsPackage = order.IsPackage,
                             DocNumDxp = order.DocNumDxp,
                             CardCode = order.CardCode,
                             DeliveryAddressId = order.ShippingAddressName,
                             IsOmigenomics = order.IsOmigenomics,
                             IsSecondary = order.IsSecondary,
                         });

            return (await this.RetryQuery(query)).ToList();
        }

        /// <inheritdoc/>
        public async Task<List<CompleteRawMaterialRequestModel>> GetCompleteRawMaterialRequestByFilters(DateTime initDate, DateTime endDate, string userId)
        {
            var query = (from header in this.databaseContext.RawMaterialRequestModel
                         join detail in this.databaseContext.RawMaterialRequestDetailModel on header.DocEntry equals detail.DocEntry
                         where header.DocDate >= initDate && header.DocDate <= endDate && !string.IsNullOrEmpty(header.RequestedUserId) && (header.RequestedUserId == userId || userId == null)
                         select new CompleteRawMaterialRequestModel
                         {
                             DocDate = header.DocDate.ToString("dd/MM/yyyy"),
                             DocEntry = header.DocEntry,
                             AdditionalComments = header.AdditionalComments,
                             IsCanceled = header.Canceled == "Y",
                             Description = detail.Description,
                             ItemCode = detail.ItemCode,
                             Quantity = detail.Quantity,
                             TargetWarehosue = detail.TargetWarehosue,
                             Unit = detail.Unit,
                             Status = header.Status,
                             RequestedUserId = header.RequestedUserId,
                         }).OrderByDescending(raw => raw.DocEntry).AsNoTracking();
            return (await this.RetryQuery(query)).ToList();
        }

        /// <inheritdoc/>
        public async Task<OrderModel> GetOrderInformationByTransaction(string idtransaction)
        {
            return await this.databaseContext.OrderModel.Where(x => x.DocNumDxp == idtransaction).FirstOrDefaultAsync();
        }

        /// <inheritdoc/>
        public async Task<List<ClientCatalogModel>> GetClientCatalogCardCode(List<string> cardCode)
        {
            return await this.databaseContext.ClientCatalogModel.Where(x => cardCode.Contains(x.ClientId)).ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<DeliveryDetailModel>> GetDeliveryDetailJoinProductByInvoicesIds(List<int> invoicesIds)
        {

            var query = from deliveryDet in this.databaseContext.DeliveryDetailModel
                        join product in this.databaseContext.ProductoModel on deliveryDet.ProductoId equals product.ProductoId
                        where 
                            product.IsWorkableProduct == "Y" &&
                            deliveryDet.InvoiceId.HasValue &&
                            invoicesIds.Contains((int)deliveryDet.InvoiceId)
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
                            Producto = product
                        };
            return await this.RetryQuery(query);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<InvoiceHeaderModel>> GetInvoiceHeaderJoinDoctorByDocNumsForSearchs(List<int> docNums)
        {
            var baseQuery = this.GetInvoiceHeaderJoinDoctorBaseQuery();
            return await baseQuery.Where(fac => docNums.Contains(fac.DocNum)).ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<InvoiceHeaderModel>> GetInvoiceHeaderJoinDoctorByDatesRangesForSearchs(DateTime startDate, DateTime endDate)
        {
            var baseQuery = this.GetInvoiceHeaderJoinDoctorBaseQuery();
            return await baseQuery
                .Where(fac =>
                    fac.FechaInicio >= startDate.Date && fac.FechaInicio <= endDate.Date)
                .ToListAsync();
        }

        private IQueryable<InvoiceHeaderModel> GetInvoiceHeaderJoinDoctorBaseQuery()
        {
            return from invoice in this.databaseContext.InvoiceHeaderModel
                   join doctor in this.databaseContext.ClientCatalogModel on invoice.CardCode equals doctor.ClientId
                   join doctordet in this.databaseContext.DoctorInfoModel.Where(x => x.AdressType == "S") on
                       new { DoctorId = invoice.CardCode, Address = invoice.ShippingAddressName }
                       equals
                       new { DoctorId = doctordet.CardCode, Address = doctordet.NickName }
                   into detalleDireccion
                   from dop in detalleDireccion.DefaultIfEmpty()
                   where invoice.Canceled == "N" && (string.IsNullOrEmpty(invoice.Refactura) || invoice.Refactura != "Si")
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
                       IsPackage = invoice.IsPackage,
                       DocNumDxp = invoice.DocNumDxp,
                       ShippingAddressName = invoice.ShippingAddressName,
                       IsOmigenomics = invoice.IsOmigenomics,
                       IsDeliveredInOffice = invoice.IsDeliveredInOffice,
                   };
        }



        /// <summary>
        /// Gets the retry.
        /// </summary>
        /// <typeparam name="T">the type.</typeparam>
        /// <param name="query">the query.</param>
        /// <returns>the data.</returns>
        private async Task<IEnumerable<T>> RetryQuery<T>(IQueryable<T> query)
        {
            return await query.ToListAsync();
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
                var datoToAssign = datoAlmacen ?? new ItemWarehouseModel();
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
                    IsLabel = !string.IsNullOrEmpty(p.IsLabel) && p.IsLabel.ToUpper() == "Y",
                });
            });

            return listToReturn;
        }

        private IQueryable<CompleteAlmacenOrderModel> GetAllOrdersForAlmacenQuery(DateTime startDate, DateTime endDate)
        {
            return (from order in this.databaseContext.OrderModel
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
                    where
                    order.FechaInicio >= startDate &&
                    order.FechaInicio <= endDate &&
                    (order.PedidoStatus == "O" || order.Canceled == "Y") &&
                    product.IsWorkableProduct == "Y"
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
                        IsPackage = order.IsPackage,
                        DocNumDxp = order.DocNumDxp,
                        IsOmigenomics = order.IsOmigenomics,
                        IsSecondary = order.IsSecondary,
                    });
        }

        /// <inheritdoc/>
        private IQueryable<CompleteOrderModel> GetAllOrdersWithDetailQuery()
        {
            return (from order in this.databaseContext.OrderModel
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
                    where product.IsWorkableProduct == "Y" && (
                order.IsOmigenomics == "1"
                || (string.IsNullOrEmpty(order.IsOmigenomics) && order.IsSecondary == "Y"))
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
                        Canceled = order.Canceled,
                        IsPackage = order.IsPackage,
                        DocNumDxp = order.DocNumDxp,
                        ShippingAddressName = order.ShippingAddressName,
                        IsOmigenomics = order.IsOmigenomics,
                    });
        }

        /// <inheritdoc/>
        private IQueryable<DeliverModel> GetDeliveryJoinDoctorQuery()
        {
            return (from delivery in this.databaseContext.DeliverModel
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
                        ShippingAddressName = delivery.ShippingAddressName,
                        Canceled = delivery.Canceled,
                        CardCode = delivery.CardCode,
                        Cliente = dop.Address2 ?? string.Empty,
                        DeliveryStatus = delivery.DeliveryStatus,
                        DocNum = delivery.DocNum,
                        FechaInicio = delivery.FechaInicio,
                        Medico = doctor.AliasName,
                        PedidoId = delivery.PedidoId,
                        TypeOrder = delivery.TypeOrder,
                        IsPackage = delivery.IsPackage,
                        DocNumDxp = delivery.DocNumDxp,
                        IsOmigenomics = delivery.IsOmigenomics,
                        IsSecondary = delivery.IsSecondary,
                    });
        }

        private IQueryable<CompleteOrderModelWrap> GetCompleteOrderyJoinDoctorQueryWrap()
        {
            return (from order in this.databaseContext.OrderModel
                    join detalle in this.databaseContext.DetallePedido on order.PedidoId equals detalle.PedidoId
                    into DetalleOrden
                    from dp in DetalleOrden.DefaultIfEmpty()
                    join producto in this.databaseContext.ProductoModel on dp.ProductoId equals producto.ProductoId
                    join asesor in this.databaseContext.AsesorModel on order.AsesorId equals asesor.AsesorId
                    join doctor in this.databaseContext.ClientCatalogModel on order.Codigo equals doctor.ClientId
                    where producto.IsMagistral == "Y"
                    select new CompleteOrderModelWrap
                    {
                        OrderModel = order,
                        ClientCatalogModel = doctor,
                        AsesorModel = asesor,
                        DetallePedidoModel = dp,
                    }).AsNoTracking();
        }
    }
}
