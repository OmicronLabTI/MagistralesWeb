// <summary>
// <copyright file="DateTimeExtensionsTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Test.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Moq;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using Omicron.Pedidos.Resources.Extensions;

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
            Assert.IsTrue(dateString == date.ToString("dd/MM/yyyy"));
            Assert.IsNotNull(dateString);
            Assert.IsInstanceOf<string>(dateString);
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
            Assert.IsNotNull(dateString);
            Assert.IsInstanceOf<string>(dateString);
        }
    }
}
