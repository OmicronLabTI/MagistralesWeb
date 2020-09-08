// <summary>
// <copyright file="PedidosCrlTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.Pedidos.Test.Api
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoFixture;
    using Microsoft.AspNetCore.Mvc;
    using Moq;
    using NUnit.Framework;
    using Omicron.Pedidos.Api.Controllers;
    using Omicron.Pedidos.Dtos.Models;
    using Omicron.Pedidos.Facade.Pedidos;
    using Omicron.Pedidos.Facade.Request;

    /// <summary>
    /// Class for tests pedidos controller.
    /// </summary>
    [TestFixture]
    public class PedidosCrlTest
    {
        private Fixture fixture;

        private PedidosController controller;

        /// <summary>
        /// Setup tests.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            this.fixture = new Fixture();
            var resultDto = this.fixture.Create<ResultDto>();
            resultDto.Success = true;

            var mockPedidoFacade = new Mock<IPedidoFacade>();
            mockPedidoFacade.SetReturnsDefault(Task.FromResult(resultDto));

            var mockRequestFacade = new Mock<IRequestFacade>();
            mockRequestFacade.SetReturnsDefault(Task.FromResult(resultDto));

            this.controller = new PedidosController(mockPedidoFacade.Object, mockRequestFacade.Object);
        }

        /// <summary>
        /// Action tests.
        /// </summary>
        [Test]
        public void ProcessOrders()
        {
            // Arrange.
            var request = this.fixture.Create<ProcessOrderDto>();

            // Act
            var result = this.controller.ProcessOrders(request).Result as OkObjectResult;

            // Assert
            Assert.IsTrue((result.Value as ResultDto).Success);
        }

        /// <summary>
        /// Action tests.
        /// </summary>
        [Test]
        public void ProcessByOrder()
        {
            // Arrange.
            var request = this.fixture.Create<ProcessByOrderDto>();

            // Act
            var result = this.controller.ProcessByOrder(request).Result as OkObjectResult;

            // Assert
            Assert.IsTrue((result.Value as ResultDto).Success);
        }

        /// <summary>
        /// Action tests.
        /// </summary>
        [Test]
        public void GetUserOrderBySalesOrder()
        {
            // Arrange.
            var request = this.fixture.Create<List<int>>();

            // Act
            var result = this.controller.GetUserOrderBySalesOrder(request).Result as OkObjectResult;

            // Assert
            Assert.IsTrue((result.Value as ResultDto).Success);
        }

        /// <summary>
        /// Action tests.
        /// </summary>
        [Test]
        public void GetUserOrderByFabOrder()
        {
            // Arrange.
            var request = this.fixture.Create<List<int>>();

            // Act
            var result = this.controller.GetUserOrderByFabOrder(request).Result as OkObjectResult;

            // Assert
            Assert.IsTrue((result.Value as ResultDto).Success);
        }

        /// <summary>
        /// Action tests.
        /// </summary>
        [Test]
        public void GetQfbOrders()
        {
            // Arrange.
            var request = this.fixture.Create<string>();

            // Act
            var result = this.controller.GetQfbOrders(request).Result as OkObjectResult;

            // Assert
            Assert.IsTrue((result.Value as ResultDto).Success);
        }

        /// <summary>
        /// Action tests.
        /// </summary>
        [Test]
        public void GetAllQfbOrders()
        {
            // Arrange.
            var request = this.fixture.Create<List<string>>();

            // Act
            var result = this.controller.GetAllQfbOrders(request).Result as OkObjectResult;

            // Assert
            Assert.IsTrue((result.Value as ResultDto).Success);
        }

        /// <summary>
        /// Action tests.
        /// </summary>
        [Test]
        public void AsignarManual()
        {
            // Arrange.
            var request = this.fixture.Create<ManualAssignDto>();

            // Act
            var result = this.controller.AsignarManual(request).Result as OkObjectResult;

            // Assert
            Assert.IsTrue((result.Value as ResultDto).Success);
        }

        /// <summary>
        /// Action tests.
        /// </summary>
        [Test]
        public void AssignarAutomatico()
        {
            // Arrange.
            var request = this.fixture.Create<AutomaticAssingDto>();

            // Act
            var result = this.controller.AssignarAutomatico(request).Result as OkObjectResult;

            // Assert
            Assert.IsTrue((result.Value as ResultDto).Success);
        }

        /// <summary>
        /// Action tests.
        /// </summary>
        [Test]
        public void UpdateFormula()
        {
            // Arrange.
            var request = this.fixture.Create<UpdateFormulaDto>();

            // Act
            var result = this.controller.UpdateFormula(request).Result as OkObjectResult;

            // Assert
            Assert.IsTrue((result.Value as ResultDto).Success);
        }

        /// <summary>
        /// Action tests.
        /// </summary>
        [Test]
        public void UpdateStatusOrder()
        {
            // Arrange.
            var request = this.fixture.Create<List<UpdateStatusOrderDto>>();

            // Act
            var result = this.controller.UpdateStatusOrder(request).Result as OkObjectResult;

            // Assert
            Assert.IsTrue((result.Value as ResultDto).Success);
        }

        /// <summary>
        /// Action tests.
        /// </summary>
        [Test]
        public void CancelOrder()
        {
            // Arrange.
            var request = this.fixture.Create<List<OrderIdDto>>();

            // Act
            var result = this.controller.CancelOrder(request).Result as OkObjectResult;

            // Assert
            Assert.IsTrue((result.Value as ResultDto).Success);
        }

        /// <summary>
        /// Action tests.
        /// </summary>
        [Test]
        public void CloseSalesOrders()
        {
            // Arrange.
            var request = this.fixture.Create<List<OrderIdDto>>();

            // Act
            var result = this.controller.CloseSalesOrders(request).Result as OkObjectResult;

            // Assert
            Assert.IsTrue((result.Value as ResultDto).Success);
        }

        /// <summary>
        /// Action tests.
        /// </summary>
        [Test]
        public void CancelFabOrder()
        {
            // Arrange.
            var request = this.fixture.Create<List<OrderIdDto>>();

            // Act
            var result = this.controller.CancelFabOrder(request).Result as OkObjectResult;

            // Assert
            Assert.IsTrue((result.Value as ResultDto).Success);
        }

        /// <summary>
        /// Action tests.
        /// </summary>
        [Test]
        public void CloseFabOrders()
        {
            // Arrange.
            var request = this.fixture.Create<List<CloseProductionOrderDto>>();

            // Act
            var result = this.controller.CloseFabOrders(request).Result as OkObjectResult;

            // Assert
            Assert.IsTrue((result.Value as ResultDto).Success);
        }

        /// <summary>
        /// Action tests.
        /// </summary>
        [Test]
        public void UpdateFabOrderComments()
        {
            // Arrange.
            var request = this.fixture.Create<List<UpdateOrderCommentsDto>>();

            // Act
            var result = this.controller.UpdateFabOrderComments(request).Result as OkObjectResult;

            // Assert
            Assert.IsTrue((result.Value as ResultDto).Success);
        }

        /// <summary>
        /// Action tests.
        /// </summary>
        /// <param name="signatureType">The signature type.</param>
        [TestCase("logistic")]
        [TestCase("technical")]
        [TestCase("qfb")]
        public void UpdateOrderSignature(string signatureType)
        {
            // Arrange.
            var request = this.fixture.Create<UpdateOrderSignatureDto>();

            // Act
            var result = this.controller.UpdateOrderSignature(signatureType, request).Result as OkObjectResult;

            // Assert
            Assert.IsTrue((result.Value as ResultDto).Success);
        }

        /// <summary>
        /// Action tests.
        /// </summary>
        [Test]
        public void GetOrderSignatures()
        {
            // Arrange.
            var request = this.fixture.Create<int>();

            // Act
            var result = this.controller.GetOrderSignatures(request).Result as OkObjectResult;

            // Assert
            Assert.IsTrue((result.Value as ResultDto).Success);
        }

        /// <summary>
        /// Action tests.
        /// </summary>
        [Test]
        public void ConnectDiApi()
        {
            // Act
            var result = this.controller.ConnectDiApi().Result as OkObjectResult;

            // Assert
            Assert.IsTrue((result.Value as ResultDto).Success);
        }

        /// <summary>
        /// Action tests.
        /// </summary>
        [Test]
        public void UpdateBatches()
        {
            // Arrange.
            var request = this.fixture.Create<List<AssignBatchDto>>();

            // Act
            var result = this.controller.UpdateBatches(request).Result as OkObjectResult;

            // Assert
            Assert.IsTrue((result.Value as ResultDto).Success);
        }

        /// <summary>
        /// Action tests.
        /// </summary>
        [Test]
        public void FinishOrder()
        {
            // Arrange.
            var request = this.fixture.Create<FinishOrderDto>();

            // Act
            var result = this.controller.FinishOrder(request).Result as OkObjectResult;

            // Assert
            Assert.IsTrue((result.Value as ResultDto).Success);
        }

        /// <summary>
        /// Action tests.
        /// </summary>
        [Test]
        public void CreateIsolatedFabricationOrder()
        {
            // Arrange.
            var request = this.fixture.Create<CreateIsolatedFabOrderDto>();

            // Act
            var result = this.controller.CreateIsolatedFabricationOrder(request).Result as OkObjectResult;

            // Assert
            Assert.IsTrue((result.Value as ResultDto).Success);
        }

        /// <summary>
        /// Action tests.
        /// </summary>
        [Test]
        public void GetFabOrders()
        {
            // Arrange.
            var request = this.fixture.Create<Dictionary<string, string>>();

            // Act
            var result = this.controller.GetFabOrders(request).Result as OkObjectResult;

            // Assert
            Assert.IsTrue((result.Value as ResultDto).Success);
        }

        /// <summary>
        /// Action tests.
        /// </summary>
        [Test]
        public void ReassignOrder()
        {
            // Arrange.
            var request = this.fixture.Create<ManualAssignDto>();

            // Act
            var result = this.controller.ReassignOrder(request).Result as OkObjectResult;

            // Assert
            Assert.IsTrue((result.Value as ResultDto).Success);
        }

        /// <summary>
        /// Action tests.
        /// </summary>
        [Test]
        public void GetProductivityData()
        {
            // Arrange.
            var request = this.fixture.Create<Dictionary<string, string>>();

            // Act
            var result = this.controller.GetProductivityData(request).Result as OkObjectResult;

            // Assert
            Assert.IsTrue((result.Value as ResultDto).Success);
        }

        /// <summary>
        /// Action tests.
        /// </summary>
        [Test]
        public void CreateCustomComponentList()
        {
            // Arrange.
            var request = this.fixture.Create<UserActionDto<CustomComponentListDto>>();

            // Act
            var result = this.controller.CreateCustomComponentList(request).Result as OkObjectResult;

            // Assert
            Assert.IsTrue((result.Value as ResultDto).Success);
        }

        /// <summary>
        /// Action tests.
        /// </summary>
        [Test]
        public void GetCustomComponentListByProductId()
        {
            // Arrange.
            var request = this.fixture.Create<string>();

            // Act
            var result = this.controller.GetCustomComponentListByProductId(request).Result as OkObjectResult;

            // Assert
            Assert.IsTrue((result.Value as ResultDto).Success);
        }

        /// <summary>
        /// Action tests.
        /// </summary>
        [Test]
        public void GetWorkLoad()
        {
            // Arrange.
            var request = this.fixture.Create<Dictionary<string, string>>();

            // Act
            var result = this.controller.GetWorkLoad(request).Result as OkObjectResult;

            // Assert
            Assert.IsTrue((result.Value as ResultDto).Success);
        }

        /// <summary>
        /// Action tests.
        /// </summary>
        [Test]
        public void CreateRawMaterialRequest()
        {
            // Arrange.
            var request = this.fixture.Create<UserActionDto<List<RawMaterialRequestDto>>>();

            // Act
            var result = this.controller.CreateRawMaterialRequest(request).Result as OkObjectResult;

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
