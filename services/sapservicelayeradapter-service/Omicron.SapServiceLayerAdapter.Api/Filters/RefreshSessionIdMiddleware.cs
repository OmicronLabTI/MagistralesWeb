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
                await this.semaphore.WaitAsync(cancellationToken);
                response = await base.SendAsync(request, cancellationToken);

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    var cookies = await this.serviceLayerAuth.RefreshSession();
                    AddNewSessionId(request, cookies);
                    response = await base.SendAsync(request, cancellationToken);
                }
            }
            finally
            {
                this.semaphore.Release();
            }

            return response;
        }

        private static void AddNewSessionId(HttpRequestMessage request, string cookies)
        {
            if (request.Headers.Contains("Cookie"))
            {
                request.Headers.Remove("Cookie");
            }

            request.Headers.Add("Cookie", cookies);
        }
    }
}