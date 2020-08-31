// <summary>
// <copyright file="FacadeTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Test.Facade
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Moq;
    using NUnit.Framework;
    using Omicron.Pedidos.Dtos.Models;
    using Omicron.Pedidos.Dtos.User;
    using Omicron.Pedidos.Entities.Model;
    using Omicron.Pedidos.Facade.Pedidos;
    using Omicron.Pedidos.Resources.Enums;
    using Omicron.Pedidos.Services.Mapping;
    using Omicron.Pedidos.Services.Pedidos;
    using Omicron.Pedidos.Services.User;

    /// <summary>
    /// Class UsersServiceTest.
    /// </summary>
    [TestFixture]
    public class FacadeTest : BaseTest
    {
        private PedidoFacade pedidoFacade;

        /// <summary>
        /// The init.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            var mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
            var mapper = mapperConfiguration.CreateMapper();

            var mockServices = new Mock<IUsersService>();
            var user = this.GetUserDto();
            IEnumerable<UserDto> listUser = new List<UserDto> { user };

            var response = new ResultModel
            {
                Success = true,
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = string.Empty,
                UserError = string.Empty,
            };

            var mockServicesPedidos = new Mock<IPedidosService>();
            mockServicesPedidos
                .Setup(m => m.ProcessOrders(It.IsAny<ProcessOrderModel>()))
                .Returns(Task.FromResult(response));

            mockServicesPedidos
                .Setup(m => m.GetUserOrderBySalesOrder(It.IsAny<List<int>>()))
                .Returns(Task.FromResult(response));

            mockServicesPedidos
                .Setup(m => m.GetUserOrderByFabOrder(It.IsAny<List<int>>()))
                .Returns(Task.FromResult(response));

            mockServicesPedidos
                .Setup(m => m.GetFabOrderByUserID(It.IsAny<string>()))
                .Returns(Task.FromResult(response));

            mockServicesPedidos
                .Setup(m => m.GetUserOrdersByUserId(It.IsAny<List<string>>()))
                .Returns(Task.FromResult(response));

            mockServicesPedidos
                .Setup(m => m.AssignOrder(It.IsAny<ManualAssignModel>()))
                .Returns(Task.FromResult(response));

            mockServicesPedidos
                .Setup(m => m.UpdateComponents(It.IsAny<UpdateFormulaModel>()))
                .Returns(Task.FromResult(response));

            mockServicesPedidos
                .Setup(m => m.UpdateStatusOrder(It.IsAny<List<UpdateStatusOrderModel>>()))
                .Returns(Task.FromResult(response));

            mockServicesPedidos
                .Setup(m => m.ConnectDiApi())
                .Returns(Task.FromResult(response));

            mockServicesPedidos
                .Setup(m => m.CancelOrder(It.IsAny<List<OrderIdModel>>()))
                .Returns(Task.FromResult(response));

            mockServicesPedidos
                .Setup(m => m.CancelFabOrder(It.IsAny<List<OrderIdModel>>()))
                .Returns(Task.FromResult(response));

            mockServicesPedidos
                .Setup(m => m.ProcessByOrder(It.IsAny<ProcessByOrderModel>()))
                .Returns(Task.FromResult(response));

            mockServicesPedidos
                .Setup(m => m.AutomaticAssign(It.IsAny<AutomaticAssingModel>()))
                .Returns(Task.FromResult(response));

            mockServicesPedidos
                            .Setup(m => m.UpdateBatches(It.IsAny<List<AssignBatchModel>>()))
                            .Returns(Task.FromResult(response));

            mockServicesPedidos
                .Setup(m => m.UpdateOrderSignature(It.IsAny<SignatureTypeEnum>(), It.IsAny<UpdateOrderSignatureModel>()))
                .Returns(Task.FromResult(response));

            mockServicesPedidos
                .Setup(m => m.GetOrderSignatures(It.IsAny<int>()))
                .Returns(Task.FromResult(response));

            mockServicesPedidos
                .Setup(m => m.FinishOrder(It.IsAny<FinishOrderModel>()))
                .Returns(Task.FromResult(response));

            mockServicesPedidos
                .Setup(m => m.UpdateFabOrderComments(It.IsAny<List<UpdateOrderCommentsModel>>()))
                .Returns(Task.FromResult(response));

            mockServicesPedidos
                .Setup(m => m.CloseSalesOrders(It.IsAny<List<OrderIdModel>>()))
                .Returns(Task.FromResult(response));

            mockServicesPedidos
                .Setup(m => m.CloseFabOrders(It.IsAny<List<OrderIdModel>>()))
                .Returns(Task.FromResult(response));

            mockServicesPedidos
                .Setup(m => m.CreateIsolatedProductionOrder(It.IsAny<CreateIsolatedFabOrderModel>()))
                .Returns(Task.FromResult(response));

            mockServicesPedidos
                .Setup(m => m.GetFabOrders(It.IsAny<Dictionary<string, string>>()))
                .Returns(Task.FromResult(response));

            this.pedidoFacade = new PedidoFacade(mockServicesPedidos.Object, mapper);
        }

        /// <summary>
        /// the processOrders.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task ProcessOrders()
        {
            // arrange
            var order = new ProcessOrderDto();

            // act
            var response = await this.pedidoFacade.ProcessOrders(order);

            // arrange
            Assert.IsNotNull(response);
        }

        /// <summary>
        /// test test.
        /// </summary>
        /// <returns>returns nothing.</returns>
        [Test]
        public async Task GetUserOrderBySalesOrder()
        {
            // arrange
            var listIds = new List<int>();

            // act
            var response = await this.pedidoFacade.GetUserOrderBySalesOrder(listIds);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
            Assert.IsNotNull(response.Response);
            Assert.IsEmpty(response.ExceptionMessage);
            Assert.IsEmpty(response.UserError);
            Assert.AreEqual(200, response.Code);
        }

        /// <summary>
        /// test test.
        /// </summary>
        /// <returns>returns nothing.</returns>
        [Test]
        public async Task GetUserOrderByFabOrder()
        {
            // arrange
            var listIds = new List<int>();

            // act
            var response = await this.pedidoFacade.GetUserOrderByFabOrder(listIds);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
            Assert.IsNotNull(response.Response);
            Assert.IsEmpty(response.ExceptionMessage);
            Assert.IsEmpty(response.UserError);
            Assert.AreEqual(200, response.Code);
        }

        /// <summary>
        /// test test.
        /// </summary>
        /// <returns>returns nothing.</returns>
        [Test]
        public async Task GetFabOrderByUserID()
        {
            // arrange
            var ids = "1";

            // act
            var response = await this.pedidoFacade.GetFabOrderByUserID(ids);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
            Assert.IsNotNull(response.Response);
            Assert.IsEmpty(response.ExceptionMessage);
            Assert.IsEmpty(response.UserError);
            Assert.AreEqual(200, response.Code);
        }

        /// <summary>
        /// test test.
        /// </summary>
        /// <returns>returns nothing.</returns>
        [Test]
        public async Task GetUserOrdersByUserId()
        {
            // arrange
            var ids = new List<string> { "1" };

            // act
            var response = await this.pedidoFacade.GetUserOrdersByUserId(ids);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
            Assert.IsNotNull(response.Response);
            Assert.IsEmpty(response.ExceptionMessage);
            Assert.IsEmpty(response.UserError);
            Assert.AreEqual(200, response.Code);
        }

        /// <summary>
        /// test test.
        /// </summary>
        /// <returns>returns nothing.</returns>
        [Test]
        public async Task AsignarManual()
        {
            // arrange
            var asignar = new ManualAssignDto
            {
                DocEntry = new List<int> { 200 },
                OrderType = "Pedido",
                UserId = "abc",
                UserLogistic = "abd",
            };

            // act
            var response = await this.pedidoFacade.AssignHeader(asignar);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
            Assert.IsNotNull(response.Response);
            Assert.IsEmpty(response.ExceptionMessage);
            Assert.IsEmpty(response.UserError);
            Assert.AreEqual(200, response.Code);
        }

        /// <summary>
        /// test test.
        /// </summary>
        /// <returns>returns nothing.</returns>
        [Test]
        public async Task UpdateComponents()
        {
            // arrange
            var components = new List<CompleteDetalleFormulaDto>
            {
                new CompleteDetalleFormulaDto { Available = 1, BaseQuantity = 1, Consumed = 1, Description = "Des", OrderFabId = 2, PendingQuantity = 1, ProductId = "Aspirina", RequiredQuantity = 1, Stock = 1, Unit = "Unit", Warehouse = "wh", WarehouseQuantity = 1 },
            };

            var asignar = new UpdateFormulaDto
            {
                Comments = "Comments",
                Components = components,
                FabOrderId = 1,
                FechaFin = DateTime.Now,
                PlannedQuantity = 1,
            };

            // act
            var response = await this.pedidoFacade.UpdateComponents(asignar);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
            Assert.IsNotNull(response.Response);
            Assert.IsEmpty(response.ExceptionMessage);
            Assert.IsEmpty(response.UserError);
            Assert.AreEqual(200, response.Code);
        }

        /// <summary>
        /// test test.
        /// </summary>
        /// <returns>returns nothing.</returns>
        [Test]
        public async Task UpdateStatusUserOrder()
        {
            // arrange
            var components = new List<UpdateStatusOrderDto>
            {
                new UpdateStatusOrderDto { Status = "Proceso", OrderId = 1, UserId = "abc" },
            };

            // act
            var response = await this.pedidoFacade.UpdateStatusOrder(components);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
            Assert.IsNotNull(response.Response);
            Assert.IsEmpty(response.ExceptionMessage);
            Assert.IsEmpty(response.UserError);
            Assert.AreEqual(200, response.Code);
        }

        /// <summary>
        /// test test.
        /// </summary>
        /// <returns>returns nothing.</returns>
        [Test]
        public async Task ConnectDiApi()
        {
            // act
            var response = await this.pedidoFacade.ConnectDiApi();

            // Assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
            Assert.IsNotNull(response.Response);
            Assert.IsEmpty(response.ExceptionMessage);
            Assert.IsEmpty(response.UserError);
            Assert.AreEqual(200, response.Code);
        }

        /// <summary>
        /// test test.
        /// </summary>
        /// <returns>returns nothing.</returns>
        [Test]
        public async Task ProcessByOrder()
        {
            // arrange
            var processOrder = new ProcessByOrderDto
            {
                PedidoId = 1,
                ProductId = new List<string> { "Aspirina" },
                UserId = "userid",
            };

            // act
            var response = await this.pedidoFacade.ProcessByOrder(processOrder);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
            Assert.IsNotNull(response.Response);
            Assert.IsEmpty(response.ExceptionMessage);
            Assert.IsEmpty(response.UserError);
            Assert.AreEqual(200, response.Code);
        }

        /// <summary>
        /// test test.
        /// </summary>
        /// <returns>returns nothing.</returns>
        [Test]
        public async Task CancelOrder()
        {
            // arrange
            var orders = new List<OrderIdDto>
            {
                new OrderIdDto { OrderId = 1, UserId = "mockUser" },
            };

            // act
            var response = await this.pedidoFacade.CancelOrder(orders);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
            Assert.IsNotNull(response.Response);
            Assert.IsEmpty(response.ExceptionMessage);
            Assert.IsEmpty(response.UserError);
            Assert.AreEqual(200, response.Code);
        }

        /// <summary>
        /// test test.
        /// </summary>
        /// <returns>returns nothing.</returns>
        [Test]
        public async Task AutomaticAssign()
        {
            // arrange
            var processOrder = new AutomaticAssingDto
            {
                DocEntry = new List<int> { 1, 2 },
                UserLogistic = "CDF",
            };

            // act
            var response = await this.pedidoFacade.AutomaticAssign(processOrder);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
            Assert.IsNotNull(response.Response);
            Assert.IsEmpty(response.ExceptionMessage);
            Assert.IsEmpty(response.UserError);
            Assert.AreEqual(200, response.Code);
        }

        /// <summary>
        /// test test.
        /// </summary>
        /// <returns>returns nothing.</returns>
        [Test]
        public async Task CancelFabOrder()
        {
            // arrange
            var orders = new List<OrderIdDto>
            {
                new OrderIdDto { OrderId = 1, UserId = "mockUser" },
            };

            // act
            var response = await this.pedidoFacade.CancelFabOrder(orders);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
            Assert.IsNotNull(response.Response);
            Assert.IsEmpty(response.ExceptionMessage);
            Assert.IsEmpty(response.UserError);
            Assert.AreEqual(200, response.Code);
        }

        /// <summary>
        /// test test.
        /// </summary>
        /// <returns>returns nothing.</returns>
        [Test]
        public async Task UpdateOrderSignature()
        {
            // arrange
            var orderSignature = new UpdateOrderSignatureDto
            {
                UserId = "New",
                FabricationOrderId = 1,
                Signature = "base64Data",
            };

            // act
            var response = await this.pedidoFacade.UpdateOrderSignature(SignatureTypeEnum.LOGISTICS, orderSignature);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
            Assert.IsNotNull(response.Response);
            Assert.IsEmpty(response.ExceptionMessage);
            Assert.IsEmpty(response.UserError);
            Assert.AreEqual(200, response.Code);
        }

        /// <summary>
        /// test test.
        /// </summary>
        /// <returns>returns nothing.</returns>
        [Test]
        public async Task GetOrderSignatures()
        {
            // arrange
            var fabricationOrder = 1;

            // act
            var response = await this.pedidoFacade.GetOrderSignatures(fabricationOrder);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
            Assert.IsNotNull(response.Response);
            Assert.IsEmpty(response.ExceptionMessage);
            Assert.IsEmpty(response.UserError);
            Assert.AreEqual(200, response.Code);
        }

        /// <summary>
        /// test test.
        /// </summary>
        /// <returns>returns nothing.</returns>
        [Test]
        public async Task UpdateBatches()
        {
            // arrange
            var updateBatches = new List<AssignBatchDto>
            {
                new AssignBatchDto { Action = "Update", AssignedQty = 10, BatchNumber = "P123", OrderId = 100 },
            };

            // act
            var response = await this.pedidoFacade.UpdateBatches(updateBatches);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
            Assert.IsNotNull(response.Response);
            Assert.IsEmpty(response.ExceptionMessage);
            Assert.IsEmpty(response.UserError);
            Assert.AreEqual(200, response.Code);
        }

        /// <summary>
        /// test test.
        /// </summary>
        /// <returns>returns nothing.</returns>
        [Test]
        public async Task UpdateFabOrderComments()
        {
            // arrange
            var orders = new List<UpdateOrderCommentsDto>
            {
                new UpdateOrderCommentsDto { OrderId = 1, UserId = "mockUser", Comments = "Hello" },
            };

            // act
            var response = await this.pedidoFacade.UpdateFabOrderComments(orders);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
            Assert.IsNotNull(response.Response);
            Assert.IsEmpty(response.ExceptionMessage);
            Assert.IsEmpty(response.UserError);
            Assert.AreEqual(200, response.Code);
        }

        /// <summary>
        /// test test.
        /// </summary>
        /// <returns>returns nothing.</returns>
        [Test]
        public async Task FinishOrder()
        {
            // arrange
            var updateBatches = new FinishOrderDto
            {
                FabricationOrderId = 200,
                TechnicalSignature = "signture",
                QfbSignature = "asf",
                UserId = "abc",
            };

            // act
            var response = await this.pedidoFacade.FinishOrder(updateBatches);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
            Assert.IsNotNull(response.Response);
            Assert.IsEmpty(response.ExceptionMessage);
            Assert.IsEmpty(response.UserError);
            Assert.AreEqual(200, response.Code);
        }

        /// <summary>
        /// test test.
        /// </summary>
        /// <returns>returns nothing.</returns>
        [Test]
        public async Task CloseSalesOrders()
        {
            // arrange
            var salesOrders = new List<OrderIdDto>
            {
                new OrderIdDto { OrderId = 1, UserId = "abc", },
            };

            // act
            var response = await this.pedidoFacade.CloseSalesOrders(salesOrders);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
            Assert.IsNotNull(response.Response);
            Assert.IsEmpty(response.ExceptionMessage);
            Assert.IsEmpty(response.UserError);
            Assert.AreEqual(200, response.Code);
        }

        /// <summary>
        /// test test.
        /// </summary>
        /// <returns>returns nothing.</returns>
        [Test]
        public async Task CloseFabOrders()
        {
            // arrange
            var salesOrders = new List<OrderIdDto>
            {
                new OrderIdDto { OrderId = 1, UserId = "abc", },
            };

            // act
            var response = await this.pedidoFacade.CloseFabOrders(salesOrders);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
            Assert.IsNotNull(response.Response);
            Assert.IsEmpty(response.ExceptionMessage);
            Assert.IsEmpty(response.UserError);
            Assert.AreEqual(200, response.Code);
        }

        /// <summary>
        /// test test.
        /// </summary>
        /// <returns>returns nothing.</returns>
        [Test]
        public async Task CreateIsolatedProductionOrder()
        {
            // arrange
            var order = new CreateIsolatedFabOrderDto
            {
                ProductCode = "product",
                UserId = "abc",
            };

            // act
            var response = await this.pedidoFacade.CreateIsolatedProductionOrder(order);

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
            var parameters = new Dictionary<string, string>();

            // act
            var response = await this.pedidoFacade.GetFabOrders(parameters);

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
