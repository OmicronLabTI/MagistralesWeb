// ------------------------------------------------------------------------------------------------
// <copyright file="APIConfig.cs" company="Ann Inc">
//  Copyright (c) 2017 Ann Inc, All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace Omicron.SapDiApi.Api.Configuration
{    
    using System.Web.Http;
    using Omicron.SapDiApi.Api.Filters;
    using Unity;

    /// <summary>
    /// Global API configuration
    /// </summary>
    public static class ApiConfig
    {
        /// <summary>
        /// Initializes API Configuration
        /// </summary>
        /// <returns>Returns the configuration object</returns>
        public static HttpConfiguration Register()
        {
            // Web API configuration and services
            HttpConfiguration config = new HttpConfiguration();
            config.Formatters.JsonFormatter.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/v1/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional });

            // Web api IOC container
            var container = new UnityContainer();
            config.DependencyResolver = new UnityResolver(container);

            // sets the filter for validating models before any action controller is called
            config.Filters.Add(new ModelFilterAttribute());

            return config;
        }
    }
}