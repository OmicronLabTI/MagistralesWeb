// <summary>
// <copyright file="ReportingController.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Reporting.Api.Controllers
{
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Net.Http.Headers;
    using Omicron.Reporting.Dtos.Model;
    using Omicron.Reporting.Facade.Request;

    /// <summary>
    /// Class reporting Controller.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ReportingController : ControllerBase
    {
        private readonly IReportingFacade reportingFacade;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportingController"/> class.
        /// </summary>
        /// <param name="reportingFacade">The reporting facade.</param>
        public ReportingController(IReportingFacade reportingFacade)
        {
            this.reportingFacade = reportingFacade;
        }

        /// <summary>
        /// Create file preview of raw material request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Report file stream.</returns>
        [Route("/preview/request/rawmaterial/pdf")]
        [HttpPost]
        public FileStreamResult GetRawMaterialRequestPdfPreview(RawMaterialRequestDto request)
        {
            var report = this.reportingFacade.CreateRawMaterialRequestPdf(request, true);

            return new FileStreamResult(report.FileStream, new MediaTypeHeaderValue("application/pdf"))
            {
                FileDownloadName = report.FileName,
            };
        }

        /// <summary>
        /// Submit file of raw material request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Operation result.</returns>
        [Route("/submit/request/rawmaterial/pdf")]
        [HttpPost]
        public async Task<IActionResult> SubmitRawMaterialRequestPdf(RawMaterialRequestDto request)
        {
            var response = await this.reportingFacade.SubmitRawMaterialRequestPdf(request);
            return this.Ok(response);
        }

        /// <summary>
        /// Submit file of raw material request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Operation result.</returns>
        [Route("/foreign/package/email")]
        [HttpPost]
        public async Task<IActionResult> SendEmailForeignPackage(SendPackageDto request)
        {
            var response = await this.reportingFacade.SendEmailForeignPackage(request);
            return this.Ok(response);
        }

        /// <summary>
        /// Submit file of raw material request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Operation result.</returns>
        [Route("/local/package/email")]
        [HttpPost]
        public async Task<IActionResult> SendEmailLocalPackage(SendLocalPackageDto request)
        {
            var response = await this.reportingFacade.SendEmailLocalPackage(request);
            return this.Ok(response);
        }

        /// <summary>
        /// Send mail for every rejected order.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Operation result.</returns>
        [Route("/rejection/order/email")]
        [HttpPost]
        public async Task<IActionResult> SendEmailRejectedOrder(SendRejectedEmailDto request)
        {
           var response = await this.reportingFacade.SendEmailRejectedOrder(request);
           return this.Ok(response);
        }

        /// <summary>
        /// Send mail when orders of a delivery are canceled.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Operation result.</returns>
        [Route("/cancel/delivery/email")]
        [HttpPost]
        public async Task<IActionResult> SendEmailCancelDeliveryOrders(List<SendCancelDeliveryDto> request)
        {
            var response = await this.reportingFacade.SendEmailCancelDeliveryOrders(request);
            return this.Ok(response);
        }

        /// <summary>
        /// Submit file of raw material request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Operation result.</returns>
        [Route("/submit/incidents/exel")]
        [HttpPost]
        public async Task<IActionResult> SubmitIncidentsExel(List<IncidentDataDto> request)
        {
            var response = await this.reportingFacade.SubmitIncidentsExel(request);
            return this.Ok(response);
        }

        /// <summary>
        /// Method Ping.
        /// </summary>
        /// <returns>Pong.</returns>
        [Route("/ping")]
        [HttpGet]
        public IActionResult Ping()
        {
            return this.Ok("Pong");
        }
    }
}