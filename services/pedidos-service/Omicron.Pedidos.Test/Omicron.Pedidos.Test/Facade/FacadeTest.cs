// <summary>
// <copyright file="FacadeTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Test.Facade
{
    using Omicron.Pedidos.Services.ProductionOrders;

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

            var response = new ResultModel
            {
                Success = true,
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = string.Empty,
                UserError = string.Empty,
            };

            var mockerAssignPedidosService = new Mock<IAssignPedidosService>();
            var mockProductivityService = new Mock<IProductivityService>();
            var mockServicesPedidos = new Mock<IPedidosService>();
            var mockCancelPedidosServices = new Mock<ICancelPedidosService>();
            var mockFormulasPedidosServices = new Mock<IFormulaPedidosService>();
            var mockProcess = new Mock<IProcessOrdersService>();
            var mockProductionOrderService = new Mock<IProductionOrdersService>();

            mockProcess.SetReturnsDefault(Task.FromResult(response));
            mockerAssignPedidosService.SetReturnsDefault(Task.FromResult(response));
            mockProductivityService.SetReturnsDefault(Task.FromResult(response));
            mockServicesPedidos.SetReturnsDefault(Task.FromResult(response));
            mockCancelPedidosServices.SetReturnsDefault(Task.FromResult(response));
            mockFormulasPedidosServices.SetReturnsDefault(Task.FromResult(response));
            mockProductionOrderService.SetReturnsDefault(Task.FromResult(response));

            this.pedidoFacade = new PedidoFacade(
                mockServicesPedidos.Object,
                mapper,
                mockerAssignPedidosService.Object,
                mockCancelPedidosServices.Object,
                mockProductivityService.Object,
                mockFormulasPedidosServices.Object,
                mockProcess.Object,
                mockProductionOrderService.Object);
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
            Assert.That(response, Is.Not.Null);
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
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success);
            Assert.That(response.Response, Is.Not.Null);
            Assert.That(response.ExceptionMessage, Is.Empty);
            Assert.That(response.UserError, Is.Empty);
            Assert.That(response.Code.Equals(200));
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
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success);
            Assert.That(response.Response, Is.Not.Null);
            Assert.That(response.ExceptionMessage, Is.Empty);
            Assert.That(response.UserError, Is.Empty);
            Assert.That(response.Code.Equals(200));
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
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success);
            Assert.That(response.Response, Is.Not.Null);
            Assert.That(response.ExceptionMessage, Is.Empty);
            Assert.That(response.UserError, Is.Empty);
            Assert.That(response.Code.Equals(200));
        }

        /// <summary>
        /// test test.
        /// </summary>
        /// <returns>returns nothing.</returns>
        [Test]
        public async Task GetQfbOrdersByStatus()
        {
            // arrange
            var status = "Asignado";
            var iduser = "abc-cde";

            // act
            var response = await this.pedidoFacade.GetQfbOrdersByStatus(status, iduser);

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success);
            Assert.That(response.Response, Is.Not.Null);
            Assert.That(response.ExceptionMessage, Is.Empty);
            Assert.That(response.UserError, Is.Empty);
            Assert.That(response.Code.Equals(200));
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
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success);
            Assert.That(response.Response, Is.Not.Null);
            Assert.That(response.ExceptionMessage, Is.Empty);
            Assert.That(response.UserError, Is.Empty);
            Assert.That(response.Code.Equals(200));
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
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success);
            Assert.That(response.Response, Is.Not.Null);
            Assert.That(response.ExceptionMessage, Is.Empty);
            Assert.That(response.UserError, Is.Empty);
            Assert.That(response.Code.Equals(200));
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
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success);
            Assert.That(response.Response, Is.Not.Null);
            Assert.That(response.ExceptionMessage, Is.Empty);
            Assert.That(response.UserError, Is.Empty);
            Assert.That(response.Code.Equals(200));
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
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success);
            Assert.That(response.Response, Is.Not.Null);
            Assert.That(response.ExceptionMessage, Is.Empty);
            Assert.That(response.UserError, Is.Empty);
            Assert.That(response.Code.Equals(200));
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
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success);
            Assert.That(response.Response, Is.Not.Null);
            Assert.That(response.ExceptionMessage, Is.Empty);
            Assert.That(response.UserError, Is.Empty);
            Assert.That(response.Code.Equals(200));
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
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success);
            Assert.That(response.Response, Is.Not.Null);
            Assert.That(response.ExceptionMessage, Is.Empty);
            Assert.That(response.UserError, Is.Empty);
            Assert.That(response.Code.Equals(200));
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
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success);
            Assert.That(response.Response, Is.Not.Null);
            Assert.That(response.ExceptionMessage, Is.Empty);
            Assert.That(response.UserError, Is.Empty);
            Assert.That(response.Code.Equals(200));
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
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success);
            Assert.That(response.Response, Is.Not.Null);
            Assert.That(response.ExceptionMessage, Is.Empty);
            Assert.That(response.UserError, Is.Empty);
            Assert.That(response.Code.Equals(200));
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
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success);
            Assert.That(response.Response, Is.Not.Null);
            Assert.That(response.ExceptionMessage, Is.Empty);
            Assert.That(response.UserError, Is.Empty);
            Assert.That(response.Code.Equals(200));
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
            var response = await this.pedidoFacade.UpdateOrderSignature(SignatureType.LOGISTICS, orderSignature);

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success);
            Assert.That(response.Response, Is.Not.Null);
            Assert.That(response.ExceptionMessage, Is.Empty);
            Assert.That(response.UserError, Is.Empty);
            Assert.That(response.Code.Equals(200));
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
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success);
            Assert.That(response.Response, Is.Not.Null);
            Assert.That(response.ExceptionMessage, Is.Empty);
            Assert.That(response.UserError, Is.Empty);
            Assert.That(response.Code.Equals(200));
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
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success);
            Assert.That(response.Response, Is.Not.Null);
            Assert.That(response.ExceptionMessage, Is.Empty);
            Assert.That(response.UserError, Is.Empty);
            Assert.That(response.Code.Equals(200));
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
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success);
            Assert.That(response.Response, Is.Not.Null);
            Assert.That(response.ExceptionMessage, Is.Empty);
            Assert.That(response.UserError, Is.Empty);
            Assert.That(response.Code.Equals(200));
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
                FabricationOrderId = new List<int> { 200 },
                TechnicalSignature = "signture",
                QfbSignature = "asf",
                UserId = "abc",
            };

            // act
            var response = await this.pedidoFacade.FinishOrder(updateBatches);

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success);
            Assert.That(response.Response, Is.Not.Null);
            Assert.That(response.ExceptionMessage, Is.Empty);
            Assert.That(response.UserError, Is.Empty);
            Assert.That(response.Code.Equals(200));
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
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success);
            Assert.That(response.Response, Is.Not.Null);
            Assert.That(response.ExceptionMessage, Is.Empty);
            Assert.That(response.UserError, Is.Empty);
            Assert.That(response.Code.Equals(200));
        }

        /// <summary>
        /// test test.
        /// </summary>
        /// <returns>returns nothing.</returns>
        [Test]
        public async Task RejectSalesOrders()
        {
            // arrange
            var salesOrders = new RejectOrdersDto();
            salesOrders.Comments = "comentarios";
            salesOrders.UserId = "123-abc";
            salesOrders.OrdersId = new List<int>
            {
                234,
                235,
            };

            // act
            var response = await this.pedidoFacade.RejectSalesOrders(salesOrders);

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success);
            Assert.That(response.Response, Is.Not.Null);
            Assert.That(response.ExceptionMessage, Is.Empty);
            Assert.That(response.UserError, Is.Empty);
            Assert.That(response.Code.Equals(200));
        }

        /// <summary>
        /// test test.
        /// </summary>
        /// <returns>returns nothing.</returns>
        [Test]
        public async Task CloseFabOrders()
        {
            // arrange
            var salesOrders = new List<CloseProductionOrderDto>
            {
                new CloseProductionOrderDto { OrderId = 1, UserId = "abc", },
            };

            // act
            var response = await this.pedidoFacade.CloseFabOrders(salesOrders);

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success);
            Assert.That(response.Response, Is.Not.Null);
            Assert.That(response.ExceptionMessage, Is.Empty);
            Assert.That(response.UserError, Is.Empty);
            Assert.That(response.Code.Equals(200));
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
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success);
            Assert.That(response.Response, Is.Not.Null);
            Assert.That(response.ExceptionMessage, Is.Empty);
            Assert.That(response.UserError, Is.Empty);
            Assert.That(response.Code.Equals(200));
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
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success);
        }

        /// <summary>
        /// test tet.
        /// </summary>
        /// <returns>test.</returns>
        [Test]
        public async Task ReassignOrder()
        {
            // arrange
            var parameters = new ManualAssignDto()
            {
                DocEntry = new List<int> { 1, 2, 3 },
                OrderType = "Pedido",
                UserId = "abc",
                UserLogistic = "abc",
            };

            // act
            var response = await this.pedidoFacade.ReassignOrder(parameters);

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success);
        }

        /// <summary>
        /// test tet.
        /// </summary>
        /// <returns>test.</returns>
        [Test]
        public async Task GetProductivityData()
        {
            // arrange
            var parameters = new Dictionary<string, string>();

            // act
            var response = await this.pedidoFacade.GetProductivityData(parameters);

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success);
        }

        /// <summary>
        /// test tet.
        /// </summary>
        /// <returns>test.</returns>
        [Test]
        public async Task CreateCustomFormula()
        {
            // arrange
            var formula = new CustomComponentListDto();

            // act
            var response = await this.pedidoFacade.CreateCustomComponentList(string.Empty, formula);

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success);
        }

        /// <summary>
        /// test tet.
        /// </summary>
        /// <returns>test.</returns>
        [Test]
        public async Task GetCustomComponentListByProductId()
        {
            // act
            var response = await this.pedidoFacade.GetCustomComponentListByProductId(string.Empty);

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success);
        }

        /// <summary>
        /// test tet.
        /// </summary>
        /// <returns>test.</returns>
        [Test]
        public async Task DeleteCustomComponentList()
        {
            // arrange
            var parameters = new Dictionary<string, string>();

            // act
            var response = await this.pedidoFacade.DeleteCustomComponentList(parameters);

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success);
        }

        /// <summary>
        /// test tet.
        /// </summary>
        /// <returns>test.</returns>
        [Test]
        public async Task GetWorkLoad()
        {
            // arrange
            var parameters = new Dictionary<string, string>();

            // act
            var response = await this.pedidoFacade.GetWorkLoad(parameters);

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success);
        }

        /// <summary>
        /// test tet.
        /// </summary>
        /// <returns>test.</returns>
        [Test]
        public async Task CompletedBatches()
        {
            // arrange
            var orderId = 1;

            // act
            var response = await this.pedidoFacade.CompletedBatches(orderId);

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success);
        }

        /// <summary>
        /// test tet.
        /// </summary>
        /// <returns>test.</returns>
        [Test]
        public async Task PrintOrders()
        {
            // arrange
            var orderId = new List<int>();

            // act
            var response = await this.pedidoFacade.PrintOrders(orderId);

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success);
        }

        /// <summary>
        /// test tet.
        /// </summary>
        /// <returns>test.</returns>
        [Test]
        public async Task UpdateSaleOrders()
        {
            // arrange
            var orderId = new UpdateOrderCommentsDto
            {
                OrderId = 100,
                Comments = "Comments",
            };

            // act
            var response = await this.pedidoFacade.UpdateSaleOrders(orderId);

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success);
        }

        /// <summary>
        /// test tet.
        /// </summary>
        /// <returns>test.</returns>
        [Test]
        public async Task UpdateDesignerLabel()
        {
            // arrange
            var orderId = new UpdateDesignerLabelDto
            {
                Details = new List<UpdateDesignerLabelDetailDto>(),
                DesignerSignature = "text",
                UserId = "id",
            };

            // act
            var response = await this.pedidoFacade.UpdateDesignerLabel(orderId);

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success);
        }

        /// <summary>
        /// test tet.
        /// </summary>
        /// <returns>test.</returns>
        [Test]
        public async Task CreateSaleOrderPdf()
        {
            // arrange
            var orders = new List<CreateOrderPdfDto>();

            // act
            var response = await this.pedidoFacade.CreateSaleOrderPdf(orders);

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success);
        }

        /// <summary>
        /// test deleteFile.
        /// </summary>
        /// <returns>test.</returns>
        [Test]
        public async Task DeleteFiles()
        {
            // act
            var response = await this.pedidoFacade.DeleteFiles();

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success);
        }

        /// <summary>
        /// test test.
        /// </summary>
        /// <returns>returns nothing.</returns>
        [Test]
        public async Task SignOrdersByTecnic()
        {
            // act
            var response = await this.pedidoFacade.SignOrdersByTecnic(new FinishOrderDto());

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success);
            Assert.That(response.Response, Is.Not.Null);
            Assert.That(response.ExceptionMessage, Is.Empty);
            Assert.That(response.UserError, Is.Empty);
            Assert.That(response.Code.Equals(200));
        }

        /// <summary>
        /// test test.
        /// </summary>
        /// <returns>returns nothing.</returns>
        [Test]
        public async Task GetInvalidOrdersByMissingTecnicSign()
        {
            // act
            var response = await this.pedidoFacade.GetInvalidOrdersByMissingTecnicSign(new List<string>());

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success);
            Assert.That(response.Response, Is.Not.Null);
            Assert.That(response.ExceptionMessage, Is.Empty);
            Assert.That(response.UserError, Is.Empty);
            Assert.That(response.Code.Equals(200));
        }

        /// <summary>
        /// test test.
        /// </summary>
        /// <returns>returns nothing.</returns>
        [Test]
        public async Task GetUserOrdersByInvoiceId()
        {
            // act
            var response = await this.pedidoFacade.GetUserOrdersByInvoiceId(new List<int>(), string.Empty);

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success);
            Assert.That(response.Response, Is.Not.Null);
            Assert.That(response.ExceptionMessage, Is.Empty);
            Assert.That(response.UserError, Is.Empty);
            Assert.That(response.Code.Equals(200));
        }

        /// <summary>
        /// FinalizeProductionOrdersAsync.
        /// </summary>
        /// <returns>returns nothing.</returns>
        [Test]
        public async Task FinalizeProductionOrdersAsync()
        {
            // arrange
            var productionOrdersToFinalize = new List<FinalizeProductionOrderModel>();

            // act
            var response = await this.pedidoFacade.FinalizeProductionOrdersAsync(productionOrdersToFinalize);

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success);
            Assert.That(response.Response, Is.Not.Null);
            Assert.That(response.ExceptionMessage, Is.Empty);
            Assert.That(response.UserError, Is.Empty);
            Assert.That(response.Code.Equals(200));
        }

        /// <summary>
        /// FinalizeProductionOrdersOnSapAsync.
        /// </summary>
        /// <returns>returns nothing.</returns>
        [Test]
        public async Task FinalizeProductionOrdersOnSapAsync()
        {
            // arrange
            var productionOrderProcessingPayload = new ProductionOrderProcessingStatusDto();

            // act
            var response = await this.pedidoFacade.FinalizeProductionOrdersOnSapAsync(productionOrderProcessingPayload);

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success);
            Assert.That(response.Response, Is.Not.Null);
            Assert.That(response.ExceptionMessage, Is.Empty);
            Assert.That(response.UserError, Is.Empty);
            Assert.That(response.Code.Equals(200));
        }

        /// <summary>
        /// FinalizeProductionOrdersOnPostgresqlAsync.
        /// </summary>
        /// <returns>returns nothing.</returns>
        [Test]
        public async Task FinalizeProductionOrdersOnPostgresqlAsync()
        {
            // arrange
            var productionOrderProcessingPayload = new ProductionOrderProcessingStatusDto();

            // act
            var response = await this.pedidoFacade.FinalizeProductionOrdersOnPostgresqlAsync(productionOrderProcessingPayload);

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success);
            Assert.That(response.Response, Is.Not.Null);
            Assert.That(response.ExceptionMessage, Is.Empty);
            Assert.That(response.UserError, Is.Empty);
            Assert.That(response.Code.Equals(200));
        }

        /// <summary>
        /// ProductionOrderPdfGenerationAsync.
        /// </summary>
        /// <returns>returns nothing.</returns>
        [Test]
        public async Task ProductionOrderPdfGenerationAsync()
        {
            // arrange
            var productionOrderProcessingPayload = new ProductionOrderProcessingStatusDto();

            // act
            var response = await this.pedidoFacade.ProductionOrderPdfGenerationAsync(productionOrderProcessingPayload);

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success);
            Assert.That(response.Response, Is.Not.Null);
            Assert.That(response.ExceptionMessage, Is.Empty);
            Assert.That(response.UserError, Is.Empty);
            Assert.That(response.Code.Equals(200));
        }
    }
}
