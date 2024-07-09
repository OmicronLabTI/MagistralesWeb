// <summary>
// <copyright file="AddB1SessionCookieMiddleware.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Api.Filters
{
    /// <summary>
    /// Middleware to add sessionId.
    /// </summary>
    public class AddB1SessionCookieMiddleware : DelegatingHandler
    {
        private readonly IServiceLayerAuth serviceLayerAuth;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddB1SessionCookieMiddleware"/> class.
        /// </summary>
        /// <param name="innerHandler">The handler.</param>
        /// <param name="serviceLayerAuth">The token handler.</param>
        public AddB1SessionCookieMiddleware(IServiceLayerAuth serviceLayerAuth)
        {
            this.serviceLayerAuth = serviceLayerAuth;
        }

        /// <inheritdoc/>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!request.Headers.Contains("Cookie"))
            {
                await this.AddCookie(request);
            }

            return await base.SendAsync(request, cancellationToken);
        }

        private async Task AddCookie(HttpRequestMessage request)
        {
            var cookies = await this.serviceLayerAuth.GetSessionIdAsync();
            request.Headers.Add("Cookie", cookies);
        }
    }
}