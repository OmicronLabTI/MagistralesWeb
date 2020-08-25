// <summary>
// <copyright file="CustomExceptionFilterAttribute.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Api.Filters
{
    using System.IO;
    using System.Net;
    using System.Text;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Omicron.LeadToCash.Resources.Exceptions;
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
            MemoryStream stream = new MemoryStream();

            var exceptionType = context.Exception.GetType();
            if (exceptionType == typeof(CustomServiceException))
            {
                var customException = (CustomServiceException)context.Exception;
                message = customException.Message ?? "Error genérico";
                status = customException.Status;
                var body = customException.ResponseBody;

                byte[] byteArray = Encoding.UTF8.GetBytes(body.ToString());
                stream = new MemoryStream(byteArray);
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
            response.Body = stream;

            var logMessage = $"ErrorType: {context.Exception.GetType()} Message: {context.Exception.Message}";
            this.logger.Error(logMessage);
        }
    }
}
