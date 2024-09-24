// <summary>
// <copyright file="ListExtensionsTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.Reporting.Test.Resourses.Extensions
{
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
            ClassicAssert.AreEqual(3, sublists.Count);
            ClassicAssert.AreEqual(3, sublists[0].Count);
            ClassicAssert.AreEqual(3, sublists[1].Count);
            ClassicAssert.AreEqual(2, sublists[2].Count);
        }
    }
}
