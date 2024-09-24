// <summary>
// <copyright file="AlmacenServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Test.Services
{
    /// <summary>
    /// Class UsersServiceTest.
    /// </summary>
    [TestFixture]
    public class AlmacenServiceTest
    {
        /// <summary>
        /// Test for simple post.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task SimplePost()
        {
            // arrange
            var responses = new Dictionary<string, MockResponse>()
            {
                {
                    "salesOrder", new MockResponse
                    {
                        Json = JsonConvert.SerializeObject(new ResultModel()),
                        StatusCode = HttpStatusCode.OK,
                    }
                },
            };

            HttpClient clientMock = new HttpClient(new MockHttpMessageHandler(responses));
            clientMock.BaseAddress = new System.Uri("http://test.com/");
            var pedidoService = new AlmacenService(clientMock);

            // act
            var result = await pedidoService.GetAlmacenOrders("salesOrder");

            // assert
            ClassicAssert.IsNotNull(result);
        }

        /// <summary>
        /// Test for simple post.
        /// </summary>
        [Test]
        public void SimpleGetError()
        {
            // arrange
            var responses = new Dictionary<string, MockResponse>()
            {
                {
                    "salesOrder", new MockResponse
                    {
                        Json = JsonConvert.SerializeObject(new ResultModel()),
                        StatusCode = HttpStatusCode.BadRequest,
                    }
                },
            };

            HttpClient clientMock = new HttpClient(new MockHttpMessageHandler(responses));
            clientMock.BaseAddress = new System.Uri("http://test.com/");
            var pedidoService = new AlmacenService(clientMock);

            // act
            ClassicAssert.ThrowsAsync<CustomServiceException>(async () => await pedidoService.GetAlmacenOrders("salesOrder"));
        }

        /// <summary>
        /// Test for simple post.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task PostAlmacenOrders()
        {
            // arrange
            var responses = new Dictionary<string, MockResponse>()
            {
                {
                    "salesOrder", new MockResponse
                    {
                        Json = JsonConvert.SerializeObject(new ResultModel()),
                        StatusCode = HttpStatusCode.OK,
                    }
                },
            };

            HttpClient clientMock = new HttpClient(new MockHttpMessageHandler(responses));
            clientMock.BaseAddress = new System.Uri("http://test.com/");
            var pedidoService = new AlmacenService(clientMock);

            // act
            var result = await pedidoService.PostAlmacenOrders("salesOrder", new { });

            // assert
            ClassicAssert.IsNotNull(result);
        }
    }
}
