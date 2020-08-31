// <summary>
// <copyright file="AutoMapperProfile.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Services.Mapping
{
    using AutoMapper;
    using Omicron.SapAdapter.Dtos.Models;
    using Omicron.SapAdapter.Dtos.User;
    using Omicron.SapAdapter.Entities.Model;
    using Omicron.SapAdapter.Entities.Model.BusinessModels;

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
            this.CreateMap<GetOrderFabDto, GetOrderFabModel>();
        }
    }
}