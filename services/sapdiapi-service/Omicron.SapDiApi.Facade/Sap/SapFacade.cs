// <summary>
// <copyright file="SapFacade.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapDiApi.Facade.Sap
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoMapper;
    using Omicron.SapDiApi.Dtos.Models;
    using Omicron.SapDiApi.Dtos.Models.Experience;
    using Omicron.SapDiApi.Entities.Context;
    using Omicron.SapDiApi.Entities.Models;
    using Omicron.SapDiApi.Entities.Models.Experience;
    using Omicron.SapDiApi.Services.SapDiApi;
    using SAPbobsCOM;
    public class SapFacade : ISapFacade
    {
        private readonly IMapper mapper;

        private readonly ISapDiApiService sapDiApiService;

        private readonly ICancelService cancelService;

        private readonly ICreateDeliveryService createDeliveryService;

        private readonly IDoctorAddress doctorAddress;

        /// <summary>
        /// Initializes a new instance of the <see cref="SapFacade"/> class.
        /// </summary>        
        /// <param name="mapper"></param>
        /// <param name="sapDiApiService">the sap di api.</param>
        /// <param name="cancelService">cancel service.</param>
        /// <param name="createDeliveryService">the create delivery service.</param>
        /// <param name="doctorAddress">The doctor addresses.</param>
        public SapFacade(IMapper mapper, ISapDiApiService sapDiApiService, ICancelService cancelService, ICreateDeliveryService createDeliveryService, IDoctorAddress doctorAddress)
        {
            this.mapper = mapper;
            this.sapDiApiService = sapDiApiService;
            this.cancelService = cancelService;
            this.createDeliveryService = createDeliveryService;
            this.doctorAddress = doctorAddress;
        }

        /// <summary>
        /// connecto to sap.
        /// </summary>
        /// <returns>connects.</returns>
        public async Task<ResultDto> Connect()
        {
            return this.mapper.Map<ResultDto>(await this.sapDiApiService.Connect());
        }

        /// <summary>
        /// creates order.
        /// </summary>
        /// <returns>the result.</returns>
        public async Task<ResultDto> CreateFabOrder(List<OrderWithDetailDto> orderWithDetailDto)
        {
            var model = this.mapper.Map<List<OrderWithDetailModel>>(orderWithDetailDto);
            return this.mapper.Map<ResultDto>(await this.sapDiApiService.InsertOrdenFab(model));
        }

        /// <summary>
        /// updates the fabriction orders.
        /// </summary>
        /// <param name="updateFabOrderDtos">the orders to update.</param>
        /// <returns>the reult.</returns>
        public async Task<ResultDto> UpdateFabOrder(List<UpdateFabOrderDto> updateFabOrderDtos)
        {
            return this.mapper.Map<ResultDto>(await this.sapDiApiService.UpdateFabOrders(this.mapper.Map<List<UpdateFabOrderModel>>(updateFabOrderDtos)));
        }

        /// <summary>
        /// Updates the formula.
        /// </summary>
        /// <param name="updateFormula">the object to update.</param>
        /// <returns>the data.</returns>
        public async Task<ResultDto> UpdateFormula(UpdateFormulaDto updateFormula)
        {
            return this.mapper.Map<ResultDto>(await this.sapDiApiService.UpdateFormula(this.mapper.Map<UpdateFormulaModel>(updateFormula)));
        }
         
        /// <summary>
        /// Cancel a prodution order
        /// </summary>
        /// <param name="productionOrder">Production order to update</param>
        /// <returns>the data.</returns>
        public async Task<ResultDto> CancelProductionOrder(CancelOrderDto productionOrder) 
        { 
            return this.mapper.Map<ResultDto>(await this.sapDiApiService.CancelProductionOrder(this.mapper.Map<CancelOrderModel>(productionOrder)));
        }

        /// <summary>
        /// Finish production orders.
        /// </summary>
        /// <param name="productionOrders">Production orders to finish.</param>
        /// <returns>Operation result.</returns>
        public ResultDto FinishOrder(List<CloseProductionOrderDto> productionOrders)
        {
            return this.mapper.Map<ResultDto>(this.sapDiApiService.FinishOrder(this.mapper.Map<List<CloseProductionOrderModel>>(productionOrders)));
        }

        /// <summary>
        /// Create new isolated production order.
        /// </summary>
        /// <param name="isolatedFabOrder">Isolated production order.</param>
        /// <returns>Operation result.</returns>
        public async Task<ResultDto> CreateIsolatedProductionOrder(CreateIsolatedFabOrderDto isolatedFabOrder)
        {
            return this.mapper.Map<ResultDto>(await this.sapDiApiService.CreateIsolatedProductionOrder(this.mapper.Map<CreateIsolatedFabOrderModel>(isolatedFabOrder)));
        }

        /// <summary>
        /// Upfate the batches.
        /// </summary>
        /// <param name="assginBatches">The assign batches.</param>
        /// <returns>the batches.</returns>
        public async Task<ResultDto> UpdateBatches(List<AssginBatchDto> assginBatches)
        {
            return this.mapper.Map<ResultDto>(await this.sapDiApiService.UpdateBatches(this.mapper.Map<List<AssignBatchModel>>(assginBatches)));
        }

        /// <summary>
        /// Creates the delivery.
        /// </summary>
        /// <param name="createDelivery">the deliveries.</param>
        /// <returns>the status.</returns>
        public async Task<ResultDto> CreateDelivery(List<CreateDeliveryDto> createDelivery)
        {
            return this.mapper.Map<ResultDto>(await this.createDeliveryService.CreateDelivery(this.mapper.Map<List<CreateDeliveryModel>>(createDelivery)));
        }

        public async Task<ResultDto> CreateDeliveryPartial(List<CreateDeliveryDto> createDeliveries)
        {
            return this.mapper.Map<ResultDto>(await this.createDeliveryService.CreateDeliveryPartial(this.mapper.Map<List<CreateDeliveryModel>>(createDeliveries)));
        }

        /// <inheritdoc/>
        public async Task<ResultDto> UpdateTracking(SendPackageDto sendPackage)
        {
            return this.mapper.Map<ResultDto>(await this.sapDiApiService.UpdateTracking(this.mapper.Map<SendPackageModel>(sendPackage)));
        }

        /// <inheritdoc/>
        public async Task<ResultDto> CancelDelivery(string type, List<CancelDeliveryDto> deliveries)
        {
            return this.mapper.Map<ResultDto>(await this.cancelService.CancelDelivery(type, this.mapper.Map<List< CancelDeliveryModel>>(deliveries)));
        }

        /// <inheritdoc/>
        public async Task<ResultDto> CreateDeliveryBatch(List<CreateDeliveryDto> createDeliveries)
        {
            return this.mapper.Map<ResultDto>(await this.createDeliveryService.CreateDeliveryBatch(this.mapper.Map<List<CreateDeliveryModel>>(createDeliveries)));
        }

        /// <inheritdoc/>
        public async Task<ResultDto> CloseMuestra(List<CloseSampleOrderDto> orderIds)
        {
            return this.mapper.Map<ResultDto>(await this.createDeliveryService.CloseMuestra(this.mapper.Map<List< CloseSampleOrderModel>>(orderIds)));
        }

        /// <inheritdoc/>
        public async Task<ResultDto> UpdateDoctorAddress(List<DoctorDeliveryAddressDto> address)
        {
            return this.mapper.Map<ResultDto>(await this.doctorAddress.UpdateDoctorDeliveryAddress(this.mapper.Map<List<DoctorDeliveryAddressModel>>(address)));
        }

        /// <inheritdoc/>
        public async Task<ResultDto> UpdateDoctorAddress(List<DoctorInvoiceAddressDto> address)
        {
            return this.mapper.Map<ResultDto>(await this.doctorAddress.UpdateDoctorDeliveryAddress(this.mapper.Map<List<DoctorInvoiceAddressModel>>(address)));
        }
    }
}
