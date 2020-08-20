// <summary>
// <copyright file="CustomExceptionFilterAttribute.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Api.Filters
{
    using System.Net;
    using Omicron.LeadToCash.Resources.Exceptions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Serilog;

    /// <summary>
    /// Class Exception Filter.
    /// </summary>
    public class CustomExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomExceptionFilterAttribute"/> class.
        /// </summary>
        /// <param name="logger">Object Logger.</param>
        public CustomExceptionFilterAttribute(ILogger logger)
        {
            this.logger = logger;
        }

        /// <inheritdoc/>
        public override void OnException(ExceptionContext context)
        {
            HttpStatusCode status = HttpStatusCode.OK;
            string message = string.Empty;

            var exceptionType = context.Exception.GetType();
            if (exceptionType == typeof(CustomServiceException))
            {
                var customException = (CustomServiceException)context.Exception;
                message = customException.Message ?? "Error genérico";
                status = customException.Status;
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
            this.logger.Error(logMessage);
        }
    }
}
