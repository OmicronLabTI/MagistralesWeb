// <summary>
// <copyright file="SapAlmacenServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Test.Services
{
    using System.Threading.Tasks;
    using AutoMapper;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using NUnit.Framework;
    using Omicron.SapAdapter.DataAccess.DAO.Sap;
    using Omicron.SapAdapter.Entities.Context;
    using Omicron.SapAdapter.Entities.Model;
    using Omicron.SapAdapter.Facade.Sap;
    using Omicron.SapAdapter.Services.Pedidos;
    using Omicron.SapAdapter.Services.Sap;
    using Serilog;

    /// <summary>
    /// Class for the QR test.
    /// </summary>
    [TestFixture]
    public class SapAlmacenServiceTest : BaseTest
    {
        private ISapAlmacenService sapService;

        private ISapDao sapDao;

        private DatabaseContext context;

        /// <summary>
        /// The set up.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "Temporal")
                .Options;

            this.context = new DatabaseContext(options);
            this.context.SaveChanges();

            var mockLog = new Mock<ILogger>();
            mockLog.Setup(m => m.Information(It.IsAny<string>()));

            var mockPedidoService = new Mock<IPedidosService>();

            this.sapDao = new SapDao(this.context, mockLog.Object);
            this.sapService = new SapAlmacenService(this.sapDao, mockPedidoService.Object);
        }

        [Test]
        public async Task GetOrders()
        {

        }
    }
}
