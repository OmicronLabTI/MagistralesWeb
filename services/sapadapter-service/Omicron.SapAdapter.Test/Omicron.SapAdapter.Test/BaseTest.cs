// <summary>
// <copyright file="BaseTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Test
{
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
                new OrderModel { PedidoId = 100, AsesorId = 1, Codigo = "Codigo", DocNum = 100, FechaFin = DateTime.Now, FechaInicio = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), Medico = "Medico", PedidoStatus = "C", AtcEntry = 1, OrderType = "MN", DocNumDxp = "A1", Patient = "paciente", ShippingAddressName = "Nombre de la dirección", ClientType = "institucional" },
                new OrderModel { PedidoId = 101, AsesorId = 1, Codigo = "Codigo", DocNum = 101, FechaFin = DateTime.Today.AddDays(1), FechaInicio = DateTime.Today, Medico = "Medico", PedidoStatus = "O", Patient = "paciente", ShippingAddressName = "3. LUIS JAVIER GARCIA AQUINO C.6019043", ClientType = "institucional", OrderType = "LN" },
                new OrderModel { PedidoId = 102, AsesorId = 1, Codigo = "Codigo", DocNum = 100, FechaFin = DateTime.Now, FechaInicio = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), Medico = "Medico", PedidoStatus = "C", AtcEntry = 2, OrderType = "MN", Patient = "paciente" },
                new OrderModel { PedidoId = 103, AsesorId = 1, Codigo = "Codigo1234", DocNum = 100, FechaFin = DateTime.Now, FechaInicio = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), Medico = "Medico", PedidoStatus = "C", AtcEntry = 1, OrderType = "MN", DocNumDxp = "A1", Patient = "paciente", ClientType = "general" },
                new OrderModel { PedidoId = 104, AsesorId = 1, Codigo = "Codigo", DocNum = 100, FechaFin = DateTime.Today.AddDays(1), FechaInicio = DateTime.Today, Medico = "Medico", PedidoStatus = "O", Patient = "paciente", ShippingAddressName = "3. LUIS JAVIER GARCIA AQUINO C.6019043", ClientType = "clinica", OrderType = "LN" },

                // For Almacen
                new OrderModel { PedidoId = 75000, DocNum = 75000, Codigo = "Codigo", FechaInicio = DateTime.Today.AddDays(-4), Medico = "Medico", PedidoStatus = "O", Address = "CDMX", OrderType = "MQ", Canceled = "N", Patient = "paciente" },
                new OrderModel { PedidoId = 75001, DocNum = 75001, Codigo = "Codigo", FechaInicio = DateTime.Today.AddDays(-4), Medico = "Medico", PedidoStatus = "O", Address = "Nuevo Le?n", Canceled = "N", Patient = "paciente", OrderType = "LN" },
                new OrderModel { PedidoId = 75002, DocNum = 75002, Codigo = "Codigo", FechaInicio = DateTime.Today.AddDays(-4), Medico = "Medico", PedidoStatus = "O", Address = null, Canceled = "N", Patient = "paciente", OrderType = "LN" },
                new OrderModel { PedidoId = 85000, DocNum = 85000, Codigo = "Codigo", FechaInicio = DateTime.Today.AddDays(-30), Medico = "Medico", PedidoStatus = "O", Address = "CDMX", OrderType = "MQ", Canceled = "N", Patient = "paciente" },

                // for packages
                new OrderModel { PedidoId = 85001, DocNum = 85001, Codigo = "Codigo", FechaInicio = DateTime.Today.AddDays(-2), Medico = "Medico", PedidoStatus = "O", Address = "CDMX", OrderType = "MX", Canceled = "N", IsPackage = "Y", DocNumDxp = "xse123", Patient = "paciente" },
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
                new DetallePedidoModel { Description = "DetallePedido", DetalleId = 2, PedidoId = 104, ProductoId = "Abc Aspirina", Container = "NA", Label = "NA", Quantity = 10 },

                // For Almacen
                new DetallePedidoModel { Description = "Magistral1", DetalleId = 0, PedidoId = 75000, ProductoId = "Magistral1", Container = "NA", Quantity = 10 },
                new DetallePedidoModel { Description = "Magistral2", DetalleId = 1, PedidoId = 75000, ProductoId = "Magistral1", Container = "NA", Quantity = 10 },
                new DetallePedidoModel { Description = "Magistral3", DetalleId = 2, PedidoId = 75000, ProductoId = "Magistral2", Container = "NA", Quantity = 10 },
                new DetallePedidoModel { Description = "Linea1", DetalleId = 3, PedidoId = 75000, ProductoId = "Linea1", Container = "NA", Quantity = 10 },
                new DetallePedidoModel { Description = "Linea1", DetalleId = 0, PedidoId = 75001, ProductoId = "Linea1", Container = "NA", Quantity = 10 },
                new DetallePedidoModel { Description = "Magistral4", DetalleId = 0, PedidoId = 85000, ProductoId = "Magistral4", Container = "NA", Quantity = 10 },

                // for graphs
                new DetallePedidoModel { Description = "Linea1", DetalleId = 0, PedidoId = 75002, ProductoId = "Linea1", Container = "NA", Quantity = 10, DocDate = DateTime.Today },
                new DetallePedidoModel { Description = "Linea1", DetalleId = 1, PedidoId = 75002, ProductoId = "Linea1", Container = "NA", Quantity = 10, DocDate = DateTime.Today },

                // for almacen packages
                new DetallePedidoModel { Description = "Linea1", DetalleId = 1, PedidoId = 85001, ProductoId = "Linea1", Container = "NA", Quantity = 10, DocDate = DateTime.Today },

                // for remissioned pieces
                new DetallePedidoModel { PedidoId = 11, Description = "Linea50", DetalleId = 1, ProductoId = "Linea50", Container = "NA", Quantity = 10, DocDate = DateTime.Today },
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
                new DetalleFormulaModel { Almacen = "MN", BaseQuantity = 10.12345678M, ConsumidoQty = 10, ItemCode = "Abc Aspirina", LineNum = 1, OrderFabId = 100, RequiredQty = 100, UnidadCode = "KG" },
                new DetalleFormulaModel { Almacen = "MN", BaseQuantity = 10, ConsumidoQty = 10, ItemCode = "Abc Aspirina", LineNum = 1, OrderFabId = 200, RequiredQty = 100, UnidadCode = "KG" },
                new DetalleFormulaModel { Almacen = "MN", BaseQuantity = 10.12M, ConsumidoQty = 10, ItemCode = "Abc Aspirina", LineNum = 1, OrderFabId = 103, RequiredQty = 100, UnidadCode = "KG" },
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

                new Batches { AbsEntry = 4, DistNumber = "Linea50", ItemCode = "Linea50", SysNumber = 1 },

                new Batches { AbsEntry = 5, DistNumber = "PROD 01", ItemCode = "PROD 01", SysNumber = 1 },
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

                new BatchesQuantity { AbsEntry = 4, ItemCode = "Linea50", SysNumber = 1, CommitQty = 10, Quantity = 100, WhsCode = "PT" },

                new BatchesQuantity { AbsEntry = 5, ItemCode = "PROD 01", SysNumber = 1, CommitQty = 10, Quantity = 100, WhsCode = "BE" },

                new BatchesQuantity { AbsEntry = 6, ItemCode = "PROD 01", SysNumber = 1, CommitQty = 10, Quantity = 100, WhsCode = "BQ" },
            };
        }

        /// <summary>
        /// returns the detalle formula.
        /// </summary>
        /// <returns>the detail.</returns>
        public List<ProductoModel> ProdcutModels()
        {
            return new List<ProductoModel>
            {
                new ProductoModel { IsLine = "Y", IsMagistral = "N", ProductoId = "Linea1" },
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
                new DeliverModel { Cliente = "cliente", CardCode = "Codigo", DeliveryStatus = "C", DocNum = 46036, FechaInicio = DateTime.Now, Medico = "Medico", PedidoId = 75000, Address = "direccion CD MX", TypeOrder = "MQ" },
                new DeliverModel { Cliente = "cliente", CardCode = "Codigo", DeliveryStatus = "C", DocNum = 46037, FechaInicio = DateTime.Now, Medico = "Medico", PedidoId = 75001, Address = "direccion Nuevo Le?n", TypeOrder = "MQ" },
                new DeliverModel { Cliente = "cliente", CardCode = "Codigo", DeliveryStatus = "C", DocNum = 46038, FechaInicio = DateTime.Now, Medico = "Medico", PedidoId = 75002, Address = "direccion CD MX", TypeOrder = "MQ" },
                new DeliverModel { Cliente = "cliente", CardCode = "Codigo", DeliveryStatus = "C", DocNum = 46039, FechaInicio = DateTime.Now, Medico = "Medico", PedidoId = 75003, Address = "direccion Oax", TypeOrder = "MQ" },

                // for almacen detail
                new DeliverModel { Cliente = "cliente", CardCode = "Codigo", DeliveryStatus = "O", DocNum = 46040, FechaInicio = DateTime.Now, Medico = "Medico", PedidoId = 46040, Address = "direccion Oax", TypeOrder = "MQ" },

                // for almacen packages
                new DeliverModel { Cliente = "cliente", CardCode = "Codigo", DeliveryStatus = "O", DocNum = 85001, FechaInicio = DateTime.Now, Medico = "Medico", PedidoId = 85001, Address = "direccion CD MX", IsPackage = "Y", TypeOrder = "MQ" },

                // for detail invoice
                new DeliverModel { Cliente = "cliente", CardCode = "Codigo", DeliveryStatus = "O", DocNum = 1, FechaInicio = DateTime.Now, Medico = "Medico", PedidoId = 1, Address = "direccion CD MX", IsPackage = "Y", TypeOrder = "MQ" },
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
                new DeliveryDetailModel { BaseEntry = 100, DeliveryId = 46038, Description = "Dsc", DocDate = DateTime.Now, ProductoId = "Linea1", Quantity = 1, InvoiceId = 1 },
                new DeliveryDetailModel { BaseEntry = 100, DeliveryId = 46039, Description = "Dsc", DocDate = DateTime.Now, ProductoId = "Linea1", Quantity = 1, InvoiceId = 2 },

                // for almacen detail
                new DeliveryDetailModel { BaseEntry = 75000, DeliveryId = 46040, Description = "Dsc", DocDate = DateTime.Now, ProductoId = "Magistral1", Quantity = 1, InvoiceId = 1 },

                // for detail invoice
                new DeliveryDetailModel { BaseEntry = 75000, DeliveryId = 1, Description = "Dsc", DocDate = DateTime.Now, ProductoId = "Magistral1", Quantity = 1, InvoiceId = 1 },

                // for detail packages
                new DeliveryDetailModel { BaseEntry = 85001, DeliveryId = 85001, Description = "Dsc", DocDate = DateTime.Now, ProductoId = "Linea1", Quantity = 1, InvoiceId = 1 },
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
                new BatchTransacitions { ItemCode = "Abc Aspirina", LogEntry = 4, DocNum = 200, DocQuantity = 10 },

                // almacen
                new BatchTransacitions { ItemCode = "Linea1", LogEntry = 2, DocNum = 46037, DocQuantity = 10, BaseEntry = 75001 },
                new BatchTransacitions { ItemCode = "Linea1", LogEntry = 3, DocNum = 1, DocQuantity = 10, BaseEntry = 75001 },
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
                new BatchesTransactionQtyModel { AllocQty = 1, LogEntry = 4, ItemCode = "Abc Aspirina", SysNumber = 1 },

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
                new ProductoModel { IsMagistral = "Y", ProductoId = "Abc Aspirina", ProductoName = "Aspirina", ManagedBatches = "Y", OnHand = 10, Unit = "PZ", LargeDescription = "Aspirina con 2%", ProductGroupId = 1, IsWorkableProduct = "Y" },
                new ProductoModel { IsMagistral = "N", ProductoId = "Ung?ento 1", ProductoName = "Ung?ento 10 GR", ManagedBatches = "Y", OnHand = 10, Unit = "PZ", LargeDescription = "Ung?ento 10 GR", ProductGroupId = 1, IsWorkableProduct = "Y" },
                new ProductoModel { IsMagistral = "N", ProductoId = "C?psula 12ML", ProductoName = "C?psula 12 GR", ManagedBatches = "Y", OnHand = 10, Unit = "PZ", LargeDescription = "C?psula 12 GR", ProductGroupId = 1, IsWorkableProduct = "Y" },
                new ProductoModel { IsMagistral = "N", ProductoId = "Buscapina", ProductoName = "Buscapina 12 GR", ManagedBatches = "Y", OnHand = 10, Unit = "PZ", LargeDescription = "C?psula 12 GR", ProductGroupId = 1, IsWorkableProduct = "Y" },
                new ProductoModel { IsMagistral = "N", ProductoId = "Magistral4", ProductoName = "Magistral 4 12 GR", ManagedBatches = "Y", OnHand = 10, Unit = "PZ", LargeDescription = "C?psula 12 GR", ProductGroupId = 1, IsWorkableProduct = "Y" },

                // For almacen
                new ProductoModel { IsMagistral = "Y", ProductoId = "Magistral1", ProductoName = "MagistralSolo1",  Unit = "PZ", LargeDescription = "MAAAAgistral1", NeedsCooling = "Y", ProductGroupId = 1, IsWorkableProduct = "Y" },
                new ProductoModel { IsMagistral = "Y", ProductoId = "Magistral2", ProductoName = "MagistralSolo2",  Unit = "PZ", LargeDescription = "MAAAAgistral2", NeedsCooling = "N", ProductGroupId = 1, IsWorkableProduct = "Y" },
                new ProductoModel { IsMagistral = "N", ProductoId = "Linea1", ProductoName = "MagistralLinea", Unit = "PZ", LargeDescription = "Liiiiinea1", NeedsCooling = "Y", BarCode = "Linea1", IsLine = "Y", ProductGroupId = 1, IsWorkableProduct = "Y" },

                new ProductoModel { IsMagistral = "N", ProductoId = "Linea50", ProductoName = "MagistralLinea", Unit = "PZ", LargeDescription = "Liiiiinea1", NeedsCooling = "Y", BarCode = "Linea50", IsLine = "Y", ProductGroupId = 1, IsWorkableProduct = "Y" },

                // For omigenomics
                new ProductoModel { IsMagistral = "N", ProductoId = "Omigenomics", ProductoName = "Omigenomics", Unit = "PZ", LargeDescription = "Omigenomics", NeedsCooling = "Y", BarCode = "Omicenomics", IsLine = "Y", ProductGroupId = 2, IsWorkableProduct = "N" },
            };
        }

        /// <summary>
        /// get the product.
        /// </summary>
        /// <returns>the product.</returns>
        public List<CatalogProductModel> GetCatalogProductModel()
        {
            return new List<CatalogProductModel>
            {
                new CatalogProductModel { CatalogName = "Dermazone", ProductGroupId = 1 },
                new CatalogProductModel { CatalogName = "Omigenomics", ProductGroupId = 2 },
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
                new OrdenFabricacionModel { ProductoId = "Abc Aspirina", OrdenId = 103, PostDate = DateTime.Now, Quantity = 2, Status = "L", PedidoId = 103, User = 1, Type = "S", OriginType = "M", CardCode = "CardCode", CompleteQuantity = 100, CreatedDate = DateTime.Now, DataSource = "O", DueDate = DateTime.Now, ProdName = "Prodname", StartDate = DateTime.Now, Unit = "KG", Wharehouse = "PT" },

                // For Almacen
                new OrdenFabricacionModel { ProductoId = "Magistral1", OrdenId = 1000, PostDate = DateTime.Now, Quantity = 10, Status = "L", PedidoId = 75000, User = 1, Type = "S", OriginType = "M", CardCode = "CardCode", CompleteQuantity = 10, CreatedDate = DateTime.Now, DataSource = "O", DueDate = DateTime.Now, ProdName = "Prodname", StartDate = DateTime.Now, Unit = "KG", Wharehouse = "PT" },
                new OrdenFabricacionModel { ProductoId = "Magistral1", OrdenId = 1001, PostDate = DateTime.Now, Quantity = 10, Status = "L", PedidoId = 75000, User = 1, Type = "S", OriginType = "M", CardCode = string.Empty, CompleteQuantity = 10, CreatedDate = DateTime.Now, DataSource = "O", DueDate = DateTime.Now, ProdName = "Prodname", StartDate = DateTime.Now, Unit = "KG", Wharehouse = "PT", Comments = "token" },
                new OrdenFabricacionModel { ProductoId = "Magistral2", OrdenId = 1002, PostDate = DateTime.Now, Quantity = 10, Status = "L", PedidoId = 75000, User = 1, Type = "S", OriginType = "M", CardCode = "CardCode", CompleteQuantity = 10, CreatedDate = DateTime.Today.AddDays(1), DataSource = "O", DueDate = DateTime.Now, ProdName = "Prodname", StartDate = DateTime.Now, Unit = "KG", Wharehouse = "PT" },
                new OrdenFabricacionModel { ProductoId = "Linea1", OrdenId = 1003, PostDate = DateTime.Now, Quantity = 10, Status = "L", PedidoId = 75001, User = 1, Type = "S", OriginType = "M", CardCode = "CardCode", CompleteQuantity = 10, CreatedDate = DateTime.Today.AddDays(1), DataSource = "O", DueDate = DateTime.Now, ProdName = "Prodname", StartDate = DateTime.Now, Unit = "KG", Wharehouse = "PT" },

                // For omigenomics
                new OrdenFabricacionModel { ProductoId = "Omigenomics", OrdenId = 101, PostDate = DateTime.Now, Quantity = 10, Status = "L", PedidoId = 100, User = 1, Type = "S", OriginType = "M", CardCode = "CardCode", CompleteQuantity = 10, CreatedDate = DateTime.Today.AddDays(1), DataSource = "O", DueDate = DateTime.Now, ProdName = "Prodname", StartDate = DateTime.Now, Unit = "KG", Wharehouse = "PT" },
                new OrdenFabricacionModel { ProductoId = "Buscapina", OrdenId = 102, PostDate = DateTime.Now, Quantity = 10, Status = "L", PedidoId = 100, User = 1, Type = "S", OriginType = "M", CardCode = "CardCode", CompleteQuantity = 10, CreatedDate = DateTime.Today.AddDays(1), DataSource = "O", DueDate = DateTime.Now, ProdName = "Prodname", StartDate = DateTime.Now, Unit = "KG", Wharehouse = "PT" },
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
                new InvoiceHeaderModel { Address = "address", Cliente = "cliente", CardCode = "C1", DocNum = 1, FechaInicio = DateTime.Now, InvoiceId = 1, InvoiceStatus = "O", Medico = "Medico", DocNumDxp = "123456", ShippingAddressName = "Address1", IsOmigenomics = "SI" },

                // for packages
                new InvoiceHeaderModel { Address = "address, Nuevo Le?n", Cliente = "cliente", CardCode = "C1", DocNum = 2, FechaInicio = DateTime.Now, InvoiceId = 2, InvoiceStatus = "O", Medico = "Medico", SalesPrsonId = 1, IsOmigenomics = "Y" },
                new InvoiceHeaderModel { Address = "address, Aguascalientes", Cliente = "cliente", CardCode = "C8", DocNum = 3, FechaInicio = DateTime.Now, InvoiceId = 3, InvoiceStatus = "O", Medico = "Medico", SalesPrsonId = 2, IsOmigenomics = "1" },
                new InvoiceHeaderModel { Address = "address, Nuevo Le?n", Cliente = "cliente", CardCode = "C1", DocNum = 4, FechaInicio = DateTime.Now, InvoiceId = 4, InvoiceStatus = "O", Medico = "Medico", SalesPrsonId = 3, IsOmigenomics = "SI" },
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
        /// Gets the invoice details.
        /// </summary>
        /// <returns>the dta.</returns>
        public List<ClientCatalogModel> GetClients()
        {
            return new List<ClientCatalogModel>
            {
                new ClientCatalogModel { ClientId = "C1", Email = "email", AliasName = "alias" },
                new ClientCatalogModel { ClientId = "C8", Email = "email", AliasName = "alias" },
                new ClientCatalogModel { ClientId = "Codigo", Email = "email", AliasName = "alias" },
                new ClientCatalogModel { ClientId = "Medico A", Email = "email", AliasName = "alias" },
                new ClientCatalogModel { ClientId = "Medico B", Email = "email", AliasName = "alias" },
            };
        }

        /// <summary>
        /// Gets the invoice details.
        /// </summary>
        /// <returns>the dta.</returns>
        public List<LblContainerModel> GetLblContainer()
        {
            return new List<LblContainerModel>
            {
                new LblContainerModel { Value = "MQ", Description = "Maquilas", FieldId = 24, TableId = "ADOC" },
                new LblContainerModel { Value = "MN", Description = "Bioelite", FieldId = 24, TableId = "ADOC" },
                new LblContainerModel { Value = "BE", Description = "Bioequal", FieldId = 24, TableId = "ADOC" },
                new LblContainerModel { Value = "MG", Description = "Magistrales", FieldId = 24, TableId = "ADOC" },
            };
        }

        /// <summary>
        /// Gets the invoice details.
        /// </summary>
        /// <returns>the dta.</returns>
        public List<SalesPersonModel> GetSalesPerson()
        {
            return new List<SalesPersonModel>
            {
                new SalesPersonModel { AsesorId = 1, EmpleadoId = 1, FirstName = "juanito", LastName = "apellido", Email = "email@email" },
                new SalesPersonModel { AsesorId = 2, EmpleadoId = 2, FirstName = "maria", LastName = "apellido", Email = "email@email" },
                new SalesPersonModel { AsesorId = 3, EmpleadoId = 3, FirstName = null, LastName = "apellido", Email = null },
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
                new UserOrderModel { Id = 1, Productionorderid = "12", Salesorderid = "12", Status = "Abierto", Userid = "123", CloseDate = new DateTime(2020, 1, 20), Comments = "comments", FinishDate = new DateTime(2020, 1, 20) },
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
                new UserModel { Id = "35642b3a-9471-4b89-9862-8bee6d98c361", Activo = 1, FirstName = "Gus", LastName = "Ramirez", Password = "asd", Role = 3, UserName = "asdf", Classification = "DZ" },
                new UserModel { Id = "db106faf-ef03-4c2e-9b7c-be0c7da8c0b7", Activo = 1, FirstName = "Gus", LastName = "Ramirez", Password = "asd", Role = 2, UserName = "asdf", Classification = "BE" },
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
        /// gets the resultdto for getuserpedidos.
        /// </summary>
        /// <returns>the data.</returns>
        public ResultDto GetDoctorsInfo()
        {
            var doctors = new List<DoctorDeliveryAddressModel>
            {
                new DoctorDeliveryAddressModel { AddressId = "Address1", DoctorId = "C1", AddressType = "doctor" },
            };

            return new ResultDto
            {
                Response = JsonConvert.SerializeObject(doctors),
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
                new UserOrderModel { Salesorderid = "75000", Comments = "Comments", FinishedLabel = 1, Status = "Finalizado", },
                new UserOrderModel { Salesorderid = "76000", Comments = "Comments", FinishedLabel = 1, Status = "Finalizado", CloseDate = new DateTime(2021, 03, 21) },
                new UserOrderModel { Salesorderid = "77000", Comments = "Comments", FinishedLabel = 1, Status = "Finalizado", CloseDate = new DateTime(2021, 03, 21), TypeOrder = "MQ" },
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
        /// Gets the user order model.
        /// </summary>
        /// <returns>the data.</returns>
        public ResultDto GetUserOrderDetailModelAlmacen()
        {
            var userOrders = new List<UserOrderModel>
            {
                new UserOrderModel { Salesorderid = "75000", Productionorderid = null, Comments = "Comments", FinishedLabel = 1, Status = "Finalizado", },
                new UserOrderModel { Salesorderid = "75000", Productionorderid = "1", Comments = "Comments", FinishedLabel = 1, Status = "Finalizado", CloseDate = new DateTime(2021, 03, 21) },
                new UserOrderModel { Salesorderid = "75000", Productionorderid = "2", Comments = "Comments", FinishedLabel = 1, Status = "Finalizado", CloseDate = new DateTime(2021, 03, 21), TypeOrder = "MQ" },
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
                new LineProductsModel { Id = 1, SaleOrderId = 75001, StatusAlmacen = "Recibir" },
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
        public ResultDto GetLineProductsRecepcionDetail()
        {
            var listProducts = new List<LineProductsModel>
            {
                new LineProductsModel { Id = 1, SaleOrderId = 75000, ItemCode = "Linea1", StatusAlmacen = "Almacenado", BatchName = JsonConvert.SerializeObject(new[] { new AlmacenBatchModel { BatchQty = 1, WarehouseCode = "PT", BatchNumber = "55" } }), DeliveryId = 12 },
                new LineProductsModel { Id = 2, SaleOrderId = 75000, ItemCode = "Linea1", StatusAlmacen = "Almacenado", BatchName = JsonConvert.SerializeObject(new[] { new AlmacenBatchModel { BatchQty = 1, WarehouseCode = "Prueba varios almacenes", BatchNumber = "55" } }) },
            };

            var incidence = new List<IncidentsModel>
            {
                new IncidentsModel { SaleOrderId = 100, ItemCode = "Linea1" },
                new IncidentsModel { SaleOrderId = 84515, ItemCode = "567   120 ML", Batches = "[{\"BatchNumber\":\"\",\"BatchQty\":2.0}]", Incidence = "producto derramado", Status = "Abierta" },
            };

            return new ResultDto
            {
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = JsonConvert.SerializeObject(listProducts),
                Success = true,
                Comments = JsonConvert.SerializeObject(incidence),
            };
        }

        /// <summary>
        /// the linse products.
        /// </summary>
        /// <returns>the data.</returns>
        public ResultDto GetIncidents()
        {
            var listProducts = new List<IncidentsModel>
            {
                new IncidentsModel { SaleOrderId = 100, ItemCode = "Linea1" },
                new IncidentsModel { SaleOrderId = 84515, ItemCode = "567   120 ML", Batches = "[{\"BatchNumber\":\"\",\"BatchQty\":2.0}]", Incidence = "producto derramado", Status = "Abierta" },
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
                new UserOrderModel { Salesorderid = "75000", Comments = "Comments", FinishedLabel = 1, Status = "Almacenado", DeliveryId = 46036 },
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
                new LineProductsModel { Id = 1, SaleOrderId = 75001, StatusAlmacen = "Almacenado", BatchName = JsonConvert.SerializeObject(batch), DeliveryId = 46037 },
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
        public ResultDto GetLineProductsRemisionPackage()
        {
            var batch = new List<AlmacenBatchModel>()
            {
                new AlmacenBatchModel { BatchNumber = "Lote1", BatchQty = 1 },
            };

            var listProducts = new List<LineProductsModel>
            {
                new LineProductsModel { Id = 1, SaleOrderId = 85001, StatusAlmacen = "Almacenado", BatchName = JsonConvert.SerializeObject(batch), DeliveryId = 85001 },
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
        /// Gets the invoice details.
        /// </summary>
        /// <returns>the dta.</returns>
        public List<LblContainerModel> GetClassificationsCatalog()
        {
            return new List<LblContainerModel>
            {
                new LblContainerModel { FieldId = 24, Value = "MG", Description = "Magistrales", TableId = "ADOC",  },
                new LblContainerModel { FieldId = 24, Value = "LN", Description = "De Linea", TableId = "ADOC",  },
                new LblContainerModel { FieldId = 24, Value = "MQ", Description = "Maquila", TableId = "ADOC",  },
                new LblContainerModel { FieldId = 24, Value = "BE", Description = "Bioelite", TableId = "ADOC",  },
                new LblContainerModel { FieldId = 24, Value = "MX", Description = "Mixto", TableId = "ADOC",  },
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
                new UserOrderModel { Salesorderid = "75000", Comments = "Comments", FinishedLabel = 1, Status = "Almacenado", DeliveryId = 1, Productionorderid = "75001", StatusAlmacen = "Almacenado" },
                new UserOrderModel { Salesorderid = "75000", Productionorderid = "75001", Comments = "Comments", FinishedLabel = 1, Status = "Almacenado", DeliveryId = 1, StatusAlmacen = "Empaquetado" },
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

        /// <summary>
        /// Gets the resultDto.
        /// </summary>
        /// <param name="dataToSend">the data to send.</param>
        /// <returns>the object.</returns>
        public ResultDto GetResultDto(object dataToSend)
        {
            return new ResultDto
            {
                Code = 200,
                Comments = null,
                Response = JsonConvert.SerializeObject(dataToSend),
            };
        }

        /// <summary>
        /// Gets the resultDto.
        /// </summary>
        /// <param name="dataToSend">the data to send.</param>
        /// <returns>the object.</returns>
        public ResultDto GetResultCatalogosDto()
        {
            var userOrders = new List<ParametersModel>
            {
                new ParametersModel { Id = 1, Field = ServiceConstants.CardCodeResponsibleMedic, Value = "C05298" },
            };

            return new ResultDto
            {
                Code = 200,
                Comments = null,
                Response = JsonConvert.SerializeObject(userOrders),
            };
        }

        /// <summary>
        /// Gets the resultDto.
        /// </summary>
        /// <param name="dataToSend">the data to send.</param>
        /// <returns>the object.</returns>
        public ResultModel GetResultModel(object dataToSend)
        {
            return new ResultModel
            {
                Code = 200,
                Comments = null,
                Response = JsonConvert.SerializeObject(dataToSend),
            };
        }

        /// <summary>
        /// gets the resultdto for getuserpedidos.
        /// </summary>
        /// <returns>the data.</returns>
        public ResultDto GetResultDtoGetPedidosService()
        {
            var users = new List<int>
            {
                123,
                234,
            };

            return new ResultDto
            {
                Success = true,
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = users,
                UserError = string.Empty,
            };
        }

        /// <summary>
        /// gets the resultdto for getuserpedidos.
        /// </summary>
        /// <returns>the data.</returns>
        public ResultDto GetResultGetUserPedidosForDoctorOrders()
        {
            var userOrders = new List<UserOrderModel>
            {
                new UserOrderModel { Id = 1, Productionorderid = "122808", Salesorderid = "84503", Status = "Finalizado", Userid = "123", CloseDate = new DateTime(2020, 1, 20), Comments = "comments", FinishDate = new DateTime(2020, 1, 20), FinishedLabel = 1 },
                new UserOrderModel { Id = 2, Productionorderid = null, Salesorderid = "84503", Status = "Finalizado", Userid = "123", CloseDate = new DateTime(2020, 1, 20), Comments = "comments", FinishDate = new DateTime(2020, 1, 20), FinishedLabel = 1 },
                new UserOrderModel { Id = 3, Productionorderid = null, Salesorderid = "84517", Status = "Finalizado", Userid = "123", CloseDate = new DateTime(2020, 1, 20), Comments = "comments", FinishDate = new DateTime(2020, 1, 20), FinishedLabel = 1 },
                new UserOrderModel { Id = 4, Productionorderid = "122824", Salesorderid = "84517", Status = "Finalizado", Userid = "123", CloseDate = new DateTime(2020, 1, 20), Comments = "comments", FinishDate = new DateTime(2020, 1, 20), FinishedLabel = 1 },
                new UserOrderModel { Id = 5, Productionorderid = "122825", Salesorderid = "84517", Status = "Finalizado", Userid = "123", CloseDate = new DateTime(2020, 1, 20), Comments = "comments", FinishDate = new DateTime(2020, 1, 20), FinishedLabel = 1 },
                new UserOrderModel { Id = 6, Productionorderid = "122821", Salesorderid = "84515", Status = "Finalizado", Userid = "123", CloseDate = new DateTime(2020, 1, 20), Comments = "comments", FinishDate = new DateTime(2020, 1, 20), FinishedLabel = 1 },
                new UserOrderModel { Id = 7, Productionorderid = null, Salesorderid = "84515", Status = "Finalizado", Userid = "123", CloseDate = new DateTime(2020, 1, 20), Comments = "comments", FinishDate = new DateTime(2020, 1, 20), FinishedLabel = 1 },
                new UserOrderModel { Id = 7, Productionorderid = "122826", Salesorderid = "84517", Status = "Finalizado", Userid = "123", CloseDate = new DateTime(2020, 1, 20), Comments = "comments", FinishDate = new DateTime(2020, 1, 20), FinishedLabel = 1 },
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
        public ResultDto GetLineProductsForDoctorOrders()
        {
            var listProducts = new List<LineProductsModel>
            {
                new LineProductsModel { Id = 1, SaleOrderId = 84503, StatusAlmacen = "Cancelado", ItemCode = "REVE 14" },
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
        /// get the product.
        /// </summary>
        /// <returns>the product.</returns>
        public List<ProductoModel> GetProductoModelForDoctorOrders()
        {
            return new List<ProductoModel>
            {
                 new ProductoModel { IsMagistral = "N", ProductoId = "REVE 14", ProductoName = "REVE 14", Unit = "PZ", LargeDescription = "Liiiiinea1", NeedsCooling = "Y", BarCode = "Linea1", IsLine = "Y", IsWorkableProduct = "Y" },
                 new ProductoModel { IsMagistral = "N", ProductoId = "REVE 12", ProductoName = "REVE 12", Unit = "PZ", LargeDescription = "Liiiiinea2", NeedsCooling = "Y", BarCode = "Linea1", IsLine = "Y", IsWorkableProduct = "Y" },
                 new ProductoModel { IsMagistral = "Y", ProductoId = "150   60 ML", ProductoName = "150   60 ML", Unit = "PZ", LargeDescription = "magi", NeedsCooling = "Y", BarCode = "mag", IsLine = "N", IsWorkableProduct = "Y" },
                 new ProductoModel { IsMagistral = "Y", ProductoId = "2643   120 ML", ProductoName = "2643   120 ML", Unit = "PZ", LargeDescription = "magi", NeedsCooling = "Y", BarCode = "mag", IsLine = "N", IsWorkableProduct = "Y" },
                 new ProductoModel { IsMagistral = "Y", ProductoId = "3567   120 ML", ProductoName = "3567   120 ML", Unit = "PZ", LargeDescription = "magi", NeedsCooling = "Y", BarCode = "mag", IsLine = "N", IsWorkableProduct = "Y" },
                 new ProductoModel { IsMagistral = "Y", ProductoId = "567   120 ML", ProductoName = "567   120 ML", Unit = "PZ", LargeDescription = "magi", NeedsCooling = "Y", BarCode = "mag", IsLine = "N", IsWorkableProduct = "Y" },
                 new ProductoModel { IsMagistral = "Y", ProductoId = "567   30 ML", ProductoName = "567   30 ML", Unit = "PZ", LargeDescription = "magi", NeedsCooling = "Y", BarCode = "mag", IsLine = "N", IsWorkableProduct = "Y" },
                 new ProductoModel { IsMagistral = "Y", ProductoId = "567   60 ML", ProductoName = "567   30 ML", Unit = "PZ", LargeDescription = "magi", NeedsCooling = "Y", BarCode = "mag", IsLine = "N", IsWorkableProduct = "Y" },
                 new ProductoModel { IsMagistral = "Y", ProductoId = "708   60 ML", ProductoName = "708   30 ML", Unit = "PZ", LargeDescription = "magi", NeedsCooling = "Y", BarCode = "mag", IsLine = "N", IsWorkableProduct = "Y" },
                 new ProductoModel { IsMagistral = "Y", ProductoId = "567   240 ML", ProductoName = "567  240 ML", Unit = "PZ", LargeDescription = "magi", NeedsCooling = "Y", BarCode = "mag", IsLine = "N", IsWorkableProduct = "Y" },
            };
        }

        /// <summary>
        /// Return the asesor.
        /// </summary>
        /// <returns>the asesor.</returns>
        public List<OrderModel> GetOrderModelForDoctorOrders()
        {
            return new List<OrderModel>
            {
                new OrderModel { PedidoId = 84503, AsesorId = 1, Codigo = "Codigo", DocNum = 84503, FechaFin = DateTime.Now, FechaInicio = DateTime.Today.AddDays(-1), Medico = "Medico A", PedidoStatus = "O", AtcEntry = 1, OrderType = "MQ", Canceled = "N", Address = "CDMX", DocNumDxp = "1234A1" },
                new OrderModel { PedidoId = 84517, AsesorId = 1, Codigo = "Codigo", DocNum = 84517, FechaFin = DateTime.Today.AddDays(1), FechaInicio = DateTime.Today.AddDays(-2), Medico = "Medico B", PedidoStatus = "O", Canceled = "N", Address = "CDMX", DocNumDxp = "1234A2" },
                new OrderModel { PedidoId = 84515, AsesorId = 1, Codigo = "Codigo", DocNum = 84515, FechaFin = DateTime.Today.AddDays(1), FechaInicio = DateTime.Today.AddDays(-2), Medico = "Medico B", PedidoStatus = "O", Canceled = "N", Address = "CDMX", DocNumDxp = "1234A3", IsOmigenomics = "Y" },
            };
        }

        /// <summary>
        /// Return the detalle pedido model.
        /// </summary>
        /// <returns>the asesor.</returns>
        public List<DetallePedidoModel> GetDetallePedidoForDoctorOrders()
        {
            return new List<DetallePedidoModel>
            {
                new DetallePedidoModel { Description = "DetallePedido", DetalleId = 0, PedidoId = 84503, ProductoId = "REVE 14", Container = "NA", Label = "NA", Quantity = 2 },
                new DetallePedidoModel { Description = "DetallePedido", DetalleId = 1, PedidoId = 84503, ProductoId = "2643   120 ML", Container = "NA", Label = "NA", Quantity = 2 },

                new DetallePedidoModel { Description = "DetallePedido", DetalleId = 0, PedidoId = 84517, ProductoId = "150   60 ML", Container = "NA", Quantity = 2, DocDate = DateTime.Today },
                new DetallePedidoModel { Description = "DetallePedido", DetalleId = 1, PedidoId = 84517, ProductoId = "708   60 ML", Container = "NA", Quantity = 2, DocDate = DateTime.Today },
                new DetallePedidoModel { Description = "DetallePedido", DetalleId = 2, PedidoId = 84517, ProductoId = "567   120 ML", Container = "NA", Quantity = 2, DocDate = DateTime.Today },

                new DetallePedidoModel { Description = "DetallePedido", DetalleId = 0, PedidoId = 84515, ProductoId = "REVE 12", Container = "NA", Quantity = 2, DocDate = DateTime.Today },
                new DetallePedidoModel { Description = "DetallePedido", DetalleId = 1, PedidoId = 84515, ProductoId = "REVE 14", Container = "NA", Quantity = 2, DocDate = DateTime.Today },
            };
        }

        /// <summary>
        /// Return the asesor.
        /// </summary>
        /// <returns>the asesor.</returns>
        public List<OrdenFabricacionModel> GetOrdenFabricacionModelForDoctorOrders()
        {
            return new List<OrdenFabricacionModel>
            {
                new OrdenFabricacionModel { ProductoId = "2643   120 ML", OrdenId = 122808, PostDate = DateTime.Now, Quantity = 10, Status = "L", PedidoId = 84503, User = 1, Type = "S", OriginType = "M", CardCode = "CardCode", CompleteQuantity = 10, CreatedDate = DateTime.Now, DataSource = "O", DueDate = DateTime.Now, ProdName = "Prodname", StartDate = DateTime.Now, Unit = "KG", Wharehouse = "PT" },
                new OrdenFabricacionModel { ProductoId = "567   120 ML", OrdenId = 122821, PostDate = DateTime.Now, Quantity = 10, Status = "L", PedidoId = 84515, User = 1, Type = "S", OriginType = "M", CardCode = string.Empty, CompleteQuantity = 10, CreatedDate = DateTime.Now, DataSource = "O", DueDate = DateTime.Now, ProdName = "Prodname", StartDate = DateTime.Now, Unit = "KG", Wharehouse = "PT", Comments = "token" },
                new OrdenFabricacionModel { ProductoId = "567   120 ML", OrdenId = 122824, PostDate = DateTime.Now, Quantity = 10, Status = "L", PedidoId = 84517, User = 1, Type = "S", OriginType = "M", CardCode = "CardCode", CompleteQuantity = 10, CreatedDate = DateTime.Today.AddDays(1), DataSource = "O", DueDate = DateTime.Now, ProdName = "Prodname", StartDate = DateTime.Now, Unit = "KG", Wharehouse = "PT" },
                new OrdenFabricacionModel { ProductoId = "708   60 ML", OrdenId = 122825, PostDate = DateTime.Now, Quantity = 10, Status = "L", PedidoId = 84517, User = 1, Type = "S", OriginType = "M", CardCode = "CardCode", CompleteQuantity = 10, CreatedDate = DateTime.Today.AddDays(1), DataSource = "O", DueDate = DateTime.Now, ProdName = "Prodname", StartDate = DateTime.Now, Unit = "KG", Wharehouse = "PT" },
                new OrdenFabricacionModel { ProductoId = "150   60 ML", OrdenId = 122826, PostDate = DateTime.Now, Quantity = 10, Status = "L", PedidoId = 84517, User = 1, Type = "S", OriginType = "M", CardCode = "CardCode", CompleteQuantity = 10, CreatedDate = DateTime.Today.AddDays(1), DataSource = "O", DueDate = DateTime.Now, ProdName = "Prodname", StartDate = DateTime.Now, Unit = "KG", Wharehouse = "PT" },
            };
        }

        /// <summary>
        /// gets the resultdto for getuserpedidos.
        /// </summary>
        /// <param name="returnsError">Indicates if pedidos services returns an error.</param>
        /// <param name="pedidoId">Pedido id.</param>
        /// <returns>the data.</returns>
        public ResultDto GetInvalidOrdersByMissingTecnicSign(bool returnsError, string pedidoId)
        {
            var listUsers = new List<string>();

            if (returnsError)
            {
                listUsers.Add(pedidoId);
            }

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
        /// Gets the attachments models.
        /// </summary>
        /// <returns>the data.</returns>
        public List<RawMaterialRequestModel> GetRawMaterialRequestModel()
        {
            return new List<RawMaterialRequestModel>
            {
               new RawMaterialRequestModel { DocEntry = 1, DocDate = DateTime.Today, Status = "O", AdditionalComments = "NOMBRE APELLIDO-3c1e872e-76db-4753-ae15-108b555bc18d", Canceled = "N", RequestedUserId = "3c1e872e-76db-4753-ae15-108b555bc18d" },
               new RawMaterialRequestModel { DocEntry = 2, DocDate = DateTime.Today, Status = "C", AdditionalComments = "NOMBRE APELLIDO-3c1e872e-76db-4753-ae15-108b555bc18d", Canceled = "N", RequestedUserId = "3c1e872e-76db-4753-ae15-108b555bc18d" },
               new RawMaterialRequestModel { DocEntry = 3, DocDate = DateTime.Today, Status = "C", AdditionalComments = "NOMBRE APELLIDO-3c1e872e-76db-4753-ae15-108b555bc18d", Canceled = "Y", RequestedUserId = "3c1e872e-76db-4753-ae15-108b555bc18d" },
               new RawMaterialRequestModel { DocEntry = 4, DocDate = DateTime.Today.AddDays(-3), Status = "O", AdditionalComments = "NOMBRE APELLIDO-db106faf-ef03-4c2e-9b7c-be0c7da8c0b7", Canceled = "N", RequestedUserId = "db106faf-ef03-4c2e-9b7c-be0c7da8c0b7" },
               new RawMaterialRequestModel { DocEntry = 5, DocDate = DateTime.Today.AddDays(-3), Status = "C", AdditionalComments = "NOMBRE APELLIDO-db106faf-ef03-4c2e-9b7c-be0c7da8c0b7", Canceled = "N", RequestedUserId = "db106faf-ef03-4c2e-9b7c-be0c7da8c0b7" },
               new RawMaterialRequestModel { DocEntry = 6, DocDate = DateTime.Today.AddDays(-3), Status = "C", AdditionalComments = "NOMBRE APELLIDO-db106faf-ef03-4c2e-9b7c-be0c7da8c0b7", Canceled = "Y", RequestedUserId = "db106faf-ef03-4c2e-9b7c-be0c7da8c0b7" },
            };
        }

        /// <summary>
        /// Gets the attachments models.
        /// </summary>
        /// <returns>the data.</returns>
        public List<RawMaterialRequestDetailModel> GetRawMaterialRequestDetailModel()
        {
            return new List<RawMaterialRequestDetailModel>
            {
               new RawMaterialRequestDetailModel { DocEntry = 1, ItemCode = "ITM 1", Description = "Descripción", Quantity = 1.234M, TargetWarehosue = "MG", Unit = "Paquete" },
               new RawMaterialRequestDetailModel { DocEntry = 2, ItemCode = "ITM 2", Description = "Descripción", Quantity = 2.234M, TargetWarehosue = "MG", Unit = "Paquete" },
               new RawMaterialRequestDetailModel { DocEntry = 3, ItemCode = "ITM 3", Description = "Descripción", Quantity = 3.234M, TargetWarehosue = "MG", Unit = "Paquete" },
               new RawMaterialRequestDetailModel { DocEntry = 4, ItemCode = "ITM 4", Description = "Descripción", Quantity = 4.234M, TargetWarehosue = "MG", Unit = "Paquete" },
               new RawMaterialRequestDetailModel { DocEntry = 5, ItemCode = "ITM 5", Description = "Descripción", Quantity = 5.234M, TargetWarehosue = "MG", Unit = "Paquete" },
               new RawMaterialRequestDetailModel { DocEntry = 6, ItemCode = "ITM 6", Description = "Descripción", Quantity = 6.234M, TargetWarehosue = "MG", Unit = "Paquete" },
            };
        }

        /// <summary>
        /// Gets the attachments models.
        /// </summary>
        /// <returns>the data.</returns>
        public List<WarehouseModel> GetWarehouse()
        {
            return new List<WarehouseModel>
            {
               new WarehouseModel { WarehouseCode = "AMP", WarehouseName = "Materias Primas Alfareros", },
               new WarehouseModel { WarehouseCode = "be", WarehouseName = "BIOEQUAL", },
            };
        }

        /// <summary>
        /// Gets the attachments models.
        /// </summary>
        /// <returns>the data.</returns>
        public List<CompleteOrderModel> GetCompleteOrderModel()
        {
            return new List<CompleteOrderModel>
            {
               new CompleteOrderModel { DocNum = 175623, Cliente = "LUIS JAVIER GARCIA AQUINO", Codigo = "C02804", FechaInicio = "28/05/2025", FechaFin = "07/06/2025", PedidoStatus = "O", OrderType = "MG", DocNumDxp = "37058e65-2de5-44ae-bb22-a5be0c891ad2", ClientType = "general" },
               new CompleteOrderModel { DocNum = 175627, Cliente = "LUIS JAVIER GARCIA AQUINO", Codigo = "C02804", FechaInicio = "28/05/2025", FechaFin = "07/06/2025", PedidoStatus = "O", OrderType = "MN", DocNumDxp = "37058e65-2de5-44ae-bb22-a5be0c891ad2", ClientType = "general" },
               new CompleteOrderModel { DocNum = 175629, Cliente = "LUIS JAVIER GARCIA AQUINO", Codigo = "C02804", FechaInicio = "28/05/2025", FechaFin = "07/06/2025", PedidoStatus = "O", OrderType = "BE", DocNumDxp = "37058e65-2de5-44ae-bb22-a5be0c891ad2", ClientType = "general" },
            };
        }

        /// <summary>
        /// Gets the attachments models.
        /// </summary>
        /// <returns>the data.</returns>
        public ResultDto GetUserOrders()
        {
            var userOrders = new List<UserOrderModel>
            {
                new UserOrderModel { Id = 1000, Productionorderid = "226274", Salesorderid = "175623", Status = "Finalizado", Userid = "8df154e0-5061-4749-b06e-6bd3a1aebef8", CloseDate = new DateTime(2025, 5, 28), Comments = "comments", FinishDate = new DateTime(2025, 5, 28), FinishedLabel = 1 },
                new UserOrderModel { Id = 1002, Productionorderid = "226278", Salesorderid = "175627", Status = "Finalizado", Userid = "8df154e0-5061-4749-b06e-6bd3a1aebef8", CloseDate = new DateTime(2025, 5, 28), Comments = "comments", FinishDate = new DateTime(2025, 5, 28), FinishedLabel = 1 },
                new UserOrderModel { Id = 1003, Productionorderid = "226280", Salesorderid = "175629", Status = "Finalizado", Userid = "8df154e0-5061-4749-b06e-6bd3a1aebef8", CloseDate = new DateTime(2025, 5, 28), Comments = "comments", FinishDate = new DateTime(2025, 5, 28), FinishedLabel = 1 },
            };

            return new ResultDto
            {
                Response = JsonConvert.SerializeObject(userOrders),
                Code = 200,
                Comments = string.Empty,
                ExceptionMessage = string.Empty,
                Success = true,
                UserError = string.Empty,
            };
        }
    }
}
