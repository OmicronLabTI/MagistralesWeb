// <summary>
// <copyright file="StringExtensionTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.Warehouses.Test.Resourses.Extensions
{
    /// <summary>
    /// Type extensions tests.
    /// </summary>
    public class StringExtensionTest
    {
        /// <summary>
        /// Test conversion string to int list.
        /// </summary>
        /// <param name="baseString">String to convert.</param>
        /// <param name="expectedItems">Expected elements.</param>
        [TestCase("", 0)]
        [TestCase("1,2,,3", 3)]
        public void ToIntList(string baseString, int expectedItems)
        {
            Assert.That(baseString.ToIntList().Count, Is.EqualTo(expectedItems));
        }
    }
}
