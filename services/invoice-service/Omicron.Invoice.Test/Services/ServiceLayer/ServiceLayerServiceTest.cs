// <summary>
// <copyright file="ServiceLayerServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Invoice.Test.Services.ServiceLayer
{
    /// <summary>
    /// The test.
    /// </summary>
    [TestFixture]
    public class ServiceLayerServiceTest : BaseTest
    {
        private SapServiceLayerAdapterService serviceLayerClient;
        private HttpClient httpClientMock;
        private Mock<ILogger> loggerMock;

        /// <summary>
        /// Init configuration.
        /// </summary>
        [OneTimeSetUp]
        public void Setup()
        {
            var result = new ResultDto
            {
                Code = 200,
                Comments = null,
                UserError = null,
                ExceptionMessage = null,
                Response = "Result",
                Success = true,
            };

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

            // Crear la instancia de SapServiceLayerAdapterService con los mocks configurados
            this.serviceLayerClient = new SapServiceLayerAdapterService(this.httpClientMock, this.loggerMock.Object);
        }

        /// <summary>
        /// Test Post async with valid data.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task PostAsyncWithValidResponseReturnsResponse()
        {
            // Arrange
            var fakeResponse = new ResultDto();
            var url = "https://example.com";
            var requestBody = "{\"key\": \"value\"}";

            // Act
            var result = await this.serviceLayerClient.PostAsync(url, requestBody);

            // Assert
            Assert.That(result.Success);
        }
    }
}