// <summary>
// <copyright file="ISapFileService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapFile.Services.SapFile
{
    using CrystalDecisions.CrystalReports.Engine;
    using CrystalDecisions.Shared;
    using Omicron.SapFile.Entities.Models;
    using Omicron.SapFile.Log;
    using Omicron.SapFile.Services.FileHelpers;
    using Omicron.SapFile.Services.ReportBuilder;
    using Omicron.SapFile.Services.Utils;
    using Org.BouncyCastle.Asn1.Esf;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;

    /// <summary>
    /// Class to create pdfs.
    /// </summary>
    public class SapFileService : ISapFileService
    {
        private readonly ILoggerProxy _loggerProxy;

        private readonly string Server;

        private readonly string User;

        private readonly string Pwd;

        private readonly string DataBase;

        private readonly string ProductionDirectoryPath;

        private readonly string TemporalDirectoryPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="SapFileService"/> class.
        /// </summary>
        public SapFileService(ILoggerProxy loggerProxy)
        {
            this._loggerProxy = loggerProxy;
            this.Server = ConfigurationManager.AppSettings["SapServer"];
            this.DataBase = ConfigurationManager.AppSettings["SapDb"];
            this.User = ConfigurationManager.AppSettings["Usuario"];
            this.Pwd = ConfigurationManager.AppSettings["UserPwd"];
            this.ProductionDirectoryPath = ConfigurationManager.AppSettings["ProductionFiles"];
            this.TemporalDirectoryPath = ConfigurationManager.AppSettings["PdfCreated"];
        }

        /// <summary>
        /// Creates the pdfs.
        /// </summary>
        /// <param name="finalizaGeneratePdfs">the data to create.</param>
        /// <returns>the data.</returns>
        public async Task<ResultModel> CreatePdfs(List<FinalizaGeneratePdfModel> finalizaGeneratePdfs)
        {
            var dictResult = new Dictionary<string, string>();
            try
            {
                var dictOrdersCreated = new Dictionary<int, int>();
                finalizaGeneratePdfs.ForEach(order =>
                {
                    try
                    {
                        if (order.OrderId != 0 && !dictOrdersCreated.ContainsKey(order.OrderId))
                        {
                            order.OrderPdfRoute = this.CreateOrderReport(order.OrderId);
                            dictOrdersCreated.Add(order.OrderId, order.OrderId);
                        }

                        if (order.FabOrderId != 0)
                        {
                            order.FabOrderPdfRoute = this.CreateFabOrderReport(order.FabOrderId);
                        }
                    }
                    catch(Exception ex)
                    {
                        this._loggerProxy.Error(ex.Message, ex);
                        dictResult.Add($"{order.OrderId}-{order.FabOrderId}", "ErrorCreatePdf");
                    }                    
                });

                finalizaGeneratePdfs.Where(x => x.OrderId.Equals(0)).ToList().ForEach(order =>
                {
                    var filePath = this.CreateFabOrderReportWithSignatures(order, true);
                    this._loggerProxy.Debug($"Create file for production order: {filePath}.");
                });

                var groupedOrders = finalizaGeneratePdfs.Where(order => order.OrderId != 0).GroupBy(order => order.OrderId);
                groupedOrders.ToList().ForEach(x => {
                    var filePath = this.CreateSalesOrderReportWithProductionOrders(x.ToList());
                    this._loggerProxy.Debug($"Create file for sales order: {filePath}.");
                });
            }
            catch(Exception ex)
            {
                this._loggerProxy.Error(ex.Message, ex);
                dictResult.Add("Error Procesar Pdf - Error Procesar Pdf", "ErrorCreatePdf");
                return ServiceUtils.CreateResult(true, 200, null, dictResult, ex.StackTrace);
            }

            return ServiceUtils.CreateResult(true, 200, null, dictResult, null);
        }

        /// <summary>
        /// Creates the report for an order.
        /// </summary>
        /// <param name="orderId">the order id.</param>
        /// <returns>the name and route of the file.</returns>
        private string CreateOrderReport(int orderId)
        {
            var report = new ReportDocument();
            var localRoute = ConfigurationManager.AppSettings["PedidoRtp"];

            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            var root = Directory.GetCurrentDirectory();
            root += localRoute;
            
            report.Load(root);
            report.DataSourceConnections[0].SetConnection(this.Server, this.DataBase, this.User, this.Pwd);

            report.SetParameterValue("DocKey@", orderId);
            report.SetParameterValue("ObjectId@", 17);

            var name = $"Order{orderId}.pdf";
            var route = ConfigurationManager.AppSettings["PdfCreated"];
            var completeRoute = @route + name;
            this.CreatePdf(report, completeRoute);
            return completeRoute;
        }

        /// <summary>
        /// Creates the report for a fabricacion orders.
        /// </summary>
        /// <param name="orderId">the order.</param>
        /// <returns>the data.</returns>
        private string CreateFabOrderReport(int orderId)
        {
            var report = new ReportDocument();
            var localRoute = ConfigurationManager.AppSettings["OrderRtp"];

            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            var root = Directory.GetCurrentDirectory();
            root += localRoute;

            report.Load(root);
            report.DataSourceConnections[0].SetConnection(this.Server, this.DataBase, this.User, this.Pwd);

            report.SetParameterValue("DocKey@", orderId);            

            var name = $"FabOrder{orderId}.pdf";
            var route = ConfigurationManager.AppSettings["PdfCreated"];
            var completeRoute = @route + name;
            this.CreatePdf(report, completeRoute);
            return completeRoute;
        }

        /// <summary>
        /// Creates the pdf
        /// </summary>
        /// <param name="report">the rport.</param>
        /// <param name="name">where is going to be stored.</param>
        private void CreatePdf(ReportDocument report, string name)
        {
            ExportOptions CrExportOptions;
            DiskFileDestinationOptions CrDiskFileDestinationOptions = new DiskFileDestinationOptions();
            PdfRtfWordFormatOptions CrFormatTypeOptions = new PdfRtfWordFormatOptions();
            CrDiskFileDestinationOptions.DiskFileName = name;
            CrExportOptions = report.ExportOptions;
            {
                CrExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                CrExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                CrExportOptions.DestinationOptions = CrDiskFileDestinationOptions;
                CrExportOptions.FormatOptions = CrFormatTypeOptions;
            }

            report.Export();
            report.Close();
            report.Dispose();
        }

        /// <summary>
        /// Create report for fabricacion order with signatures.
        /// </summary>
        /// <param name="order">the order.</param>
        /// <param name="finalReport">flag for final report (pagen and copy to production files).</param>
        /// <returns>the file path.</returns>
        private string CreateFabOrderReportWithSignatures(FinalizaGeneratePdfModel order, bool finalReport)
        {
            var signaturesFilePath = Path.Combine(TemporalDirectoryPath, $"{order.FabOrderId}_op_signatures.pdf");
            var mergedFilePath = Path.Combine(TemporalDirectoryPath, $"{order.FabOrderId}_op_merged.pdf");

            var reportBuilder = new ProductionOrderSignaturesReportBuilder(order);
            reportBuilder.BuildReport(signaturesFilePath);

            var filePaths = new List<string> { order.FabOrderPdfRoute, signaturesFilePath };
            filePaths = filePaths.Where(x => File.Exists(x)).ToList();

            PdfFileHelper.MergePdfFiles(filePaths, mergedFilePath);
            ServiceUtils.DeleteFile(order.FabOrderPdfRoute, signaturesFilePath);

            if (finalReport)
            {
                var pagedFilePath = PdfFileHelper.AddPageNumber(mergedFilePath);
                ServiceUtils.DeleteFile(mergedFilePath);
                return this.CopyFileToProductionFirectory(pagedFilePath, order.CreateDate, $"{order.ItemCode}_{order.FabOrderId}.pdf");
            }
            return mergedFilePath;
        }

        /// <summary>
        /// Create report for sales order with production orders.
        /// </summary>
        /// <param name="orders">the orders.</param>
        /// <returns>the file path.</returns>
        private string CreateSalesOrderReportWithProductionOrders(List<FinalizaGeneratePdfModel> orders)
        {
            var first = orders.First();
            var recipeRoute = orders.Select(x => x.RecipeRoute).FirstOrDefault(x => !string.IsNullOrEmpty(x));
            var orderSapPdf = orders.Select(x => x.OrderPdfRoute).FirstOrDefault(x => !string.IsNullOrEmpty(x));
            var mergedFilePath = Path.Combine(TemporalDirectoryPath, $"{first.OrderId}_ov_merged.pdf");
            orders = orders.OrderBy(x => x.FabOrderId).ToList();

            var filePaths = new List<string>() { orderSapPdf };
            orders.ForEach(x =>
            {
                filePaths.Add(this.CreateFabOrderReportWithSignatures(x, false));
            });
            filePaths.Add(recipeRoute);
            filePaths = filePaths.Where(x => File.Exists(x)).ToList();

            PdfFileHelper.MergePdfFiles(filePaths, mergedFilePath);
            var pagedFilePath = PdfFileHelper.AddPageNumber(mergedFilePath);

            filePaths.Add(mergedFilePath);
            filePaths.Remove(recipeRoute);
            ServiceUtils.DeleteFile(filePaths.ToArray());

            return this.CopyFileToProductionFirectory(pagedFilePath, first.SaleOrderCreateDate, $"{first.OrderId}_{first.MedicName}.pdf");
        }

        /// <summary>
        /// Copy file to production directory.
        /// </summary>
        /// <param name="src">Source file path.</param>
        /// <param name="datetimeAsString">Date to format directory name.</param>
        /// <param name="fileName">File name.</param>
        /// <returns>Final file path.</returns>
        private string CopyFileToProductionFirectory(string src, string datetimeAsString, string fileName)
        {
            var dateArray = datetimeAsString.Split('/');
            var datetime = new DateTime(int.Parse(dateArray[2]), int.Parse(dateArray[1]), int.Parse(dateArray[0]));
            var directoryName = datetime.ToString("MMMMyyyy", CultureInfo.GetCultureInfo("es-MX"));
            directoryName = char.ToUpper(directoryName[0]) + directoryName.Substring(1);
            var finalPath = Path.Combine(this.ProductionDirectoryPath, $@"{directoryName}\{fileName}");
            ServiceUtils.CopyFile(src, finalPath);
            ServiceUtils.DeleteFile(src);
            return finalPath;
        }
    }
}
