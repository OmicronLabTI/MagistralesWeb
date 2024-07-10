// <summary>
// <copyright file="ServiceLayerClientTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Test.Services.ServiceLayer
{
    /// <summary>
    /// The test.
    /// </summary>
    [TestFixture]
    public class ServiceLayerClientTest : BaseTest
    {
        private ServiceLayerClient serviceLayerClient;
        private HttpClient httpClientMock;
        private Mock<ILogger> loggerMock;

        /// <summary>
        /// Init configuration.
        /// </summary>
        [OneTimeSetUp]
        public void Setup()
        {
            var result = "Fake response";

            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync((HttpRequestMessage request, CancellationToken cancellationToken) =>
                {
                    // Aquí puedes acceder a la solicitud recibida y configurar la respuesta en consecuencia
                    var response = new HttpResponseMessage()
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(JsonConvert.SerializeObject(result)),
                        RequestMessage = request,
                    };

                    return response;
                })
                .Verifiable();

            this.httpClientMock = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("http://test.com/"),
            };

            // Configurar el mock de ILogger
            this.loggerMock = new Mock<ILogger>();

            // Crear la instancia de ServiceLayerClient con los mocks configurados
            this.serviceLayerClient = new ServiceLayerClient(this.httpClientMock, this.loggerMock.Object);
        }

        /// <summary>
        /// Action tests.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetAsyncWithValidResponseReturnsResponse()
        {
            // Arrange
            var fakeResponse = "Fake response";
            var url = "https://example.com";

            // Act
            var result = await this.serviceLayerClient.GetAsync(url);

            // Assert
            Assert.AreEqual(fakeResponse, result.Response.ToString().Trim('"'));
        }

        /// <summary>
        /// Test Post async with valid data.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task PostAsyncWithValidResponseReturnsResponse()
        {
            // Arrange
            var fakeResponse = "Fake response";
            var url = "https://example.com";
            var requestBody = "{\"key\": \"value\"}";

            // Act
            var result = await this.serviceLayerClient.PostAsync(url, requestBody);

            // Assert
            Assert.AreEqual(fakeResponse, result.Response.ToString().Trim('"'));
        }

        /// <summary>
        /// Test Patch async with valid data.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task PatchAsyncWithValidResponseReturnsResponse()
        {
            // Arrange
            var fakeResponse = "Fake response";
            var url = "https://example.com";
            var requestBody = "{\"key\": \"value\"}";

            // Act
            var result = await this.serviceLayerClient.PatchAsync(url, requestBody);

            // Assert
            Assert.AreEqual(fakeResponse, result.Response.ToString().Trim('"'));
        }

        /// <summary>
        /// Test DeleteAsync with valid data.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task DeleteAsyncWithValidResponseReturnsResponse()
        {
            // Arrange
            var fakeResponse = "Fake response";
            var url = "https://example.com";

            // Act
            var result = await this.serviceLayerClient.DeleteAsync(url);

            // Assert
            Assert.AreEqual(fakeResponse, result.Response.ToString().Trim('"'));
        }
    }
}
