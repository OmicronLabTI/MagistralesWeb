// <summary>
// <copyright file="ListExtensionsTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.Reporting.Test.Resourses.Extensions
{
    using System.Collections.Generic;
    using NUnit.Framework;
    using Omicron.Reporting.Resources.Extensions;

    /// <summary>
    /// Type extensions tests.
    /// </summary>
    public class ListExtensionsTest
    {
        /// <summary>
        /// Split list.
        /// </summary>
        [Test]
        public void Split()
        {
            // arrange
            var intList = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8 };

            // act
            var sublists = intList.Split(3);

            // assert
            Assert.AreEqual(3, sublists.Count);
            Assert.AreEqual(3, sublists[0].Count);
            Assert.AreEqual(3, sublists[1].Count);
            Assert.AreEqual(2, sublists[2].Count);
        }
    }
}
