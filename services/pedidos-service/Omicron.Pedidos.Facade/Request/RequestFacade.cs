// <summary>
// <copyright file="RequestFacade.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Facade.Request
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoMapper;
    using Omicron.Pedidos.Dtos.Models;
    using Omicron.Pedidos.Entities.Model.Db;
    using Omicron.Pedidos.Services.Request;

    /// <summary>
    /// the pedidos facade.
    /// </summary>
    public class RequestFacade : IRequestFacade
    {
        private readonly IMapper mapper;

        private readonly IRequestService requestService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestFacade"/> class.
        /// </summary>
        /// <param name="requestService">the pedido service.</param>
        /// <param name="mapper">the mapper.</param>
        public RequestFacade(IRequestService requestService, IMapper mapper)
        {
            this.requestService = requestService ?? throw new ArgumentNullException(nameof(requestService));
            this.mapper = mapper;
        }

        /// <summary>
        /// Create raw material request.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="requests">Requests data.</param>
        /// <returns>List with successfuly and failed creations.</returns>
        public async Task<ResultDto> CreateRawMaterialRequest(string userId, List<RawMaterialRequestDto> requests)
        {
            return this.mapper.Map<ResultDto>(await this.requestService.CreateRawMaterialRequest(userId, this.mapper.Map<List<RawMaterialRequestModel>>(requests)));
        }
    }
}
