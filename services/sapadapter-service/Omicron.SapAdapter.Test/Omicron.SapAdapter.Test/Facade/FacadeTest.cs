// <summary>
// <copyright file="FacadeTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Test.Facade
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoMapper;
    using Moq;
    using NUnit.Framework;
    using Omicron.SapAdapter.Dtos.Models;
    using Omicron.SapAdapter.Entities.Model;
    using Omicron.SapAdapter.Entities.Model.BusinessModels;
    using Omicron.SapAdapter.Facade.Sap;
    using Omicron.SapAdapter.Services.Mapping;
    using Omicron.SapAdapter.Services.Sap;
    using Omicron.SapAdapter.Services.User;

    /// <summary>
    /// Class UsersServiceTest.
    /// </summary>
    [TestFixture]
    public class FacadeTest : BaseTest
    {
        private SapFacade sapFacade;

        private IMapper mapper;

        /// <summary>
        /// The init.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            var mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
            this.mapper = mapperConfiguration.CreateMapper();

            var mockServices = new Mock<IUsersService>();

            var mockSapServices = new Mock<ISapService>();

            var response = new ResultModel
            {
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = true,
                Success = true,
                UserError = string.Empty,
            };

            mockSapServices
                .Setup(m => m.GetOrders(It.IsAny<Dictionary<string, string>>()))
                .Returns(Task.FromResult(response));

            mockSapServices
                .Setup(m => m.GetOrderDetails(It.IsAny<int>()))
                .Returns(Task.FromResult(response));

            mockSapServices
                .Setup(m => m.GetPedidoWithDetail(It.IsAny<List<int>>()))
                .Returns(Task.FromResult(response));

            mockSapServices
                .Setup(m => m.GetProdOrderByOrderItem(It.IsAny<List<string>>()))
                .Returns(Task.FromResult(response));

            mockSapServices
                .Setup(m => m.GetOrderFormula(It.IsAny<List<int>>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(response));

            mockSapServices
                .Setup(m => m.GetComponents(It.IsAny<Dictionary<string, string>>()))
                .Returns(Task.FromResult(response));

            mockSapServices
                .Setup(m => m.GetBatchesComponents(It.IsAny<int>()))
                .Returns(Task.FromResult(response));

            mockSapServices
                .Setup(m => m.GetlLastIsolatedProductionOrderId(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(response));

            mockSapServices
                .Setup(m => m.GetFabOrders(It.IsAny<GetOrderFabModel>()))
                .Returns(Task.FromResult(response));

            mockSapServices
                .Setup(m => m.GetNextBatchCode(It.IsAny<string>()))
                .Returns(Task.FromResult(response));

            mockSapServices
                .Setup(m => m.GetProductsManagmentByBatch(It.IsAny<Dictionary<string, string>>()))
                .Returns(Task.FromResult(response));

            mockSapServices
                .Setup(m => m.GetFabOrdersById(It.IsAny<List<int>>()))
                .Returns(Task.FromResult(response));

            this.sapFacade = new SapFacade(mockSapServices.Object, this.mapper);
        }

        /// <summary>
        /// Test to get the orders.
        /// </summary>
        /// <returns>get the orders.</returns>
        [Test]
        public async Task GetPedidos()
        {
            // Arrange
            var dict = new Dictionary<string, string>();

            // Act
            var response = this.sapFacade.GetOrders(dict);

            // Assert
            Assert.IsNotNull(response);
        }

        /// <summary>
        /// Get detalle de pedido.
        /// </summary>
        /// <returns>the detail.</returns>
        [Test]
        public async Task GetDetallePedidos()
        {
            // Arrange
            var docEntry = "10";

            // act
            var response = await this.sapFacade.GetDetallePedidos(docEntry);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
            Assert.IsNotNull(response.Response);
            Assert.IsEmpty(response.ExceptionMessage);
            Assert.IsEmpty(response.UserError);
            Assert.AreEqual(200, response.Code);
        }

        /// <summary>
        /// test tet.
        /// </summary>
        /// <returns>test.</returns>
        [Test]
        public async Task GetPedidoWithDetail()
        {
            // arrange
            var listDocs = new List<int> { 1, 2, 3 };

            // act
            var response = await this.sapFacade.GetPedidoWithDetail(listDocs);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
            Assert.IsNotNull(response.Response);
            Assert.IsEmpty(response.ExceptionMessage);
            Assert.IsEmpty(response.UserError);
            Assert.AreEqual(200, response.Code);
        }

        /// <summary>
        /// test tet.
        /// </summary>
        /// <returns>test.</returns>
        [Test]
        public async Task GetProdOrderByOrderItem()
        {
            // arrange
            var listDocs = new List<string> { "12" };

            // act
            var response = await this.sapFacade.GetProdOrderByOrderItem(listDocs);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
            Assert.IsNotNull(response.Response);
            Assert.IsEmpty(response.ExceptionMessage);
            Assert.IsEmpty(response.UserError);
            Assert.AreEqual(200, response.Code);
        }

        /// <summary>
        /// test tet.
        /// </summary>
        /// <returns>test.</returns>
        [Test]
        public async Task GetOrderFormula()
        {
            // arrange
            var ordenId = 1;

            // act
            var response = await this.sapFacade.GetOrderFormula(ordenId);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
            Assert.IsNotNull(response.Response);
            Assert.IsEmpty(response.ExceptionMessage);
            Assert.IsEmpty(response.UserError);
            Assert.AreEqual(200, response.Code);
        }

        /// <summary>
        /// test tet.
        /// </summary>
        /// <returns>test.</returns>
        [Test]
        public async Task GetComponente()
        {
            // arrange
            var component = new Dictionary<string, string>();

            // act
            var response = await this.sapFacade.GetComponents(component);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
            Assert.IsNotNull(response.Response);
            Assert.IsEmpty(response.ExceptionMessage);
            Assert.IsEmpty(response.UserError);
            Assert.AreEqual(200, response.Code);
        }

        /// <summary>
        /// test tet.
        /// </summary>
        /// <returns>test.</returns>
        [Test]
        public async Task GetBatchesComponents()
        {
            // arrange
            var ordenId = 1;

            // act
            var response = await this.sapFacade.GetBatchesComponents(ordenId);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
            Assert.IsNotNull(response.Response);
            Assert.IsEmpty(response.ExceptionMessage);
            Assert.IsEmpty(response.UserError);
            Assert.AreEqual(200, response.Code);
        }

        /// <summary>
        /// test tet.
        /// </summary>
        /// <returns>test.</returns>
        [Test]
        public async Task GetlLastIsolatedProductionOrderId()
        {
            // arrange
            var productId = "code";
            var uniqueId = "token";

            // act
            var response = await this.sapFacade.GetlLastIsolatedProductionOrderId(productId, uniqueId);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
            Assert.IsNotNull(response.Response);
            Assert.IsEmpty(response.ExceptionMessage);
            Assert.IsEmpty(response.UserError);
            Assert.AreEqual(200, response.Code);
        }

        /// <summary>
        /// test tet.
        /// </summary>
        /// <returns>test.</returns>
        [Test]
        public async Task GetNextBatchCode()
        {
            // arrange
            var productId = "code";

            // act
            var response = await this.sapFacade.GetNextBatchCode(productId);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
            Assert.IsNotNull(response.Response);
            Assert.IsEmpty(response.ExceptionMessage);
            Assert.IsEmpty(response.UserError);
            Assert.AreEqual(200, response.Code);
        }

        /// <summary>
        /// test tet.
        /// </summary>
        /// <returns>test.</returns>
        [Test]
        public async Task GetFabOrders()
        {
            // arrange
            var parameters = new GetOrderFabDto
            {
                Filters = new Dictionary<string, string>(),
                OrdersId = new List<int>(),
            };

            // act
            var response = await this.sapFacade.GetFabOrders(parameters);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
            Assert.IsNotNull(response.Response);
            Assert.IsEmpty(response.ExceptionMessage);
            Assert.IsEmpty(response.UserError);
            Assert.AreEqual(200, response.Code);
        }

        /// <summary>
        /// test tet.
        /// </summary>
        /// <returns>test.</returns>
        [Test]
        public async Task GetProductsManagmentByBatch()
        {
            // arrange
            var pamameters = new Dictionary<string, string>();

            // act
            var response = await this.sapFacade.GetProductsManagmentByBatch(pamameters);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
            Assert.IsNotNull(response.Response);
            Assert.IsEmpty(response.ExceptionMessage);
            Assert.IsEmpty(response.UserError);
            Assert.AreEqual(200, response.Code);
        }

         /// <summary>
        /// test tet.
        /// </summary>
        /// <returns>test.</returns>
        [Test]
        public async Task GetFabOrdersById()
        {
            // arrange
            var parameters = new List<int>();

            // act
            var response = await this.sapFacade.GetFabOrdersById(parameters);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
            Assert.IsNotNull(response.Response);
            Assert.IsEmpty(response.ExceptionMessage);
            Assert.IsEmpty(response.UserError);
            Assert.AreEqual(200, response.Code);
        }
    }
}
