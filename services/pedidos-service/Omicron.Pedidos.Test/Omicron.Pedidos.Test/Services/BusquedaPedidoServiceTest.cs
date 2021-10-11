// <summary>
// <copyright file="BusquedaPedidoServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Test.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Moq;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using Omicron.LeadToCash.Resources.Exceptions;
    using Omicron.Pedidos.DataAccess.DAO.Pedidos;
    using Omicron.Pedidos.Entities.Context;
    using Omicron.Pedidos.Entities.Model;
    using Omicron.Pedidos.Resources.Enums;
    using Omicron.Pedidos.Services.Constants;
    using Omicron.Pedidos.Services.Pedidos;
    using Omicron.Pedidos.Services.SapAdapter;
    using Omicron.Pedidos.Services.SapDiApi;
    using Omicron.Pedidos.Services.SapFile;
    using Omicron.Pedidos.Services.User;
    using Omicron.Pedidos.Services.Utils;
    using Omicron.Pedidos.Services.Reporting;
    using Omicron.Pedidos.Services.Redis;

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
            this.context.OrderLogModel.Add(this.GetOrderLogModel());
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
