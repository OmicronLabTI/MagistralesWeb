// <summary>
// <copyright file="AutoMapperProfile.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Warehouses.Services.Mapping
{
    using AutoMapper;
    using Omicron.Warehouses.Dtos.Model;
    using Omicron.Warehouses.Entities.Model;

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
            this.CreateMap<ResultModel, ResultDto>();
            this.CreateMap<ResultDto, ResultModel>();
            this.CreateMap<RawMaterialRequestDto, RawMaterialRequestModel>().ForMember(x => x.Signature, opt => opt.ConvertUsing<ConverterBase64ToByteArray, string>());
            this.CreateMap<RawMaterialRequestModel, RawMaterialRequestDto>();
            this.CreateMap<RawMaterialRequestDetailModel, RawMaterialRequestDetailDto>();
            this.CreateMap<RawMaterialRequestDetailDto, RawMaterialRequestDetailModel>();
        }
    }
}