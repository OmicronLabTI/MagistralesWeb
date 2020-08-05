// ------------------------------------------------------------------------------------------------
// <copyright file="ArrayInputAttribute.cs" company="Ann Inc">
//  Copyright (c) 2017 Ann Inc, All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace Omicron.SapDiApi.Api.Configuration
{
    using System.Linq;
    using System.Net.Http;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;

    /// <summary>
    /// ArrayInputAttribute to handle array input in GET
    /// </summary>
    /// <seealso cref="System.Web.Http.Filters.ActionFilterAttribute" />
    public class ArrayInputAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// The parameter name.
        /// </summary>
        private readonly string parameterName;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayInputAttribute"/> class.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        public ArrayInputAttribute(string parameterName)
        {
            this.parameterName = parameterName;
            this.Separator = ',';
        }

        /// <summary>
        /// Gets or sets the separator.
        /// </summary>
        public char Separator { get; set; }

        /// <summary>
        /// Occurs before the action method is invoked.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (!actionContext.ActionArguments.ContainsKey(this.parameterName))
            {
                return;
            }

            var parameters = string.Empty;

            if (actionContext.ControllerContext.RouteData.Values.ContainsKey(this.parameterName))
            {
                parameters = (string)actionContext.ControllerContext.RouteData.Values[this.parameterName];
            }
            else if (actionContext.ControllerContext.Request.RequestUri.ParseQueryString()[this.parameterName] != null)
            {
                parameters = actionContext.ControllerContext.Request.RequestUri.ParseQueryString()[this.parameterName];
            }

            actionContext.ActionArguments[this.parameterName] = parameters.Split(this.Separator);
        }
    }
}