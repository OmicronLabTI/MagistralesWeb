// <summary>
// <copyright file="TestModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Test
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using Omicron.SapAdapter.Entities.Model;
    using Omicron.SapAdapter.Entities.Model.BusinessModels;
    using Omicron.SapAdapter.Entities.Model.DbModels;
    using Omicron.SapAdapter.Entities.Model.JoinsModels;

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

            Assert.IsNotNull(asesor);
            Assert.IsNotNull(asesor.AsesorId);
            Assert.IsNotNull(asesor.AsesorName);
        }

        /// <summary>
        /// test the asesor.
        /// </summary>
        [Test]
        public void DetallePedidoModel()
        {
            var asesor = new DetallePedidoModel { Description = "DetallePedido", DetalleId = 1, PedidoId = 100, ProductoId = "Abc Aspirina", Container = "container", Label = "label", Quantity = 10 };

            Assert.IsNotNull(asesor.Description);
            Assert.IsNotNull(asesor.DetalleId);
            Assert.IsNotNull(asesor.Container);
            Assert.IsNotNull(asesor.Label);
            Assert.IsNotNull(asesor.PedidoId);
            Assert.IsNotNull(asesor.ProductoId);
            Assert.IsNotNull(asesor.Quantity);
        }

        /// <summary>
        /// test the asesor.
        /// </summary>
        [Test]
        public void OrdenFabricaionModel()
        {
            var asesor = new OrdenFabricacionModel { ProductoId = "Abc Aspirina", OrdenId = 100, PostDate = DateTime.Now, Quantity = 2, Status = "L", PedidoId = 100, CardCode = "card", Comments = "comment", CompleteQuantity = 10, CreatedDate = DateTime.Now, DataSource = "0", DueDate = DateTime.Now, OriginType = "0", ProdName = "name", StartDate = DateTime.Now, Type = "y", Unit = "U", User = 1, Wharehouse = "w" };
            Assert.IsNotNull(asesor.CardCode);
            Assert.IsNotNull(asesor.Comments);
            Assert.IsNotNull(asesor.CompleteQuantity);
            Assert.IsNotNull(asesor.CreatedDate);
            Assert.IsNotNull(asesor.DataSource);
            Assert.IsNotNull(asesor.DueDate);
            Assert.IsNotNull(asesor.OrdenId);
            Assert.IsNotNull(asesor.OriginType);
            Assert.IsNotNull(asesor.PedidoId);
            Assert.IsNotNull(asesor.PostDate);
            Assert.IsNotNull(asesor.ProdName);
            Assert.IsNotNull(asesor.ProductoId);
            Assert.IsNotNull(asesor.Quantity);
            Assert.IsNotNull(asesor.StartDate);
            Assert.IsNotNull(asesor.Status);
            Assert.IsNotNull(asesor.Type);
            Assert.IsNotNull(asesor.Unit);
            Assert.IsNotNull(asesor.User);
            Assert.IsNotNull(asesor.Wharehouse);
        }

        /// <summary>
        /// test the asesor.
        /// </summary>
        [Test]
        public void OrdenModel()
        {
            var asesor = new OrderModel { PedidoId = 100, AsesorId = 1, Codigo = "Codigo", DocNum = 100, FechaFin = DateTime.Now, FechaInicio = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), Medico = "Medico", PedidoStatus = "C" };
            Assert.IsNotNull(asesor.AsesorId);
            Assert.IsNotNull(asesor.Codigo);
            Assert.IsNotNull(asesor.DocNum);
            Assert.IsNotNull(asesor.FechaFin);
            Assert.IsNotNull(asesor.FechaInicio);
            Assert.IsNotNull(asesor.Medico);
            Assert.IsNotNull(asesor.PedidoId);
            Assert.IsNotNull(asesor.PedidoStatus);
        }

        /// <summary>
        /// test the asesor.
        /// </summary>
        [Test]
        public void ProductoModel()
        {
            var asesor = new ProductoModel { IsMagistral = "Y", ProductoId = "Abc Aspirina", ProductoName = "Aspirina", LargeDescription = "large", ManagedBatches = "batches", OnHand = 10, Unit = "KG" };
            Assert.IsNotNull(asesor.IsMagistral);
            Assert.IsNotNull(asesor.LargeDescription);
            Assert.IsNotNull(asesor.ManagedBatches);
            Assert.IsNotNull(asesor.OnHand);
            Assert.IsNotNull(asesor.ProductoId);
            Assert.IsNotNull(asesor.ProductoName);
            Assert.IsNotNull(asesor.Unit);
        }

        /// <summary>
        /// test the asesor.
        /// </summary>
        [Test]
        public void CompleteDetailOrderModel()
        {
            var asesor = new CompleteDetailOrderModel { CodigoProducto = "Abc Aspirina", DescripcionProducto = "Aspirina", FechaOf = "28/03/2020", FechaOfFin = "28/03/2020", IsChecked = false, OrdenFabricacionId = 100, Qfb = "Gustavo", QtyPlanned = 1, Status = "L", DescripcionCorta = "corta", PedidoStatus = "S", QtyPlannedDetalle = 10, HasMissingStock = false };

            Assert.IsNotNull(asesor.CodigoProducto);
            Assert.IsNotNull(asesor.DescripcionCorta);
            Assert.IsNotNull(asesor.DescripcionProducto);
            Assert.IsNotNull(asesor.FechaOf);
            Assert.IsNotNull(asesor.FechaOfFin);
            Assert.IsNotNull(asesor.IsChecked);
            Assert.IsNotNull(asesor.OrdenFabricacionId);
            Assert.IsNotNull(asesor.PedidoStatus);
            Assert.IsNotNull(asesor.Qfb);
            Assert.IsNotNull(asesor.QtyPlanned);
            Assert.IsNotNull(asesor.QtyPlannedDetalle);
            Assert.IsNotNull(asesor.Status);
            Assert.IsNotNull(asesor.HasMissingStock);
        }

        /// <summary>
        /// test the asesor.
        /// </summary>
        [Test]
        public void CompleteOrderModel()
        {
            var asesor = new CompleteOrderModel { AsesorName = "asesor", Cliente = "cliente", Codigo = "codigo", DocNum = 100, FechaFin = "fecha", FechaInicio = "fecha", IsChecked = true, Medico = "Medico", PedidoStatus = "L", Qfb = "qfb" };
            Assert.IsNotNull(asesor.AsesorName);
            Assert.IsNotNull(asesor.Cliente);
            Assert.IsNotNull(asesor.Codigo);
            Assert.IsNotNull(asesor.DocNum);
            Assert.IsNotNull(asesor.FechaFin);
            Assert.IsNotNull(asesor.FechaInicio);
            Assert.IsNotNull(asesor.Medico);
            Assert.IsNotNull(asesor.IsChecked);
            Assert.IsNotNull(asesor.PedidoStatus);
            Assert.IsNotNull(asesor.Qfb);
        }

        /// <summary>
        /// test the asesor.
        /// </summary>
        [Test]
        public void BatchesComponentModel()
        {
            var asesor = new BatchesComponentModel { Almacen = "Almacen", CodigoProducto = "codogp", DescripcionProducto = "dsc", Lotes = new List<ValidBatches>(), LotesAsignados = new List<AssignedBatches>(), TotalNecesario = 10, TotalSeleccionado = 10 };
            Assert.IsNotNull(asesor.Almacen);
            Assert.IsNotNull(asesor.CodigoProducto);
            Assert.IsNotNull(asesor.DescripcionProducto);
            Assert.IsNotNull(asesor.Lotes);
            Assert.IsNotNull(asesor.LotesAsignados);
            Assert.IsNotNull(asesor.TotalNecesario);
            Assert.IsNotNull(asesor.TotalSeleccionado);
        }

        /// <summary>
        /// test the asesor.
        /// </summary>
        [Test]
        public void CompleteFormulaWithDetalle()
        {
            var asesor = new CompleteFormulaWithDetalle { BaseDocument = 1, Client = "C", Code = "C", Comments = "C", CompleteQuantity = 10, Container = "L", Details = new List<CompleteDetalleFormulaModel>(), DueDate = "D", EndDate = "E", FabDate = "F", IsChecked = true, Number = 1, Origin = "O", PlannedQuantity = 1, ProductDescription = "P", ProductionOrderId = 1, ProductLabel = "l", RealEndDate = "L", StartDate = "S", Status = "S", Type = "T", Unit = "U", User = "U", Warehouse = "w" };

            Assert.IsNotNull(asesor.BaseDocument);
            Assert.IsNotNull(asesor.Client);
            Assert.IsNotNull(asesor.Code);
            Assert.IsNotNull(asesor.Comments);
            Assert.IsNotNull(asesor.CompleteQuantity);
            Assert.IsNotNull(asesor.Container);
            Assert.IsNotNull(asesor.Details);
            Assert.IsNotNull(asesor.DueDate);
            Assert.IsNotNull(asesor.EndDate);
            Assert.IsNotNull(asesor.FabDate);
            Assert.IsNotNull(asesor.IsChecked);
            Assert.IsNotNull(asesor.Number);
            Assert.IsNotNull(asesor.Origin);
            Assert.IsNotNull(asesor.PlannedQuantity);
            Assert.IsNotNull(asesor.ProductDescription);
            Assert.IsNotNull(asesor.ProductionOrderId);
            Assert.IsNotNull(asesor.ProductLabel);
            Assert.IsNotNull(asesor.RealEndDate);
            Assert.IsNotNull(asesor.StartDate);
            Assert.IsNotNull(asesor.Status);
            Assert.IsNotNull(asesor.Type);
            Assert.IsNotNull(asesor.Unit);
            Assert.IsNotNull(asesor.User);
            Assert.IsNotNull(asesor.Warehouse);
        }

        /// <summary>
        /// test the asesor.
        /// </summary>
        [Test]
        public void DetalleFormulaModel()
        {
            var asesor = new DetalleFormulaModel { Almacen = "A", BaseQuantity = 1, ConsumidoQty = 1, ItemCode = "I", LineNum = 1, OrderFabId = 1, RequiredQty = 1, UnidadCode = "U" };
            Assert.IsNotNull(asesor.Almacen);
            Assert.IsNotNull(asesor.BaseQuantity);
            Assert.IsNotNull(asesor.ConsumidoQty);
            Assert.IsNotNull(asesor.ItemCode);
            Assert.IsNotNull(asesor.LineNum);
            Assert.IsNotNull(asesor.OrderFabId);
            Assert.IsNotNull(asesor.RequiredQty);
            Assert.IsNotNull(asesor.UnidadCode);
        }

        /// <summary>
        /// test the asesor.
        /// </summary>
        [Test]
        public void OrderWithDetailModel()
        {
            var asesor = new OrderWithDetailModel { Detalle = new List<CompleteDetailOrderModel>(), Order = new OrderModel() };
            Assert.IsNotNull(asesor.Detalle);
            Assert.IsNotNull(asesor.Order);
        }

        /// <summary>
        /// test the asesor.
        /// </summary>
        [Test]
        public void AssignedBatches()
        {
            var asesor = new AssignedBatches { CantidadSeleccionada = 1, NumeroLote = "S", SysNumber = 1 };
            Assert.IsNotNull(asesor.CantidadSeleccionada);
            Assert.IsNotNull(asesor.NumeroLote);
            Assert.IsNotNull(asesor.SysNumber);
        }

        /// <summary>
        /// test the asesor.
        /// </summary>
        [Test]
        public void CompleteDetalleFormulaModel()
        {
            var asesor = new CompleteDetalleFormulaModel { Available = 1,  BaseQuantity = 1, Consumed = 1, Description = "D2", OrderFabId = 1, PendingQuantity = 1, ProductId = "P", RequiredQuantity = 1, Stock = 1, Unit = "U", Warehouse = "W", WarehouseQuantity = 1 };
            Assert.IsNotNull(asesor.Available);
            Assert.IsNotNull(asesor.BaseQuantity);
            Assert.IsNotNull(asesor.Consumed);
            Assert.IsNotNull(asesor.Description);
            Assert.IsNotNull(asesor.OrderFabId);
            Assert.IsNotNull(asesor.PendingQuantity);
            Assert.IsNotNull(asesor.ProductId);
            Assert.IsNotNull(asesor.RequiredQuantity);
            Assert.IsNotNull(asesor.Stock);
            Assert.IsNotNull(asesor.Unit);
            Assert.IsNotNull(asesor.Warehouse);
            Assert.IsNotNull(asesor.WarehouseQuantity);
        }

        /// <summary>
        /// test the asesor.
        /// </summary>
        [Test]
        public void Batches()
        {
            var asesor = new Batches { AbsEntry = 1, DistNumber = "S", ItemCode = "S", SysNumber = 1 };
            Assert.IsNotNull(asesor.AbsEntry);
            Assert.IsNotNull(asesor.DistNumber);
            Assert.IsNotNull(asesor.ItemCode);
            Assert.IsNotNull(asesor.SysNumber);
        }

        /// <summary>
        /// test the asesor.
        /// </summary>
        [Test]
        public void BatchesQuantity()
        {
            var asesor = new BatchesQuantity { AbsEntry = 1, CommitQty = 1, ItemCode = "S", Quantity = 1, SysNumber = 1, WhsCode = "S" };
            Assert.IsNotNull(asesor.AbsEntry);
            Assert.IsNotNull(asesor.CommitQty);
            Assert.IsNotNull(asesor.ItemCode);
            Assert.IsNotNull(asesor.Quantity);
            Assert.IsNotNull(asesor.SysNumber);
            Assert.IsNotNull(asesor.WhsCode);
        }

        /// <summary>
        /// test the asesor.
        /// </summary>
        [Test]
        public void BatchesTransactionQtyModel()
        {
            var asesor = new BatchesTransactionQtyModel { AllocQty = 1, ItemCode = "S", LogEntry = 1, SysNumber = 1 };
            Assert.IsNotNull(asesor.AllocQty);
            Assert.IsNotNull(asesor.LogEntry);
            Assert.IsNotNull(asesor.ItemCode);
            Assert.IsNotNull(asesor.SysNumber);
        }

        /// <summary>
        /// test the asesor.
        /// </summary>
        [Test]
        public void BatchTransacitions()
        {
            var asesor = new BatchTransacitions { DocNum = 2, DocQuantity = 2, ItemCode = "S", LogEntry = 1 };
            Assert.IsNotNull(asesor.DocNum);
            Assert.IsNotNull(asesor.LogEntry);
            Assert.IsNotNull(asesor.ItemCode);
            Assert.IsNotNull(asesor.DocQuantity);
        }

        /// <summary>
        /// test the asesor.
        /// </summary>
        [Test]
        public void ValidBatches()
        {
            var asesor = new ValidBatches { CantidadAsignada = 1, CantidadDisponible = 1, NumeroLote = "W", SysNumber = 1 };
            Assert.IsNotNull(asesor.CantidadAsignada);
            Assert.IsNotNull(asesor.CantidadDisponible);
            Assert.IsNotNull(asesor.NumeroLote);
            Assert.IsNotNull(asesor.SysNumber);
        }

        /// <summary>
        /// test the asesor.
        /// </summary>
        [Test]
        public void ItemWarehouseModel()
        {
            var asesor = new ItemWarehouseModel { IsCommited = 1, ItemCode = "S", OnHand = 1, OnOrder = 2, WhsCode = "S" };
            Assert.IsNotNull(asesor.IsCommited);
            Assert.IsNotNull(asesor.ItemCode);
            Assert.IsNotNull(asesor.OnHand);
            Assert.IsNotNull(asesor.OnOrder);
            Assert.IsNotNull(asesor.WhsCode);
        }

        /// <summary>
        /// test the sales asesor model.
        /// </summary>
        [Test]
        public void SalesAsesorModel()
        {
            var asesor = new SalesAsesorModel { Email = "test@test.com", Cliente = "Juan perez", OrderId = 100 };
            Assert.IsNotNull(asesor.Email);
            Assert.IsNotNull(asesor.Cliente);
            Assert.IsNotNull(asesor.OrderId);
        }

        /// <summary>
        /// test the packing required model.
        /// </summary>
        [Test]
        public void PackingRequiredModel()
        {
            var packing = new PackingRequiredModel { CodeItem = "123-abc", Description = "this is a description", Quantity = 14, Unit = "pz" };
            Assert.IsNotNull(packing.CodeItem);
            Assert.IsNotNull(packing.Description);
            Assert.IsNotNull(packing.Quantity);
            Assert.IsNotNull(packing.Unit);
        }

        /// <summary>
        /// test the order reciper model.
        /// </summary>
        [Test]
        public void OrderRecipeModel()
        {
            var order = new OrderRecipeModel { Order = 123, Recipe = "pz" };
            Assert.IsNotNull(order.Order);
            Assert.IsNotNull(order.Recipe);
        }

        /// <summary>
        /// test the order validation response.
        /// </summary>
        [Test]
        public void OrderValidationResponse()
        {
            var order = new OrderValidationResponse { Type = "pz", ListItems = new List<string>() { "ab", "cd" } };
            Assert.IsNotNull(order.Type);
            Assert.IsNotNull(order.ListItems);
        }

        /// <summary>
        /// test the order validation response.
        /// </summary>
        [Test]
        public void SalesPersonModel()
        {
            var salesPerson = new SalesPersonModel { AsesorId = 123, FirstName = "abc", LastName = "sanchez", EmpleadoId = 1, Email = "test@test.com" };
            Assert.IsNotNull(salesPerson.AsesorId);
            Assert.IsNotNull(salesPerson.FirstName);
            Assert.IsNotNull(salesPerson.LastName);
            Assert.IsNotNull(salesPerson.EmpleadoId);
            Assert.IsNotNull(salesPerson.Email);
        }

        /// <summary>
        /// test the user model.
        /// </summary>
        [Test]
        public void UserModel()
        {
            var user = new Users { UserId = 123, UserName = "myname123" };
            Assert.IsNotNull(user.UserId);
            Assert.IsNotNull(user.UserName);
        }

        /// <summary>
        /// test the attachment model.
        /// </summary>
        [Test]
        public void AttachmentModel()
        {
            var attachment = new AttachmentModel { AbsEntry = 1, TargetPath = "C:/target/", SourcePath = "C:/folder/", FileExt = "txt", FileName = "users", Line = 1, };
            Assert.IsNotNull(attachment.AbsEntry);
            Assert.IsNotNull(attachment.TargetPath);
            Assert.IsNotNull(attachment.SourcePath);
            Assert.IsNotNull(attachment.FileName);
            Assert.IsNotNull(attachment.FileExt);
            Assert.IsNotNull(attachment.Line);
            Assert.IsNotNull(attachment.CompletePath);
        }
    }
}