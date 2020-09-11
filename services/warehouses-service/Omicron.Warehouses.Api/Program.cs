// <summary>
// <copyright file="Program.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Warehouses.Api
{
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Steeltoe.Common.Hosting;
    using Steeltoe.Extensions.Configuration.PlaceholderCore;

    /// <summary>
    /// Class Program.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Main Program.
        /// </summary>
        /// <param name="args">List of arguments.</param>
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// Method to execute app.
        /// </summary>
        /// <param name="args">List of arguments.</param>
        /// <returns>Web Host Builder.</returns>
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .UseCloudHosting(5104)
            .AddPlaceholderResolver()
            .UseStartup<Startup>();
    }
}
