// <summary>
// <copyright file="Startup.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Api
{
    using Omicron.Pedidos.Api.Consumers;
    using Omicron.Pedidos.Services.MediatR.Handlers;
    using Omicron.Pedidos.Services.MediatR.Services;

    /// <summary>
    /// Class Startup.
    /// </summary>
    public static class Startup
    {
        /// <summary>
        /// Config application.
        /// </summary>
        /// <param name="webApplication">WebApplicationBuilder.</param>
        /// <returns>WebApplication.</returns>
        public static WebApplication AppConfiguration(this WebApplicationBuilder webApplication)
        {
            DependencyInjector.RegisterServices(webApplication.Services);
            DependencyInjector.AddAutoMapper();
            DependencyInjector.AddDbContext(webApplication.Configuration);
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            Log.Logger = new LoggerConfiguration().MinimumLevel.Information()
                .WriteTo.Seq(webApplication.Configuration["SeqUrl"])
                .CreateLogger();

            webApplication.Services.AddSingleton(Log.Logger);

            var mvcBuilder = webApplication.Services.AddMvc();
            mvcBuilder.AddMvcOptions(p => p.Filters.Add(new CustomActionFilterAttribute(Log.Logger)));
            mvcBuilder.AddMvcOptions(p => p.Filters.Add(new CustomExceptionFilterAttribute(Log.Logger)));

            webApplication.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Api Pedidos",
                    Description = "Api para información de pedidos",
                    Contact = new OpenApiContact
                    {
                        Name = "Axity",
                        Url = new Uri(webApplication.Configuration["AXITYURL"]),
                    },
                });

                c.OperationFilter<AddAuthorizationHeaderParameterOperationFilter>();
            });

            var sapDiApiUrl = webApplication.Configuration["DiApiAddress"];
            webApplication.Services.AddHttpClient("sapdiapi", c =>
            {
                c.BaseAddress = new Uri(sapDiApiUrl);
            })
            .AddTypedClient<ISapDiApi, SapDiApi>();

            webApplication.Services.AddHttpClient("sapadapter", c =>
            {
                c.BaseAddress = new Uri(webApplication.Configuration["SapAdapterUrl"]);
            })
            .AddTypedClient<ISapAdapter, SapAdapter>();

            webApplication.Services.AddHttpClient("usuariosservice", c =>
            {
                c.BaseAddress = new Uri(webApplication.Configuration["UserUrl"]);
            })
            .AddTypedClient<IUsersService, UsersService>();

            webApplication.Services.AddHttpClient("sapfileService", c =>
            {
                c.BaseAddress = new Uri(webApplication.Configuration["SapFileUrl"]);
            })
            .AddTypedClient<ISapFileService, SapFileService>();

            webApplication.Services.AddHttpClient("almacenService", c =>
            {
                c.BaseAddress = new Uri(webApplication.Configuration["AlmacenUrl"]);
            })
            .AddTypedClient<IAlmacenService, AlmacenService>();

            webApplication.Services.AddHttpClient("reportingService", c =>
            {
                c.BaseAddress = new Uri(webApplication.Configuration["ReportingService"]);
            })
            .AddTypedClient<IReportingService, ReportingService>();

            webApplication.Services.AddHttpClient("saporderadapter", c =>
            {
                c.BaseAddress = new Uri(webApplication.Configuration["SapServiceLayerAdapterUrl"]);
            })
            .AddTypedClient<ISapServiceLayerAdapterService, SapServiceLayerAdapterService>();

            webApplication.AddRedis();
            webApplication.AddCorsSvc();

            webApplication.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CancelProductionOrderHandler).Assembly));
            webApplication.Services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
            webApplication.Services.AddHostedService<QueuedHostedService>();

            webApplication.Services.AddHostedService<KafkaConsumerFinalizeProductionOrderSap>();
            webApplication.Services.AddHostedService<KafkaConsumerFinalizeProductionOrderPostgresql>();
            webApplication.Services.AddHostedService<KafkaConsumerProductionOrderPdfGeneration>();
            webApplication.Services.AddHostedService<KafkaConsumerRetryFinalizeProductionOrder>();

            webApplication.Services.Configure<GzipCompressionProviderOptions>(options => options.Level = System.IO.Compression.CompressionLevel.Fastest);
            webApplication.Services.AddResponseCompression();

            return webApplication.Build();
        }

        /// <summary>
        /// Use application.
        /// </summary>
        /// <param name="app">WebApplicationBuilder.</param>
        /// <returns>WebApplication.</returns>
        public static WebApplication UseApplication(this WebApplication app)
        {
            if (app.Environment.IsProduction())
            {
                app.UseAllElasticApm(app.Configuration);
            }

            app.UseStaticFiles();
            app.UseSwagger(c =>
            {
                var basepath = app.Configuration["SwaggerAddress"];

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
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Api Pedidos");
                c.RoutePrefix = string.Empty;
            });

            app.UseResponseCompression();

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Images")),
                RequestPath = "/resources",
            });

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Delivery")),
                RequestPath = "/resources/delivery",
            });

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Invoice")),
                RequestPath = "/resources/invoice",
            });
            app.UseMetricServer();
            app.UseMiddleware<ResponseMiddleware>();

            app.UseRouting();
            app.UseCors("CorsPolicy");
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            return app;
        }

        /// <summary>
        /// Adds the cors SVC.
        /// </summary>
        /// <param name="webApplication">WebApplicationBuilder webApplication.</param>
        private static void AddCorsSvc(this WebApplicationBuilder webApplication)
        {
            webApplication.Services.AddCors(options =>
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
        /// <param name="webApplication">WebApplicationBuilder webApplication.</param>
        private static void AddRedis(this WebApplicationBuilder webApplication)
        {
            try
            {
                var configuration = ConfigurationOptions.Parse(webApplication.Configuration["redis:hostname"], true);
                configuration.ResolveDns = true;

                ConnectionMultiplexer cm = ConnectionMultiplexer.Connect(configuration);
                webApplication.Services.AddSingleton<IConnectionMultiplexer>(cm);
            }
            catch (Exception)
            {
                Log.Error("Srvicio Pedidos: No se econtro Redis");
            }
        }
    }
}
