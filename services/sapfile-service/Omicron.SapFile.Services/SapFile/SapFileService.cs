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
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
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
        }

        /// <summary>
        /// Creates the pdfs.
        /// </summary>
        /// <param name="finalizaGeneratePdfs">the data to create.</param>
        /// <returns>the data.</returns>
        public async Task<ResultModel> CreatePdfs(List<FinalizaGeneratePdfModel> finalizaGeneratePdfs)
        {
            try
            {
                var dictOrdersCreated = new Dictionary<int, int>();
                finalizaGeneratePdfs.ForEach(order =>
                {
                    if (order.OrderId != 0 && !dictOrdersCreated.ContainsKey(order.OrderId))
                    {
                        order.OrderPdfRoute = this.CreateOrderReport(order.OrderId);
                        dictOrdersCreated.Add(order.OrderId, order.OrderId);
                    }

                    order.FabOrderPdfRoute = this.CreateFabOrderReport(order.FabOrderId);
                });
            }
            catch(Exception ex)
            {
                return new ResultModel();
            }

            //To do Send to method to concatenate.
            return new ResultModel();
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
    }
}
