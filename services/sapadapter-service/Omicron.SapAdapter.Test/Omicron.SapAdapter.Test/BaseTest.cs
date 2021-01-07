// <summary>
// <copyright file="BaseTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Test
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Omicron.SapAdapter.Dtos.Models;
    using Omicron.SapAdapter.Entities.Model;
    using Omicron.SapAdapter.Entities.Model.AlmacenModels;
    using Omicron.SapAdapter.Entities.Model.DbModels;
    using Omicron.SapAdapter.Entities.Model.JoinsModels;

    /// <summary>
    /// Class Base Test.
    /// </summary>
    public abstract class BaseTest
    {
        /// <summary>
        /// Return the asesor.
        /// </summary>
        /// <returns>the asesor.</returns>
        public AsesorModel GetAsesorModel()
        {
            return new AsesorModel
            {
                AsesorId = 1,
                AsesorName = "Gustavo",
            };
        }

        /// <summary>
        /// Return the asesor.
        /// </summary>
        /// <returns>the asesor.</returns>
        public List<OrderModel> GetOrderModel()
        {
            return new List<OrderModel>
            {
                new OrderModel { PedidoId = 100, AsesorId = 1, Cliente = "cliente", Codigo = "Codigo", DocNum = 100, FechaFin = DateTime.Now, FechaInicio = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), Medico = "Medico", PedidoStatus = "C", AtcEntry = 1 },
                new OrderModel { PedidoId = 101, AsesorId = 1, Cliente = "cliente", Codigo = "Codigo", DocNum = 101, FechaFin = DateTime.Today.AddDays(1), FechaInicio = DateTime.Today, Medico = "Medico", PedidoStatus = "O" },
                new OrderModel { PedidoId = 102, AsesorId = 1, Cliente = "cliente", Codigo = "Codigo", DocNum = 100, FechaFin = DateTime.Now, FechaInicio = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), Medico = "Medico", PedidoStatus = "C", AtcEntry = 2 },

                // For Almacen
                new OrderModel { PedidoId = 75000, Cliente = "cliente", DocNum = 75000, FechaInicio = DateTime.Today.AddDays(-4), Medico = "Medico", PedidoStatus = "O", },
                new OrderModel { PedidoId = 75001, Cliente = "cliente", DocNum = 75001, FechaInicio = DateTime.Today.AddDays(-4), Medico = "Medico", PedidoStatus = "O", },
            };
        }

        /// <summary>
        /// Gets the attachments models.
        /// </summary>
        /// <returns>the data.</returns>
        public List<AttachmentModel> GetAttachmentModel()
        {
            return new List<AttachmentModel>
            {
               new AttachmentModel { AbsEntry = 1, FileExt = "pdf", FileName = "filenmae", Line = 1, SourcePath = "test", TargetPath = @"C:\algo\algo2" },
               new AttachmentModel { AbsEntry = 2, FileExt = "pdf", FileName = string.Empty, Line = 1, SourcePath = "test", TargetPath = @"C:\algo\algo2" },
            };
        }

        /// <summary>
        /// Return the asesor.
        /// </summary>
        /// <returns>the asesor.</returns>
        public List<DetallePedidoModel> GetDetallePedido()
        {
            return new List<DetallePedidoModel>
            {
                new DetallePedidoModel { Description = "DetallePedido", DetalleId = 1, PedidoId = 100, ProductoId = "Abc Aspirina", Container = "NA", Label = "NA", Quantity = 10 },
                new DetallePedidoModel { Description = "DetallePedido", DetalleId = 2, PedidoId = 100, ProductoId = "Buscapina", Container = "NA", Label = "NA", Quantity = 10 },
                new DetallePedidoModel { Description = "DetallePedido", DetalleId = 2, PedidoId = 101, ProductoId = "Abc Aspirina", Container = "NA", Label = "NA", Quantity = 10 },

                // For Almacen
                new DetallePedidoModel { Description = "Magistral1", DetalleId = 0, PedidoId = 75000, ProductoId = "Magistral1", Container = "NA", Quantity = 10 },
                new DetallePedidoModel { Description = "Magistral2", DetalleId = 1, PedidoId = 75000, ProductoId = "Magistral1", Container = "NA", Quantity = 10 },
                new DetallePedidoModel { Description = "Magistral3", DetalleId = 2, PedidoId = 75000, ProductoId = "Magistral2", Container = "NA", Quantity = 10 },
                new DetallePedidoModel { Description = "Linea1", DetalleId = 0, PedidoId = 75001, ProductoId = "Linea1", Container = "NA", Quantity = 10 },
            };
        }

        /// <summary>
        /// returns the user.
        /// </summary>
        /// <returns>the user.</returns>
        public List<Users> GetSapUsers()
        {
            return new List<Users>
            {
                new Users { UserId = 1, UserName = "Gus" },
            };
        }

        /// <summary>
        /// returns the detalle formula.
        /// </summary>
        /// <returns>the detail.</returns>
        public List<DetalleFormulaModel> GetDetalleFormula()
        {
            return new List<DetalleFormulaModel>
            {
                new DetalleFormulaModel { Almacen = "MN", BaseQuantity = 10, ConsumidoQty = 10, ItemCode = "Abc Aspirina", LineNum = 1, OrderFabId = 100, RequiredQty = 100, UnidadCode = "KG" },
                new DetalleFormulaModel { Almacen = "MN", BaseQuantity = 10, ConsumidoQty = 10, ItemCode = "Abc Aspirina", LineNum = 1, OrderFabId = 200, RequiredQty = 100, UnidadCode = "KG" },
            };
        }

        /// <summary>
        /// returns the detalle formula.
        /// </summary>
        /// <returns>the detail.</returns>
        public List<ItemWarehouseModel> GetItemWareHouse()
        {
            return new List<ItemWarehouseModel>
            {
                new ItemWarehouseModel { IsCommited = 10, ItemCode = "Abc Aspirina", OnHand = 10, OnOrder = 10, WhsCode = "MN" },
            };
        }

        /// <summary>
        /// returns the detalle formula.
        /// </summary>
        /// <returns>the detail.</returns>
        public List<Batches> GetBatches()
        {
            return new List<Batches>
            {
                new Batches { AbsEntry = 1, DistNumber = "Lote1", ItemCode = "Abc Aspirina", SysNumber = 1 },

                // For Almacen
                new Batches { AbsEntry = 2, DistNumber = "Lote1", ItemCode = "Linea1", SysNumber = 1 },
            };
        }

        /// <summary>
        /// returns the detalle formula.
        /// </summary>
        /// <returns>the detail.</returns>
        public List<BatchesQuantity> GetBatchesQuantity()
        {
            return new List<BatchesQuantity>
            {
                new BatchesQuantity { AbsEntry = 1, ItemCode = "Abc Aspirina", SysNumber = 1, CommitQty = 10, Quantity = 10, WhsCode = "MN" },

                // For Almacen
                new BatchesQuantity { AbsEntry = 2, ItemCode = "Linea1", SysNumber = 1, CommitQty = 10, Quantity = 100, WhsCode = "PT" },
                new BatchesQuantity { AbsEntry = 3, ItemCode = "Linea1", SysNumber = 1, CommitQty = 10, Quantity = 100, WhsCode = "PT" },
            };
        }

        /// <summary>
        /// Gets the delivery header.
        /// </summary>
        /// <returns>the data.</returns>
        public List<DeliverModel> DeliveryModel()
        {
            return new List<DeliverModel>
            {
                new DeliverModel { Cliente = "cliente", DeliveryStatus = "C", DocNum = 46036, FechaInicio = DateTime.Now, Medico = "Medico", PedidoId = 75000 },
                new DeliverModel { Cliente = "cliente", DeliveryStatus = "C", DocNum = 46037, FechaInicio = DateTime.Now, Medico = "Medico", PedidoId = 75001 },
            };
        }

        /// <summary>
        /// Gets the deliveries.
        /// </summary>
        /// <returns>the data.</returns>
        public List<DeliveryDetailModel> GetDeliveryDetail()
        {
            return new List<DeliveryDetailModel>
            {
                new DeliveryDetailModel { BaseEntry = 75000, DeliveryId = 46036, Description = "Dsc", DocDate = DateTime.Now, ProductoId = "Magistral1", Quantity = 1, InvoiceId = 1 },
                new DeliveryDetailModel { BaseEntry = 75001, DeliveryId = 46037, Description = "Dsc", DocDate = DateTime.Now, ProductoId = "Linea1", Quantity = 1, InvoiceId = 1 },
            };
        }

        /// <summary>
        /// returns  data.
        /// </summary>
        /// <returns>the data.</returns>
        public List<BatchTransacitions> GetBatchTransacitions()
        {
            return new List<BatchTransacitions>
            {
                new BatchTransacitions { ItemCode = "Abc Aspirina", LogEntry = 1, DocNum = 100, DocQuantity = 10 },

                // almacen
                new BatchTransacitions { ItemCode = "Linea1", LogEntry = 2, DocNum = 46037, DocQuantity = 10 },
                new BatchTransacitions { ItemCode = "Linea1", LogEntry = 3, DocNum = 1, DocQuantity = 10 },
            };
        }

        /// <summary>
        /// gets the transaction qty model.
        /// </summary>
        /// <returns>the model.</returns>
        public List<BatchesTransactionQtyModel> GetBatchesTransactionQtyModel()
        {
            return new List<BatchesTransactionQtyModel>
            {
                new BatchesTransactionQtyModel { AllocQty = 1, LogEntry = 1, ItemCode = "Abc Aspirina", SysNumber = 1 },

                // almacen
                new BatchesTransactionQtyModel { AllocQty = 1, LogEntry = 2, ItemCode = "Linea1", SysNumber = 1 },
                new BatchesTransactionQtyModel { AllocQty = 1, LogEntry = 3, ItemCode = "Linea1", SysNumber = 1 },
            };
        }

        /// <summary>
        /// get the product.
        /// </summary>
        /// <returns>the product.</returns>
        public List<ProductoModel> GetProductoModel()
        {
            return new List<ProductoModel>
            {
                new ProductoModel { IsMagistral = "Y", ProductoId = "Abc Aspirina", ProductoName = "Aspirina", ManagedBatches = "Y", OnHand = 10, Unit = "PZ", LargeDescription = "Aspirina con 2%" },
                new ProductoModel { IsMagistral = "N", ProductoId = "Ungüento 1", ProductoName = "Ungüento 10 GR", ManagedBatches = "Y", OnHand = 10, Unit = "PZ", LargeDescription = "Ungüento 10 GR" },
                new ProductoModel { IsMagistral = "N", ProductoId = "Cápsula 12ML", ProductoName = "Cápsula 12 GR", ManagedBatches = "Y", OnHand = 10, Unit = "PZ", LargeDescription = "Cápsula 12 GR" },

                // For almacen
                new ProductoModel { IsMagistral = "Y", ProductoId = "Magistral1", ProductoName = "MagistralSolo1",  Unit = "PZ", LargeDescription = "MAAAAgistral1", NeedsCooling = "Y" },
                new ProductoModel { IsMagistral = "Y", ProductoId = "Magistral2", ProductoName = "MagistralSolo2",  Unit = "PZ", LargeDescription = "MAAAAgistral2", NeedsCooling = "N" },
                new ProductoModel { IsMagistral = "N", ProductoId = "Linea1", ProductoName = "MagistralLinea", Unit = "PZ", LargeDescription = "Liiiiinea1", NeedsCooling = "Y", BarCode = "Linea1" },
            };
        }

        /// <summary>
        /// Return the asesor.
        /// </summary>
        /// <returns>the asesor.</returns>
        public List<OrdenFabricacionModel> GetOrdenFabricacionModel()
        {
            return new List<OrdenFabricacionModel>
            {
                new OrdenFabricacionModel { ProductoId = "Abc Aspirina", OrdenId = 100, PostDate = DateTime.Now, Quantity = 2, Status = "L", PedidoId = 100, User = 1, Type = "S", OriginType = "M", CardCode = "CardCode", CompleteQuantity = 100, CreatedDate = DateTime.Now, DataSource = "O", DueDate = DateTime.Now, ProdName = "Prodname", StartDate = DateTime.Now, Unit = "KG", Wharehouse = "PT" },
                new OrdenFabricacionModel { ProductoId = "Abc Aspirina", OrdenId = 110, PostDate = DateTime.Now, Quantity = 1, Status = "L", PedidoId = 0, User = 1, Type = "S", OriginType = "M", CardCode = string.Empty, CompleteQuantity = 0, CreatedDate = DateTime.Now, DataSource = "O", DueDate = DateTime.Now, ProdName = "Prodname", StartDate = DateTime.Now, Unit = "KG", Wharehouse = "PT", Comments = "token" },
                new OrdenFabricacionModel { ProductoId = "Abc Aspirina", OrdenId = 120, PostDate = DateTime.Now, Quantity = 2, Status = "L", PedidoId = 100, User = 1, Type = "S", OriginType = "M", CardCode = "CardCode", CompleteQuantity = 100, CreatedDate = DateTime.Today.AddDays(1), DataSource = "O", DueDate = DateTime.Now, ProdName = "Prodname", StartDate = DateTime.Now, Unit = "KG", Wharehouse = "PT" },
                new OrdenFabricacionModel { ProductoId = "Abc Aspirina", OrdenId = 130, PostDate = DateTime.Now, Quantity = 1, Status = "L", PedidoId = 0, User = 1, Type = "S", OriginType = "M", CardCode = string.Empty, CompleteQuantity = 0, CreatedDate = DateTime.Today.AddDays(1), DataSource = "O", DueDate = DateTime.Now, ProdName = "Prodname", StartDate = DateTime.Now, Unit = "KG", Wharehouse = "PT", Comments = "token" },

                // For Almacen
                new OrdenFabricacionModel { ProductoId = "Magistral1", OrdenId = 1000, PostDate = DateTime.Now, Quantity = 10, Status = "L", PedidoId = 75000, User = 1, Type = "S", OriginType = "M", CardCode = "CardCode", CompleteQuantity = 10, CreatedDate = DateTime.Now, DataSource = "O", DueDate = DateTime.Now, ProdName = "Prodname", StartDate = DateTime.Now, Unit = "KG", Wharehouse = "PT" },
                new OrdenFabricacionModel { ProductoId = "Magistral1", OrdenId = 1001, PostDate = DateTime.Now, Quantity = 10, Status = "L", PedidoId = 75000, User = 1, Type = "S", OriginType = "M", CardCode = string.Empty, CompleteQuantity = 10, CreatedDate = DateTime.Now, DataSource = "O", DueDate = DateTime.Now, ProdName = "Prodname", StartDate = DateTime.Now, Unit = "KG", Wharehouse = "PT", Comments = "token" },
                new OrdenFabricacionModel { ProductoId = "Magistral2", OrdenId = 1002, PostDate = DateTime.Now, Quantity = 10, Status = "L", PedidoId = 75000, User = 1, Type = "S", OriginType = "M", CardCode = "CardCode", CompleteQuantity = 10, CreatedDate = DateTime.Today.AddDays(1), DataSource = "O", DueDate = DateTime.Now, ProdName = "Prodname", StartDate = DateTime.Now, Unit = "KG", Wharehouse = "PT" },
            };
        }

        /// <summary>
        /// Return the asesor.
        /// </summary>
        /// <returns>the asesor.</returns>
        public List<CompleteDetailOrderModel> GetCompleteDetailOrderModel()
        {
            return new List<CompleteDetailOrderModel>
            {
                new CompleteDetailOrderModel { CodigoProducto = "Abc Aspirina", DescripcionProducto = "Aspirina", FechaOf = "28/03/2020", FechaOfFin = "28/03/2020", IsChecked = false, OrdenFabricacionId = 100, Qfb = "Gustavo", QtyPlanned = 1, Status = "L", },
                new CompleteDetailOrderModel { CodigoProducto = "Buscapina", DescripcionProducto = "Aspirina", FechaOf = "28/03/2020", FechaOfFin = "28/03/2020", IsChecked = false, OrdenFabricacionId = 101, Qfb = "Gustavo", QtyPlanned = 1, Status = "L", },
            };
        }

        /// <summary>
        /// Gets the invoice details.
        /// </summary>
        /// <returns>the dta.</returns>
        public List<InvoiceHeaderModel> GetInvoiceHeader()
        {
            return new List<InvoiceHeaderModel>
            {
                new InvoiceHeaderModel { Address = "address", Cliente = "cliente", DocNum = 1, FechaInicio = DateTime.Now, InvoiceId = 1, InvoiceStatus = "O", Medico = "Medico" },

                // for packages
                new InvoiceHeaderModel { Address = "address, Nuevo León", Cliente = "cliente", DocNum = 2, FechaInicio = DateTime.Now, InvoiceId = 2, InvoiceStatus = "O", Medico = "Medico" },
                new InvoiceHeaderModel { Address = "address, Aguascalientes", Cliente = "cliente", DocNum = 3, FechaInicio = DateTime.Now, InvoiceId = 3, InvoiceStatus = "O", Medico = "Medico" },
            };
        }

        /// <summary>
        /// Gets the invoice details.
        /// </summary>
        /// <returns>the dta.</returns>
        public List<InvoiceDetailModel> GetInvoiceDetails()
        {
            return new List<InvoiceDetailModel>
            {
                new InvoiceDetailModel { BaseEntry = 1, Container = "con", Description = "desc", DocDate = DateTime.Now, InvoiceId = 1, LineNum = 0, ProductoId = "Linea1", Quantity = 1 },

                // for packages
                new InvoiceDetailModel { BaseEntry = 2, Container = "con", Description = "desc", DocDate = DateTime.Now, InvoiceId = 2, LineNum = 0, ProductoId = "Linea1", Quantity = 1 },
                new InvoiceDetailModel { BaseEntry = 3, Container = "con", Description = "desc", DocDate = DateTime.Now, InvoiceId = 3, LineNum = 0, ProductoId = "Linea1", Quantity = 1 },
            };
        }

        /// <summary>
        /// gets the resultdto for getuserpedidos.
        /// </summary>
        /// <returns>the data.</returns>
        public ResultDto GetResultGetUserPedidos()
        {
            var listUsers = new List<UserOrderModel>
            {
                new UserOrderModel { Id = 1, Productionorderid = "12", Salesorderid = "12", Status = "Abierto", Userid = "123", CloseDate = "20/01/2020", Comments = "comments", FinishDate = "20/01/2020" },
            };

            return new ResultDto
            {
                Response = JsonConvert.SerializeObject(listUsers),
                Code = 200,
                Comments = string.Empty,
                ExceptionMessage = string.Empty,
                Success = true,
                UserError = string.Empty,
            };
        }

        /// <summary>
        /// gets the resultdto for getuserpedidos.
        /// </summary>
        /// <returns>the data.</returns>
        public ResultDto GetResultDtoGetUsersById()
        {
            var users = new List<UserModel>
            {
                new UserModel { Id = "123", Activo = 1, FirstName = "Gus", LastName = "Ramirez", Password = "asd", Role = 1, UserName = "asdf" },
            };

            return new ResultDto
            {
                Response = JsonConvert.SerializeObject(users),
                Code = 200,
                Comments = string.Empty,
                ExceptionMessage = string.Empty,
                Success = true,
                UserError = string.Empty,
            };
        }

        /// <summary>
        /// Gets the user order model.
        /// </summary>
        /// <returns>the data.</returns>
        public ResultDto GetUserOrderModelAlmacen()
        {
            var userOrders = new List<UserOrderModel>
            {
                new UserOrderModel { Salesorderid = "75000", Comments = "Comments", FinishedLabel = 1, Status = "Finalizado" },
            };

            return new ResultDto
            {
                Code = 200,
                ExceptionMessage = JsonConvert.SerializeObject(new List<int>()),
                Response = JsonConvert.SerializeObject(userOrders),
                Success = true,
                Comments = "15",
            };
        }

        /// <summary>
        /// the linse products.
        /// </summary>
        /// <returns>the data.</returns>
        public ResultDto GetLineProducts()
        {
            var listProducts = new List<LineProductsModel>
            {
                new LineProductsModel { Id = 1, SaleOrderId = 75001, StatusAlmacen = "Almacenado" },
            };

            return new ResultDto
            {
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = JsonConvert.SerializeObject(listProducts),
                Success = true,
                Comments = "15",
            };
        }

        /// <summary>
        /// Gets the user order model.
        /// </summary>
        /// <returns>the data.</returns>
        public ResultDto GetUserOrderRemision()
        {
            var userOrders = new List<UserOrderModel>
            {
                new UserOrderModel { Salesorderid = "75000", Comments = "Comments", FinishedLabel = 1, Status = "Almacenado" },
            };

            return new ResultDto
            {
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = JsonConvert.SerializeObject(userOrders),
                Success = true,
                Comments = "15",
            };
        }

        /// <summary>
        /// the linse products.
        /// </summary>
        /// <returns>the data.</returns>
        public ResultDto GetLineProductsRemision()
        {
            var batch = new List<AlmacenBatchModel>()
            {
                new AlmacenBatchModel { BatchNumber = "Lote1", BatchQty = 1 },
            };

            var listProducts = new List<LineProductsModel>
            {
                new LineProductsModel { Id = 1, SaleOrderId = 75001, StatusAlmacen = "Almacenado", BatchName = JsonConvert.SerializeObject(batch) },
            };

            return new ResultDto
            {
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = JsonConvert.SerializeObject(listProducts),
                Success = true,
                Comments = "15",
            };
        }

        /// <summary>
        /// Gets the user order model.
        /// </summary>
        /// <returns>the data.</returns>
        public ResultDto GetUserOrderInvoice()
        {
            var userOrders = new List<UserOrderModel>
            {
                new UserOrderModel { Salesorderid = "75000", Comments = "Comments", FinishedLabel = 1, Status = "Almacenado" },
                new UserOrderModel { Salesorderid = "75000", Productionorderid = "75001", Comments = "Comments", FinishedLabel = 1, Status = "Almacenado" },
            };

            return new ResultDto
            {
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = JsonConvert.SerializeObject(userOrders),
                Success = true,
                Comments = "15",
            };
        }

        /// <summary>
        /// the linse products.
        /// </summary>
        /// <returns>the data.</returns>
        public ResultDto GetLineProductsInvoide()
        {
            var listProducts = new List<LineProductsModel>
            {
                new LineProductsModel { Id = 1, SaleOrderId = 75001, StatusAlmacen = "Almacenado" },
                new LineProductsModel { Id = 1, SaleOrderId = 75001, StatusAlmacen = "Almacenado", ItemCode = "Linea1" },
            };

            return new ResultDto
            {
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = JsonConvert.SerializeObject(listProducts),
                Success = true,
                Comments = "15",
            };
        }

        /// <summary>
        /// the linse products.
        /// </summary>
        /// <returns>the data.</returns>
        public ResultDto GetLineProductsScannInvoice()
        {
            var batch = new List<AlmacenBatchModel>()
            {
                new AlmacenBatchModel { BatchNumber = "Lote1", BatchQty = 1 },
            };

            var listProducts = new List<LineProductsModel>
            {
                new LineProductsModel { Id = 1, SaleOrderId = 75001, StatusAlmacen = "Almacenado", BatchName = JsonConvert.SerializeObject(batch), ItemCode = "Linea1" },
            };

            return new ResultDto
            {
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = JsonConvert.SerializeObject(listProducts),
                Success = true,
                Comments = "15",
            };
        }
    }
}
