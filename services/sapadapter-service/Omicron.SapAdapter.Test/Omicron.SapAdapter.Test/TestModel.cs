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

            ClassicAssert.IsNotNull(asesor);
            ClassicAssert.IsNotNull(asesor.AsesorId);
            ClassicAssert.IsNotNull(asesor.AsesorName);
        }

        /// <summary>
        /// test the asesor.
        /// </summary>
        [Test]
        public void DetallePedidoModel()
        {
            var asesor = new DetallePedidoModel { Description = "DetallePedido", DetalleId = 1, PedidoId = 100, ProductoId = "Abc Aspirina", Container = "container", Label = "label", Quantity = 10 };

            ClassicAssert.IsNotNull(asesor.Description);
            ClassicAssert.IsNotNull(asesor.DetalleId);
            ClassicAssert.IsNotNull(asesor.Container);
            ClassicAssert.IsNotNull(asesor.Label);
            ClassicAssert.IsNotNull(asesor.PedidoId);
            ClassicAssert.IsNotNull(asesor.ProductoId);
            ClassicAssert.IsNotNull(asesor.Quantity);
        }

        /// <summary>
        /// test the asesor.
        /// </summary>
        [Test]
        public void OrdenFabricaionModel()
        {
            var asesor = new OrdenFabricacionModel { ProductoId = "Abc Aspirina", OrdenId = 100, PostDate = DateTime.Now, Quantity = 2, Status = "L", PedidoId = 100, CardCode = "card", Comments = "comment", CompleteQuantity = 10, CreatedDate = DateTime.Now, DataSource = "0", DueDate = DateTime.Now, OriginType = "0", ProdName = "name", StartDate = DateTime.Now, Type = "y", Unit = "U", User = 1, Wharehouse = "w" };
            ClassicAssert.IsNotNull(asesor.CardCode);
            ClassicAssert.IsNotNull(asesor.Comments);
            ClassicAssert.IsNotNull(asesor.CompleteQuantity);
            ClassicAssert.IsNotNull(asesor.CreatedDate);
            ClassicAssert.IsNotNull(asesor.DataSource);
            ClassicAssert.IsNotNull(asesor.DueDate);
            ClassicAssert.IsNotNull(asesor.OrdenId);
            ClassicAssert.IsNotNull(asesor.OriginType);
            ClassicAssert.IsNotNull(asesor.PedidoId);
            ClassicAssert.IsNotNull(asesor.PostDate);
            ClassicAssert.IsNotNull(asesor.ProdName);
            ClassicAssert.IsNotNull(asesor.ProductoId);
            ClassicAssert.IsNotNull(asesor.Quantity);
            ClassicAssert.IsNotNull(asesor.StartDate);
            ClassicAssert.IsNotNull(asesor.Status);
            ClassicAssert.IsNotNull(asesor.Type);
            ClassicAssert.IsNotNull(asesor.Unit);
            ClassicAssert.IsNotNull(asesor.User);
            ClassicAssert.IsNotNull(asesor.Wharehouse);
        }

        /// <summary>
        /// test the asesor.
        /// </summary>
        [Test]
        public void OrdenModel()
        {
            var asesor = new OrderModel { PedidoId = 100, AsesorId = 1, Codigo = "Codigo", DocNum = 100, FechaFin = DateTime.Now, FechaInicio = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), Medico = "Medico", PedidoStatus = "C" };
            ClassicAssert.IsNotNull(asesor.AsesorId);
            ClassicAssert.IsNotNull(asesor.Codigo);
            ClassicAssert.IsNotNull(asesor.DocNum);
            ClassicAssert.IsNotNull(asesor.FechaFin);
            ClassicAssert.IsNotNull(asesor.FechaInicio);
            ClassicAssert.IsNotNull(asesor.Medico);
            ClassicAssert.IsNotNull(asesor.PedidoId);
            ClassicAssert.IsNotNull(asesor.PedidoStatus);
        }

        /// <summary>
        /// test the asesor.
        /// </summary>
        [Test]
        public void ProductoModel()
        {
            var asesor = new ProductoModel { IsMagistral = "Y", ProductoId = "Abc Aspirina", ProductoName = "Aspirina", LargeDescription = "large", ManagedBatches = "batches", OnHand = 10, Unit = "KG" };
            ClassicAssert.IsNotNull(asesor.IsMagistral);
            ClassicAssert.IsNotNull(asesor.LargeDescription);
            ClassicAssert.IsNotNull(asesor.ManagedBatches);
            ClassicAssert.IsNotNull(asesor.OnHand);
            ClassicAssert.IsNotNull(asesor.ProductoId);
            ClassicAssert.IsNotNull(asesor.ProductoName);
            ClassicAssert.IsNotNull(asesor.Unit);
        }

        /// <summary>
        /// test the asesor.
        /// </summary>
        [Test]
        public void CompleteDetailOrderModel()
        {
            var asesor = new CompleteDetailOrderModel { CodigoProducto = "Abc Aspirina", DescripcionProducto = "Aspirina", FechaOf = "28/03/2020", FechaOfFin = "28/03/2020", IsChecked = false, OrdenFabricacionId = 100, Qfb = "Gustavo", QtyPlanned = 1, Status = "L", DescripcionCorta = "corta", PedidoStatus = "S", QtyPlannedDetalle = 10, HasMissingStock = false };

            ClassicAssert.IsNotNull(asesor.CodigoProducto);
            ClassicAssert.IsNotNull(asesor.DescripcionCorta);
            ClassicAssert.IsNotNull(asesor.DescripcionProducto);
            ClassicAssert.IsNotNull(asesor.FechaOf);
            ClassicAssert.IsNotNull(asesor.FechaOfFin);
            ClassicAssert.IsNotNull(asesor.IsChecked);
            ClassicAssert.IsNotNull(asesor.OrdenFabricacionId);
            ClassicAssert.IsNotNull(asesor.PedidoStatus);
            ClassicAssert.IsNotNull(asesor.Qfb);
            ClassicAssert.IsNotNull(asesor.QtyPlanned);
            ClassicAssert.IsNotNull(asesor.QtyPlannedDetalle);
            ClassicAssert.IsNotNull(asesor.Status);
            ClassicAssert.IsNotNull(asesor.HasMissingStock);
        }

        /// <summary>
        /// test the asesor.
        /// </summary>
        [Test]
        public void CompleteOrderModel()
        {
            var asesor = new CompleteOrderModel { AsesorName = "asesor", Cliente = "cliente", Codigo = "codigo", DocNum = 100, FechaFin = "fecha", FechaInicio = "fecha", IsChecked = true, Medico = "Medico", PedidoStatus = "L", Qfb = "qfb" };
            ClassicAssert.IsNotNull(asesor.AsesorName);
            ClassicAssert.IsNotNull(asesor.Cliente);
            ClassicAssert.IsNotNull(asesor.Codigo);
            ClassicAssert.IsNotNull(asesor.DocNum);
            ClassicAssert.IsNotNull(asesor.FechaFin);
            ClassicAssert.IsNotNull(asesor.FechaInicio);
            ClassicAssert.IsNotNull(asesor.Medico);
            ClassicAssert.IsNotNull(asesor.IsChecked);
            ClassicAssert.IsNotNull(asesor.PedidoStatus);
            ClassicAssert.IsNotNull(asesor.Qfb);
        }

        /// <summary>
        /// test the asesor.
        /// </summary>
        [Test]
        public void BatchesComponentModel()
        {
            var asesor = new BatchesComponentModel { Almacen = "Almacen", CodigoProducto = "codogp", DescripcionProducto = "dsc", Lotes = new List<ValidBatches>(), LotesAsignados = new List<AssignedBatches>(), TotalNecesario = 10, TotalSeleccionado = 10 };
            ClassicAssert.IsNotNull(asesor.Almacen);
            ClassicAssert.IsNotNull(asesor.CodigoProducto);
            ClassicAssert.IsNotNull(asesor.DescripcionProducto);
            ClassicAssert.IsNotNull(asesor.Lotes);
            ClassicAssert.IsNotNull(asesor.LotesAsignados);
            ClassicAssert.IsNotNull(asesor.TotalNecesario);
            ClassicAssert.IsNotNull(asesor.TotalSeleccionado);
        }

        /// <summary>
        /// test the asesor.
        /// </summary>
        [Test]
        public void CompleteFormulaWithDetalle()
        {
            var asesor = new CompleteFormulaWithDetalle { BaseDocument = 1, Client = "C", Code = "C", Comments = "C", CompleteQuantity = 10, Container = "L", Details = new List<CompleteDetalleFormulaModel>(), DueDate = "D", EndDate = "E", FabDate = "F", IsChecked = true, Number = 1, Origin = "O", PlannedQuantity = 1, ProductDescription = "P", ProductionOrderId = 1, ProductLabel = "l", RealEndDate = "L", StartDate = "S", Status = "S", Type = "T", Unit = "U", User = "U", Warehouse = "w" };

            ClassicAssert.IsNotNull(asesor.BaseDocument);
            ClassicAssert.IsNotNull(asesor.Client);
            ClassicAssert.IsNotNull(asesor.Code);
            ClassicAssert.IsNotNull(asesor.Comments);
            ClassicAssert.IsNotNull(asesor.CompleteQuantity);
            ClassicAssert.IsNotNull(asesor.Container);
            ClassicAssert.IsNotNull(asesor.Details);
            ClassicAssert.IsNotNull(asesor.DueDate);
            ClassicAssert.IsNotNull(asesor.EndDate);
            ClassicAssert.IsNotNull(asesor.FabDate);
            ClassicAssert.IsNotNull(asesor.IsChecked);
            ClassicAssert.IsNotNull(asesor.Number);
            ClassicAssert.IsNotNull(asesor.Origin);
            ClassicAssert.IsNotNull(asesor.PlannedQuantity);
            ClassicAssert.IsNotNull(asesor.ProductDescription);
            ClassicAssert.IsNotNull(asesor.ProductionOrderId);
            ClassicAssert.IsNotNull(asesor.ProductLabel);
            ClassicAssert.IsNotNull(asesor.RealEndDate);
            ClassicAssert.IsNotNull(asesor.StartDate);
            ClassicAssert.IsNotNull(asesor.Status);
            ClassicAssert.IsNotNull(asesor.Type);
            ClassicAssert.IsNotNull(asesor.Unit);
            ClassicAssert.IsNotNull(asesor.User);
            ClassicAssert.IsNotNull(asesor.Warehouse);
        }

        /// <summary>
        /// test the asesor.
        /// </summary>
        [Test]
        public void DetalleFormulaModel()
        {
            var asesor = new DetalleFormulaModel { Almacen = "A", BaseQuantity = 1, ConsumidoQty = 1, ItemCode = "I", LineNum = 1, OrderFabId = 1, RequiredQty = 1, UnidadCode = "U" };
            ClassicAssert.IsNotNull(asesor.Almacen);
            ClassicAssert.IsNotNull(asesor.BaseQuantity);
            ClassicAssert.IsNotNull(asesor.ConsumidoQty);
            ClassicAssert.IsNotNull(asesor.ItemCode);
            ClassicAssert.IsNotNull(asesor.LineNum);
            ClassicAssert.IsNotNull(asesor.OrderFabId);
            ClassicAssert.IsNotNull(asesor.RequiredQty);
            ClassicAssert.IsNotNull(asesor.UnidadCode);
        }

        /// <summary>
        /// test the asesor.
        /// </summary>
        [Test]
        public void OrderWithDetailModel()
        {
            var asesor = new OrderWithDetailModel { Detalle = new List<CompleteDetailOrderModel>(), Order = new OrderModel() };
            ClassicAssert.IsNotNull(asesor.Detalle);
            ClassicAssert.IsNotNull(asesor.Order);
        }

        /// <summary>
        /// test the asesor.
        /// </summary>
        [Test]
        public void AssignedBatches()
        {
            var asesor = new AssignedBatches { CantidadSeleccionada = 1, NumeroLote = "S", SysNumber = 1 };
            ClassicAssert.IsNotNull(asesor.CantidadSeleccionada);
            ClassicAssert.IsNotNull(asesor.NumeroLote);
            ClassicAssert.IsNotNull(asesor.SysNumber);
        }

        /// <summary>
        /// test the asesor.
        /// </summary>
        [Test]
        public void CompleteDetalleFormulaModel()
        {
            var asesor = new CompleteDetalleFormulaModel { Available = 1,  BaseQuantity = 1, Consumed = 1, Description = "D2", OrderFabId = 1, PendingQuantity = 1, ProductId = "P", RequiredQuantity = 1, Stock = 1, Unit = "U", Warehouse = "W", WarehouseQuantity = 1 };
            ClassicAssert.IsNotNull(asesor.Available);
            ClassicAssert.IsNotNull(asesor.BaseQuantity);
            ClassicAssert.IsNotNull(asesor.Consumed);
            ClassicAssert.IsNotNull(asesor.Description);
            ClassicAssert.IsNotNull(asesor.OrderFabId);
            ClassicAssert.IsNotNull(asesor.PendingQuantity);
            ClassicAssert.IsNotNull(asesor.ProductId);
            ClassicAssert.IsNotNull(asesor.RequiredQuantity);
            ClassicAssert.IsNotNull(asesor.Stock);
            ClassicAssert.IsNotNull(asesor.Unit);
            ClassicAssert.IsNotNull(asesor.Warehouse);
            ClassicAssert.IsNotNull(asesor.WarehouseQuantity);
        }

        /// <summary>
        /// test the asesor.
        /// </summary>
        [Test]
        public void Batches()
        {
            var asesor = new Batches { AbsEntry = 1, DistNumber = "S", ItemCode = "S", SysNumber = 1 };
            ClassicAssert.IsNotNull(asesor.AbsEntry);
            ClassicAssert.IsNotNull(asesor.DistNumber);
            ClassicAssert.IsNotNull(asesor.ItemCode);
            ClassicAssert.IsNotNull(asesor.SysNumber);
        }

        /// <summary>
        /// test the asesor.
        /// </summary>
        [Test]
        public void BatchesQuantity()
        {
            var asesor = new BatchesQuantity { AbsEntry = 1, CommitQty = 1, ItemCode = "S", Quantity = 1, SysNumber = 1, WhsCode = "S" };
            ClassicAssert.IsNotNull(asesor.AbsEntry);
            ClassicAssert.IsNotNull(asesor.CommitQty);
            ClassicAssert.IsNotNull(asesor.ItemCode);
            ClassicAssert.IsNotNull(asesor.Quantity);
            ClassicAssert.IsNotNull(asesor.SysNumber);
            ClassicAssert.IsNotNull(asesor.WhsCode);
        }

        /// <summary>
        /// test the asesor.
        /// </summary>
        [Test]
        public void BatchesTransactionQtyModel()
        {
            var asesor = new BatchesTransactionQtyModel { AllocQty = 1, ItemCode = "S", LogEntry = 1, SysNumber = 1 };
            ClassicAssert.IsNotNull(asesor.AllocQty);
            ClassicAssert.IsNotNull(asesor.LogEntry);
            ClassicAssert.IsNotNull(asesor.ItemCode);
            ClassicAssert.IsNotNull(asesor.SysNumber);
        }

        /// <summary>
        /// test the asesor.
        /// </summary>
        [Test]
        public void BatchTransacitions()
        {
            var asesor = new BatchTransacitions { DocNum = 2, DocQuantity = 2, ItemCode = "S", LogEntry = 1 };
            ClassicAssert.IsNotNull(asesor.DocNum);
            ClassicAssert.IsNotNull(asesor.LogEntry);
            ClassicAssert.IsNotNull(asesor.ItemCode);
            ClassicAssert.IsNotNull(asesor.DocQuantity);
        }

        /// <summary>
        /// test the asesor.
        /// </summary>
        [Test]
        public void ValidBatches()
        {
            var asesor = new ValidBatches { CantidadAsignada = 1, CantidadDisponible = 1, NumeroLote = "W", SysNumber = 1 };
            ClassicAssert.IsNotNull(asesor.CantidadAsignada);
            ClassicAssert.IsNotNull(asesor.CantidadDisponible);
            ClassicAssert.IsNotNull(asesor.NumeroLote);
            ClassicAssert.IsNotNull(asesor.SysNumber);
        }

        /// <summary>
        /// test the asesor.
        /// </summary>
        [Test]
        public void ItemWarehouseModel()
        {
            var asesor = new ItemWarehouseModel { IsCommited = 1, ItemCode = "S", OnHand = 1, OnOrder = 2, WhsCode = "S" };
            ClassicAssert.IsNotNull(asesor.IsCommited);
            ClassicAssert.IsNotNull(asesor.ItemCode);
            ClassicAssert.IsNotNull(asesor.OnHand);
            ClassicAssert.IsNotNull(asesor.OnOrder);
            ClassicAssert.IsNotNull(asesor.WhsCode);
        }

        /// <summary>
        /// test the sales asesor model.
        /// </summary>
        [Test]
        public void SalesAsesorModel()
        {
            var asesor = new SalesAsesorModel { Email = "test@test.com", Cliente = "Juan perez", OrderId = 100 };
            ClassicAssert.IsNotNull(asesor.Email);
            ClassicAssert.IsNotNull(asesor.Cliente);
            ClassicAssert.IsNotNull(asesor.OrderId);
        }

        /// <summary>
        /// test the packing required model.
        /// </summary>
        [Test]
        public void PackingRequiredModel()
        {
            var packing = new PackingRequiredModel { CodeItem = "123-abc", Description = "this is a description", Quantity = 14, Unit = "pz" };
            ClassicAssert.IsNotNull(packing.CodeItem);
            ClassicAssert.IsNotNull(packing.Description);
            ClassicAssert.IsNotNull(packing.Quantity);
            ClassicAssert.IsNotNull(packing.Unit);
        }

        /// <summary>
        /// test the order reciper model.
        /// </summary>
        [Test]
        public void OrderRecipeModel()
        {
            var order = new OrderRecipeModel { Order = 123, Recipe = "pz" };
            ClassicAssert.IsNotNull(order.Order);
            ClassicAssert.IsNotNull(order.Recipe);
        }

        /// <summary>
        /// test the order validation response.
        /// </summary>
        [Test]
        public void OrderValidationResponse()
        {
            var order = new OrderValidationResponse { Type = "pz", ListItems = new List<string>() { "ab", "cd" } };
            ClassicAssert.IsNotNull(order.Type);
            ClassicAssert.IsNotNull(order.ListItems);
        }

        /// <summary>
        /// test the order validation response.
        /// </summary>
        [Test]
        public void SalesPersonModel()
        {
            var salesPerson = new SalesPersonModel { AsesorId = 123, FirstName = "abc", LastName = "sanchez", EmpleadoId = 1, Email = "test@test.com" };
            ClassicAssert.IsNotNull(salesPerson.AsesorId);
            ClassicAssert.IsNotNull(salesPerson.FirstName);
            ClassicAssert.IsNotNull(salesPerson.LastName);
            ClassicAssert.IsNotNull(salesPerson.EmpleadoId);
            ClassicAssert.IsNotNull(salesPerson.Email);
        }

        /// <summary>
        /// test the user model.
        /// </summary>
        [Test]
        public void UserModel()
        {
            var user = new Users { UserId = 123, UserName = "myname123" };
            ClassicAssert.IsNotNull(user.UserId);
            ClassicAssert.IsNotNull(user.UserName);
        }

        /// <summary>
        /// test the attachment model.
        /// </summary>
        [Test]
        public void AttachmentModel()
        {
            var attachment = new AttachmentModel { AbsEntry = 1, TargetPath = "C:/target/", SourcePath = "C:/folder/", FileExt = "txt", FileName = "users", Line = 1, };
            ClassicAssert.IsNotNull(attachment.AbsEntry);
            ClassicAssert.IsNotNull(attachment.TargetPath);
            ClassicAssert.IsNotNull(attachment.SourcePath);
            ClassicAssert.IsNotNull(attachment.FileName);
            ClassicAssert.IsNotNull(attachment.FileExt);
            ClassicAssert.IsNotNull(attachment.Line);
            ClassicAssert.IsNotNull(attachment.CompletePath);
        }
    }
}