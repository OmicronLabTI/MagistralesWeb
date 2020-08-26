// <summary>
// <copyright file="PedidoServiceTest.cs" company="Axity">
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
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
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
    using Omicron.Pedidos.Services.User;

    /// <summary>
    /// class for the test.
    /// </summary>
    [TestFixture]
    public class PedidoServiceTest : BaseTest
    {
        private IPedidosService pedidosService;

        private IPedidosDao pedidosDao;

        private Mock<ISapAdapter> sapAdapter;

        private ISapDiApi sapDiApi;

        private Mock<IUsersService> usersService;

        private DatabaseContext context;

        /// <summary>
        /// The set up.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "Temporal")
                .Options;

            this.context = new DatabaseContext(options);
            this.context.UserOrderModel.AddRange(this.GetUserOrderModel());
            this.context.OrderLogModel.Add(this.GetOrderLogModel());
            this.context.UserOrderSignatureModel.AddRange(this.GetSignature());
            this.context.SaveChanges();

            this.sapAdapter = new Mock<ISapAdapter>();
            this.sapAdapter
                .Setup(m => m.PostSapAdapter(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultModelGetFabricacionModel()));

            this.sapAdapter
                .Setup(m => m.GetSapAdapter(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetFormulaDetalle()));

            var mockSaDiApi = new Mock<ISapDiApi>();
            mockSaDiApi
                .Setup(x => x.PostToSapDiApi(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultCreateOrder()));

            mockSaDiApi
                .Setup(x => x.GetSapDiApi(It.IsAny<string>()))
                .Returns(Task.FromResult(new ResultModel()));

            this.usersService = new Mock<IUsersService>();

            this.pedidosDao = new PedidosDao(this.context);
            this.pedidosService = new PedidosService(this.sapAdapter.Object, this.pedidosDao, mockSaDiApi.Object, this.usersService.Object);
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task ProcessOrders()
        {
            // arrange
            var process = new ProcessOrderModel
            {
                ListIds = new List<int> { 100 },
                User = "abc",
            };

            var localSapAdapter = new Mock<ISapAdapter>();
            localSapAdapter
                .SetupSequence(m => m.PostSapAdapter(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultModelCompleteDetailModel()))
                .Returns(Task.FromResult(this.GetResultModelGetFabricacionModel()));

            var mockSaDiApi = new Mock<ISapDiApi>();
            mockSaDiApi
                .Setup(x => x.PostToSapDiApi(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultCreateOrder()));

            mockSaDiApi
                .Setup(x => x.GetSapDiApi(It.IsAny<string>()))
                .Returns(Task.FromResult(new ResultModel()));

            var pedidosServiceLocal = new PedidosService(localSapAdapter.Object, this.pedidosDao, mockSaDiApi.Object, this.usersService.Object);

            // act
            var response = await pedidosServiceLocal.ProcessOrders(process);

            // assert
            Assert.IsNotNull(response);
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task GetUserOrderBySalesOrder()
        {
            // arrange
            var listIds = new List<int> { 1, 2, 3 };

            // act
            var response = await this.pedidosService.GetUserOrderBySalesOrder(listIds);

            // assert
            Assert.IsNotNull(response);
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task GetUserOrderByFabOrder()
        {
            // arrange
            var listIds = new List<int> { 100, 101, 102 };

            // act
            var response = await this.pedidosService.GetUserOrderByFabOrder(listIds);

            // assert
            Assert.IsNotNull(response);
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task GetFabOrderByUserID()
        {
            // arrange
            var id = "abc";

            this.sapAdapter
                .Setup(m => m.GetSapAdapter(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetFormulaDetalle()));

            var mockSaDiApi = new Mock<ISapDiApi>();
            mockSaDiApi
                .Setup(x => x.PostToSapDiApi(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultCreateOrder()));

            var pedidosServiceLocal = new PedidosService(this.sapAdapter.Object, this.pedidosDao, mockSaDiApi.Object, this.usersService.Object);

            // act
            var response = await pedidosServiceLocal.GetFabOrderByUserID(id);

            // assert
            Assert.IsNotNull(response);
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task GetUserOrdersByUserId()
        {
            // arrange
            var id = new List<string> { "abc" };

            // act
            var response = await this.pedidosService.GetUserOrdersByUserId(id);

            // assert
            Assert.IsNotNull(response);
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task AssignOrderPedido()
        {
            // arrange
            var assign = new ManualAssignModel
            {
                DocEntry = new List<int> { 100 },
                OrderType = "Pedido",
                UserId = "abc",
                UserLogistic = "abd",
            };

            var mockSaDiApi = new Mock<ISapDiApi>();
            mockSaDiApi
                .Setup(x => x.PostToSapDiApi(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultUpdateOrder()));

            this.sapAdapter
                .Setup(m => m.GetSapAdapter(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetListCompleteDetailOrderModel()));

            var pedidosServiceLocal = new PedidosService(this.sapAdapter.Object, this.pedidosDao, mockSaDiApi.Object, this.usersService.Object);

            // act
            var response = await pedidosServiceLocal.AssignOrder(assign);

            // assert
            Assert.IsNotNull(response);
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task AssignOrder()
        {
            // arrange
            var assign = new ManualAssignModel
            {
                DocEntry = new List<int> { 100 },
                OrderType = "Orden",
                UserId = "abc",
                UserLogistic = "abd",
            };

            var mockSaDiApi = new Mock<ISapDiApi>();
            mockSaDiApi
                .Setup(x => x.PostToSapDiApi(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultUpdateOrder()));

            var pedidosServiceLocal = new PedidosService(this.sapAdapter.Object, this.pedidosDao, mockSaDiApi.Object, this.usersService.Object);

            // act
            var response = await pedidosServiceLocal.AssignOrder(assign);

            // assert
            Assert.IsNotNull(response);
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task UpdateComponents()
        {
            // arrange
            var components = new List<CompleteDetalleFormulaModel>
            {
                new CompleteDetalleFormulaModel { Available = 1, BaseQuantity = 1, Consumed = 1, Description = "Des", OrderFabId = 2, PendingQuantity = 1, ProductId = "Aspirina", RequiredQuantity = 1, Stock = 1, Unit = "Unit", Warehouse = "wh", WarehouseQuantity = 1 },
            };

            var asignar = new UpdateFormulaModel
            {
                Comments = "Comments",
                Components = components,
                FabOrderId = 1,
                FechaFin = DateTime.Now,
                PlannedQuantity = 1,
            };

            var mockSaDiApi = new Mock<ISapDiApi>();
            mockSaDiApi
                .Setup(x => x.PostToSapDiApi(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultUpdateOrder()));

            var pedidosServiceLocal = new PedidosService(this.sapAdapter.Object, this.pedidosDao, mockSaDiApi.Object, this.usersService.Object);

            // act
            var response = await pedidosServiceLocal.UpdateComponents(asignar);

            // assert
            Assert.IsNotNull(response);
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task UpdateUserOrderStatus()
        {
            // arrange
            var components = new List<UpdateStatusOrderModel>
            {
                new UpdateStatusOrderModel { UserId = "abcc", OrderId = 100, Status = "Proceso" },
            };

            // act
            var response = await this.pedidosService.UpdateStatusOrder(components);

            // assert
            Assert.IsNotNull(response);
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task ConnectDiApi()
        {
            // act
            var response = await this.pedidosService.ConnectDiApi();

            // assert
            Assert.IsNotNull(response);
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task ProcessByOrder()
        {
            // arrange
            var process = new ProcessByOrderModel
            {
                UserId = "abc",
                ProductId = new List<string> { "Aspirina" },
                PedidoId = 100,
            };

            var localSapAdapter = new Mock<ISapAdapter>();
            localSapAdapter
                .SetupSequence(m => m.PostSapAdapter(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultModelCompleteDetailModel()))
                .Returns(Task.FromResult(this.GetResultModelGetFabricacionModel()));

            var mockSaDiApi = new Mock<ISapDiApi>();
            mockSaDiApi
                .Setup(x => x.PostToSapDiApi(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultCreateOrder()));

            mockSaDiApi
                .Setup(x => x.GetSapDiApi(It.IsAny<string>()))
                .Returns(Task.FromResult(new ResultModel()));

            var pedidosServiceLocal = new PedidosService(localSapAdapter.Object, this.pedidosDao, mockSaDiApi.Object, this.usersService.Object);

            // act
            var response = await pedidosServiceLocal.ProcessByOrder(process);

            // assert
            Assert.IsNotNull(response);
        }

        /// <summary>
        /// Update status to cancelled.
        /// </summary>
        /// <param name="orderId">Order to update.</param>
        /// <returns>Nothing.</returns>
        [Test]
        [TestCase(101)]
        [TestCase(100)]
        public async Task CancelOrder_WithAffectRecords(int orderId)
        {
            // arrange
            var userId = "abc";

            var orderToUpdate = new List<OrderIdModel>
            {
                new OrderIdModel { UserId = userId, OrderId = orderId },
            };

            // act
            var response = await this.pedidosService.CancelOrder(orderToUpdate);

            // assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
        }

        /// <summary>
        /// Update status to cancelled.
        /// </summary>
        /// <param name="orderId">Order to update.</param>
        /// <returns>Nothing.</returns>
        [Test]
        [TestCase(103)]
        public async Task CancelOrder_WithOutAffectRecords(int orderId)
        {
            // arrange
            var userId = "abc";

            var orderToUpdate = new List<OrderIdModel>
            {
                new OrderIdModel { UserId = userId, OrderId = orderId },
            };

            // act
            var response = await this.pedidosService.CancelOrder(orderToUpdate);

            // assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
        }

        /// <summary>
        /// Update status to cancelled.
        /// </summary>
        /// <param name="orderId">Order to update.</param>
        /// <returns>Nothing.</returns>
        [Test]
        [TestCase(104)]
        public async Task CancelFabOrder(int orderId)
        {
            // arrange
            var userId = "abc";

            var orderToUpdate = new List<OrderIdModel>
            {
                new OrderIdModel { UserId = userId, OrderId = orderId },
            };

            // act
            var response = await this.pedidosService.CancelFabOrder(orderToUpdate);

            // assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
        }

        /// <summary>
        /// the automatic assign test.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task AutomaticAssign()
        {
            var assign = new AutomaticAssingModel
            {
                DocEntry = new List<int> { 100 },
                UserLogistic = "abc",
            };

            var mockUsers = new Mock<IUsersService>();
            mockUsers
                .Setup(m => m.SimpleGetUsers(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetUsersByRole()));

            var mockSaDiApiLocal = new Mock<ISapDiApi>();
            mockSaDiApiLocal
                .Setup(x => x.PostToSapDiApi(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultCreateOrder()));

            var sapAdapterLocal = new Mock<ISapAdapter>();
            sapAdapterLocal
                .Setup(m => m.PostSapAdapter(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultModelCompleteDetailModel()));

            var pedidoServiceLocal = new PedidosService(sapAdapterLocal.Object, this.pedidosDao, mockSaDiApiLocal.Object, mockUsers.Object);

            // act
            var response = await pedidoServiceLocal.AutomaticAssign(assign);

            // assert
            Assert.IsNotNull(response);
        }

        /// <summary>
        /// Update order signatures.
        /// </summary>
        /// <returns>Nothing.</returns>
        [Test]
        public async Task UpdateOrderSignature()
        {
            // arrange
            var imageData = File.ReadAllText("SignatureBase64.txt");

            var signatures = new UpdateOrderSignatureModel
            {
                UserId = "abc",
                FabricationOrderId = 101,
                Signature = imageData,
            };

            // act
            var response = await this.pedidosService.UpdateOrderSignature(SignatureTypeEnum.LOGISTICS, signatures);

            // assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
        }

        /// <summary>
        /// Update fabrication order comments.
        /// </summary>
        /// <param name="orderId">Order to update.</param>
        /// <returns>Nothing.</returns>
        [Test]
        [TestCase(103)]
        public async Task UpdateFabOrderComments(int orderId)
        {
            // arrange
            var userId = "abc";

            var orderToUpdate = new List<UpdateOrderCommentsModel>
            {
                new UpdateOrderCommentsModel { UserId = userId, OrderId = orderId, Comments = "Hello" },
            };

            // act
            var response = await this.pedidosService.UpdateFabOrderComments(orderToUpdate);

            // assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
        }

        /// <summary>
        /// Get order signatures.
        /// </summary>
        /// <returns>Nothing.</returns>
        [Test]
        public async Task GetOrderSignatures()
        {
            // arrange
            int productionOrderId = 102;

            // act
            var response = await this.pedidosService.GetOrderSignatures(productionOrderId);

            // assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task UpdateBatches()
        {
            // arrange
            var update = new AssignBatchModel
            {
                Action = "insert",
                AssignedQty = 10,
                BatchNumber = "ABC",
                ItemCode = "102",
                OrderId = 10,
            };

            var mockSaDiApiLocal = new Mock<ISapDiApi>();
            mockSaDiApiLocal
                .Setup(m => m.PostToSapDiApi(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultAssignBatch()));

            var mockUsers = new Mock<IUsersService>();
            var localSapAdapter = new Mock<ISapAdapter>();
            var pedidoServiceLocal = new PedidosService(localSapAdapter.Object, this.pedidosDao, mockSaDiApiLocal.Object, mockUsers.Object);

            // act
            var response = await pedidoServiceLocal.UpdateBatches(new List<AssignBatchModel> { update });

            // assert
            Assert.IsNotNull(response);
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task FinishOrder()
        {
            // arrange
            var update = new UpdateOrderSignatureModel
            {
                FabricationOrderId = 100,
                Signature = "asdf",
                UserId = "abc",
            };

            var mockSaDiApiLocal = new Mock<ISapDiApi>();
            var mockUsers = new Mock<IUsersService>();
            var localSapAdapter = new Mock<ISapAdapter>();

            localSapAdapter
                .Setup(m => m.GetSapAdapter(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetBatches()));
            var pedidoServiceLocal = new PedidosService(localSapAdapter.Object, this.pedidosDao, mockSaDiApiLocal.Object, mockUsers.Object);

            // act
            var response = await pedidoServiceLocal.FinishOrder(update);

            // assert
            Assert.IsNotNull(response);
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task FinishOrderNewSignature()
        {
            // arrange
            var update = new UpdateOrderSignatureModel
            {
                FabricationOrderId = 101,
                Signature = "asdf",
                UserId = "abc",
            };

            var mockSaDiApiLocal = new Mock<ISapDiApi>();
            var mockUsers = new Mock<IUsersService>();
            var localSapAdapter = new Mock<ISapAdapter>();

            localSapAdapter
                .Setup(m => m.GetSapAdapter(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetBatches()));
            var pedidoServiceLocal = new PedidosService(localSapAdapter.Object, this.pedidosDao, mockSaDiApiLocal.Object, mockUsers.Object);

            // act
            var response = await pedidoServiceLocal.FinishOrder(update);

            // assert
            Assert.IsNotNull(response);
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task FinishOrderHasError()
        {
            // arrange
            var update = new UpdateOrderSignatureModel
            {
                FabricationOrderId = 101,
                Signature = "asdf",
                UserId = "abc",
            };

            var mockSaDiApiLocal = new Mock<ISapDiApi>();
            var mockUsers = new Mock<IUsersService>();
            var localSapAdapter = new Mock<ISapAdapter>();

            localSapAdapter
                .Setup(m => m.GetSapAdapter(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetMissingBatches()));
            var pedidoServiceLocal = new PedidosService(localSapAdapter.Object, this.pedidosDao, mockSaDiApiLocal.Object, mockUsers.Object);

            // act
            Assert.ThrowsAsync<CustomServiceException>(async () => await pedidoServiceLocal.FinishOrder(update));
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task FinishBySalesOrder()
        {
            // arrange
            var salesOrders = new List<OrderIdModel>
            {
                new OrderIdModel { OrderId = 104, UserId = "abc", },
            };
            var mockContent = new Dictionary<int, string> { { 0, "Ok" } };
            var mockSaDiApiLocal = new Mock<ISapDiApi>();
            var mockResult = new ResultModel();
            mockResult.Success = true;
            mockResult.Code = 200;
            mockResult.Response = JsonConvert.SerializeObject(mockContent);

            mockSaDiApiLocal
                .Setup(m => m.PostToSapDiApi(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(mockResult));

            var mockUsers = new Mock<IUsersService>();
            var localSapAdapter = new Mock<ISapAdapter>();
            var pedidoServiceLocal = new PedidosService(localSapAdapter.Object, this.pedidosDao, mockSaDiApiLocal.Object, mockUsers.Object);

            // act
            var response = await pedidoServiceLocal.FinishBySalesOrder(salesOrders);

            // assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
        }
    }
}
