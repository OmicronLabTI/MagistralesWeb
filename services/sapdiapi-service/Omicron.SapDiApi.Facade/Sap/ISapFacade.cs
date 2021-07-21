// <summary>
// <copyright file="ResultDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapDiApi.Facade.Sap
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Omicron.SapDiApi.Dtos.Models;
    using Omicron.SapDiApi.Dtos.Models.Experience;

    public interface ISapFacade
    {
        /// <summary>
        /// creates order.
        /// </summary>
        /// <returns>the result.</returns>
        Task<ResultDto> CreateFabOrder(List<OrderWithDetailDto> orderWithDetailDto);

        /// <summary>
        /// updates the fabriction orders.
        /// </summary>
        /// <param name="updateFabOrderDtos">the orders to update.</param>
        /// <returns>the reult.</returns>
        Task<ResultDto> UpdateFabOrder(List<UpdateFabOrderDto> updateFabOrderDtos);

        /// <summary>
        /// Updates the formula.
        /// </summary>
        /// <param name="updateFormula">the object to update.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> UpdateFormula(UpdateFormulaDto updateFormula);

        /// <summary>
        /// Cancel a Production order
        /// </summary>
        /// <param name="productionOrder">Production order to update</param>
        /// <returns>the data.</returns>
        Task<ResultDto> CancelProductionOrder(CancelOrderDto productionOrder);

        /// <summary>
        /// Upfate the batches.
        /// </summary>
        /// <param name="assginBatches">Assign batches.</param>
        /// <returns>the batches.</returns>
        Task<ResultDto> UpdateBatches(List<AssginBatchDto> assginBatches);

        /// <summary>
        /// Finish production orders.
        /// </summary>
        /// <param name="productionOrders">Production orders to finish.</param>
        /// <returns>Operation result.</returns>
        ResultDto FinishOrder(List<CloseProductionOrderDto> productionOrders);

        /// <summary>
        /// Create new isolated production order.
        /// </summary>
        /// <param name="isolatedFabOrder">Isolated production order.</param>
        /// <returns>Operation result.</returns>
        Task<ResultDto> CreateIsolatedProductionOrder(CreateIsolatedFabOrderDto isolatedFabOrder);

        /// <summary>
        /// Creates the delivery.
        /// </summary>
        /// <param name="createDelivery">the deliveries.</param>
        /// <returns>the status.</returns>
        Task<ResultDto> CreateDelivery(List<CreateDeliveryDto> createDelivery);

        /// <summary>
        /// Creates the delivery by products.
        /// </summary>
        /// <param name="createDeliveries">the deliveries.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> CreateDeliveryPartial(List<CreateDeliveryDto> createDeliveries);

        /// <summary>
        /// Updates the value for tracking.
        /// </summary>
        /// <param name="sendPackage">the package.</param>
        /// <returns></returns>
        Task<ResultDto> UpdateTracking(SendPackageDto sendPackage);

        /// <summary>
        /// Cancel a Delivery.
        /// </summary>
        /// <param name="deliveries">the delivery ids.</param>
        /// <param name="type">the type.</param>
        /// <returns>the cancellation.</returns>
        Task<ResultDto> CancelDelivery(string type, List<CancelDeliveryDto> deliveries);

        /// <summary>
        /// Creates a delivery with multiple sales.
        /// </summary>
        /// <param name="createDeliveries">the sales for 1 delivery.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> CreateDeliveryBatch(List<CreateDeliveryDto> createDeliveries);

        /// <summary>
        /// cerates the output and closes de muestra.
        /// </summary>
        /// <param name="orderIds">the order id.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> CloseMuestra(List<CloseSampleOrderDto> orderIds);

        /// <summary>
        /// Updates or add nes addresses.
        /// </summary>
        /// <param name="address">the address.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> UpdateDoctorAddress(List<DoctorDeliveryAddressDto> address);

        /// <summary>
        /// Updates or add nes addresses.
        /// </summary>
        /// <param name="address">the address.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> UpdateDoctorAddress(List<DoctorInvoiceAddressDto> address);

        /// <summary>
        /// Set as default an address.
        /// </summary>
        /// <param name="address">the address.</param>
        /// <returns>the dta.</returns>
        Task<ResultDto> UpdateDoctorDefaultAddress(DoctorDefaultAddressDto address);

        /// <summary>
        /// Set the profile info by doctor.
        /// </summary>
        /// <param name="profileDto">Profile Info</param>
        /// <returns>The data.</returns>
        Task<ResultDto> UpdateDoctorProfileInfo(DoctorProfileDto profileDto);


        /// <summary>
        /// Set the profile info by advisor.
        /// </summary>
        /// <param name="profileDto">Profile Info</param>
        /// <returns>The data.</returns>
        Task<ResultDto> UpdateAdvisorProfileInfo(AdvisorProfileDto profileDto);

        /// <summary>
        /// connecto to sap.
        /// </summary>
        /// <returns>connects.</returns>
        Task<ResultDto> Connect();
    }
}
