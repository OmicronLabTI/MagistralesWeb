// <summary>
// <copyright file="SuccessFailResultsTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Warehouses.Test.Entities.Model
{
    using NUnit.Framework;
    using Omicron.Warehouses.Entities.Model;

    /// <summary>
    /// class for the test.
    /// </summary>
    [TestFixture]
    public class SuccessFailResultsTest : BaseTest
    {
        private SuccessFailResults<RawMaterialRequestModel> results;

        /// <summary>
        /// The set up.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            this.results = new SuccessFailResults<RawMaterialRequestModel>();
        }

        /// <summary>
        /// Convert Base64 string to ArrayByte.
        /// </summary>
        [Test]
        public void DistinctResults_RepeatedItems_DistinctElements()
        {
            // arrange
            var item = AutoFixtureProvider.Create<RawMaterialRequestModel>();
            this.results.AddFailedResult(item, string.Empty);
            this.results.AddFailedResult(item, string.Empty);
            this.results.AddSuccesResult(item);
            this.results.AddSuccesResult(item);

            // act
            this.results = this.results.DistinctResults();

            // assert
            Assert.AreEqual(1, this.results.Success.Count);
            Assert.AreEqual(1, this.results.Failed.Count);
        }
    }
}
