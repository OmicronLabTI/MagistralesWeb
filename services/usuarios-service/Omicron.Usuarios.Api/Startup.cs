// <summary>
// <copyright file="Startup.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Usuarios.Api
{
    using System;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.ResponseCompression;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.OpenApi.Models;
    using Omicron.Usuarios.Api.Filters;
    using Omicron.Usuarios.DependencyInjection;
    using Omicron.Usuarios.Services.Pedidos;
    using Omicron.Usuarios.Services.SapAdapter;
    using Prometheus;
    using Serilog;
    using Serilog.Events;
    using StackExchange.Redis;
    using Steeltoe.Common.Http.Discovery;
    using Steeltoe.Discovery.Client;

    /// <summary>
    /// Class Startup.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">App Configuration.</param>
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="Startup"/> class.
        /// </summary>
        ~Startup()
        {
            Log.CloseAndFlush();
        }

        /// <summary>
        /// Gets configuration.
        /// </summary>
        /// <value>
        /// App Settings Configuration.
        /// </value>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Method to configure services.
        /// </summary>
        /// <param name="services">Service Collection.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Latest);

            DependencyInjector.RegisterServices(services);
            DependencyInjector.AddAutoMapper();
            DependencyInjector.AddDbContext(this.Configuration);

            Log.Logger = new LoggerConfiguration().MinimumLevel.Information()
                .WriteTo.Seq(this.Configuration["SeqUrl"])
                .CreateLogger();

            ILoggerFactory loggerFactory = new LoggerFactory();
            loggerFactory.AddSerilog();
            services.AddSingleton(loggerFactory);

            services.AddDiscoveryClient(this.Configuration);

            var mvcBuilder = services.AddMvc();
            mvcBuilder.AddMvcOptions(p => p.Filters.Add(new CustomActionFilterAttribute(Log.Logger)));
            mvcBuilder.AddMvcOptions(p => p.Filters.Add(new CustomExceptionFilterAttribute(Log.Logger)));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Version = "v1",
                    Title = "Api Users",
                    Description = "Api para información de usuarios",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact
                    {
                        Name = "Axity",
                        Url = new System.Uri(this.Configuration["AxityUrl"]),
                    },
                });

                c.OperationFilter<AddAuthorizationHeaderParameterOperationFilter>();
            });

            services.AddHttpClient("pedidoservice", c =>
            {
                c.BaseAddress = new Uri(this.Configuration["PedidosUrl"]);
            })
            .AddHttpMessageHandler<DiscoveryHttpMessageHandler>()
            .AddTypedClient<IPedidosService, PedidosService>();

            services.AddHttpClient("sapadapterservice", c =>
            {
                c.BaseAddress = new Uri(this.Configuration["SapAdapterUrl"]);
            })
            .AddHttpMessageHandler<DiscoveryHttpMessageHandler>()
            .AddTypedClient<ISapAdapter, SapAdapter>();

            this.AddRedis(services, Log.Logger);

            services.Configure<GzipCompressionProviderOptions>(options => options.Level = System.IO.Compression.CompressionLevel.Fastest);
            services.AddResponseCompression();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">Application Builder.</param>
        /// <param name="env">Hosting Environment.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger(c =>
            {
                var basepath = this.Configuration["SwaggerAddress"];

                c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
                {
                    var paths = new OpenApiPaths();
                    foreach (var path in swaggerDoc.Paths)
                    {
                        paths.Add(basepath + path.Key, path.Value);
                    }

                    swaggerDoc.Paths = paths;
                });
            });

            app.UseStaticFiles();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"{this.Configuration["SwaggerAddress"]}/swagger/v1/swagger.json", "Api Users");
                c.RoutePrefix = string.Empty;
            });
            app.UseResponseCompression();

            app.UseDiscoveryClient();
            app.UseMetricServer();
            app.UseMiddleware<ResponseMiddleware>();

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        /// <summary>
        /// Add configuration Redis.
        /// </summary>
        /// <param name="services">Service Collection.</param>
        /// <param name="logger">The logger.</param>
        private void AddRedis(IServiceCollection services, Serilog.ILogger logger)
        {
            try
            {
                var configuration = ConfigurationOptions.Parse(this.Configuration["redis:hostname"], true);
                configuration.ResolveDns = true;

                ConnectionMultiplexer cm = ConnectionMultiplexer.Connect(configuration);
                services.AddSingleton<IConnectionMultiplexer>(cm);
            }
            catch (Exception)
            {
                logger.Error("No se econtro Redis");
            }
        }
    }
}
