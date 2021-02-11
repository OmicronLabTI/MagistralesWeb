// <summary>
// <copyright file="DictionaryExtensionTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.SapAdapter.Test.Resourses.Extensions
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using Omicron.SapAdapter.Resources.Extensions;

    /// <summary>
    /// Dictionary extensions tests.
    /// </summary>
    public class DictionaryExtensionTest
    {
        /// <summary>
        /// Get numeric values or defaults.
        /// </summary>
        /// <param name="key">Element key.</param>
        /// <param name="defaultValue">Default value.</param>
        /// <param name="expectedResult">Expected result.</param>
        [TestCase("offset", 0, 20)]
        [TestCase("limit", 0, 10)]
        [TestCase("offset2", 1, 1)]
        [TestCase("limit2", 2, 2)]
        public void TryGet_NumericValues(string key, int defaultValue, int expectedResult)
        {
            // Arrange
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "offset",  "20" },
                { "limit",  "10" },
                { "offset2",  "invalid" },
                { "limit2",  "invalid" },
            };

            // Act
            var parseOperationResult = parameters.TryGet<string, string, int>(key, defaultValue, out int result);

            // Assert
            Assert.IsTrue(parseOperationResult);
            Assert.AreEqual(expectedResult, result);
        }

        /// <summary>
        /// Get numeric values.
        /// </summary>
        /// <param name="key">Element key.</param>
        /// <param name="expectedResult">Expected result.</param>
        [TestCase("offset", 20)]
        [TestCase("limit", 10)]
        public void Get_NumericValues(string key, int expectedResult)
        {
            // Arrange
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "offset",  "20" },
                { "limit",  "10" },
            };

            // Act
            var result = parameters.Get<string, string, int>(key);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        /// <summary>
        /// Get numeric values.
        /// </summary>
        /// <param name="key">Element key.</param>
        /// <param name="expectedResult">Expected result.</param>
        [TestCase("offset", 0)]
        public void Get_NumericValuesErrorValueZero(string key, int expectedResult)
        {
            // Arrange
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "offset",  string.Empty },
            };

            // Act
            var result = parameters.Get<string, string, int>(key);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        /// <summary>
        /// Get numeric values.
        /// </summary>
        /// <param name="key">Element key.</param>
        /// <param name="value">value set.</param>
        [TestCase("offset", 20)]
        public void Get_NumericValuesError(string key, int value)
        {
            // Arrange
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "aaa",  string.Empty },
                { "nnnn",  string.Empty },
            };

            // Act
            Assert.Throws<KeyNotFoundException>(() => parameters.Get<string, string, int>(key));
        }

        /// <summary>
        /// Decode query string parameters.
        /// </summary>
        /// <param name="key">Element key.</param>
        /// <param name="encodeValue">Encode value.</param>
        /// <param name="expectedResult">Expected result.</param>
        [TestCase("chips", "%C3%9C", "Ü")]
        [TestCase("chips", "%C3%BC", "ü")]
        public void DecodeQueryString(string key, string encodeValue, string expectedResult)
        {
            // Arrange
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                { key,  encodeValue },
            };

            // Act
            var result = parameters.DecodeQueryString();

            // Assert
            Assert.AreEqual(expectedResult, result[key]);
        }
    }
}
