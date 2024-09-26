// <summary>
// <copyright file="BusquedaPedidosFacadeTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Test.Facade
{
    /// <summary>
    /// Class UsersServiceTest.
    /// </summary>
    [TestFixture]
    public class BusquedaPedidosFacadeTest : BaseTest
    {
        private BusquedaPedidoFacade busquedaFacade;

        /// <summary>
        /// The init.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            var mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
            var mapper = mapperConfiguration.CreateMapper();

            var response = new ResultModel
            {
                Success = true,
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = string.Empty,
                UserError = string.Empty,
            };

            var mockBusquedaService = new Mock<IBusquedaPedidoService>();
            var mockPedidosDxpService = new Mock<IPedidosDxpService>();

            mockBusquedaService.SetReturnsDefault(Task.FromResult(response));
            mockPedidosDxpService.SetReturnsDefault(Task.FromResult(response));

            this.busquedaFacade = new BusquedaPedidoFacade(
                mapper,
                mockBusquedaService.Object,
                mockPedidosDxpService.Object);
        }

        /// <summary>
        /// the processOrders.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task GetOrders()
        {
            // arrange
            var order = new Dictionary<string, string>();

            // act
            var response = await this.busquedaFacade.GetOrders(order);

            // arrange
            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// the possible orders active for dxp service.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task GetOrdersActive()
        {
            // arrange
            var order = new Dictionary<string, string>();

            // act
            var response = await this.busquedaFacade.GetOrdersActive(new List<int>());

            // arrange
            Assert.That(response, Is.Not.Null);
        }
    }
}
