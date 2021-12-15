// <summary>
// <copyright file="DependencyInjector.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.DependencyInjection
{
    using AutoMapper;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Omicron.SapAdapter.DataAccess.DAO.Sap;
    using Omicron.SapAdapter.Entities.Context;
    using Omicron.SapAdapter.Facade.Sap;
    using Omicron.SapAdapter.Services.Almacen;
    using Omicron.SapAdapter.Services.Catalog;
    using Omicron.SapAdapter.Services.Mapping;
    using Omicron.SapAdapter.Services.Redis;
    using Omicron.SapAdapter.Services.Sap;
    using Omicron.SapAdapter.Services.User;
    using Omicron.SapAdapter.Services.Utils;

    /// <summary>
    /// Class for DependencyInjector.
    /// </summary>
    public static class DependencyInjector
    {
        private static IServiceCollection Services { get; set; }

        /// <summary>
        /// Method to register Services.
        /// </summary>
        /// <param name="services">Service Collection.</param>
        /// <returns>Interface Service Collection.</returns>
        public static IServiceCollection RegisterServices(IServiceCollection services)
        {
            Services = services;
            Services.AddTransient<IUsersService, UsersService>();
            Services.AddTransient<ICatalogsService, CatalogsService>();
            Services.AddTransient<ISapFacade, SapFacade>();
            Services.AddTransient<ISapService, SapService>();
            Services.AddTransient<IAlmacenService, AlmacenService>();
            Services.AddTransient<ISapDao, SapDao>();
            Services.AddTransient<ISapAlmacenFacade, SapAlmacenFacade>();
            Services.AddTransient<ISapAlmacenService, SapAlmacenService>();
            Services.AddTransient<ISapAlmacenDeliveryService, SapAlmacenDeliveryService>();
            Services.AddTransient<ISapInvoiceService, SapInvoiceService>();
            Services.AddTransient<IDatabaseContext, DatabaseContext>();
            Services.AddTransient<IGetProductionOrderUtils, GetProductionOrderUtils>();
            Services.AddTransient<IRedisService, RedisService>();
            Services.AddTransient<IComponentsService, ComponentsService>();
            Services.AddTransient<IAdvanceLookService, AdvanceLookService>();
            Services.AddTransient<IAlmacenOrderDoctorService, AlmacenOrderDoctorService>();
            Services.AddTransient<ISapDxpService, SapDxpService>();
            return Services;
        }

        /// <summary>
        /// Method to add Db Context.
        /// </summary>
        /// <param name="configuration">Configuration Options.</param>
        public static void AddDbContext(IConfiguration configuration)
        {
            Services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(configuration.GetConnectionString(nameof(DatabaseContext)), action => action.EnableRetryOnFailure(4)), ServiceLifetime.Transient);
        }

        /// <summary>
        /// Add configuration Auto Mapper.
        /// </summary>
        public static void AddAutoMapper()
        {
            var mappingConfig = new MapperConfiguration(mc => { mc.AddProfile(new AutoMapperProfile()); });
            Services.AddSingleton(mappingConfig.CreateMapper());
        }
    }
}