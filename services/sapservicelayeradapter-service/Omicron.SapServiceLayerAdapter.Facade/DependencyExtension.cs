// <summary>
// <copyright file="DependencyExtension.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Facade
{
    /// <summary>
    /// DependencyExtension class.
    /// </summary>
    public static class DependencyExtension
    {
        /// <summary>
        /// Method that extend IServiceCollection.
        /// </summary>
        /// <param name="services">Service collection startup.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddFacade(this IServiceCollection services)
        {
            services.AddScoped<IOrderFacade, OrderFacade>();
            services.AddScoped<IInvoiceFacade, InvoiceFacade>();
            services.AddScoped<IDeliveryNoteFacade, DeliveryNoteFacade>();
            services.AddScoped<IEmployeeInfoFacade, EmployeeInfoFacade>();
            services.AddScoped<IDoctorFacade, DoctorFacade>();
            services.AddScoped<IInventoryTransferRequestFacade, InventoryTransferRequestFacade>();
            services.AddScoped<IProductionOrderFacade, ProductionOrderFacade>();
            return services;
        }
    }
}