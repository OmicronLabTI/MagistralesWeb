// <summary>
// <copyright file="Startup.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Api
{
    using System;
    using Elastic.Apm.NetCoreAll;
    using HealthChecks.UI.Client;
    using MediatR;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Diagnostics.HealthChecks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.ResponseCompression;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using Microsoft.Extensions.Hosting;
    using Microsoft.OpenApi.Models;
    using Omicron.SapAdapter.Api.Filters;
    using Omicron.SapAdapter.DependencyInjection;
    using Omicron.SapAdapter.Services.Almacen;
    using Omicron.SapAdapter.Services.Catalog;
    using Omicron.SapAdapter.Services.Doctors;
    using Omicron.SapAdapter.Services.Mediator.Handlers;
    using Omicron.SapAdapter.Services.Pedidos;
    using Omicron.SapAdapter.Services.ProccessPayments;
    using Omicron.SapAdapter.Services.User;
    using Prometheus;
    using Serilog;
    using StackExchange.Redis;

    /// <summary>
    /// Class Startup.
    /// </summary>
    public class Startup
    {
        private const string AXITYURL = "https://www.axity.com/";

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

            var healthBuilder = services.AddHealthChecks();

            healthBuilder.AddCheck(
                "HealthDbChecks",
                new SqlConnectionHealthCheck(this.Configuration.GetConnectionString("DatabaseContext")),
                HealthStatus.Unhealthy,
                new string[] { "healthdb" });

            Log.Logger = new LoggerConfiguration().MinimumLevel.Information()
                .WriteTo.Seq(this.Configuration["SeqUrl"])
                .CreateLogger();

            services.AddSingleton(Log.Logger);

            var mvcBuilder = services.AddMvc();
            mvcBuilder.AddMvcOptions(p => p.Filters.Add(new CustomActionFilterAttribute(Log.Logger)));
            mvcBuilder.AddMvcOptions(p => p.Filters.Add(new CustomExceptionFilterAttribute()));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Version = "v1",
                    Title = "Api Sap Adapter",
                    Description = "Api para información de Sap Adapter",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact
                    {
                        Name = "Axity",
                        Url = new System.Uri(AXITYURL),
                    },
                });

                c.OperationFilter<AddAuthorizationHeaderParameterOperationFilter>();
            });

            services.AddHttpClient("pedidos", c =>
            {
                c.BaseAddress = new Uri(this.Configuration["PedidoService"]);
            })
            .AddTypedClient<IPedidosService, PedidoService>();

            services.AddHttpClient("users", c =>
            {
                c.BaseAddress = new Uri(this.Configuration["UserService"]);
            })
            .AddTypedClient<IUsersService, UsersService>();

            services.AddHttpClient("almacen", c =>
            {
                c.BaseAddress = new Uri(this.Configuration["AlmacenService"]);
            })
            .AddTypedClient<IAlmacenService, AlmacenService>();

            services.AddHttpClient("catalogos", c =>
            {
                c.BaseAddress = new Uri(this.Configuration["CatalogService"]);
            })
            .AddTypedClient<ICatalogsService, CatalogsService>();

            services.AddHttpClient("proccespayments", c =>
            {
                c.BaseAddress = new Uri(this.Configuration["ProccessPaymentsService"]);
            })
            .AddTypedClient<IProccessPayments, ProccessPayments>();

            services.AddHttpClient("doctors", c =>
            {
                c.BaseAddress = new Uri(this.Configuration["DoctorsService"]);
            })
            .AddTypedClient<IDoctorService, DoctorService>();

            this.AddRedis(services, Log.Logger);
            this.AddCorsSvc(services);

            services.Configure<GzipCompressionProviderOptions>(options => options.Level = System.IO.Compression.CompressionLevel.Fastest);
            services.AddResponseCompression();

            services.AddMediatR(
                typeof(PaymentsByTransactionHandler),
                typeof(DoctorDeliveryAddressHandler));
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">Application Builder.</param>
        /// <param name="env">Hosting Environment.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsProduction())
            {
                app.UseAllElasticApm(this.Configuration);
            }

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
                c.SwaggerEndpoint($"{this.Configuration["SwaggerAddress"]}/swagger/v1/swagger.json", "Api SapAdapter");
                c.RoutePrefix = string.Empty;
            });
            app.UseResponseCompression();

            app.UseMetricServer();
            app.UseMiddleware<ResponseMiddleware>();

            //// URL For healthcheks http://localhost:5102/healthchecks-ui#/healthchecks.
            app.UseHealthChecks("/hc", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
            });

            app.UseRouting();
            app.UseCors("CorsPolicy");
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        /// <summary>
        /// Adds the cors SVC.
        /// </summary>
        /// <param name="services">The services.</param>
        private void AddCorsSvc(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(
                    "CorsPolicy",
                    builder => builder
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .SetIsOriginAllowed(host => true)
                    .AllowCredentials());
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
