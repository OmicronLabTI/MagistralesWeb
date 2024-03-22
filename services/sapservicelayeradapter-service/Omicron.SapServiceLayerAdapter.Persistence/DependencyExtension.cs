// <summary>
// <copyright file="DependencyExtension.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Persistence
{
    /// <summary>
    /// DependencyExtension class.
    /// </summary>
    public static class DependencyExtension
    {
        /// <summary>
        /// Method that extend IServiceCollection to IoC.
        /// </summary>
        /// <param name="services">Service collection startup.</param>
        /// <param name="configuration">Configuration startup.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddPersistence(
            this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContextPool<DatabaseContext>(options =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString(nameof(DatabaseContext)),
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(15),
                        errorNumbersToAdd: null);
                    });
            });

            services.AddScoped<IUsersDao, UsersDao>();
            
            return services;
        }
    }
}