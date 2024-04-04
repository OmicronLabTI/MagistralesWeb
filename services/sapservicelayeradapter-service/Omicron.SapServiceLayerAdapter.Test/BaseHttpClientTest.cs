// <summary>
// <copyright file="BaseHttpClientTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Test
{
    /// <summary>
    /// Base class for http clients.
    /// </summary>
    /// <typeparam name="T">Client type.</typeparam>
    public abstract class BaseHttpClientTest<T>
        where T : class
    {
        /// <summary>
        /// Create a new client.
        /// </summary>
        /// <returns>Client.</returns>
        public T CreateClient()
        {
            var fixture = new Fixture();
            var result = fixture.Create<ResultModel>();
            result.Success = true;

            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(result)),
               })
               .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("http://test.com/"),
            };

            var mockLog = new Mock<ILogger>();

            mockLog
                .Setup(m => m.Information(It.IsAny<string>()));

            return (T)Activator.CreateInstance(typeof(T), new object[] { httpClient, mockLog.Object });
        }

        /// <summary>
        /// Create a new client.
        /// </summary>
        /// <returns>Client.</returns>
        public T CreateClientFailure()
        {
            var fixture = new Fixture();
            var result = fixture.Create<ResultModel>();
            result.Success = true;

            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.BadRequest,
                   Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(result)),
               })
               .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("http://test.com/"),
            };

            var mockLog = new Mock<ILogger>();

            mockLog
                .Setup(m => m.Information(It.IsAny<string>()));

            return (T)Activator.CreateInstance(typeof(T), new object[] { httpClient, mockLog.Object });
        }

        /// <summary>
        /// Create a new client.
        /// </summary>
        /// <returns>Client.</returns>
        public T CreateClientWithErrorResponse()
        {
            var fixture = new Fixture();
            var result = fixture.Create<ResultModel>();
            result.Success = false;
            result.UserError = "Error controlado";

            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(result)),
               })
               .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("http://test.com/"),
            };

            var mockLog = new Mock<ILogger>();

            mockLog
                .Setup(m => m.Information(It.IsAny<string>()));

            return (T)Activator.CreateInstance(typeof(T), new object[] { httpClient, mockLog.Object });
        }
    }
}
