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
            Assert.That(result.Response.ToString().Trim('"'), Is.EqualTo(fakeResponse));
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
            Assert.That(result.Response.ToString().Trim('"'), Is.EqualTo(fakeResponse));
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
            Assert.That(result.Response.ToString().Trim('"'), Is.EqualTo(fakeResponse));
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
            Assert.That(result.Response.ToString().Trim('"'), Is.EqualTo(fakeResponse));
        }

        /// <summary>
        /// Test DeleteAsync with valid data.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SendBatchAsync_SendAsyncThrows_ExceptionHandledAndReturns500()
        {
            // Arrange: SendAsync throws
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ThrowsAsync(new InvalidOperationException("network boom"));

            var client = new HttpClient(handlerMock.Object) { BaseAddress = new Uri("http://test/") };
            var serviceLayerClient = new ServiceLayerClient(client, this.loggerMock.Object);

            var ops = new List<BatchOperationDto>
            {
                new BatchOperationDto { ContentId = "1", Method = HttpMethod.Post, Url = "/X", Body = "{}" },
            };

            // Act
            var result = await serviceLayerClient.SendBatchAsync(ops);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ExceptionMessage, Does.Contain("network boom"));
            Assert.That(result.Code.Equals((int)HttpStatusCode.InternalServerError));
        }

        /// <summary>
        /// Test DeleteAsync with valid data.
        /// </summary>
        [Test]
        public void BuildChangesetContent_MultipleOperations_ContainsAllPartsAndCorrectContentLength()
        {
            // Arrange
            var ops = new List<BatchOperationDto>
            {
                new BatchOperationDto { ContentId = "10", Method = HttpMethod.Post, Url = "/Items", Body = new { ItemCode = "A1" } },
                new BatchOperationDto { ContentId = "11", Method = HttpMethod.Put, Url = "/Items(11)", Body = "{\"raw\":true}" },
            };

            // Act
            var content = this.InvokePrivateGeneric<string>(this.serviceLayerClient, "BuildChangesetContent", ops);

            // Assert: content includes both content-ids and both request lines
            Assert.That(content, Does.Contain("Content-ID: 10"));
            Assert.That(content, Does.Contain("Content-ID: 11"));
            Assert.That(content, Does.Contain("POST /Items HTTP/1.1"));
            Assert.That(content, Does.Contain("PUT /Items(11) HTTP/1.1"));

            // Ensure string-body was not double-serialized (no surrounding quotes)
            Assert.That(content, Does.Contain("\"raw\":true"));

            // Validate Content-Length lines correspond to the actual UTF8 byte length of the JSON body for first op
            var firstOpJson = JsonConvert.SerializeObject(new { ItemCode = "A1" });
            int actualLength = Encoding.UTF8.GetByteCount(firstOpJson);
            Assert.That(content, Does.Contain($"Content-Length: {actualLength}"));
        }

        /// <summary>
        /// Test DeleteAsync with valid data.
        /// </summary>
        [Test]
        public void BuildBatchContent_IncludesChangesetAndBatchBoundaries()
        {
            // Arrange
            string fakeChangeset = "--changesetX\r\nDATA\r\n--changesetX--";

            // Act
            var batch = this.InvokePrivateGeneric<string>(
                this.serviceLayerClient,
                "BuildBatchContent",
                fakeChangeset);

            // Assert: batch boundary correcto
            Assert.That(batch, Does.Contain("--batch_boundary"));

            // Assert: boundary declarado para el multipart
            Assert.That(batch, Does.Contain("Content-Type: multipart/mixed; boundary=changeset_"));

            // Assert: cambioset insertado
            Assert.That(batch, Does.Contain(fakeChangeset));
        }

        /// <summary>
        /// Test DeleteAsync with valid data.
        /// </summary>
        [Test]
        public void ParseResponseContent_NoBoundary_LogsAndReturnsEmpty()
        {
            // Arrange
            string resp = "--something\r\ninvalid";
            string contentType = "multipart/mixed"; // no boundary param

            // Act
            var parsed = this.InvokePrivateGeneric<List<ParsedBatchResultDto>>(this.serviceLayerClient, "ParseResponseContent", resp, contentType);

            // Assert
            Assert.That(!parsed.Any());
            this.loggerMock.Verify(l => l.Error(It.Is<string>(s => s.Contains("No se pudo encontrar el boundary"))), Times.Once);
        }

        /// <summary>
        /// Test DeleteAsync with valid data.
        /// </summary>
        [Test]
        public void TryToParseResponse_InvalidJson_ReturnsRawString()
        {
            // Arrange
            string raw = "not a json";

            // Act
            var parsed = this.InvokePrivateGeneric<string>(this.serviceLayerClient, "TryToParseResponse", raw);

            // Assert
            Assert.That(raw.Equals(parsed));
        }

        /// <summary>
        /// Test DeleteAsync with valid data.
        /// </summary>
        [Test]
        public void TryToParseResponse_ValidServiceLayerJson_ReturnsMessageValue()
        {
            // Arrange
            string errorJson = @"{ ""error"": { ""message"": { ""value"": ""This is the message"" } } }";

            // Act
            var parsed = this.InvokePrivateGeneric<string>(this.serviceLayerClient, "TryToParseResponse", errorJson);
            Assert.That(parsed.Equals("This is the message"));
        }

        /// <summary>
        /// Test DeleteAsync with valid data.
        /// </summary>
        [Test]
        public void ParseResponseContent_EmptyResponse_ReturnsEmptyList()
        {
            // Arrange
            string boundary = "batch_empty";
            string full = string.Empty; // empty response
            string contentType = $"multipart/mixed; boundary={boundary}";

            // Act
            var parsed = this.InvokePrivateGeneric<List<ParsedBatchResultDto>>(this.serviceLayerClient, "ParseResponseContent", full, contentType);

            // Assert
            Assert.That(parsed, Is.Not.Null);
            Assert.That(!parsed.Any());
        }

        /// <summary>
        /// Test DeleteAsync with valid data.
        /// </summary>
        [Test]
        public void ParseResponseContent_NoBoundary_ReturnsEmptyList()
        {
            var result = this.Invoke<List<ParsedBatchResultDto>>(
                "ParseResponseContent",
                "something",
                "multipart/mixed");

            Assert.That(result, Is.Empty);
        }

        /// <summary>
        /// Test DeleteAsync with valid data.
        /// </summary>
        [Test]
        public void ParseResponseContent_ValidBoundary_NoParts_ReturnsEmpty()
        {
            string body = "--batchX--";
            string contentType = "multipart/mixed; boundary=batchX";

            var result = this.Invoke<List<ParsedBatchResultDto>>(
                "ParseResponseContent",
                body,
                contentType);

            Assert.That(result, Is.Empty);
        }

        /// <summary>
        /// Test DeleteAsync with valid data.
        /// </summary>
        [Test]
        public void ParseResponseContent_InvalidPart_Ignored()
        {
            string boundary = "b1";
            string contentType = $"multipart/mixed; boundary={boundary}";

            string full =
    $@"--{boundary}
Content-Type: application/http

INVALID LINE WITHOUT HTTP/1.1
--{boundary}--";

            var result = this.Invoke<List<ParsedBatchResultDto>>(
                "ParseResponseContent",
                full,
                contentType);

            Assert.That(result.Count, Is.EqualTo(0));
        }

        /// <summary>
        /// Test DeleteAsync with valid data.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        private object InvokePrivate(ServiceLayerClient client, string methodName, params object[] args)
        {
            var method = client.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (method == null)
            {
                throw new InvalidOperationException($"Method {methodName} not found via reflection.");
            }

            return method.Invoke(client, args);
        }

        /// <summary>
        /// Test DeleteAsync with valid data.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        private T InvokePrivateGeneric<T>(ServiceLayerClient client, string methodName, params object[] args)
        {
            return (T)this.InvokePrivate(client, methodName, args);
        }

        private T Invoke<T>(string method, params object[] args)
        {
            return (T)typeof(ServiceLayerClient)
                .GetMethod(method, BindingFlags.NonPublic | BindingFlags.Instance)
                .Invoke(this.serviceLayerClient, args);
        }
    }
}
