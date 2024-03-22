// <summary>
// <copyright file="DependencyExtension.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

using Steeltoe.Extensions.Configuration;

namespace Omicron.SapServiceLayerAdapter.Api
{
    /// <summary>
    /// DependencyExtension static.
    /// </summary>
    public static class DependencyExtension
    {
        private const string AXITYURL = "https://www.axity.com/";

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
            webApplication.Services.AddPersistence(webApplication.Configuration);
            webApplication.Services.AddServices();
            webApplication.Services.AddAutoMapper();

            webApplication.Services.AddEndpointsApiExplorer();
            webApplication.Services.AddSwaggerGen();

            webApplication.Services.AddScoped<AddB1SessionCookieMiddleware>();
            webApplication.Services.AddScoped<RefreshSessionIdMiddleware>();

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
