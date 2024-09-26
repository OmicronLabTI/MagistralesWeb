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
            Assert.That(dateString == date.ToString("dd/MM/yyyy"));
            Assert.That(dateString, Is.Not.Null);
            Assert.That(dateString, Is.InstanceOf<string>());
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
            Assert.That(dateString, Is.Not.Null);
            Assert.That(dateString, Is.InstanceOf<string>());
        }
    }
}
