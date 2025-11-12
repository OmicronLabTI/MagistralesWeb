// <summary>
// <copyright file="DependencyExtension.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Invoice.Api
{
    using MediatR;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Omicron.Invoice.Facade;
    using Omicron.Invoice.Persistence;
    using Omicron.Invoice.Services;
    using Serilog;
    using StackExchange.Redis;


    /// <summary>
    /// DependencyExtension static.
    /// </summary>
    public static class DependencyExtension
    {
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

            webApplication.Services.AddKafka(webApplication.Configuration, Log.Logger);
            webApplication.Services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
            webApplication.Services.AddHostedService<QueuedHostedService>();

            webApplication.Services.AddApplicationInsightsTelemetry();
            webApplication.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyExtension).Assembly));
            webApplication.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateInvoiceHandler).Assembly));

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

            webApplication.Services.AddHttpClient("sapadapter", c =>
            {
                c.BaseAddress = new Uri(webApplication.Configuration["SapAdapterUrl"]);
            })
            .AddTypedClient<ISapAdapter, SapAdapter>();

            webApplication.Services.AddHttpClient("service layer adapter", c =>
            {
                c.BaseAddress = new Uri(webApplication.Configuration["SapServiceLayerAdapterUrl"]);
            })
            .AddTypedClient<ISapServiceLayerAdapterService, SapServiceLayerAdapterService>();

            webApplication.Services.AddHttpClient("catalogs", c =>
            {
                c.BaseAddress = new Uri(webApplication.Configuration["CatalogUrl"]);
            })
            .AddTypedClient<ICatalogsService, CatalogsService>();

            webApplication.Services.AddHttpClient("users", c =>
            {
                c.BaseAddress = new Uri(webApplication.Configuration["UserService"]);
            })
            .AddTypedClient<IUsersService, UsersService>();

            // 🔹 Configurar CORS (permitir cualquier origen, método y encabezado)
            webApplication.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    policy => policy
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });

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

            // 🔹 Activar CORS antes de autorización y mapeo de controladores
            app.UseCors("AllowAll");

            app.UseAuthorization();
            app.MapControllers();

            return app;
        }
    }
}
