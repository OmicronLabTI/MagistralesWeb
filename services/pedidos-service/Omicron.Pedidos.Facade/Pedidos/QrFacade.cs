// <summary>
// <copyright file="QrFacade.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Facade.Pedidos
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoMapper;
    using Omicron.Pedidos.Dtos.Models;
    using Omicron.Pedidos.Services.Pedidos;

    /// <summary>
    /// Generates the QR.
    /// </summary>
    public class QrFacade : IQrFacade
    {
        private readonly IMapper mapper;

        private readonly IQrService qrsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="QrFacade"/> class.
        /// </summary>
        /// <param name="mapper">the mapper.</param>
        /// <param name="qrsService">The qr services.</param>
        public QrFacade(IMapper mapper, IQrService qrsService)
        {
            this.mapper = mapper;
            this.qrsService = qrsService ?? throw new ArgumentNullException(nameof(qrsService));
        }

        /// <inheritdoc/>
        public async Task<ResultDto> CreateMagistralQr(List<int> ordersId)
        {
            return this.mapper.Map<ResultDto>(await this.qrsService.CreateMagistralQr(ordersId));
        }

        /// <inheritdoc/>
        public async Task<ResultDto> CreateRemisionQr(List<int> ordersId)
        {
            return this.mapper.Map<ResultDto>(await this.qrsService.CreateRemisionQr(ordersId));
        }
    }
}
