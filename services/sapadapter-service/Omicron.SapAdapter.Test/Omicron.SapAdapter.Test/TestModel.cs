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
    using NUnit.Framework;
    using Omicron.SapAdapter.Entities.Model;

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
        }

        /// <summary>
        /// test the asesor.
        /// </summary>
        [Test]
        public void DetallePedidoModel()
        {
            var asesor = new DetallePedidoModel { Description = "DetallePedido", DetalleId = 1, PedidoId = 100, ProductoId = "Abc Aspirina" };
            Assert.IsNotNull(asesor);
        }

        /// <summary>
        /// test the asesor.
        /// </summary>
        [Test]
        public void OrdenFabricaionModel()
        {
            var asesor = new OrdenFabricacionModel { ProductoId = "Abc Aspirina", OrdenId = 100, PostDate = DateTime.Now, Quantity = 2, Status = "L", PedidoId = 100 };
            Assert.IsNotNull(asesor);
        }

        /// <summary>
        /// test the asesor.
        /// </summary>
        [Test]
        public void OrdenModel()
        {
            var asesor = new OrderModel { PedidoId = 100, AsesorId = 1, Cliente = "cliente", Codigo = "Codigo", DocNum = 100, FechaFin = DateTime.Now, FechaInicio = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), Medico = "Medico", PedidoStatus = "C" };
            Assert.IsNotNull(asesor);
        }

        /// <summary>
        /// test the asesor.
        /// </summary>
        [Test]
        public void ProductoModel()
        {
            var asesor = new ProductoModel { IsMagistral = "Y", ProductoId = "Abc Aspirina", ProductoName = "Aspirina" };
            Assert.IsNotNull(asesor);
        }

        /// <summary>
        /// test the asesor.
        /// </summary>
        [Test]
        public void CompleteDetailOrderModel()
        {
            var asesor = new CompleteDetailOrderModel { CodigoProducto = "Abc Aspirina", DescripcionProducto = "Aspirina", FechaOf = "28/03/2020", FechaOfFin = "28/03/2020", IsChecked = false, OrdenFabricacionId = 100, Qfb = "Gustavo", QtyPlanned = 1, Status = "L", };
            Assert.IsNotNull(asesor);
        }

        /// <summary>
        /// test the asesor.
        /// </summary>
        [Test]
        public void CompleteOrderModel()
        {
            var asesor = new CompleteOrderModel { AsesorName = "asesor", Cliente = "cliente", Codigo = "codigo", DocNum = 100, FechaFin = "fecha", FechaInicio = "fecha", IsChecked = true, Medico = "Medico", PedidoStatus = "L" };
            Assert.IsNotNull(asesor);
        }
    }
}
