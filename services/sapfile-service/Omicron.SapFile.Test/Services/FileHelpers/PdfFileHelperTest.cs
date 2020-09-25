// <summary>
// <copyright file="PdfFileHelperTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.SapFile.Test.Services.FileHelpers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Omicron.SapFile.Services.FileHelpers;

    /// <summary>
    /// Test class for <see cref="PdfFileHelper"/>.
    /// </summary>
    [TestClass]
    public class PdfFileHelperTest
    {
        /// <summary>
        /// Test method for Merge Files
        /// </summary>
        [TestMethod]
        public void MergeFiles()
        {
            // arrange
            var filesToMerge = new List<string> { @"TestFiles/Pedido.pdf", @"TestFiles/Orden.pdf", @"TestFiles/signatures.pdf", @"TestFiles/Orden_II.pdf", @"TestFiles/signatures.pdf", @"TestFiles/Orden_III.pdf", @"TestFiles/signatures.pdf", @"TestFiles/receta.pdf" };
            var outpufFilePath = @"Pedido_Orden_Merge.pdf";

            // act
            PdfFileHelper.MergePdfFiles(filesToMerge, outpufFilePath);

            // assert
            Assert.IsTrue(File.Exists(outpufFilePath));
        }

        /// <summary>
        /// Test method for Paginate file
        /// </summary>
        [TestMethod]
        public void AddPageNumber()
        {
            // arrange
            var filePath = @"TestFiles/Pedido_Orden_Merge.pdf";

            // act
            PdfFileHelper.AddPageNumber(filePath);

            // assert
            Assert.IsTrue(File.Exists(@"TestFiles/Pedido_Orden_Merge_paged.pdf"));
        }

        /// <summary>
        /// Test method for Rotate file
        /// </summary>
        [TestMethod]
        public void RotateFile()
        {
            // arrange
            var filesToRotate = new List<string> { @"TestFiles/Orden.pdf", @"TestFiles/Orden_II.pdf", @"TestFiles/Orden_III.pdf" };
            var resultFiles = new List<string> { @"TestFiles/Orden_rotate.pdf", @"TestFiles/Orden_II_rotate.pdf", @"TestFiles/Orden_III_rotate.pdf" };

            // act
            foreach (var file in filesToRotate)
            {
                PdfFileHelper.RotateFile(file);
            }

            // assert
            foreach (var file in resultFiles)
            {
                Assert.IsTrue(File.Exists(file));
            }
        }
    }
}
