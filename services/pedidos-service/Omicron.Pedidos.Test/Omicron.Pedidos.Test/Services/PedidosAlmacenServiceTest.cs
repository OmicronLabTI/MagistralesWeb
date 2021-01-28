// <summary>
// <copyright file="PedidosAlmacenServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Test.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using NUnit.Framework;
    using Omicron.Pedidos.DataAccess.DAO.Pedidos;
    using Omicron.Pedidos.Entities.Context;
    using Omicron.Pedidos.Entities.Model;
    using Omicron.Pedidos.Services.Constants;
    using Omicron.Pedidos.Services.Pedidos;

    /// <summary>
    /// class for the test.
    /// </summary>
    [TestFixture]
    public class PedidosAlmacenServiceTest : BaseTest
    {
        private IPedidosAlmacenService pedidosAlmacen;

        private IPedidosDao pedidosDao;

        private DatabaseContext context;

        /// <summary>
        /// The set up.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "TemporalAlmacen")
                .Options;

            this.context = new DatabaseContext(options);
            this.context.UserOrderModel.AddRange(this.GetUserOrderModel());
            this.context.OrderLogModel.Add(this.GetOrderLogModel());
            this.context.UserOrderSignatureModel.AddRange(this.GetSignature());
            this.context.SaveChanges();

            this.pedidosDao = new PedidosDao(this.context);
            this.pedidosAlmacen = new PedidosAlmacenService(this.pedidosDao);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetOrdersForAlmacen()
        {
            // act
            var result = await this.pedidosAlmacen.GetOrdersForAlmacen();

            // assert
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task UpdateUserOrders()
        {
            // arrange
            var listorders = new List<UserOrderModel>
            {
                new UserOrderModel { Id = 1, Status = "Almacenado" },
            };

            // act
            var result = await this.pedidosAlmacen.UpdateUserOrders(listorders);

            // assert
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetOrdersForDelivery()
        {
            // act
            var result = await this.pedidosAlmacen.GetOrdersForDelivery();

            // assert
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetOrdersForInvoice()
        {
            // act
            var result = await this.pedidosAlmacen.GetOrdersForInvoice();

            // assert
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <param name="type">the type.</param>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetOrdersForPackages()
        {
            // arrange
            var dict = new Dictionary<string, string>
            {
                { "status", "Empaquetado,Enviado" },
            };

            // act
            var result = await this.pedidosAlmacen.GetOrdersForPackages(dict);

            // assert
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <param name="type">the type.</param>
        /// <returns>the data.</returns>
        [Test]
        public async Task UpdateSentOrders()
        {
            // arrange
            var listUserOrders = new List<UserOrderModel>
            {
                new UserOrderModel { InvoiceId = 100, StatusInvoice = "Enviado" },
            };

            // act
            var result = await this.pedidosAlmacen.UpdateSentOrders(listUserOrders);

            // assert
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetAlmacenGraphData()
        {
            // arrange
            var yesterday = DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy");
            var today = DateTime.Now.ToString("dd/MM/yyyy");
            var dict = new Dictionary<string, string>
            {
                { ServiceConstants.FechaInicio, $"{yesterday}-{today}" },
            };

            // act
            var result = await this.pedidosAlmacen.GetAlmacenGraphData(dict);

            // assert
            Assert.IsNotNull(result);
        }
    }
}
