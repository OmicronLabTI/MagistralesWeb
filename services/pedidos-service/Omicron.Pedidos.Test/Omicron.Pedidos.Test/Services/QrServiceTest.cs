// <summary>
// <copyright file="QrServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Test.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using NUnit.Framework;
    using Omicron.Pedidos.DataAccess.DAO.Pedidos;
    using Omicron.Pedidos.Entities.Context;
    using Omicron.Pedidos.Services.Pedidos;

    /// <summary>
    /// class for the test.
    /// </summary>
    [TestFixture]
    public class QrServiceTest : BaseTest
    {
        private IPedidosDao pedidosDao;

        private DatabaseContext context;

        private QrService qrsService;

        /// <summary>
        /// The set up.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "TemporalQr")
                .Options;

            this.context = new DatabaseContext(options);
            this.context.UserOrderModel.AddRange(this.GetUserOrderModel());
            this.context.SaveChanges();

            this.pedidosDao = new PedidosDao(this.context);
            this.qrsService = new QrService(this.pedidosDao);
        }

        /// <summary>
        /// Test the creation of the Qr.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task CreateQrMagistral()
        {
            var listOrdersId = new List<int> { 300, 301 };

            var response = await this.qrsService.CreateMagistralQr(listOrdersId);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
        }
    }
}
