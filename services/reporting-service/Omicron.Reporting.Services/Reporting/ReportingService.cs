// <summary>
// <copyright file="ReportingService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.Reporting.Services
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Omicron.Reporting.Entities.Model;
    using Omicron.Reporting.Services.Clients;
    using Omicron.Reporting.Services.Constants;
    using Omicron.Reporting.Services.ReportBuilder;

    /// <summary>
    /// Implementations for request service.
    /// </summary>
    public class ReportingService : IReportingService
    {
        private readonly ICatalogsService catalogsService;
        private readonly IOmicronMailClient omicronMailClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportingService"/> class.
        /// </summary>
        /// <param name="catalogsService">The catalogs service.</param>
        /// <param name="omicronMailClient">The email service.</param>
        public ReportingService(ICatalogsService catalogsService, IOmicronMailClient omicronMailClient)
        {
            this.catalogsService = catalogsService;
            this.omicronMailClient = omicronMailClient;
        }

        /// <summary>
        /// Create raw material request.
        /// </summary>
        /// <param name="request">Requests data.</param>
        /// <param name="preview">Flag for preview file.</param>
        /// <returns>Report file stream.</returns>
        public FileResultModel CreateRawMaterialRequestPdf(RawMaterialRequestModel request, bool preview)
        {
            var file = this.BuildPdfFile(request, preview);
            return new FileResultModel { Success = true, Code = 200, FileStream = file.FileStream, FileName = file.FileName };
        }

        /// <summary>
        /// Submit raw material request.
        /// </summary>
        /// <param name="request">Requests data.</param>
        /// <returns>Operation result.</returns>
        public async Task<ResultModel> SubmitRawMaterialRequestPdf(RawMaterialRequestModel request)
        {
            var file = this.BuildPdfFile(request, false);
            var mailStatus = await this.SendRawMaterialRequestMail(file.FileStream, file.FileName);
            return new ResultModel { Success = true, Code = 200, Response = mailStatus, Comments = file.FileName };
        }

        /// <inheritdoc/>
        public async Task<ResultModel> SendEmailForeignPackage(SendPackageModel request)
        {
            var smtpConfig = await this.catalogsService.GetSmtpConfig();

            var body = string.Format(ServiceConstants.SentPackageBody, request.PackageId, request.TransportMode, request.TrackingNumber);
            var mailStatus = await this.omicronMailClient.SendMail(
                smtpConfig,
                request.DestinyEmail,
                ServiceConstants.SentPackage,
                body,
                request.DestinyEmail);

            return new ResultModel { Success = true, Code = 200, Response = mailStatus };
        }

        /// <summary>
        /// Build pdf file.
        /// </summary>
        /// <param name="request">Requests data.</param>
        /// <param name="preview">Preview flag.</param>
        /// <returns>Report file.</returns>
        private (MemoryStream FileStream, string FileName) BuildPdfFile(RawMaterialRequestModel request, bool preview)
        {
            var reportBuilder = new RawMaterialRequestReportBuilder(request);
            var report = reportBuilder.BuildReport();
            var date = DateTime.Now.ToString(DateConstants.RawMaterialRequestFormat);
            var requestIdentifier = preview ? "PREVIEW" : $"{request.Id}";
            var fileName = string.Format(ServiceConstants.RawMaterialRequestFileNamePattern, $"{date}_{requestIdentifier}");
            return (report, fileName);
        }

        /// <summary>
        /// Sed mail with raw material request file.
        /// </summary>
        /// <param name="fileStream">File stream.</param>
        /// <param name="fileName">File name.</param>
        /// <returns>Nothing.</returns>
        private async Task<bool> SendRawMaterialRequestMail(MemoryStream fileStream, string fileName)
        {
            var smtpConfig = await this.catalogsService.GetSmtpConfig();
            var mailConfig = await this.catalogsService.GetRawMaterialEmailConfig();
            return await this.omicronMailClient.SendMail(
                smtpConfig,
                mailConfig.Addressee,
                ServiceConstants.RawMaterialRequestEmailSubject,
                ServiceConstants.RawMaterialRequestEmailBody,
                mailConfig.CopyTo,
                new System.Collections.Generic.Dictionary<string, MemoryStream> { { fileName, fileStream } });
        }
    }
}
