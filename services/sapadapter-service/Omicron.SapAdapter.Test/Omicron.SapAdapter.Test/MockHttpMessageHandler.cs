// <summary>
// <copyright file="MockHttpMessageHandler.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Test
{
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Abstract Class Base Test.
    /// </summary>
    public class MockHttpMessageHandler : HttpMessageHandler
    {
        /// <summary>
        /// The responses.
        /// </summary>
        private Dictionary<string, MockResponse> responses;

        /// <summary>
        /// Initializes a new instance of the <see cref="MockHttpMessageHandler"/> class.
        /// </summary>
        /// <param name="responses">The responses.</param>
        public MockHttpMessageHandler(Dictionary<string, MockResponse> responses)
        {
            this.responses = responses != null ? responses : new Dictionary<string, MockResponse>();
        }

        /// <summary>
        /// send as an asynchronous operation.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;HttpResponseMessage&gt;.</returns>
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            string jsonResponse = string.Empty;
            HttpStatusCode statusCode = HttpStatusCode.OK;

            foreach (KeyValuePair<string, MockResponse> condition in this.responses)
            {
                if (request.RequestUri.AbsolutePath.Contains(condition.Key))
                {
                    jsonResponse = condition.Value.Json;
                    statusCode = condition.Value.StatusCode;
                    break;
                }
            }

            var responseMessage = new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(jsonResponse),
            };

            return await Task.FromResult(responseMessage);
        }
    }
}
