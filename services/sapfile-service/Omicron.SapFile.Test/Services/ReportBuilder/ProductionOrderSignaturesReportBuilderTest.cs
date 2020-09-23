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
            var reportBuilder = new ProductionOrderSignaturesReportBuilder(null);
            var outpufFilePath = @"signatures.pdf";

            // act
            reportBuilder.BuildReport(outpufFilePath);

            // assert
            Assert.IsTrue(File.Exists(outpufFilePath));
        }
    }
}
