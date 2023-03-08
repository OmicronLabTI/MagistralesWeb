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
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using DocumentFormat.OpenXml.Spreadsheet;
    using Microsoft.EntityFrameworkCore.Internal;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;
    using Omicron.Reporting.Dtos.Model;
    using Omicron.Reporting.Entities.Model;
    using Omicron.Reporting.Services.AzureServices;
    using Omicron.Reporting.Services.Clients;
    using Omicron.Reporting.Services.Constants;
    using Omicron.Reporting.Services.ReportBuilder;
    using Omicron.Reporting.Services.SapDiApi;
    using Omicron.Reporting.Services.Utils;

    /// <summary>
    /// Implementations for request service.
    /// </summary>
    public class ReportingService : IReportingService
    {
        private readonly ICatalogsService catalogsService;
        private readonly IOmicronMailClient omicronMailClient;
        private readonly IConfiguration configuration;
        private readonly IAzureService azureService;
        private readonly ISapDiApi sapDiApi;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportingService"/> class.
        /// </summary>
        /// <param name="catalogsService">The catalogs service.</param>
        /// <param name="omicronMailClient">The email service.</param>
        /// <param name="azureService">the azure service.</param>
        /// <param name="configuration">the configuration.</param>
        /// <param name="sapDiApi">the sapdiapi.</param>
        public ReportingService(ICatalogsService catalogsService, IOmicronMailClient omicronMailClient, IConfiguration configuration, IAzureService azureService, ISapDiApi sapDiApi)
        {
            this.catalogsService = catalogsService;
            this.omicronMailClient = omicronMailClient;
            this.configuration = configuration;
            this.azureService = azureService;
            this.sapDiApi = sapDiApi.ThrowIfNull(nameof(sapDiApi));
        }

        /// <summary>
        /// Create raw material request.
        /// </summary>
        /// <param name="request">Requests data.</param>
        /// <param name="preview">Flag for preview file.</param>
        /// <returns>Report file stream.</returns>
        public FileResultModel CreateRawMaterialRequestPdf(RawMaterialRequestModel request, bool preview)
        {
            request.RequestNumber = "0123456789";
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
            var mailStatus = true;
            var createdFileNames = new List<string>();
            var fileName = string.Empty;
            var isLabelProducts = false;
            var allProducts = request.OrderedProducts;

            foreach (var category in ServiceConstants.LabelProductCategory)
            {
                isLabelProducts = category == ServiceConstants.LabelProduct;
                var products = allProducts.Where(op => op.IsLabel == isLabelProducts).ToList();
                if (products.Any())
                {
                    request.OrderedProducts = products;
                    (mailStatus, fileName) = await this.SubmitRawMaterialRequestPdfByLabelProductCategory(request, isLabelProducts);
                    createdFileNames.Add(fileName);
                }
            }

            return new ResultModel { Success = true, Code = 200, Response = mailStatus, Comments = createdFileNames.Where(x => !string.IsNullOrEmpty(x)).ToList() };
        }

        /// <inheritdoc/>
        public async Task<ResultModel> SendEmailForeignPackage(SendPackageModel request)
        {
            var emailParty = $"{request.TransportMode.ToLower().Replace(" ", string.Empty)}{ServiceConstants.DelPartyEmail}";
            var listToLook = new List<string> { emailParty, ServiceConstants.EmailLogoUrl };
            listToLook.AddRange(ServiceConstants.ValuesForEmail);

            var config = await this.catalogsService.GetParams(listToLook);

            var smtpConfig = this.GetSmtpConfig(config);
            var deliveryEmailModel = config.FirstOrDefault(x => x.Field.Contains(emailParty));
            var email = deliveryEmailModel == null ? string.Empty : deliveryEmailModel.Value;
            var logoUrl = string.Format(ServiceConstants.LogoMailHeader, config.FirstOrDefault(x => x.Field == ServiceConstants.EmailLogoUrl).Value);

            var sendEmailOrTel = email.Contains("http") ? ServiceConstants.PaqueteEmail : ServiceConstants.TelefonoEmail;
            var sendEmailLink = email.Contains("http") ? string.Format(ServiceConstants.PlaceLink, email) : email;

            var greeting = string.Format(ServiceConstants.SentForeignPackage, request.ClientName, request.SalesOrders, request.PackageId, request.TrackingNumber, sendEmailOrTel, sendEmailLink);
            var body = string.Format(ServiceConstants.SendEmailHtmlBaseAlmacen, logoUrl, greeting, string.Empty, ServiceConstants.RefundPolicy);
            var invoiceAttachment = await this.GetInvoiceAttachment(new SendLocalPackageModel { Status = ServiceConstants.Enviado, PackageId = request.PackageId });

            var destinityEmail = request.DestinyEmail.Split(";").FirstOrDefault(x => !string.IsNullOrEmpty(x)) ?? string.Empty;
            var mailStatus = await this.omicronMailClient.SendMail(
                smtpConfig,
                destinityEmail,
                string.Format(ServiceConstants.ForeignEmailSubject, request.SalesOrders),
                body,
                $"{smtpConfig.EmailCCDelivery};{request.SalesPrsonEmail}",
                invoiceAttachment);

            return new ResultModel { Success = true, Code = 200, Response = mailStatus };
        }

        /// <inheritdoc/>
        public async Task<ResultModel> SendEmailLocalPackage(SendLocalPackageModel sendLocalPackage)
        {
            var listToLook = new List<string> { ServiceConstants.EmailLogoUrl, ServiceConstants.EmailDeliveredNotDeliveredCopy, ServiceConstants.CustomerServiceEmail };
            listToLook.AddRange(ServiceConstants.ValuesForEmail);

            var config = await this.catalogsService.GetParams(listToLook);
            var smtpConfig = this.GetSmtpConfig(config);

            var customerServiceEmail = config.FirstOrDefault(x => x.Field == ServiceConstants.CustomerServiceEmail).Value;
            var logoUrl = string.Format(ServiceConstants.LogoMailHeader, config.FirstOrDefault(x => x.Field == ServiceConstants.EmailLogoUrl).Value);
            var destinityEmailList = sendLocalPackage.DestinyEmail.Split(";").Where(x => !string.IsNullOrEmpty(x)).ToList();
            var destinityEmail = destinityEmailList.FirstOrDefault();
            var copyEmails = string.Empty;
            destinityEmailList.Where(x => x != destinityEmail).Select(x => $"{x};").ToList().ForEach(x => copyEmails += x.Trim());
            copyEmails += sendLocalPackage.SalesPersonEmail != string.Empty ? $"{customerServiceEmail};{sendLocalPackage.SalesPersonEmail}" : customerServiceEmail;
            copyEmails = CommonCall.CalculateTernary(sendLocalPackage.Status == ServiceConstants.NoEntregado, $"{copyEmails};{config.FirstOrDefault(x => x.Field == ServiceConstants.EmailDeliveredNotDeliveredCopy).Value}", copyEmails);

            var text = this.GetBodyForLocal(sendLocalPackage, logoUrl);
            var invoiceAttachment = await this.GetInvoiceAttachment(sendLocalPackage);
            var mailStatus = await this.omicronMailClient.SendMail(
                smtpConfig,
                string.IsNullOrEmpty(destinityEmail) ? smtpConfig.EmailCCDelivery : destinityEmail,
                text.Item1,
                text.Item2,
                copyEmails,
                invoiceAttachment);

            await this.SendDeliveredNotDeliveredCommentsEmail(sendLocalPackage, smtpConfig, config, logoUrl);
            return new ResultModel { Success = true, Code = 200, Response = mailStatus };
        }

        /// <inheritdoc/>
        public async Task<ResultModel> SendEmailRejectedOrder(SendRejectedEmailModel request)
        {
            var listToLook = new List<string> { ServiceConstants.CustomerServiceEmail, ServiceConstants.EmailCCRejected };
            listToLook.AddRange(ServiceConstants.ValuesForEmail);

            var config = await this.catalogsService.GetParams(listToLook);
            var smtpConfig = this.GetSmtpConfig(config);
            var configCustomerServiceEmail = config.FirstOrDefault(x => x.Field.Equals(ServiceConstants.CustomerServiceEmail)).Value;
            var rejectedCc = config.FirstOrDefault(x => x.Field.Equals(ServiceConstants.EmailCCRejected)).Value;
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
                    $"{configCustomerServiceEmail};{rejectedCc}");
                    resultados.Add(new ResultModel { Success = true, Code = 200, Response = mailStatus });
                }));

                await Task.Delay(1000);
            }

            return new ResultModel { Success = true, Response = resultados };
        }

        /// <inheritdoc/>
        public async Task<ResultModel> SendEmailCancelDeliveryOrders(List<SendCancelDeliveryModel> request)
        {
            var listToLook = new List<string> { ServiceConstants.CustomerServiceEmail, ServiceConstants.LogisticEmailCc2Field, ServiceConstants.LogisticEmailCc3Field, ServiceConstants.EmailBioEqual, ServiceConstants.EmailBioElite, ServiceConstants.EmailLogoUrl };
            listToLook.AddRange(ServiceConstants.ValuesForEmail);

            var config = await this.catalogsService.GetParams(listToLook);
            var smtpConfig = this.GetSmtpConfig(config);

            var logoUrl = string.Format(ServiceConstants.LogoMailHeader, config.FirstOrDefault(x => x.Field == ServiceConstants.EmailLogoUrl).Value);
            var customerServiceEmail = config.FirstOrDefault(x => x.Field.Equals(ServiceConstants.CustomerServiceEmail)).Value;
            var logisticEmail = config.FirstOrDefault(x => x.Field.Equals(ServiceConstants.LogisticEmailCc2Field)).Value;
            var logisticEmail2 = config.FirstOrDefault(x => x.Field.Equals(ServiceConstants.LogisticEmailCc3Field)).Value;

            List<ResultModel> results = new List<ResultModel> { };
            var deliveryLists = this.GetGroupsOfList(request, 3);

            foreach (var deliverySubList in deliveryLists)
            {
                await Task.WhenAll(deliverySubList.Select(async delivery =>
                {
                    var text = this.GetBodyForCancelDeliveryEmail(delivery, logoUrl);
                    var mailStatus = await this.omicronMailClient.SendMail(
                        smtpConfig,
                        customerServiceEmail,
                        text.Item1,
                        text.Item2,
                        string.Empty);
                    results.Add(new ResultModel { Success = true, Code = 200, Response = mailStatus });

                    if (delivery.DeliveryOrderType.ToUpper().Contains(ServiceConstants.Mixto.ToUpper()))
                    {
                        mailStatus = await this.omicronMailClient.SendMail(
                            smtpConfig,
                            logisticEmail,
                            text.Item1,
                            text.Item2,
                            $"{logisticEmail2}{this.CalculateExtraCcCancelation(delivery, config)}");
                        results.Add(new ResultModel { Success = true, Code = 200, Response = mailStatus });
                    }
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

            var text = this.GetBodyForIncidentEmail(request);
            var mailStatus = await this.omicronMailClient.SendMail(
                smtpConfig,
                incidentEmail.Value,
                text.Item1,
                text.Item2,
                string.Empty,
                dictFile);

            return new ResultModel { Success = true, Code = 200, Response = mailStatus };
        }

        /// <inheritdoc/>
        public async Task<ResultModel> SendEmails(List<EmailGenericDto> emails)
        {
            var config = await this.catalogsService.GetParams(ServiceConstants.ValuesForEmail);
            var smtpConfig = this.GetSmtpConfig(config);
            var newListEmails = this.GetGroupsOfList(emails, 3);

            foreach (var orderList in newListEmails)
            {
                await Task.WhenAll(orderList.Select(async x =>
                {
                    var atachments = this.GetAtachments(x.Atachments, x.AtachmentFormat, x.AtachmentName);

                    var mailStatus = await this.omicronMailClient.SendMail(
                    smtpConfig,
                    x.DestinityEmail,
                    x.Subject,
                    x.BodyEmail,
                    x.CopyEmails,
                    atachments);
                }));

                await Task.Delay(1000);
            }

            return new ResultModel { Success = true, Code = 200, Response = true };
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
                EmailMiddlewareUser = parameters.FirstOrDefault(x => x.Field.Equals("EmailMiddlewareUser")).Value,
            };
        }

        /// <summary>
        /// Gets the text for the subjkect.
        /// </summary>
        /// <param name="package">the data.</param>
        /// <returns>the text.</returns>
        private Tuple<string, string> GetBodyForLocal(SendLocalPackageModel package, string logo)
        {
            package.SalesOrders = string.IsNullOrEmpty(package.SalesOrders) ? string.Empty : package.SalesOrders;
            var orders = package.SalesOrders.Replace('[', ' ').Replace(']', ' ').Replace("\"", string.Empty);

            var button = string.Format(ServiceConstants.ButtonEmail, package.DxpRoute);

            if (string.IsNullOrEmpty(package.ReasonNotDelivered) && package.Status != ServiceConstants.Entregado)
            {
                var subject = string.Format(ServiceConstants.InWayEmailSubject, orders);
                var greeting = string.Format(ServiceConstants.SentLocalPackage, package.ClientName, orders, button, package.PackageId);
                var body = string.Format(ServiceConstants.SendEmailHtmlBaseAlmacen, logo, greeting, string.Empty, ServiceConstants.RefundPolicy);
                return new Tuple<string, string>(subject, body);
            }

            if (package.Status == ServiceConstants.Entregado)
            {
                var subject = string.Format(ServiceConstants.DeliveryEmailSubject, orders);
                var greeting = string.Format(ServiceConstants.SentLocalPackageDelivery, package.ClientName, orders, button, package.PackageId);
                var body = string.Format(ServiceConstants.SendEmailHtmlBaseAlmacen, logo, greeting, string.Empty, ServiceConstants.RefundPolicy);
                return new Tuple<string, string>(subject, body);
            }

            var subjectError = string.Format(ServiceConstants.PackageNotDelivered, orders);
            var greetingError = string.Format(ServiceConstants.PackageNotDeliveredBody, package.ClientName, orders, button, package.PackageId);
            var bodyError = string.Format(ServiceConstants.SendEmailHtmlBaseAlmacen, logo, greetingError, string.Empty, ServiceConstants.RefundPolicy);

            return new Tuple<string, string>(subjectError, bodyError);
        }

        private async Task SendDeliveredNotDeliveredCommentsEmail(SendLocalPackageModel sendLocalPackage, SmtpConfigModel smtpConfig, List<ParametersModel> parametersModels, string logo)
        {
            if (sendLocalPackage.Status == ServiceConstants.Entregado)
            {
                var destinyEmail = $"{sendLocalPackage.SalesPersonEmail}";
                var copyEmail = $"{parametersModels.FirstOrDefault(x => x.Field == ServiceConstants.EmailDeliveredNotDeliveredCopy).Value}";

                sendLocalPackage.SalesOrders = string.IsNullOrEmpty(sendLocalPackage.SalesOrders) ? string.Empty : sendLocalPackage.SalesOrders;
                var orders = sendLocalPackage.SalesOrders.Replace('[', ' ').Replace(']', ' ').Replace("\"", string.Empty);

                var subject = string.Format(ServiceConstants.DeliveredCommentsEmailSubject, orders);
                var greeting = string.Format(ServiceConstants.DelivereCommentsBody, orders, sendLocalPackage.DeliveryName, sendLocalPackage.DeliveredComments, sendLocalPackage.PackageId);
                var body = string.Format(ServiceConstants.SendEmailHtmlBase, logo, greeting, string.Empty, string.Empty);

                await this.omicronMailClient.SendMail(
                smtpConfig,
                destinyEmail,
                subject,
                body,
                copyEmail);
            }
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
        private Tuple<string, string> GetBodyForCancelDeliveryEmail(SendCancelDeliveryModel delivery, string logo)
        {
            var salesOrder = new StringBuilder();
            delivery.SalesOrders.ForEach(x => salesOrder.Append($" {x},"));
            salesOrder.Remove(salesOrder.Length - 1, 1);
            var subject = string.Format(ServiceConstants.InCancelDeliveryEmailSubject, delivery.DeliveryId);
            var greeting = string.Format(ServiceConstants.SentCancelDelivery, delivery.DeliveryId, salesOrder.ToString());
            var body = string.Format(ServiceConstants.SendEmailHtmlBaseAlmacen, logo, greeting, ServiceConstants.EmailFarewallCancelDelivery, ServiceConstants.EmailCancelDeliveryClosing);
            return new Tuple<string, string>(subject, body);
        }

        /// <summary>
        /// Gets the text for the subjkect.
        /// </summary>
        /// <returns>the text.</returns>
        private Tuple<string, string> GetBodyForIncidentEmail(List<IncidentDataModel> request)
        {
            var endDate = DateTime.Today;
            var startDate = endDate.AddDays(-4);

            var reportIncident = "<ul>";
            request.GroupBy(x => x.Incident).ToList().ForEach(x =>
            {
                var wordIncident = x.Count() <= 1 ? "incidencia" : "incidencias";
                reportIncident += $" <li> {x.FirstOrDefault().Incident} : {x.Count()} {wordIncident} </li>";
            });
            reportIncident += "</ul>";

            var subject = string.Format(ServiceConstants.SubjectIncidentReport, endDate.ToString("dd/MM/yyyy"));
            var greeting = string.Format(ServiceConstants.BodyIncidentReport, startDate.ToString("dd/MM/yyyy"), endDate.ToString("dd/MM/yyyy"));
            var report = string.Format(ServiceConstants.ReportIncidentClosing, reportIncident);
            var body = string.Format(ServiceConstants.SendEmailIncidentHtmlBase, greeting, report);
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
        /// Send Raw Material Request Mail.
        /// </summary>
        /// <param name="transferRequestId">Transfer request id.</param>
        /// <param name="isLabel">Is label o no label.</param>
        /// <param name="pdfFiles">Prdf files to send.</param>
        /// <returns>True if sendend correctly otherwise false.</returns>
        private async Task<bool> SendRawMaterialRequestMail(int transferRequestId, bool isLabel, Dictionary<string, MemoryStream> pdfFiles)
        {
            var parameterNames = ServiceConstants.ValuesForEmail;
            var mailToParam = CommonCall.CalculateTernary(isLabel, ServiceConstants.LabelMailParam, ServiceConstants.NoLabelMailParam);
            parameterNames.Add(mailToParam);
            parameterNames.AddRange(ServiceConstants.RawMaterialRequestCopyMails);
            var parameters = await this.catalogsService.GetParams(parameterNames);
            var smtpConfig = CommonCall.CreateSmtpConfigModel(parameters);
            var bodyMail = string.Format(ServiceConstants.RawMaterialRequestEmailBody, transferRequestId);

            (string mailTo, string copyMails) = this.CalculateMailsToSendRawMaterialRequest(parameters, mailToParam);

            return await this.omicronMailClient.SendMail(
                smtpConfig,
                mailTo,
                ServiceConstants.RawMaterialRequestEmailSubject,
                bodyMail,
                copyMails,
                pdfFiles);
        }

        /// <summary>
        /// Gets the extra CC.
        /// </summary>
        /// <param name="delivery">the delivery.</param>
        /// <param name="parametersModels">the parameters.</param>
        /// <returns>the extra cc.</returns>
        private string CalculateExtraCcCancelation(SendCancelDeliveryModel delivery, List<ParametersModel> parametersModels)
        {
            var bioEliteEmail = parametersModels.FirstOrDefault(x => x.Field.Equals(ServiceConstants.EmailBioElite)).Value;
            var bioEqualEmail = parametersModels.FirstOrDefault(x => x.Field.Equals(ServiceConstants.EmailBioEqual)).Value;
            switch (delivery.DeliveryType)
            {
                case ServiceConstants.BioElite:
                    return $";{bioEliteEmail}";

                case ServiceConstants.BioEqual:
                    return $";{bioEqualEmail}";

                case ServiceConstants.Mixture:
                    return $";{bioEliteEmail};{bioEqualEmail}";

                default:
                    return string.Empty;
            }
        }

        private Dictionary<string, MemoryStream> GetAtachments(List<byte[]> atachments, string format, string name)
        {
            if (atachments == null || !atachments.Any())
            {
                return null;
            }

            var dictionaryToReturn = new Dictionary<string, MemoryStream>();
            var count = 0;
            atachments.ForEach(x =>
            {
                var stream = new MemoryStream(x);
                var fileName = string.IsNullOrEmpty(name) ? $"File{count}.{format}" : name;
                dictionaryToReturn.Add(fileName, stream);
                count++;
            });

            return dictionaryToReturn;
        }

        private async Task<Dictionary<string, MemoryStream>> GetInvoiceAttachment(SendLocalPackageModel localPackage)
        {
            if (!ServiceConstants.ValidStatusToGetInvoiceAttachment.Contains(localPackage.Status))
            {
                return null;
            }

            var dictFiles = new Dictionary<string, MemoryStream>();

            foreach (var file in new List<string> { string.Format(ServiceConstants.InvoicePdfName, localPackage.PackageId), string.Format(ServiceConstants.InvoiceXmlName, localPackage.PackageId) })
            {
                var url = $"{this.configuration[ServiceConstants.InvoicePdfAzureroute]}{file}";
                var invoicePdf = await this.azureService.GetlementFromAzure(this.configuration[ServiceConstants.AzureAccountName], this.configuration[ServiceConstants.AzureAccountKey], url);

                if (invoicePdf != null)
                {
                    var ms = new MemoryStream();
                    invoicePdf.Content.CopyTo(ms);
                    ms.Position = 0;
                    dictFiles.Add($"{file}", ms);
                }
            }

            return dictFiles;
        }

        /// <summary>
        /// Calculate Mails To Send Raw Material Request.
        /// </summary>
        /// <param name="parameters">Parameters.</param>
        /// <param name="mailToParam">Mailto param.</param>
        /// <returns>Emails.</returns>
        private (string, string) CalculateMailsToSendRawMaterialRequest(List<ParametersModel> parameters, string mailToParam)
        {
            var mailTo = parameters.Where(p => p.Field == mailToParam).Select(p => p.Value).ToList();
            var copyMails = parameters.Where(p => ServiceConstants.RawMaterialRequestCopyMails.Contains(p.Field)).Select(p => p.Value).ToList();
            return (string.Join(";", mailTo), string.Join(";", copyMails));
        }

        /// <summary>
        /// Submit Raw Material Request Pdf By Label Product Category.
        /// </summary>
        /// <param name="request">request.</param>
        /// <param name="isLabel">is label.</param>
        /// <returns>Result.</returns>
        private async Task<(bool, string)> SubmitRawMaterialRequestPdfByLabelProductCategory(RawMaterialRequestModel request, bool isLabel)
        {
            (bool isSuccessful, int transferRequestId) = await this.CreateRawMaterialRequestOnDiApi(request);

            if (!isSuccessful)
            {
                return (false, string.Empty);
            }

            request.RequestNumber = transferRequestId.ToString();
            var file = this.BuildPdfFile(request, false);
            var pdfFiles = new Dictionary<string, MemoryStream>
            {
                { file.FileName, file.FileStream },
            };

            var mailStatus = await this.SendRawMaterialRequestMail(transferRequestId, isLabel, pdfFiles);
            return (mailStatus, file.FileName);
        }

        /// <summary>
        /// Create Raw Material Request On DiApi.
        /// </summary>
        /// <param name="request">Request.</param>
        /// <returns>Result.</returns>
        private async Task<(bool, int)> CreateRawMaterialRequestOnDiApi(RawMaterialRequestModel request)
        {
            var isSuccessful = false;
            var transferRequestId = 0;

            var transferRequestToDiApi = new TransferRequestHeaderDto
            {
                UserInfo = $"{request.SigningUserName}-{request.SigningUserId}",
                TransferRequestDetail = request.OrderedProducts
                    .Select(op => new TransferRequestDetailDto
                    {
                        ItemCode = op.ProductId,
                        Quantity = decimal.ToDouble(op.RequestQuantity),
                        SourceWarehosue = ServiceConstants.WareHouseMp,
                        TargetWarehosue = CommonCall.CalculateTernary(op.Warehouse == ServiceConstants.WarehouseDz, ServiceConstants.WarehouseMg, op.Warehouse),
                    }).ToList(),
            };

            var response = await this.sapDiApi.PostToSapDiApi(new List<TransferRequestHeaderDto> { transferRequestToDiApi }, ServiceConstants.EndpointCreateTransferRequest);
            var result = JsonConvert.DeserializeObject<List<TransferRequestResult>>(response.Response.ToString());

            var rawMaterialResponse = result.FirstOrDefault(x => string.IsNullOrEmpty(x.Error));

            if (rawMaterialResponse != null)
            {
                return (true, rawMaterialResponse.TransferRequestId);
            }

            return (isSuccessful, transferRequestId);
        }
    }
}
