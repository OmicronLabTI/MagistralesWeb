// <summary>
// <copyright file="TypeExtensionsTests.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.SapAdapter.Test.Resourses.Extensions
{
    using System;
    using NUnit.Framework;
    using Omicron.SapAdapter.Resources.Extensions;

    /// <summary>
    /// Type extensions tests.
    /// </summary>
    public class TypeExtensionsTests
    {
        /// <summary>
        /// Validate numeric type is true.
        /// </summary>
        /// <param name="type">Type to validate.</param>
        [TestCase(typeof(int))]
        [TestCase(typeof(decimal))]
        [TestCase(typeof(double))]
        public void IsNumericType(Type type)
        {
            Assert.IsTrue(type.IsNumericType());
        }

        /// <summary>
        /// Validate numeric type is false.
        /// </summary>
        /// <param name="type">Type to validate.</param>
        [TestCase(typeof(string))]
        [TestCase(typeof(char))]
        public void IsNumericType_NotNumeric(Type type)
        {
            Assert.IsFalse(type.IsNumericType());
        }
    }
}
