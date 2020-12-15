// <summary>
// <copyright file="SapAlmacenFacade.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Facade.Sap
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoMapper;
    using Omicron.SapAdapter.Dtos.Models;
    using Omicron.SapAdapter.Facade.FacadeConstants;
    using Omicron.SapAdapter.Services.Sap;

    /// <summary>
    /// Class for sap almacen facade.
    /// </summary>
    public class SapAlmacenFacade : ISapAlmacenFacade
    {
        private readonly IMapper mapper;

        private readonly ISapAlmacenService almacenService;

        private readonly ISapAlmacenDeliveryService sapAlmacenDeliveryService;

        private readonly ISapInvoiceService sapInvoiceService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SapAlmacenFacade"/> class.
        /// </summary>
        /// <param name="mapper">the mapper.</param>
        /// <param name="sapAlmacenService">the sap almacen service.</param>
        /// <param name="sapAlmacenDelivery">The sap almacen delivery.</param>
        /// <param name="sapInvoiceService">The sap invoice service.</param>
        public SapAlmacenFacade(IMapper mapper, ISapAlmacenService sapAlmacenService, ISapAlmacenDeliveryService sapAlmacenDelivery, ISapInvoiceService sapInvoiceService)
        {
            this.mapper = mapper;
            this.almacenService = sapAlmacenService ?? throw new ArgumentNullException(nameof(sapAlmacenService));
            this.sapAlmacenDeliveryService = sapAlmacenDelivery ?? throw new ArgumentNullException(nameof(sapAlmacenDelivery));
            this.sapInvoiceService = sapInvoiceService ?? throw new ArgumentException(nameof(sapInvoiceService));
        }

        /// <summary>
        /// Gets the orders for almacen.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>the data.</returns>
        public async Task<ResultDto> GetOrders(Dictionary<string, string> parameters)
        {
            return this.mapper.Map<ResultDto>(await this.almacenService.GetOrders(parameters));
        }

        /// <summary>
        /// Gets the data for the scanned qr or bar code.
        /// </summary>
        /// <param name="type">the type of the scan.</param>
        /// <param name="code">the code scanned.</param>
        /// <returns>the data.</returns>
        public async Task<ResultDto> GetScannedData(string type, string code)
        {
            if (type.Equals(FacadeConstants.Magistral))
            {
                return this.mapper.Map<ResultDto>(await this.almacenService.GetMagistralScannedData(code));
            }

            return this.mapper.Map<ResultDto>(await this.almacenService.GetLineScannedData(code));
        }

        /// <summary>
        /// Gets all the details.
        /// </summary>
        /// <param name="orderId">the order id.</param>
        /// <returns>the data.</returns>
        public async Task<ResultDto> GetCompleteDetail(int orderId)
        {
            return this.mapper.Map<ResultDto>(await this.almacenService.GetCompleteDetail(orderId));
        }

        /// <inheritdoc/>
        public async Task<ResultDto> GetDeliveryBySaleOrderId(List<int> ordersId)
        {
            return this.mapper.Map<ResultDto>(await this.almacenService.GetDeliveryBySaleOrderId(ordersId));
        }

        /// <inheritdoc/>
        public async Task<ResultDto> GetDelivery(Dictionary<string, string> parameters)
        {
            return this.mapper.Map<ResultDto>(await this.sapAlmacenDeliveryService.GetDelivery(parameters));
        }

        /// <inheritdoc/>
        public async Task<ResultDto> GetInvoice(Dictionary<string, string> parameters)
        {
            return this.mapper.Map<ResultDto>(await this.sapInvoiceService.GetInvoice(parameters));
        }
    }
}
