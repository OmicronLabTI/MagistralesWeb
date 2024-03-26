// <summary>
// <copyright file="RefreshSessionIdMiddleware.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Api.Filters
{
    /// <summary>
    /// Middleware to refresh sessionId.
    /// </summary>
    public class RefreshSessionIdMiddleware : DelegatingHandler
    {
        private readonly IServiceLayerAuth serviceLayerAuth;
        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        /// <summary>
        /// Initializes a new instance of the <see cref="RefreshSessionIdMiddleware"/> class.
        /// </summary>
        /// <param name="innerHandler">The handler.</param>
        /// <param name="serviceLayerAuth">The serviceLayerAuth.</param>
        public RefreshSessionIdMiddleware(IServiceLayerAuth serviceLayerAuth)
        {
            this.serviceLayerAuth = serviceLayerAuth;
        }

        /// <inheritdoc/>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpResponseMessage response = null;

            try
            {
                await this.semaphore.WaitAsync();
                response = await base.SendAsync(request, cancellationToken);

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    var sessionId = await this.serviceLayerAuth.RefreshSession();
                    var clonedRequest = await this.CloneRequest(request, sessionId);
                    response = await base.SendAsync(clonedRequest, cancellationToken);
                }
            }
            finally
            {
                this.semaphore.Release();
            }

            return response;
        }

        private async Task<HttpRequestMessage> CloneRequest(HttpRequestMessage request, string sessionId)
        {
            var clonedRequest = new HttpRequestMessage(request.Method, request.RequestUri);
            foreach (var header in request.Headers)
            {
                clonedRequest.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            if (clonedRequest.Headers.Contains("Cookie"))
            {
                clonedRequest.Headers.Remove("Cookie");
            }

            clonedRequest.Headers.Add("Cookie", $"B1SESSION={sessionId}; Path=/b1s/v1; Secure; HttpOnly;");

            if (request.Content != null)
            {
                var content = await request.Content.ReadAsByteArrayAsync();
                clonedRequest.Content = new ByteArrayContent(content);
                clonedRequest.Content.Headers.ContentType = request.Content.Headers.ContentType;
            }

            return clonedRequest;
        }
    }
}