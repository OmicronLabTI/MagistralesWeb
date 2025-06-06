// <summary>
// <copyright file="CatalogosServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Usuarios.Test.Services
{
    /// <summary>
    /// Class UsersServiceTest.
    /// </summary>
    [TestFixture]
    public class CatalogosServiceTest
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
                    ServiceConstants.GetClassifications, new MockResponse
                    {
                        Json = JsonConvert.SerializeObject(new ResultModel()),
                        StatusCode = HttpStatusCode.OK,
                    }
                },
            };

            HttpClient clientMock = new HttpClient(new MockHttpMessageHandler(responses));
            clientMock.BaseAddress = new System.Uri("http://test.com/");
            var catalogosService = new CatalogosService(clientMock);

            // act
            var result = await catalogosService.GetCatalogos(ServiceConstants.GetClassifications);

            // assert
            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// Test for simple post.
        /// </summary>
        [Test]
        public void SimplePostError()
        {
            // arrange
            var responses = new Dictionary<string, MockResponse>()
            {
                {
                    ServiceConstants.GetClassifications, new MockResponse
                    {
                        Json = JsonConvert.SerializeObject(new ResultModel()),
                        StatusCode = HttpStatusCode.BadRequest,
                    }
                },
            };

            HttpClient clientMock = new HttpClient(new MockHttpMessageHandler(responses));
            clientMock.BaseAddress = new System.Uri("http://test.com/");
            var catalogosService = new CatalogosService(clientMock);

            // act
            Assert.ThrowsAsync<CustomServiceException>(async () => await catalogosService.GetCatalogos(ServiceConstants.GetClassifications));
        }
    }
}
