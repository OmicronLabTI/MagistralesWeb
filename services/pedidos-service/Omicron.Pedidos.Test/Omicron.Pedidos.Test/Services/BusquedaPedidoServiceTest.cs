// <summary>
// <copyright file="BusquedaPedidoServiceTest.cs" company="Axity">
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
    public class BusquedaPedidoServiceTest : BaseTest
    {
        private IPedidosDao pedidosDao;

        private DatabaseContext context;

        private BusquedaPedidoService busqedaService;

        /// <summary>
        /// The set up.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "TemporalBusqueda")
                .Options;

            this.context = new DatabaseContext(options);
            this.context.UserOrderModel.AddRange(this.GetUserOrderModel());
            this.context.UserOrderSignatureModel.AddRange(this.GetSignature());
            this.context.SaveChanges();

            this.pedidosDao = new PedidosDao(this.context);
            this.busqedaService = new BusquedaPedidoService(this.pedidosDao);
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task GetOrders()
        {
            // arrange
            var listIds = new Dictionary<string, string>();
            listIds.Add("ffin", "27/08/2020-30/08/2020");

            // act
            var response = await this.busqedaService.GetOrders(listIds);

            // assert
            Assert.IsNotNull(response);
        }
    }
}
