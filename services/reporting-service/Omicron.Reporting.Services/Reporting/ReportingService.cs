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
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
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
            var emailParty = $"{request.TransportMode.ToLower().Replace(" ", string.Empty)}{ServiceConstants.DelPartyEmail}";
            var listToLook = new List<string> { emailParty };
            listToLook.AddRange(ServiceConstants.ValuesForEmail);

            var config = await this.catalogsService.GetParams(listToLook);

            var smtpConfig = this.GetSmtpConfig(config);
            var deliveryEmailModel = config.FirstOrDefault(x => x.Field.Contains(emailParty));
            var email = deliveryEmailModel == null ? string.Empty : deliveryEmailModel.Value;

            var sendEmailOrTel = email.Contains("http") ? ServiceConstants.PaqueteEmail : ServiceConstants.TelefonoEmail;
            var sendEmailLink = email.Contains("http") ? string.Format(ServiceConstants.PlaceLink, email) : email;

            var greeting = string.Format(ServiceConstants.SentForeignPackage, request.SalesOrders, request.TrackingNumber, sendEmailOrTel, sendEmailLink);
            var payment = string.Format(ServiceConstants.FooterPayment, request.PackageId);
            var body = string.Format(ServiceConstants.SendEmailHtmlBase, greeting, payment, ServiceConstants.RefundPolicy);

            var mailStatus = await this.omicronMailClient.SendMail(
                smtpConfig,
                request.DestinyEmail,
                string.Format(ServiceConstants.ForeignEmailSubject, request.SalesOrders),
                body,
                smtpConfig.EmailCCDelivery);

            return new ResultModel { Success = true, Code = 200, Response = mailStatus };
        }

        /// <inheritdoc/>
        public async Task<ResultModel> SendEmailLocalPackage(SendLocalPackageModel sendLocalPackage)
        {
            var smtpConfig = await this.catalogsService.GetSmtpConfig();

            var text = this.GetBodyForLocal(sendLocalPackage);
            var mailStatus = await this.omicronMailClient.SendMail(
                smtpConfig,
                sendLocalPackage.DestinyEmail,
                text.Item1,
                text.Item2,
                smtpConfig.EmailCCDelivery);

            return new ResultModel { Success = true, Code = 200, Response = mailStatus };
        }

        /// <summary>
        /// Gets the smtp config.
        /// </summary>
        /// <param name="parameters">the parameters.</param>
        /// <returns>the data.</returns>
        private SmtpConfigModel GetSmtpConfig(List<ParametersModel> parameters)
        {
            return new SmtpConfigModel
            {
                SmtpServer = parameters.FirstOrDefault(x => x.Field.Equals("SmtpServer")).Value,
                SmtpPort = int.Parse(parameters.FirstOrDefault(x => x.Field.Equals("SmtpPort")).Value),
                SmtpDefaultPassword = parameters.FirstOrDefault(x => x.Field.Equals("EmailMiddlewarePassword")).Value,
                SmtpDefaultUser = parameters.FirstOrDefault(x => x.Field.Equals("EmailMiddleware")).Value,
                EmailCCDelivery = parameters.FirstOrDefault(x => x.Field.Equals("EmailCCDelivery")).Value,
            };
        }

        /// <summary>
        /// Gets the text for the subjkect.
        /// </summary>
        /// <param name="package">the data.</param>
        /// <returns>the text.</returns>
        private Tuple<string, string> GetBodyForLocal(SendLocalPackageModel package)
        {
            var payment = string.Format(ServiceConstants.FooterPayment, package.PackageId);

            if (string.IsNullOrEmpty(package.ReasonNotDelivered))
            {
                var subject = string.Format(ServiceConstants.InWayEmailSubject, package.SalesOrders);
                var greeting = string.Format(ServiceConstants.SentLocalPackage, package.SalesOrders);
                var body = string.Format(ServiceConstants.SendEmailHtmlBase, greeting, payment, ServiceConstants.RefundPolicy);
                return new Tuple<string, string>(subject, body);
            }

            var subjectError = string.Format(ServiceConstants.PackageNotDelivered, package.SalesOrders);
            var greetingError = string.Format(ServiceConstants.PackageNotDeliveredBody, package.SalesOrders);
            var bodyError = string.Format(ServiceConstants.SendEmailHtmlBase, greetingError, payment, ServiceConstants.RefundPolicy);

            return new Tuple<string, string>(subjectError, bodyError);
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
