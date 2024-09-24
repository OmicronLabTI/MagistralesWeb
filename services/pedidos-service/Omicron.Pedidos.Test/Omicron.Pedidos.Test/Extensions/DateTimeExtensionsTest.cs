// <summary>
// <copyright file="DateTimeExtensionsTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Test.Extensions
{
    /// <summary>
    /// class for the test.
    /// </summary>
    [TestFixture]
    public class DateTimeExtensionsTest
    {
        /// <summary>
        /// the processs.
        /// </summary>
        [Test]
        public void FormatedDate()
        {
            // act
            var date = DateTime.Now;
            var dateString = date.FormatedDate();

            // assert
            ClassicAssert.IsTrue(dateString == date.ToString("dd/MM/yyyy"));
            ClassicAssert.IsNotNull(dateString);
            ClassicAssert.IsInstanceOf<string>(dateString);
        }

        /// <summary>
        /// the processs.
        /// </summary>
        [Test]
        public void FormatedLargeDate()
        {
            // act
            var date = DateTime.Now;
            var dateString = date.FormatedLargeDate();

            // assert
            ClassicAssert.IsNotNull(dateString);
            ClassicAssert.IsInstanceOf<string>(dateString);
        }
    }
}
