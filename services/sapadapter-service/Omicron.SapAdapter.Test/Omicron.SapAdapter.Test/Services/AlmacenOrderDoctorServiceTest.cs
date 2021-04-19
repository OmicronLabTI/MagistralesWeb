// <summary>
// <copyright file="AlmacenOrderDoctorServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Test.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using NUnit.Framework;
    using Omicron.SapAdapter.DataAccess.DAO.Sap;
    using Omicron.SapAdapter.Entities.Context;
    using Omicron.SapAdapter.Services.Almacen;
    using Omicron.SapAdapter.Services.Pedidos;
    using Omicron.SapAdapter.Services.Sap;
    using Omicron.SapAdapter.Services.User;
    using Serilog;

    /// <summary>
    /// Class UsersServiceTest.
    /// </summary>
    [TestFixture]
    public class AlmacenOrderDoctorServiceTest : BaseTest
    {
        private IAlmacenOrderDoctorService almacenOrderDoctorService;

        private ISapDao sapDao;

        private DatabaseContext context;

        /// <summary>
        /// The set up.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "TemporalAlmacenOrdersService")
                .Options;

            this.context = new DatabaseContext(options);
            this.context.OrderModel.AddRange(this.GetOrderModel());
            this.context.DetallePedido.AddRange(this.GetDetallePedido());
            this.context.DeliveryDetailModel.AddRange(this.GetDeliveryDetail());
            this.context.DeliverModel.AddRange(this.DeliveryModel());
            this.context.InvoiceHeaderModel.AddRange(this.GetInvoiceHeader());
            this.context.InvoiceDetailModel.AddRange(this.GetInvoiceDetails());
            this.context.ProductoModel.AddRange(this.GetProductoModel());
            this.context.AsesorModel.AddRange(this.GetAsesorModel());
            this.context.SaveChanges();

            var mockPedidoService = new Mock<IPedidosService>();
            var mockAlmacen = new Mock<IAlmacenService>();
            var userMock = new Mock<IUsersService>();

            mockPedidoService
                .Setup(m => m.GetUserPedidos(It.IsAny<List<int>>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultGetUserPedidos()));

            mockAlmacen
                .Setup(m => m.PostAlmacenOrders(It.IsAny<string>(), It.IsAny<List<int>>()))
                .Returns(Task.FromResult(this.GetLineProducts()));

            var mockLog = new Mock<ILogger>();

            mockLog
                .Setup(m => m.Information(It.IsAny<string>()));

            this.sapDao = new SapDao(this.context, mockLog.Object);
            this.almacenOrderDoctorService = new AlmacenOrderDoctorService(this.sapDao, mockPedidoService.Object, mockAlmacen.Object);
        }
    }
}
