// ------------------------------------------------------------------------------------------------
// <copyright file="UnityResolver.cs" company="Ann Inc">
//  Copyright (c) 2017 Ann Inc, All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace Omicron.SapDiApi.Api.Configuration
{
    using System;
    using System.Collections.Generic;    
    using System.Web.Http.Dependencies;
    using AutoMapper;
    using Omicron.SapDiApi.Dtos.Models;
    using Omicron.SapDiApi.Entities.Models;
    using Omicron.SapDiApi.Facade.Sap;
    using Omicron.SapDiApi.Log;
    using Omicron.SapDiApi.Services.SapDiApi;
    using Unity;    

    /// <summary>
    /// IOC Resolver for wrpping the unity container
    /// </summary>
    public class UnityResolver : IDependencyResolver
    {
        /// <summary>
        /// Unity container
        /// </summary>
        private IUnityContainer container;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnityResolver"/> class.
        /// </summary>
        /// <param name="container">Unity container</param>
        public UnityResolver(IUnityContainer container)
        {
            var mappingConfig = new MapperConfiguration(cfg => 
            {
                cfg.CreateMap<ResultModel, ResultDto>();
                cfg.CreateMap<ResultDto, ResultModel>();
                cfg.CreateMap<OrderDto, OrderModel>();
                cfg.CreateMap<OrderModel, OrderDto>();
                cfg.CreateMap<CompleteDetailModel, CompleteDetailDto>();
                cfg.CreateMap<CompleteDetailDto, CompleteDetailModel>();
                cfg.CreateMap<OrderWithDetailDto, OrderWithDetailModel>();
                cfg.CreateMap<OrderWithDetailModel, OrderWithDetailDto>();
                cfg.CreateMap<UpdateFabOrderDto, UpdateFabOrderModel>();
                cfg.CreateMap<UpdateFabOrderModel, UpdateFabOrderDto>();
                cfg.CreateMap<CompleteDetalleFormulaModel, CompleteDetalleFormulaDto>();
                cfg.CreateMap<CompleteDetalleFormulaDto, CompleteDetalleFormulaModel>();
                cfg.CreateMap<UpdateFormulaModel, UpdateFormulaDto>();
                cfg.CreateMap<UpdateFormulaDto, UpdateFormulaModel>();
                cfg.CreateMap<CancelOrderDto, CancelOrderModel>();
                cfg.CreateMap<AssginBatchDto, AssignBatchModel>();
                cfg.CreateMap<CreateIsolatedFabOrderDto, CreateIsolatedFabOrderModel>();
                cfg.CreateMap<CreateIsolatedFabOrderModel, CreateIsolatedFabOrderDto>();
                cfg.CreateMap<CloseProductionOrderDto, CloseProductionOrderModel>();
                cfg.CreateMap<CloseProductionOrderModel, CloseProductionOrderDto>();
                cfg.CreateMap<BatchesConfigurationDto, BatchesConfigurationModel>();
                cfg.CreateMap<BatchesConfigurationModel, BatchesConfigurationDto>();
                cfg.CreateMap<CreateDeliveryDto, CreateDeliveryModel>();
                cfg.CreateMap<AlmacenBatchDto, AlmacenBatchesModel>();
                cfg.CreateMap<SendPackageDto, SendPackageModel>();
                cfg.CreateMap<ProductDeliveryDto, ProductDeliveryModel>();
                cfg.CreateMap<CancelDeliveryDto, CancelDeliveryModel>();
                cfg.CreateMap<CloseSampleOrderDto, CloseSampleOrderModel>();
            });
            container.RegisterInstance<IMapper>(mappingConfig.CreateMapper());

            container.RegisterType<ISapFacade, SapFacade>();
            container.RegisterType<ISapDiApiService, SapDiApiService>();
            container.RegisterType<ILoggerProxy, LoggerProxy>();
            container.RegisterType<ICancelService, CancelService>();
            container.RegisterType<ICreateDeliveryService, CreateDeliveryService>();

            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            this.container = container;
        }

        /// <summary>
        /// Creates one instance of a type
        /// </summary>
        /// <param name="serviceType">Service type</param>
        /// <returns>Type instance</returns>
        public object GetService(Type serviceType)
        {
            try
            {
                return this.container.Resolve(serviceType);
            }
            catch (ResolutionFailedException)
            {
                return null;
            }
        }

        /// <summary>
        /// Creates a collection of objets of the specified type
        /// </summary>
        /// <param name="serviceType">Service type</param>
        /// <returns>Type instances</returns>
        public IEnumerable<object> GetServices(Type serviceType)
        {
            try
            {
                return this.container.ResolveAll(serviceType);
            }
            catch (ResolutionFailedException)
            {
                return new List<object>();
            }
        }

        /// <summary>
        /// Generates a child container
        /// </summary>
        /// <returns>Child container</returns>
        public IDependencyScope BeginScope()
        {
            var child = this.container.CreateChildContainer();
            return new UnityResolver(child);
        }

        /// <summary>
        /// Releases the managed resources
        /// </summary>
        public void Dispose()
        {
            this.container.Dispose();
        }
    }
}