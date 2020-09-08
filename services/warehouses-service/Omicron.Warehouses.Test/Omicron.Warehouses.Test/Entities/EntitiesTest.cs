// <summary>
// <copyright file="EntitiesTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.Warehouses.Test.Entities
{
    using NUnit.Framework;
    using Omicron.Warehouses.Entities.Model;

    /// <summary>
    /// Class for tests entities.
    /// </summary>
    [TestFixture]
    public class EntitiesTest : BaseEntitiesTest
    {
        /// <summary>
        /// TypeCases.
        /// </summary>
        private static readonly object[] TypeCases =
        {
            new ResultModel(),
            new SuccessFailResults<RawMaterialRequestModel>(),
            new RawMaterialRequestModel(),
            new RawMaterialRequestDetailModel(),
        };

        /// <summary>
        /// Validate instance for type.
        /// </summary>
        /// <typeparam name="T">Generic type.</typeparam>
        /// <param name="instance">Type.</param>
        /// [AutoData]
        [Test]
        [TestCaseSource("TypeCases")]
        public void TestInstance<T>(T instance)
            where T : class
        {
            // Act
            this.TestTypes<T>(instance);
        }
    }
}
