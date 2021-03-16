// <summary>
// <copyright file="ReportingFacade.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Reporting.Facade.Request
{
    using System;
    using System.Threading.Tasks;
    using AutoMapper;
    using Omicron.Reporting.Dtos.Model;
    using Omicron.Reporting.Entities.Model;
    using Omicron.Reporting.Services;

    /// <summary>
    /// the reporting facade implementation.
    /// </summary>
    public class ReportingFacade : IReportingFacade
    {
        private readonly IMapper mapper;

        private readonly IReportingService reportingService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportingFacade"/> class.
        /// </summary>
        /// <param name="reportingService">the reporting service.</param>
        /// <param name="mapper">the mapper.</param>
        public ReportingFacade(IReportingService reportingService, IMapper mapper)
        {
            this.reportingService = reportingService ?? throw new ArgumentNullException(nameof(reportingService));
            this.mapper = mapper;
        }

        /// <summary>
        /// Create raw material request.
        /// </summary>
        /// <param name="request">Requests data.</param>
        /// <param name="preview">Flag for preview file.</param>
        /// <returns>Report file stream.</returns>
        public FileResultDto CreateRawMaterialRequestPdf(RawMaterialRequestDto request, bool preview)
        {
            return this.mapper.Map<FileResultDto>(this.reportingService.CreateRawMaterialRequestPdf(this.mapper.Map<RawMaterialRequestModel>(request), preview));
        }

        /// <summary>
        /// Submit raw material request.
        /// </summary>
        /// <param name="request">Requests data.</param>
        /// <returns>Operation result.</returns>
        public async Task<ResultDto> SubmitRawMaterialRequestPdf(RawMaterialRequestDto request)
        {
            return this.mapper.Map<ResultDto>(await this.reportingService.SubmitRawMaterialRequestPdf(this.mapper.Map<RawMaterialRequestModel>(request)));
        }

        /// <inheritdoc/>
        public async Task<ResultDto> SendEmailForeignPackage(SendPackageDto request)
        {
            return this.mapper.Map<ResultDto>(await this.reportingService.SendEmailForeignPackage(this.mapper.Map<SendPackageModel>(request)));
        }

        /// <inheritdoc/>
        public async Task<ResultDto> SendEmailLocalPackage(SendLocalPackageDto sendLocalPackage)
        {
            return this.mapper.Map<ResultDto>(await this.reportingService.SendEmailLocalPackage(this.mapper.Map<SendLocalPackageModel>(sendLocalPackage)));
        }

        /// <inheritdoc/>
        public async Task<ResultDto> SendEmailRejectedOrder(SendRejectedEmailDto request)
        {
           return this.mapper.Map<ResultDto>(await this.reportingService.SendEmailRejectedOrder(this.mapper.Map<SendRejectedEmailModel>(request)));
        }

        /// <inheritdoc/>
        public async Task<ResultDto> SendEmailCancelDeliveryOrders(SendCancelDeliveryDto request)
        {
            return this.mapper.Map<ResultDto>(await this.reportingService.SendEmailCancelDeliveryOrders(this.mapper.Map<SendCancelDeliveryModel>(request)));
        }
    }
}
