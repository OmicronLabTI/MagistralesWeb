// <summary>
// <copyright file="FacadeTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Test.Facade
{
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

            var mockSapServices = new Mock<ISapService>();
            var mockComponentes = new Mock<IComponentsService>();
            var mockSapdxpService = new Mock<ISapDxpService>();

            var response = new ResultModel
            {
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = true,
                Success = true,
                UserError = string.Empty,
            };

            mockSapServices.SetReturnsDefault(Task.FromResult(response));
            mockComponentes.SetReturnsDefault(Task.FromResult(response));
            mockSapdxpService.SetReturnsDefault(Task.FromResult(response));

            this.sapFacade = new SapFacade(mockSapServices.Object, this.mapper, mockComponentes.Object, mockSapdxpService.Object);
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
            var response = await this.sapFacade.GetOrders(dict);

            // assert
            this.AssertResponse(response);
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

            // assert
            this.AssertResponse(response);
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

            // assert
            this.AssertResponse(response);
        }

        /// <summary>
        /// test tet.
        /// </summary>
        /// <returns>test.</returns>
        [Test]
        public async Task GetPedidoWithDetailAndDxp()
        {
            // arrange
            var listDocs = new List<int> { 1, 2, 3 };

            // act
            var response = await this.sapFacade.GetPedidoWithDetailAndDxp(listDocs);

            // assert
            this.AssertResponse(response);
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

            // assert
            this.AssertResponse(response);
        }

        /// <summary>
        /// test tet.
        /// </summary>
        /// <returns>test.</returns>
        [Test]
        public async Task GetAsesorsByOrderId()
        {
            // arrange
            var salesOrders = new List<int>
            {
                12,
                13,
            };

            // act
            var response = await this.sapFacade.GetAsesorsByOrderId(salesOrders);

            // assert
            this.AssertResponse(response);
        }

        /// <summary>
        /// test tet.
        /// </summary>
        /// <returns>test.</returns>
        [Test]
        public async Task GetOrderFormula()
        {
            // arrange
            var ordenId = new List<int> { 1 };

            // act
            var response = await this.sapFacade.GetOrderFormula(ordenId, true, true);

            // assert
            this.AssertResponse(response);
        }

        /// <summary>
        /// test tet.
        /// </summary>
        /// <returns>test.</returns>
        [Test]
        public async Task GetValidationQuatitiesOrdersFormula()
        {
            // arrange
            var orderIds = new List<int> { 1 };

            // act
            var response = await this.sapFacade.GetValidationQuatitiesOrdersFormula(orderIds);

            // assert
            this.AssertResponse(response);
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

            // assert
            this.AssertResponse(response);
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

            // assert
            this.AssertResponse(response);
        }

        /// <summary>
        /// Test GetBatchesComponentsByItemCodeAndWarehouses.
        /// </summary>
        /// <returns>test.</returns>
        [Test]
        public async Task GetBatchesComponentsByItemCodeAndWarehouses()
        {
            // arrange
            var parameters = new Dictionary<string, string>();

            // act
            var response = await this.sapFacade.GetBatchesComponentsByItemCodeAndWarehouses(parameters);

            // assert
            this.AssertResponse(response);
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

            // assert
            this.AssertResponse(response);
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

            // assert
            this.AssertResponse(response);
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

            // assert
            this.AssertResponse(response);
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

            // assert
            this.AssertResponse(response);
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

            // assert
            this.AssertResponse(response);
        }

        /// <summary>
        /// test tet.
        /// </summary>
        /// <returns>test.</returns>
        [Test]
        public async Task ValidateIfExistsBatchCodeByItemCode()
        {
            // act
            var response = await this.sapFacade.ValidateIfExistsBatchCodeByItemCode(string.Empty, string.Empty);

            // assert
            this.AssertResponse(response);
        }

        /// <summary>
        /// test tet.
        /// </summary>
        /// <returns>test.</returns>
        [Test]
        public async Task GetFormulaBySalesOrdersOrProductionOrders()
        {
            // act
            var response = await this.sapFacade.GetFabricationOrdersByCriterial(new List<int>(), new List<int>(), true);

            // assert
            this.AssertResponse(response);
        }

        /// <summary>
        /// Gets the recipe.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetRecipe()
        {
            // arrange
            var orderId = 1;

            // act
            var response = await this.sapFacade.GetRecipe(orderId);

            // assert
            this.AssertResponse(response);
        }

        /// <summary>
        /// Gets the recipe.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetRecipes()
        {
            // arrange
            var orderId = new List<int>();

            // act
            var response = await this.sapFacade.GetRecipes(orderId);

            // assert
            this.AssertResponse(response);
        }

        /// <summary>
        /// Gets the recipe.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task ValidateOrder()
        {
            // arrange
            var orderId = new List<int>();

            // act
            var response = await this.sapFacade.ValidateOrder(orderId);

            // assert
            this.AssertResponse(response);
        }

        /// <summary>
        /// Gets the recipe.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetDetails()
        {
            // arrange
            var orderId = new Dictionary<string, string>();

            // act
            var response = await this.sapFacade.GetDetails(orderId, "ped");

            // assert
            this.AssertResponse(response);
        }

        /// <summary>
        /// Gets the recipe.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetMostCommonComponents()
        {
            // act
            var response = await this.sapFacade.GetMostCommonComponents(new Dictionary<string, string>());

            // assert
            this.AssertResponse(response);
        }

        /// <summary>
        /// Get packing required for order in assigned status.
        /// </summary>
        /// <returns>the detail.</returns>
        [Test]
        public async Task GetPackingRequiredForOrderInAssignedStatus()
        {
            // Arrange
            var userId = "abc-123";

            // act
            var response = await this.sapFacade.GetPackingRequiredForOrderInAssignedStatus(userId);

            // assert
            this.AssertResponse(response);
        }

        /// <summary>
        /// Get possible orders active to dxp project.
        /// </summary>
        /// <returns>the detail.</returns>
        [Test]
        public async Task GetOrdersActive()
        {
            // Arrange
            var ordersId = new List<int>();

            // act
            var response = await this.sapFacade.GetOrdersActive(ordersId);

            // assert
            this.AssertResponse(response);
        }

        /// <summary>
        /// Get possible orders active to dxp project.
        /// </summary>
        /// <returns>the detail.</returns>
        [Test]
        public async Task GetRawMaterialRequest()
        {
            // act
            var response = await this.sapFacade.GetRawMaterialRequest(new Dictionary<string, string>());

            // assert
            this.AssertResponse(response);
        }

        /// <summary>
        /// Get possible orders active to dxp project.
        /// </summary>
        /// <returns>the detail.</returns>
        [Test]
        public async Task GetOrderInformationByTransaction()
        {
            // act
            var response = await this.sapFacade.GetOrderInformationByTransaction(new Dictionary<string, string>());

            // assert
            this.AssertResponse(response);
        }

        /// <summary>
        /// Gets the possible active orders for the dxp project.
        /// </summary>
        /// <returns> The detail of the available warehouses. </returns>
        [Test]
        public async Task GetWarehouses()
        {
            // act
            var response = await this.sapFacade.GetWarehouses(new List<string>());

            // assert
            this.AssertResponse(response);
        }

        /// <summary>
        /// Gets the classification data based on the provided parameters.
        /// </summary>
        /// <returns> containing the classification data. </returns>
        [Test]
        public async Task GetClassificationsByDescription()
        {
            // act
            var response = await this.sapFacade.GetClassificationsByDescription(new List<string>());

            // assert
            this.AssertResponse(response);
        }

        /// <summary>
        /// Gets the possible active orders for the dxp project.
        /// </summary>
        /// <returns> The detail of the available warehouses. </returns>
        [Test]
        public async Task GetUnitProducts()
        {
            // act
            var response = await this.sapFacade.GetUnitProducts(new List<string>());

            // assert
            this.AssertResponse(response);
        }

        /// <summary>
        /// Assert response.
        /// </summary>
        /// <param name="response">Response to validate.</param>
        public void AssertResponse(ResultDto response)
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success);
            Assert.That(response.Response, Is.Not.Null);
            Assert.That(response.ExceptionMessage, Is.Empty);
            Assert.That(response.UserError, Is.Empty);
            Assert.That(response.Code.Equals(200));
        }
    }
}
