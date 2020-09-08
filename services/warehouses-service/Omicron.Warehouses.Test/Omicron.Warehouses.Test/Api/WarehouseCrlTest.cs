// <summary>
// <copyright file="WarehouseCrlTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.Warehouses.Test.Api
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Moq;
    using NUnit.Framework;
    using Omicron.Warehouses.Api.Controllers;
    using Omicron.Warehouses.Dtos.Model;
    using Omicron.Warehouses.Facade.Request;
    using StackExchange.Redis;

    /// <summary>
    /// Class for tests pedidos controller.
    /// </summary>
    [TestFixture]
    public class WarehouseCrlTest
    {
        private WarehousesController controller;

        /// <summary>
        /// Setup tests.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            var resultDto = AutoFixtureProvider.Create<ResultDto>();
            resultDto.Success = true;

            var mockRequestFacade = new Mock<IRequestFacade>();
            mockRequestFacade.SetReturnsDefault(Task.FromResult(resultDto));

            var mockRedis = new Mock<IConnectionMultiplexer>();

            this.controller = new WarehousesController(mockRequestFacade.Object, mockRedis.Object);
        }

        /// <summary>
        /// Action tests.
        /// </summary>
        [Test]
        public void CreateRawMaterialRequest()
        {
            // Arrange.
            var request = AutoFixtureProvider.Create<UserActionDto<List<RawMaterialRequestDto>>>();

            // Act
            var result = this.controller.CreateRawMaterialRequest(request).Result as OkObjectResult;

            // Assert
            Assert.IsTrue((result.Value as ResultDto).Success);
        }

        /// <summary>
        /// Action tests.
        /// </summary>
        [Test]
        public void UpdateRawMaterialRequest()
        {
            // Arrange.
            var request = AutoFixtureProvider.Create<UserActionDto<List<RawMaterialRequestDto>>>();

            // Act
            var result = this.controller.UpdateRawMaterialRequest(request).Result as OkObjectResult;

            // Assert
            Assert.IsTrue((result.Value as ResultDto).Success);
        }

        /// <summary>
        /// Action tests.
        /// </summary>
        [Test]
        public void Ping()
        {
            // Act
            var result = this.controller.Ping() as OkObjectResult;

            // Assert
            Assert.AreEqual("Pong", result.Value as string);
        }
    }
}
