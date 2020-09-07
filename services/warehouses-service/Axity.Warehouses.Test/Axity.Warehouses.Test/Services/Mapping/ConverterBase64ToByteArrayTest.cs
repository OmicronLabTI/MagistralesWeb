// <summary>
// <copyright file="ConverterBase64ToByteArrayTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Warehouses.Test.Services.Mapping
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Axity.Warehouses.DataAccess.DAO.Request;
    using Axity.Warehouses.Entities.Context;
    using Axity.Warehouses.Entities.Model;
    using Axity.Warehouses.Services.Mapping;
    using Axity.Warehouses.Test;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using Omicron.Warehouses.Services.Request;

    /// <summary>
    /// class for the test.
    /// </summary>
    [TestFixture]
    public class ConverterBase64ToByteArrayTest : BaseTest
    {
        private ConverterBase64ToByteArray converter;

        /// <summary>
        /// The set up.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            this.converter = new ConverterBase64ToByteArray();
        }

        /// <summary>
        /// Convert Base64 string to ArrayByte.
        /// </summary>
        [Test]
        public void Convert_Base64Image_ArrayByte()
        {
            // act
            var result = this.converter.Convert(File.ReadAllText("SignatureBase64.txt"), null);

            // assert
            Assert.IsFalse(result.SequenceEqual(new byte[0]));
        }

        /// <summary>
        /// Convert empty string to ArrayByte.
        /// </summary>
        [Test]
        public void Convert_EmptyString_EmptyArrayByte()
        {
            // act
            var result = this.converter.Convert(string.Empty, null);

            // assert
            Assert.IsTrue(result.SequenceEqual(new byte[0]));
        }
    }
}
