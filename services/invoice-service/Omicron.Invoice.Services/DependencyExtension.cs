// <summary>
// <copyright file="DependencyExtension.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Invoice.Services
{
    /// <summary>
    /// DependencyExtension class.
    /// </summary>
    public static class DependencyExtension
    {
        /// <summary>
        /// Add configuration Auto Mapper.
        /// </summary>
        /// <param name="services">Service Collection.</param>
        /// <returns>Interface Service Collection.</returns>
        public static IServiceCollection AddAutoMapper(this IServiceCollection services)
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new AutoMapperProfile());
            });

            services.AddSingleton(mappingConfig.CreateMapper());
            return services;
        }

        /// <summary>
        /// Method that extend IServiceCollection to IoC.
        /// </summary>
        /// <param name="services">Service collection startup.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IInvoiceService, InvoiceService>();
            services.AddScoped<IRedisService, RedisService>();
            services.AddScoped<ISapServiceLayerAdapterService, SapServiceLayerAdapterService>();
            services.AddScoped<ISapAdapter, SapAdapter.SapAdapter>();
            return services;
        }
    }
}