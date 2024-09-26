// <summary>
// <copyright file="TestModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Test
{
    /// <summary>
    /// this testmodels.
    /// </summary>
    [TestFixture]
    public class TestModel
    {
        /// <summary>
        /// test the asesor.
        /// </summary>
        [Test]
        public void AsesorModelTest()
        {
            var asesor = new AsesorModel
            {
                AsesorId = 1,
                AsesorName = "Gustavo",
            };

            Assert.That(asesor, Is.Not.Null);
            Assert.That(asesor.AsesorId, Is.Not.Null);
            Assert.That(asesor.AsesorName, Is.Not.Null);
        }

        /// <summary>
        /// test the asesor.
        /// </summary>
        [Test]
        public void DetallePedidoModel()
        {
            var asesor = new DetallePedidoModel { Description = "DetallePedido", DetalleId = 1, PedidoId = 100, ProductoId = "Abc Aspirina", Container = "container", Label = "label", Quantity = 10 };

            Assert.That(asesor.Description, Is.Not.Null);
            Assert.That(asesor.DetalleId, Is.Not.Null);
            Assert.That(asesor.Container, Is.Not.Null);
            Assert.That(asesor.Label, Is.Not.Null);
            Assert.That(asesor.PedidoId, Is.Not.Null);
            Assert.That(asesor.ProductoId, Is.Not.Null);
            Assert.That(asesor.Quantity, Is.Not.Null);
        }

        /// <summary>
        /// test the asesor.
        /// </summary>
        [Test]
        public void OrdenFabricaionModel()
        {
            var asesor = new OrdenFabricacionModel { ProductoId = "Abc Aspirina", OrdenId = 100, PostDate = DateTime.Now, Quantity = 2, Status = "L", PedidoId = 100, CardCode = "card", Comments = "comment", CompleteQuantity = 10, CreatedDate = DateTime.Now, DataSource = "0", DueDate = DateTime.Now, OriginType = "0", ProdName = "name", StartDate = DateTime.Now, Type = "y", Unit = "U", User = 1, Wharehouse = "w" };
            Assert.That(asesor.CardCode, Is.Not.Null);
            Assert.That(asesor.Comments, Is.Not.Null);
            Assert.That(asesor.CompleteQuantity, Is.Not.Null);
            Assert.That(asesor.CreatedDate, Is.Not.Null);
            Assert.That(asesor.DataSource, Is.Not.Null);
            Assert.That(asesor.DueDate, Is.Not.Null);
            Assert.That(asesor.OrdenId, Is.Not.Null);
            Assert.That(asesor.OriginType, Is.Not.Null);
            Assert.That(asesor.PedidoId, Is.Not.Null);
            Assert.That(asesor.PostDate, Is.Not.Null);
            Assert.That(asesor.ProdName, Is.Not.Null);
            Assert.That(asesor.ProductoId, Is.Not.Null);
            Assert.That(asesor.Quantity, Is.Not.Null);
            Assert.That(asesor.StartDate, Is.Not.Null);
            Assert.That(asesor.Status, Is.Not.Null);
            Assert.That(asesor.Type, Is.Not.Null);
            Assert.That(asesor.Unit, Is.Not.Null);
            Assert.That(asesor.User, Is.Not.Null);
            Assert.That(asesor.Wharehouse, Is.Not.Null);
        }

        /// <summary>
        /// test the asesor.
        /// </summary>
        [Test]
        public void OrdenModel()
        {
            var asesor = new OrderModel { PedidoId = 100, AsesorId = 1, Codigo = "Codigo", DocNum = 100, FechaFin = DateTime.Now, FechaInicio = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), Medico = "Medico", PedidoStatus = "C" };
            Assert.That(asesor.AsesorId, Is.Not.Null);
            Assert.That(asesor.Codigo, Is.Not.Null);
            Assert.That(asesor.DocNum, Is.Not.Null);
            Assert.That(asesor.FechaFin, Is.Not.Null);
            Assert.That(asesor.FechaInicio, Is.Not.Null);
            Assert.That(asesor.Medico, Is.Not.Null);
            Assert.That(asesor.PedidoId, Is.Not.Null);
            Assert.That(asesor.PedidoStatus, Is.Not.Null);
        }

        /// <summary>
        /// test the asesor.
        /// </summary>
        [Test]
        public void ProductoModel()
        {
            var asesor = new ProductoModel { IsMagistral = "Y", ProductoId = "Abc Aspirina", ProductoName = "Aspirina", LargeDescription = "large", ManagedBatches = "batches", OnHand = 10, Unit = "KG" };
            Assert.That(asesor.IsMagistral, Is.Not.Null);
            Assert.That(asesor.LargeDescription, Is.Not.Null);
            Assert.That(asesor.ManagedBatches, Is.Not.Null);
            Assert.That(asesor.OnHand, Is.Not.Null);
            Assert.That(asesor.ProductoId, Is.Not.Null);
            Assert.That(asesor.ProductoName, Is.Not.Null);
            Assert.That(asesor.Unit, Is.Not.Null);
        }

        /// <summary>
        /// test the asesor.
        /// </summary>
        [Test]
        public void CompleteDetailOrderModel()
        {
            var asesor = new CompleteDetailOrderModel { CodigoProducto = "Abc Aspirina", DescripcionProducto = "Aspirina", FechaOf = "28/03/2020", FechaOfFin = "28/03/2020", IsChecked = false, OrdenFabricacionId = 100, Qfb = "Gustavo", QtyPlanned = 1, Status = "L", DescripcionCorta = "corta", PedidoStatus = "S", QtyPlannedDetalle = 10, HasMissingStock = false };

            Assert.That(asesor.CodigoProducto, Is.Not.Null);
            Assert.That(asesor.DescripcionCorta, Is.Not.Null);
            Assert.That(asesor.DescripcionProducto, Is.Not.Null);
            Assert.That(asesor.FechaOf, Is.Not.Null);
            Assert.That(asesor.FechaOfFin, Is.Not.Null);
            Assert.That(asesor.IsChecked, Is.Not.Null);
            Assert.That(asesor.OrdenFabricacionId, Is.Not.Null);
            Assert.That(asesor.PedidoStatus, Is.Not.Null);
            Assert.That(asesor.Qfb, Is.Not.Null);
            Assert.That(asesor.QtyPlanned, Is.Not.Null);
            Assert.That(asesor.QtyPlannedDetalle, Is.Not.Null);
            Assert.That(asesor.Status, Is.Not.Null);
            Assert.That(asesor.HasMissingStock, Is.Not.Null);
        }

        /// <summary>
        /// test the asesor.
        /// </summary>
        [Test]
        public void CompleteOrderModel()
        {
            var asesor = new CompleteOrderModel { AsesorName = "asesor", Cliente = "cliente", Codigo = "codigo", DocNum = 100, FechaFin = "fecha", FechaInicio = "fecha", IsChecked = true, Medico = "Medico", PedidoStatus = "L", Qfb = "qfb" };
            Assert.That(asesor.AsesorName, Is.Not.Null);
            Assert.That(asesor.Cliente, Is.Not.Null);
            Assert.That(asesor.Codigo, Is.Not.Null);
            Assert.That(asesor.DocNum, Is.Not.Null);
            Assert.That(asesor.FechaFin, Is.Not.Null);
            Assert.That(asesor.FechaInicio, Is.Not.Null);
            Assert.That(asesor.Medico, Is.Not.Null);
            Assert.That(asesor.IsChecked, Is.Not.Null);
            Assert.That(asesor.PedidoStatus, Is.Not.Null);
            Assert.That(asesor.Qfb, Is.Not.Null);
        }

        /// <summary>
        /// test the asesor.
        /// </summary>
        [Test]
        public void BatchesComponentModel()
        {
            var asesor = new BatchesComponentModel { Almacen = "Almacen", CodigoProducto = "codogp", DescripcionProducto = "dsc", Lotes = new List<ValidBatches>(), LotesAsignados = new List<AssignedBatches>(), TotalNecesario = 10, TotalSeleccionado = 10 };
            Assert.That(asesor.Almacen, Is.Not.Null);
            Assert.That(asesor.CodigoProducto, Is.Not.Null);
            Assert.That(asesor.DescripcionProducto, Is.Not.Null);
            Assert.That(asesor.Lotes, Is.Not.Null);
            Assert.That(asesor.LotesAsignados, Is.Not.Null);
            Assert.That(asesor.TotalNecesario, Is.Not.Null);
            Assert.That(asesor.TotalSeleccionado, Is.Not.Null);
        }

        /// <summary>
        /// test the asesor.
        /// </summary>
        [Test]
        public void CompleteFormulaWithDetalle()
        {
            var asesor = new CompleteFormulaWithDetalle { BaseDocument = 1, Client = "C", Code = "C", Comments = "C", CompleteQuantity = 10, Container = "L", Details = new List<CompleteDetalleFormulaModel>(), DueDate = "D", EndDate = "E", FabDate = "F", IsChecked = true, Number = 1, Origin = "O", PlannedQuantity = 1, ProductDescription = "P", ProductionOrderId = 1, ProductLabel = "l", RealEndDate = "L", StartDate = "S", Status = "S", Type = "T", Unit = "U", User = "U", Warehouse = "w" };

            Assert.That(asesor.BaseDocument, Is.Not.Null);
            Assert.That(asesor.Client, Is.Not.Null);
            Assert.That(asesor.Code, Is.Not.Null);
            Assert.That(asesor.Comments, Is.Not.Null);
            Assert.That(asesor.CompleteQuantity, Is.Not.Null);
            Assert.That(asesor.Container, Is.Not.Null);
            Assert.That(asesor.Details, Is.Not.Null);
            Assert.That(asesor.DueDate, Is.Not.Null);
            Assert.That(asesor.EndDate, Is.Not.Null);
            Assert.That(asesor.FabDate, Is.Not.Null);
            Assert.That(asesor.IsChecked, Is.Not.Null);
            Assert.That(asesor.Number, Is.Not.Null);
            Assert.That(asesor.Origin, Is.Not.Null);
            Assert.That(asesor.PlannedQuantity, Is.Not.Null);
            Assert.That(asesor.ProductDescription, Is.Not.Null);
            Assert.That(asesor.ProductionOrderId, Is.Not.Null);
            Assert.That(asesor.ProductLabel, Is.Not.Null);
            Assert.That(asesor.RealEndDate, Is.Not.Null);
            Assert.That(asesor.StartDate, Is.Not.Null);
            Assert.That(asesor.Status, Is.Not.Null);
            Assert.That(asesor.Type, Is.Not.Null);
            Assert.That(asesor.Unit, Is.Not.Null);
            Assert.That(asesor.User, Is.Not.Null);
            Assert.That(asesor.Warehouse, Is.Not.Null);
        }

        /// <summary>
        /// test the asesor.
        /// </summary>
        [Test]
        public void DetalleFormulaModel()
        {
            var asesor = new DetalleFormulaModel { Almacen = "A", BaseQuantity = 1, ConsumidoQty = 1, ItemCode = "I", LineNum = 1, OrderFabId = 1, RequiredQty = 1, UnidadCode = "U" };
            Assert.That(asesor.Almacen, Is.Not.Null);
            Assert.That(asesor.BaseQuantity, Is.Not.Null);
            Assert.That(asesor.ConsumidoQty, Is.Not.Null);
            Assert.That(asesor.ItemCode, Is.Not.Null);
            Assert.That(asesor.LineNum, Is.Not.Null);
            Assert.That(asesor.OrderFabId, Is.Not.Null);
            Assert.That(asesor.RequiredQty, Is.Not.Null);
            Assert.That(asesor.UnidadCode, Is.Not.Null);
        }

        /// <summary>
        /// test the asesor.
        /// </summary>
        [Test]
        public void OrderWithDetailModel()
        {
            var asesor = new OrderWithDetailModel { Detalle = new List<CompleteDetailOrderModel>(), Order = new OrderModel() };
            Assert.That(asesor.Detalle, Is.Not.Null);
            Assert.That(asesor.Order, Is.Not.Null);
        }

        /// <summary>
        /// test the asesor.
        /// </summary>
        [Test]
        public void AssignedBatches()
        {
            var asesor = new AssignedBatches { CantidadSeleccionada = 1, NumeroLote = "S", SysNumber = 1 };
            Assert.That(asesor.CantidadSeleccionada, Is.Not.Null);
            Assert.That(asesor.NumeroLote, Is.Not.Null);
            Assert.That(asesor.SysNumber, Is.Not.Null);
        }

        /// <summary>
        /// test the asesor.
        /// </summary>
        [Test]
        public void CompleteDetalleFormulaModel()
        {
            var asesor = new CompleteDetalleFormulaModel { Available = 1,  BaseQuantity = 1, Consumed = 1, Description = "D2", OrderFabId = 1, PendingQuantity = 1, ProductId = "P", RequiredQuantity = 1, Stock = 1, Unit = "U", Warehouse = "W", WarehouseQuantity = 1 };
            Assert.That(asesor.Available, Is.Not.Null);
            Assert.That(asesor.BaseQuantity, Is.Not.Null);
            Assert.That(asesor.Consumed, Is.Not.Null);
            Assert.That(asesor.Description, Is.Not.Null);
            Assert.That(asesor.OrderFabId, Is.Not.Null);
            Assert.That(asesor.PendingQuantity, Is.Not.Null);
            Assert.That(asesor.ProductId, Is.Not.Null);
            Assert.That(asesor.RequiredQuantity, Is.Not.Null);
            Assert.That(asesor.Stock, Is.Not.Null);
            Assert.That(asesor.Unit, Is.Not.Null);
            Assert.That(asesor.Warehouse, Is.Not.Null);
            Assert.That(asesor.WarehouseQuantity, Is.Not.Null);
        }

        /// <summary>
        /// test the asesor.
        /// </summary>
        [Test]
        public void Batches()
        {
            var asesor = new Batches { AbsEntry = 1, DistNumber = "S", ItemCode = "S", SysNumber = 1 };
            Assert.That(asesor.AbsEntry, Is.Not.Null);
            Assert.That(asesor.DistNumber, Is.Not.Null);
            Assert.That(asesor.ItemCode, Is.Not.Null);
            Assert.That(asesor.SysNumber, Is.Not.Null);
        }

        /// <summary>
        /// test the asesor.
        /// </summary>
        [Test]
        public void BatchesQuantity()
        {
            var asesor = new BatchesQuantity { AbsEntry = 1, CommitQty = 1, ItemCode = "S", Quantity = 1, SysNumber = 1, WhsCode = "S" };
            Assert.That(asesor.AbsEntry, Is.Not.Null);
            Assert.That(asesor.CommitQty, Is.Not.Null);
            Assert.That(asesor.ItemCode, Is.Not.Null);
            Assert.That(asesor.Quantity, Is.Not.Null);
            Assert.That(asesor.SysNumber, Is.Not.Null);
            Assert.That(asesor.WhsCode, Is.Not.Null);
        }

        /// <summary>
        /// test the asesor.
        /// </summary>
        [Test]
        public void BatchesTransactionQtyModel()
        {
            var asesor = new BatchesTransactionQtyModel { AllocQty = 1, ItemCode = "S", LogEntry = 1, SysNumber = 1 };
            Assert.That(asesor.AllocQty, Is.Not.Null);
            Assert.That(asesor.LogEntry, Is.Not.Null);
            Assert.That(asesor.ItemCode, Is.Not.Null);
            Assert.That(asesor.SysNumber, Is.Not.Null);
        }

        /// <summary>
        /// test the asesor.
        /// </summary>
        [Test]
        public void BatchTransacitions()
        {
            var asesor = new BatchTransacitions { DocNum = 2, DocQuantity = 2, ItemCode = "S", LogEntry = 1 };
            Assert.That(asesor.DocNum, Is.Not.Null);
            Assert.That(asesor.LogEntry, Is.Not.Null);
            Assert.That(asesor.ItemCode, Is.Not.Null);
            Assert.That(asesor.DocQuantity, Is.Not.Null);
        }

        /// <summary>
        /// test the asesor.
        /// </summary>
        [Test]
        public void ValidBatches()
        {
            var asesor = new ValidBatches { CantidadAsignada = 1, CantidadDisponible = 1, NumeroLote = "W", SysNumber = 1 };
            Assert.That(asesor.CantidadAsignada, Is.Not.Null);
            Assert.That(asesor.CantidadDisponible, Is.Not.Null);
            Assert.That(asesor.NumeroLote, Is.Not.Null);
            Assert.That(asesor.SysNumber, Is.Not.Null);
        }

        /// <summary>
        /// test the asesor.
        /// </summary>
        [Test]
        public void ItemWarehouseModel()
        {
            var asesor = new ItemWarehouseModel { IsCommited = 1, ItemCode = "S", OnHand = 1, OnOrder = 2, WhsCode = "S" };
            Assert.That(asesor.IsCommited, Is.Not.Null);
            Assert.That(asesor.ItemCode, Is.Not.Null);
            Assert.That(asesor.OnHand, Is.Not.Null);
            Assert.That(asesor.OnOrder, Is.Not.Null);
            Assert.That(asesor.WhsCode, Is.Not.Null);
        }

        /// <summary>
        /// test the sales asesor model.
        /// </summary>
        [Test]
        public void SalesAsesorModel()
        {
            var asesor = new SalesAsesorModel { Email = "test@test.com", Cliente = "Juan perez", OrderId = 100 };
            Assert.That(asesor.Email, Is.Not.Null);
            Assert.That(asesor.Cliente, Is.Not.Null);
            Assert.That(asesor.OrderId, Is.Not.Null);
        }

        /// <summary>
        /// test the packing required model.
        /// </summary>
        [Test]
        public void PackingRequiredModel()
        {
            var packing = new PackingRequiredModel { CodeItem = "123-abc", Description = "this is a description", Quantity = 14, Unit = "pz" };
            Assert.That(packing.CodeItem, Is.Not.Null);
            Assert.That(packing.Description, Is.Not.Null);
            Assert.That(packing.Quantity, Is.Not.Null);
            Assert.That(packing.Unit, Is.Not.Null);
        }

        /// <summary>
        /// test the order reciper model.
        /// </summary>
        [Test]
        public void OrderRecipeModel()
        {
            var order = new OrderRecipeModel { Order = 123, Recipe = "pz" };
            Assert.That(order.Order, Is.Not.Null);
            Assert.That(order.Recipe, Is.Not.Null);
        }

        /// <summary>
        /// test the order validation response.
        /// </summary>
        [Test]
        public void OrderValidationResponse()
        {
            var order = new OrderValidationResponse { Type = "pz", ListItems = new List<string>() { "ab", "cd" } };
            Assert.That(order.Type, Is.Not.Null);
            Assert.That(order.ListItems, Is.Not.Null);
        }

        /// <summary>
        /// test the order validation response.
        /// </summary>
        [Test]
        public void SalesPersonModel()
        {
            var salesPerson = new SalesPersonModel { AsesorId = 123, FirstName = "abc", LastName = "sanchez", EmpleadoId = 1, Email = "test@test.com" };
            Assert.That(salesPerson.AsesorId, Is.Not.Null);
            Assert.That(salesPerson.FirstName, Is.Not.Null);
            Assert.That(salesPerson.LastName, Is.Not.Null);
            Assert.That(salesPerson.EmpleadoId, Is.Not.Null);
            Assert.That(salesPerson.Email, Is.Not.Null);
        }

        /// <summary>
        /// test the user model.
        /// </summary>
        [Test]
        public void UserModel()
        {
            var user = new Users { UserId = 123, UserName = "myname123" };
            Assert.That(user.UserId, Is.Not.Null);
            Assert.That(user.UserName, Is.Not.Null);
        }

        /// <summary>
        /// test the attachment model.
        /// </summary>
        [Test]
        public void AttachmentModel()
        {
            var attachment = new AttachmentModel { AbsEntry = 1, TargetPath = "C:/target/", SourcePath = "C:/folder/", FileExt = "txt", FileName = "users", Line = 1, };
            Assert.That(attachment.AbsEntry, Is.Not.Null);
            Assert.That(attachment.TargetPath, Is.Not.Null);
            Assert.That(attachment.SourcePath, Is.Not.Null);
            Assert.That(attachment.FileName, Is.Not.Null);
            Assert.That(attachment.FileExt, Is.Not.Null);
            Assert.That(attachment.Line, Is.Not.Null);
            Assert.That(attachment.CompletePath, Is.Not.Null);
        }
    }
}