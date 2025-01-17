// <summary>
// <copyright file="PedidosDxpServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Test.Services
{
    /// <summary>
    /// class for the test.
    /// </summary>
    [TestFixture]
    public class PedidosDxpServiceTest : BaseTest
    {
        private IPedidosDxpService pedidosDxpService;

        private IPedidosDao pedidosDao;

        private DatabaseContext context;

        /// <summary>
        /// The set up.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "TemporalPedidoDxp")
                .Options;

            this.context = new DatabaseContext(options);
            this.context.UserOrderModel.AddRange(this.GetUserOrderModel());
            this.context.SaveChanges();

            this.pedidosDao = new PedidosDao(this.context);
            this.pedidosDxpService = new PedidosDxpService(this.pedidosDao);
        }

        /// <summary>
        /// the test for get orders active for dxp project.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task GetOrdersActive()
        {
            // arrange
            var listIds = new List<int> { 100, 101, 102 };

            // act
            var response = await this.pedidosDxpService.GetOrdersActive(listIds);

            // assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Response, Is.Not.Null);
            Assert.That(response.Success);
            Assert.That(response.UserError, Is.Null);
        }

        /// <summary>
        /// the test for get orders active for dxp project.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task GetDeliveredPayments()
        {
            // arrange
            var listIds = new List<int> { 204, 205 };

            // act
            var response = await this.pedidosDxpService.GetDeliveredPayments(listIds);

            // assert
            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// the test for get orders active for dxp project.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task GetOrdersHeaderStatus()
        {
            // arrange
            var listIds = new List<string> { "204", "205" };

            // act
            var response = await this.pedidosDxpService.GetOrdersHeaderStatus(listIds);

            // assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Code == 200);
            Assert.That(response.Comments, Is.Not.Null);
            Assert.That(response.Response, Is.Not.Null);
            Assert.That(response.Success);
            Assert.That(response.UserError, Is.Null);
            Assert.That(response.ExceptionMessage, Is.Null);
        }
    }
}
