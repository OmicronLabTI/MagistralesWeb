// <summary>
// <copyright file="DependencyInjector.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.DependencyInjection
{
    using AutoMapper;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Omicron.Pedidos.DataAccess.DAO.Pedidos;
    using Omicron.Pedidos.Entities.Context;
    using Omicron.Pedidos.Facade.Pedidos;
    using Omicron.Pedidos.Services.Mapping;
    using Omicron.Pedidos.Services.Pedidos;
    using Omicron.Pedidos.Services.SapAdapter;
    using Omicron.Pedidos.Services.SapDiApi;
    using Omicron.Pedidos.Services.SapFile;
    using Omicron.Pedidos.Services.User;

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
            Services.AddTransient<ISapFileService, SapFileService>();
            Services.AddTransient<IUsersService, UsersService>();
            Services.AddTransient<IPedidoFacade, PedidoFacade>();
            Services.AddTransient<IPedidosService, PedidosService>();
            Services.AddTransient<IAssignPedidosService, AssignPedidosService>();
            Services.AddTransient<ICancelPedidosService, CancelPedidosService>();
            Services.AddTransient<IProductivityService, ProductivityService>();
            Services.AddTransient<IFormulaPedidosService, FormulaPedidosService>();
            Services.AddTransient<IProcessOrdersService, ProcessOrdersService>();
            Services.AddTransient<IPedidosDao, PedidosDao>();
            Services.AddTransient<ISapDiApi, SapDiApi>();
            Services.AddTransient<ISapAdapter, SapAdapter>();
            Services.AddTransient<IQrFacade, QrFacade>();
            Services.AddTransient<IQrService, QrService>();
            Services.AddTransient<IDatabaseContext, DatabaseContext>();
            return Services;
        }

        /// <summary>
        /// Method to add Db Context.
        /// </summary>
        /// <param name="configuration">Configuration Options.</param>
        public static void AddDbContext(IConfiguration configuration)
        {
            Services.AddDbContextPool<DatabaseContext>(options => options.UseNpgsql(configuration.GetConnectionString(nameof(DatabaseContext))));
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
