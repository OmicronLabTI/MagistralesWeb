// <summary>
// <copyright file="ResponseMiddleware.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Logs.Api.Filters
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Prometheus;

    /// <summary>
    /// Class Response Middlewere.
    /// </summary>
    public class ResponseMiddleware
    {
        private readonly RequestDelegate next;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseMiddleware"/> class.
        /// </summary>
        /// <param name="next">Request Delegate.</param>
        public ResponseMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        /// <summary>
        /// Method to generate histogram response time for request.
        /// </summary>
        /// <param name="context">Http Context.</param>
        /// <returns>Task.</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value;
            var method = context.Request.Method;

            var counter = Metrics.CreateCounter("api_request_total", "HTTP Requests Total", new CounterConfiguration
            {
                LabelNames = new[] { "path", "method", "status" },
            });

            try
            {
                var sw = Stopwatch.StartNew();
                await this.next.Invoke(context);
                sw.Stop();

                var histogram =
                    Metrics
                        .CreateHistogram(
                            "api_response_time_seconds",
                            "API Response Time in seconds",
                            new[] { "path", "method" });

                histogram
                    .WithLabels(context.Request.Method, context.Request.Path)
                    .Observe(sw.Elapsed.TotalSeconds);
            }
            catch (Exception)
            {
                counter.Labels(path, method, "500").Inc();

                throw;
            }

            if (path != "/metrics")
            {
                counter.Labels(path, method, context.Response.StatusCode.ToString()).Inc();
            }
        }
    }
}
