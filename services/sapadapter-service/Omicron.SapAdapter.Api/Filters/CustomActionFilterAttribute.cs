// <summary>
// <copyright file="CustomActionFilterAttribute.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Api.Filters
{
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Routing;
    using Serilog;

    /// <summary>
    /// Class Action Filter.
    /// </summary>
    public class CustomActionFilterAttribute : ActionFilterAttribute
    {
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomActionFilterAttribute"/> class.
        /// </summary>
        /// <param name="logger">Object Logger.</param>
        public CustomActionFilterAttribute(ILogger logger)
        {
            this.logger = logger;
        }

        /// <inheritdoc/>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            this.Log("OnActionExecuting", context.RouteData);
        }

        /// <inheritdoc/>
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            this.Log("OnActionExecuted", context.RouteData);
        }

        /// <inheritdoc/>
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            this.Log("OnResultExecuting", context.RouteData);
        }

        /// <inheritdoc/>
        public override void OnResultExecuted(ResultExecutedContext context)
        {
            this.Log("OnResultExecuted", context.RouteData);
        }

        private void Log(string methodName, RouteData routeData)
        {
            var controllerName = routeData.Values["controller"];
            var actionName = routeData.Values["action"];

            var message = string.Format("{0} Controller: {1} Action:{2}", methodName, controllerName, actionName);
            this.logger.Information(message);
        }
    }
}
