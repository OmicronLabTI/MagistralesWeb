// <summary>
// <copyright file="IReportingService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Reporting.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Omicron.Reporting.Dtos.Model;
    using Omicron.Reporting.Entities.Model;

    /// <summary>
    /// Contract for request service.
    /// </summary>
    public interface IReportingService
    {
        /// <summary>
        /// Create raw material request.
        /// </summary>
        /// <param name="request">Requests data.</param>
        /// <param name="preview">Flag for preview file.</param>
        /// <returns>Report file stream.</returns>
        FileResultModel CreateRawMaterialRequestPdf(RawMaterialRequestModel request, bool preview);

        /// <summary>
        /// Submit raw material request.
        /// </summary>
        /// <param name="request">Requests data.</param>
        /// <returns>Operation result.</returns>
        Task<ResultModel> SubmitRawMaterialRequestPdf(RawMaterialRequestModel request);

        /// <summary>
        /// Sends the emial to the receiver.
        /// </summary>
        /// <param name="request">the request.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> SendEmailForeignPackage(SendPackageModel request);

        /// <summary>
        /// Sends the email for local packages.
        /// </summary>
        /// <param name="sendLocalPackage">the data.</param>
        /// <returns>the data to rturn.</returns>
        Task<ResultModel> SendEmailLocalPackage(SendLocalPackageModel sendLocalPackage);

        /// <summary>
        /// Submit email.
        /// </summary>
        /// <param name="request">Requests data.</param>
        /// <returns>Operation result.</returns>
        Task<ResultModel> SendEmailRejectedOrder(SendRejectedEmailModel request);

        /// <summary>
        /// Send mail when orders of a delivery are canceled.
        /// </summary>
        /// <param name="request">Requests data.</param>
        /// <returns>Operation result.</returns>
        Task<ResultModel> SendEmailCancelDeliveryOrders(List<SendCancelDeliveryModel> request);

        /// <summary>
        /// Send mail when orders of a delivery are canceled.
        /// </summary>
        /// <param name="request">Requests data.</param>
        /// <returns>Operation result.</returns>
        Task<ResultModel> SubmitIncidentsExel(List<IncidentDataModel> request);

        /// <summary>
        /// Method To send Emails.
        /// </summary>
        /// <param name="emails">the emails to send.</param>
        /// <returns>Pong.</returns>
        Task<ResultModel> SendEmails(List<EmailGenericDto> emails);
    }
}
