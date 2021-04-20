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
            var body = string.Format(ServiceConstants.SendEmailHtmlBaseAlmacen, greeting, payment, ServiceConstants.RefundPolicy);

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
            var destinityEmailList = sendLocalPackage.DestinyEmail.Split(";").Where(x => !string.IsNullOrEmpty(x)).ToList();
            var destinityEmail = destinityEmailList.FirstOrDefault();
            var copyEmails = string.Empty;
            destinityEmailList.Where(x => x != destinityEmail).Select(x => $"{x};").ToList().ForEach(x => copyEmails += x.Trim());
            copyEmails += sendLocalPackage.SalesPersonEmail != string.Empty ? $"{smtpConfig.EmailCCDelivery};{sendLocalPackage.SalesPersonEmail}" : smtpConfig.EmailCCDelivery;

            var text = this.GetBodyForLocal(sendLocalPackage);
            var mailStatus = await this.omicronMailClient.SendMail(
                smtpConfig,
                string.IsNullOrEmpty(destinityEmail) ? smtpConfig.EmailCCDelivery : destinityEmail,
                text.Item1,
                text.Item2,
                copyEmails);

            return new ResultModel { Success = true, Code = 200, Response = mailStatus };
        }

        /// <inheritdoc/>
        public async Task<ResultModel> SendEmailRejectedOrder(SendRejectedEmailModel request)
        {
            var customerServiceEmail = ServiceConstants.CustomerServiceEmail;
            var listToLook = new List<string> { customerServiceEmail };
            listToLook.AddRange(ServiceConstants.ValuesForEmail);
            var config = await this.catalogsService.GetParams(listToLook);
            var smtpConfig = this.GetSmtpConfig(config);
            var configCustomerServiceEmail = config.FirstOrDefault(x => x.Field.Equals(customerServiceEmail)).Value;
            List<ResultModel> resultados = new List<ResultModel> { };
            var rejectedOrderList = this.GetGroupsOfList(request.RejectedOrder, 3);

            foreach (var orderList in rejectedOrderList)
            {
                await Task.WhenAll(orderList.Select(async x =>
                {
                    var destinyEmail = x.DestinyEmail != string.Empty ? x.DestinyEmail : configCustomerServiceEmail;
                    var text = this.GetBodyForRejectedEmail(x);
                    var mailStatus = await this.omicronMailClient.SendMail(
                    smtpConfig,
                    destinyEmail,
                    text.Item1,
                    text.Item2,
                    configCustomerServiceEmail);
                    resultados.Add(new ResultModel { Success = true, Code = 200, Response = mailStatus });
                }));

                await Task.Delay(1000);
            }

            return new ResultModel { Success = true, Response = resultados };
        }

        /// <inheritdoc/>
        public async Task<ResultModel> SendEmailCancelDeliveryOrders(List<SendCancelDeliveryModel> request)
        {
            var listToLook = new List<string> { ServiceConstants.CustomerServiceEmail, ServiceConstants.LogisticEmailCc2Field, ServiceConstants.LogisticEmailCc3Field };
            listToLook.AddRange(ServiceConstants.ValuesForEmail);

            var config = await this.catalogsService.GetParams(listToLook);
            var smtpConfig = this.GetSmtpConfig(config);
            var customerServiceEmail = config.FirstOrDefault(x => x.Field.Equals(ServiceConstants.CustomerServiceEmail)).Value;
            var logisticEmail = config.FirstOrDefault(x => x.Field.Equals(ServiceConstants.LogisticEmailCc2Field)).Value;
            var logisticEmail2 = config.FirstOrDefault(x => x.Field.Equals(ServiceConstants.LogisticEmailCc3Field)).Value;
            List<ResultModel> results = new List<ResultModel> { };
            var deliveryLists = this.GetGroupsOfList(request, 3);

            foreach (var deliverySubList in deliveryLists)
            {
                await Task.WhenAll(deliverySubList.Select(async delivery =>
                {
                    var text = this.GetBodyForCancelDeliveryEmail(delivery);
                    var mailStatus = await this.omicronMailClient.SendMail(
                        smtpConfig,
                        logisticEmail,
                        text.Item1,
                        text.Item2,
                        $"{customerServiceEmail};{logisticEmail2}");
                    results.Add(new ResultModel { Success = true, Code = 200, Response = mailStatus });
                }));

                await Task.Delay(1000);
            }

            return new ResultModel { Success = true, Code = 200, Response = results };
        }

        /// <inheritdoc/>
        public async Task<ResultModel> SubmitIncidentsExel(List<IncidentDataModel> request)
        {
            var eb = new ExcelBuilder();
            var ms = eb.CreateIncidentExcel(request);

            var listToLook = new List<string> { ServiceConstants.EmailIncidentReport };
            listToLook.AddRange(ServiceConstants.ValuesForEmail);

            var config = await this.catalogsService.GetParams(listToLook);
            var smtpConfig = this.GetSmtpConfig(config);
            var incidentEmail = config.FirstOrDefault(x => x.Field.Contains(ServiceConstants.EmailIncidentReport));

            var dictFile = new Dictionary<string, MemoryStream>
            {
                { ms.Item2, ms.Item1 },
            };

            var mailStatus = await this.omicronMailClient.SendMail(
                smtpConfig,
                incidentEmail.Value,
                ServiceConstants.SubjectIncidentReport,
                ServiceConstants.BodyIncidentReport,
                string.Empty,
                dictFile);

            return new ResultModel { Success = true, Code = 200, Response = null };
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
            package.SalesOrders = string.IsNullOrEmpty(package.SalesOrders) ? string.Empty : package.SalesOrders;
            var orders = package.SalesOrders.Replace('[', ' ').Replace(']', ' ');

            if (string.IsNullOrEmpty(package.ReasonNotDelivered) && package.Status != ServiceConstants.Entregado)
            {
                var subject = string.Format(ServiceConstants.InWayEmailSubject, orders);
                var greeting = string.Format(ServiceConstants.SentLocalPackage, orders);
                var body = string.Format(ServiceConstants.SendEmailHtmlBaseAlmacen, greeting, payment, ServiceConstants.RefundPolicy);
                return new Tuple<string, string>(subject, body);
            }

            if (package.Status == ServiceConstants.Entregado)
            {
                var subject = string.Format(ServiceConstants.DeliveryEmailSubject, orders);
                var greeting = string.Format(ServiceConstants.SentLocalPackageDelivery, orders);
                var body = string.Format(ServiceConstants.SendEmailHtmlBaseAlmacen, greeting, payment, ServiceConstants.RefundPolicy);
                return new Tuple<string, string>(subject, body);
            }

            var subjectError = string.Format(ServiceConstants.PackageNotDelivered, orders);
            var greetingError = string.Format(ServiceConstants.PackageNotDeliveredBody, orders);
            var bodyError = string.Format(ServiceConstants.SendEmailHtmlBaseAlmacen, greetingError, payment, ServiceConstants.RefundPolicy);

            return new Tuple<string, string>(subjectError, bodyError);
        }

        /// <summary>
        /// Gets a list divided in sublists.
        /// </summary>
        /// <typeparam name="Tsource">the original list.</typeparam>
        /// <param name="listToSplit">the original list to split.</param>
        /// <param name="maxCount">the max count per group.</param>
        /// <returns>the list of list.</returns>
        private List<List<Tsource>> GetGroupsOfList<Tsource>(List<Tsource> listToSplit, int maxCount)
        {
            var listToReturn = new List<List<Tsource>>();
            var offset = 0;
            while (offset < listToSplit.Count)
            {
                var sublist = new List<Tsource>();
                sublist.AddRange(listToSplit.Skip(offset).Take(maxCount).ToList());
                listToReturn.Add(sublist);
                offset += maxCount;
            }

            return listToReturn;
        }

        /// <summary>
        /// Gets the text for the subjkect.
        /// </summary>
        /// <param name="order">the data.</param>
        /// <returns>the text.</returns>
        private Tuple<string, string> GetBodyForRejectedEmail(RejectedOrdersModel order)
        {
            var subject = string.Format(ServiceConstants.InRejectedEmailSubject, order.SalesOrders, order.CustomerName);
            var greeting = string.Format(ServiceConstants.SentRejectedOrder, order.SalesOrders, order.CustomerName);
            var commment = order.Comments != string.Empty ? string.Format(ServiceConstants.SentComentRejectedOrder, order.Comments) : string.Empty;
            var body = string.Format(ServiceConstants.SendEmailHtmlBase, greeting, commment, ServiceConstants.EmailFarewall, ServiceConstants.EmailRejectedOrderClosing);
            return new Tuple<string, string>(subject, body);
        }

        /// <summary>
        /// Gets the text for the subjkect.
        /// </summary>
        /// <param name="delivery">the data.</param>
        /// <returns>the text.</returns>
        private Tuple<string, string> GetBodyForCancelDeliveryEmail(SendCancelDeliveryModel delivery)
        {
            var subject = string.Format(ServiceConstants.InCancelDeliveryEmailSubject, delivery.DeliveryId);
            var greeting = string.Format(ServiceConstants.SentCancelDelivery, delivery.DeliveryId, delivery.SalesOrders);
            var body = string.Format(ServiceConstants.SendEmailHtmlBaseAlmacen, greeting, ServiceConstants.EmailFarewallCancelDelivery, ServiceConstants.EmailCancelDeliveryClosing);
            return new Tuple<string, string>(subject, body);
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
