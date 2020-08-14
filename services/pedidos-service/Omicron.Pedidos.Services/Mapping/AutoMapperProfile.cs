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
        }
    }
}