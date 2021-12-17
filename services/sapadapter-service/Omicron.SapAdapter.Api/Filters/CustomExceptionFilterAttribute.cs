// <summary>
// <copyright file="CustomExceptionFilterAttribute.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Api.Filters
{
    using System;
    using System.Net;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Hosting;
    using Omicron.LeadToCash.Resources.Exceptions;
    using Serilog;

    /// <summary>
    /// Class Exception Filter.
    /// </summary>
    public class CustomExceptionFilterAttribute : TypeFilterAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomExceptionFilterAttribute"/> class.
        /// </summary>
        public CustomExceptionFilterAttribute()
            : base(typeof(CustomException))
        {
        }

        private class CustomException : IExceptionFilter
        {
            private readonly ILogger logger;

            private readonly IHostApplicationLifetime lifetime;

            /// <summary>
            /// Initializes a new instance of the <see cref="CustomException"/> class.
            /// </summary>
            /// <param name="logger">The logger.</param>
            /// <param name="lifeTime">The lifeTime.</param>
            public CustomException(ILogger logger, IHostApplicationLifetime lifeTime)
            {
                this.logger = logger;
                this.lifetime = lifeTime;
            }

            public void OnException(ExceptionContext context)
            {
                string message = string.Empty;

                var exceptionType = context.Exception.GetType();
                HttpStatusCode status = HttpStatusCode.Conflict;
                if (exceptionType == typeof(CustomServiceException))
                {
                    var customException = (CustomServiceException)context.Exception;
                    message = customException.Message;
                    status = (int)customException.Status == 0 ? status : customException.Status;
                }
                else if (exceptionType == typeof(InvalidOperationException))
                {
                    var logMessageOp = $"ErrorType: {context.Exception.GetType()} Message: {context.Exception.Message}";
                    this.logger.Error(context.Exception, logMessageOp);
                    this.lifetime.StopApplication();
                }
                else
                {
                    message = context.Exception.Message;
                    status = HttpStatusCode.NotFound;
                }

                context.ExceptionHandled = true;

                var response = context.HttpContext.Response;
                response.StatusCode = (int)status;
                response.ContentType = "application/json";
                response.WriteAsync(message);

                var logMessage = $"ErrorType: {context.Exception.GetType()} Message: {context.Exception.Message}";
                this.logger.Error(context.Exception, logMessage);
            }
        }
    }
}
