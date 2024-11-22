// <summary>
// <copyright file="DependencyExtension.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Api
{
    /// <summary>
    /// DependencyExtension static.
    /// </summary>
    public static class DependencyExtension
    {
        /// <summary>
        /// Method to register Services.
        /// </summary>
        /// <param name="services">Service Collection.</param>
        /// <returns>Interface Service Collection.</returns>
        public static IServiceCollection RegisterServices(IServiceCollection services)
        {
            services.AddTransient<IServiceLayerClient, ServiceLayerClient>();
            services.AddTransient<IServiceLayerAuth, ServiceLayerAuth>();
            services.AddTransient<IDeliveryNoteService, DeliveryNoteService>();
            services.AddTransient<ISapFileService, SapFileService>();
            return services;
        }

        /// <summary>
        /// Config application.
        /// </summary>
        /// <param name="webApplication">WebApplicationBuilder.</param>
        /// <returns>WebApplication.</returns>
        public static WebApplication AppConfiguration(this WebApplicationBuilder webApplication)
        {
            webApplication.AddPlaceholderResolver();

            webApplication.Host.UseSerilog();

            webApplication.Services.AddControllers(options =>
            {
                options.Filters.Add<CustomActionFilterAttribute>();
                options.Filters.Add<GlobalExceptionFilterAttribute>();
            });

            webApplication.Services.AddFacade();
            webApplication.Services.AddServices();
            webApplication.Services.AddAutoMapper();

            webApplication.Services.AddEndpointsApiExplorer();
            webApplication.Services.AddSwaggerGen();

            webApplication.Services.AddScoped<AddB1SessionCookieMiddleware>();
            webApplication.Services.AddScoped<RefreshSessionIdMiddleware>();

            DependencyExtension.RegisterServices(webApplication.Services);

            webApplication.Services.AddHttpClient("service layer auth", c =>
            {
                c.BaseAddress = new Uri(webApplication.Configuration["SAPServiceLayerService"]);
            })
            .AddTypedClient<IServiceLayerAuth, ServiceLayerAuth>()
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                var httpClientHandler = new HttpClientHandler();
                httpClientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
                return httpClientHandler;
            });

            webApplication.Services.AddHttpClient<IServiceLayerClient, ServiceLayerClient>("service layer client", c =>
            {
                c.BaseAddress = new Uri(webApplication.Configuration["SAPServiceLayerService"]);
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                var httpClientHandler = new HttpClientHandler();
                httpClientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
                return httpClientHandler;
            })
            .AddHttpMessageHandler<AddB1SessionCookieMiddleware>()
            .AddHttpMessageHandler<RefreshSessionIdMiddleware>();

            webApplication.Services.AddHttpClient("sapfileService", c =>
            {
                c.BaseAddress = new Uri(webApplication.Configuration["SapFileUrl"]);
            })
            .AddTypedClient<ISapFileService, SapFileService>();

            webApplication.Services.AddKafka(webApplication.Configuration, Log.Logger);

            webApplication.Services.AddApplicationInsightsTelemetry();

            try
            {
                var configuration = ConfigurationOptions.Parse(webApplication.Configuration["redis:hostname"], true);
                configuration.ResolveDns = true;
                webApplication.Services.AddSingleton<IConnectionMultiplexer>(cm => ConnectionMultiplexer.Connect(configuration));
            }
            catch (Exception)
            {
                Log.Logger.Error("No se encontro Redis");
            }

            return webApplication.Build();
        }

        /// <summary>
        /// Use application.
        /// </summary>
        /// <param name="app">WebApplicationBuilder.</param>
        /// <returns>WebApplication.</returns>
        public static WebApplication UseApplication(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            return app;
        }
    }
}
