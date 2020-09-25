// <summary>
// <copyright file="ProductionOrderSignaturesReportBuilderTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.SapFile.Test.Services.FileHelpers
{
    using System.IO;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Omicron.SapFile.Entities.Models;
    using Omicron.SapFile.Services.ReportBuilder;

    /// <summary>
    /// Test class for <see cref="ProductionOrderSignaturesReportBuilderTest"/>.
    /// </summary>
    [TestClass]
    public class ProductionOrderSignaturesReportBuilderTest
    {
        /// <summary>
        /// Test method for build report
        /// </summary>
        [TestMethod]
        public void BuildReport()
        {
            // arrange
            var order = new FinalizaGeneratePdfModel();
            order.OrderId = 10;
            order.FabOrderId = 10;
            order.QfbName = "Nombre del QFB";
            order.QfbSignature= File.ReadAllBytes(@"TestFiles/signature.png");
            order.TechnicalSignature = File.ReadAllBytes(@"TestFiles/signature.png");

            var reportBuilder = new ProductionOrderSignaturesReportBuilder(order);
            var outpufFilePath = @"signatures.pdf";

            // act
            reportBuilder.BuildReport(outpufFilePath);

            // assert
            Assert.IsTrue(File.Exists(outpufFilePath));
        }

        /// <summary>
        /// Test method for build report
        /// </summary>
        [TestMethod]
        public void BuildReport_IsolatedPO()
        {
            // arrange
            var order = new FinalizaGeneratePdfModel();
            order.OrderId = 0;
            order.FabOrderId = 10;
            order.QfbName = "Nombre del QFB";
            order.QfbSignature = File.ReadAllBytes(@"TestFiles/signature.png");
            order.TechnicalSignature = File.ReadAllBytes(@"TestFiles/signature.png");

            var reportBuilder = new ProductionOrderSignaturesReportBuilder(order);
            var outpufFilePath = @"signatures_isolated_po.pdf";

            // act
            reportBuilder.BuildReport(outpufFilePath);

            // assert
            Assert.IsTrue(File.Exists(outpufFilePath));
        }
    }
}
