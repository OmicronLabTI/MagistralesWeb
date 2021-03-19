// <summary>
// <copyright file="AutoMapperProfile.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Services.Mapping
{
    using AutoMapper;
    using Omicron.Pedidos.Dtos.Models;
    using Omicron.Pedidos.Dtos.User;
    using Omicron.Pedidos.Entities.Model;
    using Omicron.Pedidos.Entities.Model.Db;

    /// <summary>
    /// Class Automapper.
    /// </summary>
    public class AutoMapperProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoMapperProfile"/> class.
        /// </summary>
        public AutoMapperProfile()
        {
            this.CreateMap<UserModel, UserDto>();
            this.CreateMap<UserDto, UserModel>();
            this.CreateMap<ResultDto, ResultModel>();
            this.CreateMap<ResultModel, ResultDto>();
            this.CreateMap<ProcessOrderModel, ProcessOrderDto>();
            this.CreateMap<ProcessOrderDto, ProcessOrderModel>();
            this.CreateMap<ManualAssignDto, ManualAssignModel>();
            this.CreateMap<ManualAssignModel, ManualAssignDto>();
            this.CreateMap<CompleteDetalleFormulaModel, CompleteDetalleFormulaDto>();
            this.CreateMap<CompleteDetalleFormulaDto, CompleteDetalleFormulaModel>();
            this.CreateMap<UpdateFormulaModel, UpdateFormulaDto>();
            this.CreateMap<UpdateFormulaDto, UpdateFormulaModel>();
            this.CreateMap<UpdateStatusOrderModel, UpdateStatusOrderDto>();
            this.CreateMap<UpdateStatusOrderDto, UpdateStatusOrderModel>();
            this.CreateMap<ProcessByOrderModel, ProcessByOrderDto>();
            this.CreateMap<ProcessByOrderDto, ProcessByOrderModel>();
            this.CreateMap<OrderIdDto, OrderIdModel>();
            this.CreateMap<OrderIdModel, OrderIdDto>();
            this.CreateMap<RejectOrdersDto, RejectOrdersModel>();
            this.CreateMap<RejectOrdersModel, RejectOrdersDto>();
            this.CreateMap<AutomaticAssingDto, AutomaticAssingModel>();
            this.CreateMap<AutomaticAssingModel, AutomaticAssingDto>();
            this.CreateMap<AssignBatchDto, AssignBatchModel>();
            this.CreateMap<AssignBatchModel, AssignBatchDto>();
            this.CreateMap<UpdateOrderSignatureDto, UpdateOrderSignatureModel>();
            this.CreateMap<UpdateOrderSignatureModel, UpdateOrderSignatureDto>();
            this.CreateMap<UpdateOrderCommentsDto, UpdateOrderCommentsModel>();
            this.CreateMap<UpdateOrderCommentsModel, UpdateOrderCommentsDto>();
            this.CreateMap<CreateIsolatedFabOrderModel, CreateIsolatedFabOrderDto>();
            this.CreateMap<CreateIsolatedFabOrderDto, CreateIsolatedFabOrderModel>();
            this.CreateMap<FinishOrderDto, FinishOrderModel>();
            this.CreateMap<CloseProductionOrderDto, CloseProductionOrderModel>();
            this.CreateMap<CloseProductionOrderModel, CloseProductionOrderDto>();
            this.CreateMap<BatchesConfigurationDto, BatchesConfigurationModel>();
            this.CreateMap<BatchesConfigurationModel, BatchesConfigurationDto>();
            this.CreateMap<CustomComponentListModel, CustomComponentListDto>();
            this.CreateMap<CustomComponentListDto, CustomComponentListModel>();
            this.CreateMap<ComponentCustomComponentListModel, ComponentCustomComponentListDto>();
            this.CreateMap<ComponentCustomComponentListDto, ComponentCustomComponentListModel>();
            this.CreateMap<UpdateDesignerLabelDto, UpdateDesignerLabelModel>();
            this.CreateMap<UpdateDesignerLabelDetailDto, UpdateDesignerLabelDetailModel>();
            this.CreateMap<UserOrderDto, UserOrderModel>();
            this.CreateMap<CancelDeliveryPedidoDto, CancelDeliveryPedidoModel>();
            this.CreateMap<DetallePedidoDto, DetallePedidoModel>();
            this.CreateMap<CancelDeliveryPedidoCompleteDto, CancelDeliveryPedidoCompleteModel>();
        }
    }
}