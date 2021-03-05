// <summary>
// <copyright file="IReportingFacade.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.Reporting.Facade.Request
{
    using System.Threading.Tasks;
    using Omicron.Reporting.Dtos.Model;

    /// <summary>
    /// interfaces for the reporting facade.
    /// </summary>
    public interface IReportingFacade
    {
        /// <summary>
        /// Create raw material request.
        /// </summary>
        /// <param name="request">Requests data.</param>
        /// <param name="preview">Flag for preview file.</param>
        /// <returns>Report file stream.</returns>
        FileResultDto CreateRawMaterialRequestPdf(RawMaterialRequestDto request, bool preview);

        /// <summary>
        /// Submit raw material request.
        /// </summary>
        /// <param name="request">Requests data.</param>
        /// <returns>Operation result.</returns>
        Task<ResultDto> SubmitRawMaterialRequestPdf(RawMaterialRequestDto request);

        /// <summary>
        /// Sends the email.
        /// </summary>
        /// <param name="request">the request.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> SendEmailForeignPackage(SendPackageDto request);

        /// <summary>
        /// Sends the email for local packages.
        /// </summary>
        /// <param name="sendLocalPackage">the local package.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> SendEmailLocalPackage(SendLocalPackageDto sendLocalPackage);

        /// <summary>
        /// Send mail for every rejected order.
        /// </summary>
        /// <param name="request">Requests data.</param>
        /// <returns>Operation result.</returns>
        Task<ResultDto> SendEmailRejectedOrder(SendRejectedEmailDto request);
    }
}
