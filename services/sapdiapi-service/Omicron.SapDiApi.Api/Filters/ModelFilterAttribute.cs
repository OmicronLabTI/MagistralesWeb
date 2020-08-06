// ------------------------------------------------------------------------------------------------
// <copyright file="ModelFilterAttribute.cs" company="Ann Inc">
//  Copyright (c) 2017 Ann Inc, All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace Omicron.SapDiApi.Api.Filters
{
    using System.Net;
    using System.Net.Http;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;

    /// <summary>
    /// Filter for model validation.
    /// </summary>
    public sealed class ModelFilterAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// It get executed before any action controller takes place.
        /// </summary>
        /// <param name="actionContext">the context for the current action</param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (!actionContext.ModelState.IsValid)
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, actionContext.ModelState);
            }
        }
    }
}