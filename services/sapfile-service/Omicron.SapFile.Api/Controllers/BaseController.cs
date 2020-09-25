// ------------------------------------------------------------------------------------------------
// <copyright file="BaseController.cs" company="Ann Inc">
//  Copyright (c) 2017 Ann Inc, All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace Omicron.SapFile.Api.Controllers
{
    using System;
    
    using System.Data.Entity.Validation;
    
    using System.Net.Http;
    using System.Text;
    
    using System.Web.Http;    
    
    using AutoMapper;    

    /// <summary>
    ///  Base class for any controller
    /// </summary>
    public abstract class BaseController : ApiController
    {
        /// <summary>
        /// Gets or sets Automapper instance for mappings beetwen types.
        /// </summary>
        protected IMapper Mapper { get; set; }

        /// <summary>
        /// Releases managed resources
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources</param>
        [NonAction]
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        /// <summary>
        /// Register an error/message to the log
        /// </summary>
        /// <param name="shortMessage">Brief description</param>
        /// <param name="ex">Exception instance</param>
        /// <param name="parameters">A string representing the parameters and values to be logged</param>
        [NonAction]
        protected void HandleError(string shortMessage, Exception ex, string parameters)
        {
            try
            {
                // Adds custom telemetry exception info
                var telemetry = new Microsoft.ApplicationInsights.TelemetryClient();
                System.Collections.Generic.Dictionary<string, string> properties = null;
                if (!string.IsNullOrEmpty(shortMessage) || !string.IsNullOrEmpty(parameters))
                {
                    properties = new System.Collections.Generic.Dictionary<string, string>();

                    if (!string.IsNullOrEmpty(shortMessage))
                    {
                        properties.Add("Message", shortMessage);
                    }

                    if (!string.IsNullOrEmpty(parameters))
                    {
                        properties.Add("Parameters", parameters);
                    }
                }

                telemetry.TrackException(ex, properties);
            }
            catch
            {
                // nothing to do
            }

            var responseError = Request.CreateErrorResponse(System.Net.HttpStatusCode.InternalServerError, ex != null ? ex.Message : string.Empty);
            throw new HttpResponseException(responseError);
        }

        /// <summary>
        /// Register an error/message to the log
        /// </summary>
        /// <param name="shortMessage">Brief description</param>
        /// <param name="ex">Exception instance</param>
        [NonAction]
        protected void HandleError(string shortMessage, Exception ex)
        {
            this.HandleError(shortMessage, ex, string.Empty);
        }
    }
}