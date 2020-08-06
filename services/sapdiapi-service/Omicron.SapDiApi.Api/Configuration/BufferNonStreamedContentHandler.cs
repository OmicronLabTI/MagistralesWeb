// ------------------------------------------------------------------------------------------------
// <copyright file="BufferNonStreamedContentHandler.cs" company="Ann Inc">
//  Copyright (c) 2017 Ann Inc, All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace Omicron.SapDiApi.Api.Configuration
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http.Hosting;

    /// <summary>
    /// BufferNonStreamedContentHandler class
    /// </summary>
    public class BufferNonStreamedContentHandler : DelegatingHandler
    {
        /// <summary>
        /// SendAsync method
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken"> The cancelation token.</param>
        /// <returns>The response.</returns>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);
            if (response.Content != null)
            {
                var services = request.GetConfiguration().Services;
                var bufferPolicy = (IHostBufferPolicySelector)services.GetService(typeof(IHostBufferPolicySelector));

                // If the host is going to buffer it anyway
                if (bufferPolicy.UseBufferedOutputStream(response))
                {
                    // Buffer it now so we can catch the exception
                    await response.Content.LoadIntoBufferAsync();
                }
            }

            return response;
        }
    }
}