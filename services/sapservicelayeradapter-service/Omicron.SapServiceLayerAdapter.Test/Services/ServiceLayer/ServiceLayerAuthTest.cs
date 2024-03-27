// <summary>
// <copyright file="ServiceLayerAuthTest.cs" company="Axity">
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
    public class ServiceLayerAuthTest : BaseTest
    {
        private ServiceLayerAuth serviceLayerAuth;
        private Mock<IConfiguration> config;
        private HttpClient httpClient;
        private Mock<ILogger> logger;

        /// <summary>
        /// Init configuration.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            this.config = new Mock<IConfiguration>();
            this.config.SetupGet(x => x[It.Is<string>(s => s == ServiceConstants.SAPServiceLayerUserEnvName)]).Returns("user");
            this.config.SetupGet(x => x[It.Is<string>(s => s == ServiceConstants.SAPServiceLayerPasswordEnvName)]).Returns("password");
            this.config.SetupGet(x => x[It.Is<string>(s => s == ServiceConstants.SAPServiceLayerDatabaseName)]).Returns("database");

            var result = new { SessionId = "sessionId" }; // Crear el objeto de resultado manualmente

            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(result)),
                })
                .Verifiable();

            this.httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("http://test.com/"),
            };

            this.logger = new Mock<ILogger>();

            this.serviceLayerAuth = new ServiceLayerAuth(this.httpClient, this.config.Object, this.logger.Object);
        }

        /// <summary>
        /// Action tests.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task ValidateGetSessionId()
        {
            var result = new { SessionId = "sessionId" }; // Crear el objeto de resultado manualmente

            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(result)),
                })
                .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("http://test.com/"),
            };

            this.logger = new Mock<ILogger>();
            var serviceLayerAuth = new ServiceLayerAuth(httpClient, this.config.Object, this.logger.Object);
            var sessionId = await serviceLayerAuth.GetSessionIdAsync();
            Assert.AreEqual("sessionId", sessionId);
        }

        /// <summary>
        /// Action tests.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task ValidateRefreshSession()
        {
            var result = new { SessionId = "sessionId" }; // Crear el objeto de resultado manualmente

            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(result)),
                })
                .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("http://test.com/"),
            };

            this.logger = new Mock<ILogger>();
            var serviceLayerAuth = new ServiceLayerAuth(httpClient, this.config.Object, this.logger.Object);
            var sessionRefresed = await serviceLayerAuth.RefreshSession();
            Assert.AreEqual("sessionId", sessionRefresed);
        }
    }
}
