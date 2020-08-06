// <summary>
// <copyright file="AutoMapperProfile.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapDiApi.Services.Mapping
{
    using AutoMapper;
    using Omicron.SapDiApi.Dtos.Models;
    using Omicron.SapDiApi.Entities.Models;

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
            this.CreateMap<ResultDto, ResultModel>();
            this.CreateMap<ResultModel, ResultDto>();

            this.CreateMap<OrderDto, OrderModel>();
            this.CreateMap<OrderModel, OrderDto>();
            this.CreateMap<CompleteDetailModel, CompleteDetailDto>();
            this.CreateMap<CompleteDetailDto, CompleteDetailModel>();
            this.CreateMap<OrderWithDetailDto, OrderWithDetailModel>();
            this.CreateMap<OrderWithDetailModel, OrderWithDetailDto>();

        }
    }
}
