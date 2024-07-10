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
        private readonly Serilog.ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RefreshSessionIdMiddleware"/> class.
        /// </summary>
        /// <param name="serviceLayerAuth">The serviceLayerAuth.</param>
        /// <param name="logger">The logger.</param>
        public RefreshSessionIdMiddleware(IServiceLayerAuth serviceLayerAuth, Serilog.ILogger logger)
        {
            this.serviceLayerAuth = serviceLayerAuth;
            this.logger = logger;
        }

        /// <inheritdoc/>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpResponseMessage response = null;
            var uuid = Guid.NewGuid().ToString("D");
            try
            {
                await this.semaphore.WaitAsync(cancellationToken);
                response = await base.SendAsync(request, cancellationToken);

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    request.Headers.TryGetValues("Cookie", out var token);
                    this.logger.Error($"SAP Service Layer Refresh {uuid} - Cookie a refrescar: {JsonConvert.SerializeObject(token)}");
                    var cookies = await this.serviceLayerAuth.RefreshSession();
                    var clonedRequest = await CloneHttpRequestMessageAsync(request);
                    AddNewSessionId(clonedRequest, cookies);
                    this.logger.Error($"SAP Service Layer Refresh {uuid} - {JsonConvert.SerializeObject(clonedRequest)}");
                    this.logger.Error($"SAP Service Layer Refresh {uuid} - Cookie nuevo: {cookies}");
                    response = await base.SendAsync(clonedRequest, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                this.logger.Error($"SAP Service Layer Refresh {uuid} RefreshSessionIdError: {ex.Message} - {ex.StackTrace}");
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

        private static async Task<HttpRequestMessage> CloneHttpRequestMessageAsync(HttpRequestMessage request)
        {
            var clone = new HttpRequestMessage(request.Method, request.RequestUri)
            {
                Content = request.Content,
                Version = request.Version,
            };

            // Copiar los encabezados
            foreach (var header in request.Headers)
            {
                var value = header.Value;
                clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            // Copiar los encabezados de contenido
            if (request.Content != null)
            {
                clone.Content = new StreamContent(await request.Content.ReadAsStreamAsync());
                foreach (var contentHeader in request.Content.Headers)
                {
                    clone.Content.Headers.TryAddWithoutValidation(contentHeader.Key, contentHeader.Value);
                }
            }

            return clone;
        }
    }
}