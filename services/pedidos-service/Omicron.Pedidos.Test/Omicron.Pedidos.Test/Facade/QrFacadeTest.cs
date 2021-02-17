// <summary>
// <copyright file="QrFacadeTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Test.Facade
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoMapper;
    using Moq;
    using NUnit.Framework;
    using Omicron.Pedidos.Dtos.Models;
    using Omicron.Pedidos.Entities.Model;
    using Omicron.Pedidos.Entities.Model.Db;
    using Omicron.Pedidos.Facade.Pedidos;
    using Omicron.Pedidos.Resources.Enums;
    using Omicron.Pedidos.Services.Mapping;
    using Omicron.Pedidos.Services.Pedidos;

    /// <summary>
    /// Class for the QR test.
    /// </summary>
    [TestFixture]
    public class QrFacadeTest : BaseTest
    {
        private QrFacade qrsFacade;

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

            var mockService = new Mock<IQrService>();

            mockService
                .Setup(m => m.CreateMagistralQr(It.IsAny<List<int>>()))
                .Returns(Task.FromResult(response));

            mockService
                .Setup(m => m.CreateRemisionQr(It.IsAny<List<int>>()))
                .Returns(Task.FromResult(response));

            mockService
                .Setup(m => m.CreateInvoiceQr(It.IsAny<List<int>>()))
                .Returns(Task.FromResult(response));

            this.qrsFacade = new QrFacade(mapper, mockService.Object);
        }

        /// <summary>
        /// the processOrders.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task ProcessOrders()
        {
            // arrange
            var order = new List<int>();

            // act
            var response = await this.qrsFacade.CreateMagistralQr(order);

            // arrange
            Assert.IsNotNull(response);
        }

        /// <summary>
        /// the processOrders.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task CreateRemisionQr()
        {
            // arrange
            var order = new List<int>();

            // act
            var response = await this.qrsFacade.CreateRemisionQr(order);

            // arrange
            Assert.IsNotNull(response);
        }

        /// <summary>
        /// the processOrders.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task CreateInvoiceQr()
        {
            // arrange
            var order = new List<int>();

            // act
            var response = await this.qrsFacade.CreateInvoiceQr(order);

            // arrange
            Assert.IsNotNull(response);
        }
    }
}
