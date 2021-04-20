// <summary>
// <copyright file="AutoMapperProfile.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Reporting.Services.Mapping
{
    using AutoMapper;
    using Omicron.Reporting.Dtos.Model;
    using Omicron.Reporting.Entities.Model;

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
            this.CreateMap<FileResultDto, FileResultModel>();
            this.CreateMap<FileResultModel, FileResultDto>();
            this.CreateMap<RawMaterialRequestDto, RawMaterialRequestModel>();
            this.CreateMap<RawMaterialRequestModel, RawMaterialRequestDto>();
            this.CreateMap<RawMaterialRequestDetailModel, RawMaterialRequestDetailDto>();
            this.CreateMap<RawMaterialRequestDetailDto, RawMaterialRequestDetailModel>();
            this.CreateMap<SendPackageDto, SendPackageModel>();
            this.CreateMap<SendLocalPackageDto, SendLocalPackageModel>();
            this.CreateMap<RejectedOrdersDto, RejectedOrdersModel>();
            this.CreateMap<SendRejectedEmailDto, SendRejectedEmailModel>();
            this.CreateMap<SendCancelDeliveryDto, SendCancelDeliveryModel>();
            this.CreateMap<SendCancelDeliveryModel, SendCancelDeliveryDto>();
            this.CreateMap<IncidentDataDto, IncidentDataModel>();
        }
    }
}