// ------------------------------------------------------------------------------------------------
// <copyright file="Startup.cs" company="Ann Inc">
//  Copyright (c) 2017 Ann Inc, All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

[assembly: Microsoft.Owin.OwinStartup(typeof(Omicron.SapFile.Api.Startup))]

namespace Omicron.SapFile.Api
{
    using System.Web.Http;
    using Owin;

    /// <summary>
    /// Starts the Owin host
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Gets The http configuration object for Owin
        /// </summary>
        public static HttpConfiguration HttpConfiguration { get; private set; }

        /// <summary>
        /// Sets the configuration
        /// </summary>
        /// <param name="app">The http configuration</param>
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration = this.GetConfiguration();
            app.UseWebApi(HttpConfiguration);

            // sets the help page for the Web Api
            System.Web.Mvc.AreaRegistration.RegisterAllAreas();
        }

        /// <summary>
        /// Get configuration
        /// </summary>
        /// <returns>Returns the htt configuration</returns>
        public virtual System.Web.Http.HttpConfiguration GetConfiguration()
        {
            return Omicron.SapFile.Api.Configuration.ApiConfig.Register();
        }
    }
}