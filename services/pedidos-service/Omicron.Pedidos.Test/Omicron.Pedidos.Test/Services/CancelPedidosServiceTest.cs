// <summary>
// <copyright file="CancelPedidosServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Test.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Moq;
    using NUnit.Framework;
    using Omicron.Pedidos.DataAccess.DAO.Pedidos;
    using Omicron.Pedidos.Entities.Context;
    using Omicron.Pedidos.Entities.Model;
    using Omicron.Pedidos.Services.Constants;
    using Omicron.Pedidos.Services.Pedidos;
    using Omicron.Pedidos.Services.SapAdapter;
    using Omicron.Pedidos.Services.SapDiApi;

    /// <summary>
    /// class for the test.
    /// </summary>
    [TestFixture]
    public class CancelPedidosServiceTest : BaseTest
    {
        private ICancelPedidosService cancelPedidosService;

        private IPedidosDao pedidosDao;

        private Mock<ISapAdapter> mockSapAdapter;

        private Mock<ISapDiApi> mockDiApiService;

        private DatabaseContext context;

        private string userId = "abc";

        /// <summary>
        /// Get user order models.
        /// </summary>
        /// <returns>the user.</returns>
        public List<UserOrderModel> GetUserOrderModelsForCancellationTests()
        {
            return new List<UserOrderModel>
            {
                new UserOrderModel { Id = 50, Productionorderid = "1050", Salesorderid = string.Empty, Status = "Proceso", Userid = "abcd" },

                new UserOrderModel { Id = 51, Productionorderid = null, Salesorderid = "10051", Status = "Finalizado", Userid = "abcd" },
                new UserOrderModel { Id = 52, Productionorderid = "1052", Salesorderid = "10051", Status = "Reasignado", Userid = "abcd" },

                new UserOrderModel { Id = 53, Productionorderid = null, Salesorderid = "10053", Status = "Proceso", Userid = "abcd" },
                new UserOrderModel { Id = 54, Productionorderid = "1054", Salesorderid = "10053", Status = "Cancelado", Userid = "abcd" },
                new UserOrderModel { Id = 55, Productionorderid = "1055", Salesorderid = "10053", Status = "Proceso", Userid = "abcd" },

                new UserOrderModel { Id = 56, Productionorderid = "1056", Salesorderid = string.Empty, Status = "Cancelado", Userid = "abcd" },

                new UserOrderModel { Id = 57, Productionorderid = null, Salesorderid = "10057", Status = "Proceso", Userid = "abcd" },
                new UserOrderModel { Id = 58, Productionorderid = "1057", Salesorderid = "10057", Status = "Proceso", Userid = "abcd" },
                new UserOrderModel { Id = 59, Productionorderid = "1058", Salesorderid = "10057", Status = "Proceso", Userid = "abcd" },

                new UserOrderModel { Id = 60, Productionorderid = "1060", Salesorderid = string.Empty, Status = "Finalizado", Userid = "abcd" },

                new UserOrderModel { Id = 61, Productionorderid = null, Salesorderid = "10061", Status = "Proceso", Userid = "abcd" },
                new UserOrderModel { Id = 62, Productionorderid = "1062", Salesorderid = "10061", Status = "Finalizado", Userid = "abcd" },
                new UserOrderModel { Id = 63, Productionorderid = "1063", Salesorderid = "10061", Status = "Proceso", Userid = "abcd" },
            };
        }

        /// <summary>
        /// The set up.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            var options = CreateNewContextOptions();
            this.context = new DatabaseContext(options);
            this.context.UserOrderModel.AddRange(this.GetUserOrderModelsForCancellationTests());
            this.context.SaveChanges();
            this.pedidosDao = new PedidosDao(this.context);
        }

        /// <summary>
        /// Build a service instance.
        /// </summary>
        /// <param name="sapAdapterResponseSalesOrderContent">Sap adapter response.</param>
        /// <param name="diapiResponseContent">Di api response.</param>
        /// <param name="sapAdapteProdOrdersResponseContent">Sap adapter production orders response.</param>
        /// <returns>Service instance.</returns>
        public CancelPedidosService BuildService(object sapAdapterResponseSalesOrderContent, object diapiResponseContent, object sapAdapteProdOrdersResponseContent = null)
        {
            var mockResultDiapi = new ResultModel();
            mockResultDiapi.Success = true;
            mockResultDiapi.Code = 200;
            mockResultDiapi.Response = diapiResponseContent;

            var mockResultSalesOrdersSapAdapter = new ResultModel();
            mockResultSalesOrdersSapAdapter.Success = true;
            mockResultSalesOrdersSapAdapter.Code = 200;
            mockResultSalesOrdersSapAdapter.Response = sapAdapterResponseSalesOrderContent;

            var mockResultProdutionOrdersSapAdapter = new ResultModel();
            mockResultProdutionOrdersSapAdapter.Success = true;
            mockResultProdutionOrdersSapAdapter.Code = 200;
            mockResultProdutionOrdersSapAdapter.Response = sapAdapteProdOrdersResponseContent;

            this.mockSapAdapter = new Mock<ISapAdapter>();
            this.mockSapAdapter
                .Setup(m => m.PostSapAdapter(It.IsAny<object>(), ServiceConstants.GetOrderWithDetail))
                .Returns(Task.FromResult(mockResultSalesOrdersSapAdapter));

            this.mockSapAdapter
                .Setup(m => m.PostSapAdapter(It.IsAny<object>(), ServiceConstants.GetFabOrdersByFilter))
                .Returns(Task.FromResult(mockResultProdutionOrdersSapAdapter));

            this.mockDiApiService = new Mock<ISapDiApi>();
            this.mockDiApiService
                .Setup(x => x.PostToSapDiApi(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(mockResultDiapi));

            return new CancelPedidosService(this.mockSapAdapter.Object, this.pedidosDao, this.mockDiApiService.Object);
        }

        /// <summary>
        /// Cancel a single production order.
        /// </summary>
        /// <param name="orderId">Order to update.</param>
        /// <returns>Nothing.</returns>
        [Test]
        [TestCase(1050)]
        [TestCase(1058)]
        public async Task CancelFabricationOrders_AffectSingleOrder_SuccessResults(int orderId)
        {
            // arrange
            this.cancelPedidosService = this.BuildService(null, "Ok");
            var orderToUpdate = new List<OrderIdModel>
            {
                new OrderIdModel { UserId = this.userId, OrderId = orderId },
            };

            // act
            var response = await this.cancelPedidosService.CancelFabricationOrders(orderToUpdate);

            // assert
            Assert.IsTrue(this.CheckAction(response, true, 1, 0, 1));
            Assert.AreEqual(orderId.ToString(), this.context.OrderLogModel.FirstOrDefault().Noid);
            this.mockDiApiService.Verify(v => v.PostToSapDiApi(It.IsAny<object>(), It.IsAny<string>()), Times.Once);
        }

        /// <summary>
        /// Cancel fabrication orders already cancelled.
        /// </summary>
        /// <param name="orderId">Order to update.</param>
        /// <returns>Nothing.</returns>
        [Test]
        [TestCase(1054)]
        [TestCase(1056)]
        public async Task CancelFabricationOrders_AlreadyCancelled_SuccessResults(int orderId)
        {
            // arrange
            this.cancelPedidosService = this.BuildService(null, null);
            var orderToUpdate = new List<OrderIdModel>
            {
                new OrderIdModel { UserId = this.userId, OrderId = orderId },
            };

            // act
            var response = await this.cancelPedidosService.CancelFabricationOrders(orderToUpdate);

            // assert
            Assert.IsTrue(this.CheckAction(response, true, 1, 0, 0));
            this.mockDiApiService.Verify(v => v.PostToSapDiApi(It.IsAny<object>(), It.IsAny<string>()), Times.Never);
        }

        /// <summary>
        /// Cancel fabrication orders with finished status.
        /// </summary>
        /// <param name="orderId">Order to update.</param>
        /// <returns>Nothing.</returns>
        [Test]
        [TestCase(1060)]
        public async Task CancelFabricationOrders_FinishedFabricationOrder_FailedResults(int orderId)
        {
            // arrange
            this.cancelPedidosService = this.BuildService(null, null);
            var orderToUpdate = new List<OrderIdModel>
            {
                new OrderIdModel { UserId = this.userId, OrderId = orderId },
            };

            // act
            var response = await this.cancelPedidosService.CancelFabricationOrders(orderToUpdate);

            // assert
            Assert.IsTrue(this.CheckAction(response, true, 0, 1, 0));
            this.mockDiApiService.Verify(v => v.PostToSapDiApi(It.IsAny<object>(), It.IsAny<string>()), Times.Never);
        }

        /// <summary>
        /// Cancel fabrication orders not found.
        /// </summary>
        /// <param name="orderId">Order to update.</param>
        /// <returns>Nothing.</returns>
        [Test]
        [TestCase(1000)]
        public async Task CancelFabricationOrders_NotFound_FailResults(int orderId)
        {
            // arrange
            this.cancelPedidosService = this.BuildService(null, null, new List<string>());
            var orderToUpdate = new List<OrderIdModel>
            {
                new OrderIdModel { UserId = this.userId, OrderId = orderId },
            };

            // act
            var response = await this.cancelPedidosService.CancelFabricationOrders(orderToUpdate);

            // assert
            Assert.IsTrue(this.CheckAction(response, true, 0, 1, 0));
            this.mockDiApiService.Verify(v => v.PostToSapDiApi(It.IsAny<object>(), It.IsAny<string>()), Times.Never);
        }

        /// <summary>
        /// Cancel fabrication orders missing local order.
        /// </summary>
        /// <param name="orderId">Order to update.</param>
        /// <returns>Nothing.</returns>
        [Test]
        [TestCase(997)]
        public async Task CancelFabricationOrders_GetFromSap_SuccessResults(int orderId)
        {
            // arrange
            this.cancelPedidosService = this.BuildService(this.GetSapAdapterOrder(), "Ok", this.GetSapAdapterProductionOrder());
            var orderToUpdate = new List<OrderIdModel>
            {
                new OrderIdModel { UserId = this.userId, OrderId = orderId },
            };

            // act
            var response = await this.cancelPedidosService.CancelFabricationOrders(orderToUpdate);

            // assert
            Assert.IsTrue(this.CheckAction(response, true, 1, 0, 1));
            this.mockDiApiService.Verify(v => v.PostToSapDiApi(It.IsAny<object>(), It.IsAny<string>()), Times.Once);
            this.mockSapAdapter.Verify(v => v.PostSapAdapter(It.IsAny<object>(), ServiceConstants.GetOrderWithDetail), Times.Once);
            this.mockSapAdapter.Verify(v => v.PostSapAdapter(It.IsAny<object>(), ServiceConstants.GetFabOrdersByFilter), Times.Once);
        }

        /// <summary>
        /// Cancel fabrication orders missing local order.
        /// </summary>
        /// <param name="orderId">Order to update.</param>
        /// <returns>Nothing.</returns>
        [Test]
        [TestCase(997)]
        public async Task CancelFabricationOrders_GetIsolatedFromSap_SuccessResults(int orderId)
        {
            // arrange
            this.cancelPedidosService = this.BuildService(null, "Ok", this.GetSapAdapterIsolatedProductionOrder());
            var orderToUpdate = new List<OrderIdModel>
            {
                new OrderIdModel { UserId = this.userId, OrderId = orderId },
            };

            // act
            var response = await this.cancelPedidosService.CancelFabricationOrders(orderToUpdate);

            // assert
            Assert.IsTrue(this.CheckAction(response, true, 1, 0, 1));
            this.mockDiApiService.Verify(v => v.PostToSapDiApi(It.IsAny<object>(), It.IsAny<string>()), Times.Once);
            this.mockSapAdapter.Verify(v => v.PostSapAdapter(It.IsAny<object>(), ServiceConstants.GetOrderWithDetail), Times.Never);
            this.mockSapAdapter.Verify(v => v.PostSapAdapter(It.IsAny<object>(), ServiceConstants.GetFabOrdersByFilter), Times.Once);
        }

        /// <summary>
        /// Cancel fabrication orders with sales order cancellation.
        /// </summary>
        /// <param name="orderId">Order to update.</param>
        /// <returns>Nothing.</returns>
        [Test]
        [TestCase(1055)]
        public async Task CancelFabricationOrders_AffectSalesOrder_SuccessResults(int orderId)
        {
            // arrange
            this.cancelPedidosService = this.BuildService(null, "Ok");
            var orderToUpdate = new List<OrderIdModel>
            {
                new OrderIdModel { UserId = this.userId, OrderId = orderId },
            };

            // act
            var response = await this.cancelPedidosService.CancelFabricationOrders(orderToUpdate);

            // assert
            Assert.IsTrue(response.Success);
            Assert.IsTrue(this.CheckAction(response, true, 1, 0, 2));
            this.mockDiApiService.Verify(v => v.PostToSapDiApi(It.IsAny<object>(), It.IsAny<string>()), Times.Once);
        }

        /// <summary>
        /// Cancel fabrication orders with sap error.
        /// </summary>
        /// <param name="orderId">Order to update.</param>
        /// <returns>Nothing.</returns>
        [Test]
        [TestCase(1055)]
        public async Task CancelFabricationOrders_SapDiApiError_FailResults(int orderId)
        {
            // arrange
            this.cancelPedidosService = this.BuildService(null, "Fail");
            var orderToUpdate = new List<OrderIdModel>
            {
                new OrderIdModel { UserId = this.userId, OrderId = orderId },
            };

            // act
            var response = await this.cancelPedidosService.CancelFabricationOrders(orderToUpdate);

            // assert
            Assert.IsTrue(this.CheckAction(response, true, 0, 1, 0));
            this.mockDiApiService.Verify(v => v.PostToSapDiApi(It.IsAny<object>(), It.IsAny<string>()), Times.Once);
        }

        /// <summary>
        /// Cancel sales orders successfuly.
        /// </summary>
        /// <param name="orderId">Order to update.</param>
        /// <param name="affectedRecords">Number of affected records.</param>
        /// <returns>Nothing.</returns>
        [Test]
        [TestCase(10057, 3)]
        public async Task CancelSalesOrder_ValidRelatedProductionOrders_SuccessResults(int orderId, int affectedRecords)
        {
            // arrange
            this.cancelPedidosService = this.BuildService(null, "Ok");
            var orderToUpdate = new List<OrderIdModel>
            {
                new OrderIdModel { UserId = this.userId, OrderId = orderId },
            };

            // act
            var response = await this.cancelPedidosService.CancelSalesOrder(orderToUpdate);

            // assert
            Assert.IsTrue(this.CheckAction(response, true, 1, 0, affectedRecords));
            this.mockDiApiService.Verify(v => v.PostToSapDiApi(It.IsAny<object>(), It.IsAny<string>()), Times.Exactly(affectedRecords - 1));
            this.mockSapAdapter.Verify(v => v.PostSapAdapter(It.IsAny<object>(), It.IsAny<string>()), Times.Never);
        }

        /// <summary>
        /// Cancel sales orders failed.
        /// </summary>
        /// <param name="orderId">Order to update.</param>
        /// <returns>Nothing.</returns>
        [Test]
        [TestCase(10061)]
        public async Task CancelSalesOrder_RelatedProductionOrdersFinished_FailResults(int orderId)
        {
            // arrange
            this.cancelPedidosService = this.BuildService(null, "Ok");
            var orderToUpdate = new List<OrderIdModel>
            {
                new OrderIdModel { UserId = this.userId, OrderId = orderId },
            };

            // act
            var response = await this.cancelPedidosService.CancelSalesOrder(orderToUpdate);

            // assert
            Assert.IsTrue(this.CheckAction(response, true, 0, 1, 0));
            this.mockDiApiService.Verify(v => v.PostToSapDiApi(It.IsAny<object>(), It.IsAny<string>()), Times.Never);
            this.mockSapAdapter.Verify(v => v.PostSapAdapter(It.IsAny<object>(), It.IsAny<string>()), Times.Never);
        }

        /// <summary>
        /// Cancel sales orders failed.
        /// </summary>
        /// <param name="orderId">Order to update.</param>
        /// <returns>Nothing.</returns>
        [Test]
        [TestCase(10051)]
        public async Task CancelSalesOrder_FinishedSalesOrder_FailResults(int orderId)
        {
            // arrange
            this.cancelPedidosService = this.BuildService(null, "Ok");
            var orderToUpdate = new List<OrderIdModel>
            {
                new OrderIdModel { UserId = this.userId, OrderId = orderId },
            };

            // act
            var response = await this.cancelPedidosService.CancelSalesOrder(orderToUpdate);

            // assert
            Assert.IsTrue(this.CheckAction(response, true, 0, 1, 0));
            this.mockDiApiService.Verify(v => v.PostToSapDiApi(It.IsAny<object>(), It.IsAny<string>()), Times.Never);
            this.mockSapAdapter.Verify(v => v.PostSapAdapter(It.IsAny<object>(), It.IsAny<string>()), Times.Never);
        }

        /// <summary>
        /// Cancel sales orders failed.
        /// </summary>
        /// <param name="orderId">Order to update.</param>
        /// <param name="affectedRecords">Affected records.</param>
        /// <returns>Nothing.</returns>
        [Test]
        [TestCase(997, 3)]
        public async Task CancelSalesOrder_GetFromSap_SuccessResults(int orderId, int affectedRecords)
        {
            // arrange
            this.cancelPedidosService = this.BuildService(this.GetSapAdapterOrder(), "Ok");
            var orderToUpdate = new List<OrderIdModel>
            {
                new OrderIdModel { UserId = this.userId, OrderId = orderId },
            };

            // act
            var response = await this.cancelPedidosService.CancelSalesOrder(orderToUpdate);

            // assert
            Assert.IsTrue(this.CheckAction(response, true, 1, 0, affectedRecords));
            this.mockDiApiService.Verify(v => v.PostToSapDiApi(It.IsAny<object>(), It.IsAny<string>()), Times.Exactly(affectedRecords - 1));
            this.mockSapAdapter.Verify(v => v.PostSapAdapter(It.IsAny<object>(), ServiceConstants.GetOrderWithDetail), Times.Once);
        }

        /// <summary>
        /// Cancel sales orders failed.
        /// </summary>
        /// <param name="orderId">Order to update.</param>
        /// <returns>Nothing.</returns>
        [Test]
        [TestCase(997)]
        public async Task CancelSalesOrder_GetFromSapWithFinishedProductionOrders_FailResults(int orderId)
        {
            // arrange
            this.cancelPedidosService = this.BuildService(this.GetSapAdapterOrderWithFinishedProductionOrders(), "Ok");
            var orderToUpdate = new List<OrderIdModel>
            {
                new OrderIdModel { UserId = this.userId, OrderId = orderId },
            };

            // act
            var response = await this.cancelPedidosService.CancelSalesOrder(orderToUpdate);

            // assert
            Assert.IsTrue(this.CheckAction(response, true, 0, 1, 0));
            this.mockDiApiService.Verify(v => v.PostToSapDiApi(It.IsAny<object>(), It.IsAny<string>()), Times.Never);
            this.mockSapAdapter.Verify(v => v.PostSapAdapter(It.IsAny<object>(), ServiceConstants.GetOrderWithDetail), Times.Once);
        }

        /// <summary>
        /// Cancel sales orders failed.
        /// </summary>
        /// <param name="orderId">Order to update.</param>
        /// <returns>Nothing.</returns>
        [Test]
        [TestCase(997)]
        public async Task CancelSalesOrder_GetFromSapWithFinishedSalesOrder_FailResults(int orderId)
        {
            // arrange
            this.cancelPedidosService = this.BuildService(this.GetSapAdapterOrderWithFinishedSalesOrder(), "Ok");
            var orderToUpdate = new List<OrderIdModel>
            {
                new OrderIdModel { UserId = this.userId, OrderId = orderId },
            };

            // act
            var response = await this.cancelPedidosService.CancelSalesOrder(orderToUpdate);

            // assert
            Assert.IsTrue(this.CheckAction(response, true, 0, 1, 0));
            this.mockDiApiService.Verify(v => v.PostToSapDiApi(It.IsAny<object>(), It.IsAny<string>()), Times.Never);
            this.mockSapAdapter.Verify(v => v.PostSapAdapter(It.IsAny<object>(), ServiceConstants.GetOrderWithDetail), Times.Once);
        }

        private static DbContextOptions<DatabaseContext> CreateNewContextOptions()
        {
            // Create a fresh service provider, and therefore a fresh.
            // InMemory database instance.
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            // Create a new options instance telling the context to use an
            // InMemory database and the new service provider.
            var builder = new DbContextOptionsBuilder<DatabaseContext>();
            builder.UseInMemoryDatabase("TemporalCancelPedidos")
                   .UseInternalServiceProvider(serviceProvider);

            return builder.Options;
        }

        /// <summary>
        /// Gets sap adapter order.
        /// </summary>
        /// <returns>Sap order as json string.</returns>
        private List<OrderWithDetailModel> GetSapAdapterOrder()
        {
            var detalle = new List<CompleteDetailOrderModel>
            {
                new CompleteDetailOrderModel { OrdenFabricacionId = 998, Status = "P" },
                new CompleteDetailOrderModel { OrdenFabricacionId = 999, Status = "P" },
            };

            var orders = new List<OrderWithDetailModel>
            {
                new OrderWithDetailModel
                {
                    Detalle = new List<CompleteDetailOrderModel>(detalle),
                    Order = new OrderModel { DocNum = 997, PedidoId = 997, PedidoStatus = "L" },
                },
            };
            return orders;
        }

        /// <summary>
        /// Gets sap adapter production order.
        /// </summary>
        /// <returns>Sap order as json string.</returns>
        private List<FabricacionOrderModel> GetSapAdapterProductionOrder()
        {
            var orders = new List<FabricacionOrderModel>
            {
                new FabricacionOrderModel
                {
                    PedidoId = 997,
                    OrdenId = 998,
                    Status = "P",
                },
            };
            return orders;
        }

        /// <summary>
        /// Gets sap adapter isolated production order.
        /// </summary>
        /// <returns>Sap order as json string.</returns>
        private List<FabricacionOrderModel> GetSapAdapterIsolatedProductionOrder()
        {
            var orders = this.GetSapAdapterProductionOrder();
            orders[0].PedidoId = null;
            return orders;
        }

        /// <summary>
        /// Gets sap adapter order.
        /// </summary>
        /// <returns>Sap order as json string.</returns>
        private List<OrderWithDetailModel> GetSapAdapterOrderWithFinishedProductionOrders()
        {
            var orders = this.GetSapAdapterOrder();
            orders[0].Detalle[0].Status = "L";
            return orders;
        }

        /// <summary>
        /// Gets sap adapter order.
        /// </summary>
        /// <returns>Sap order as json string.</returns>
        private List<OrderWithDetailModel> GetSapAdapterOrderWithFinishedSalesOrder()
        {
            var orders = this.GetSapAdapterOrder();
            orders[0].Order.PedidoStatus = "C";
            return orders;
        }

        /// <summary>
        /// Check response results.
        /// </summary>
        /// <param name="result">Result.</param>
        /// <param name="success">Expected success.</param>
        /// <param name="numberOfSucceess">Expected success results.</param>
        /// <param name="numberOfFails">Expected fail results.</param>
        /// <param name="numberOfLogs">Expected logs.</param>
        /// <returns>Validation flag.</returns>
        private bool CheckAction(ResultModel result, bool success, int numberOfSucceess, int numberOfFails, int numberOfLogs)
        {
            var content = (SuccessFailResults<OrderIdModel>)result.Response;

            return result.Success.Equals(success) &&
                    numberOfFails.Equals(content.Failed.Count) &&
                    numberOfSucceess.Equals(content.Success.Count) &&
                    this.context.OrderLogModel.Count().Equals(numberOfLogs);
        }
    }
}
