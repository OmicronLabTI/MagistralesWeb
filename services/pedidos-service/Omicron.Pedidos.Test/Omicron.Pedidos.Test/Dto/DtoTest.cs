// <summary>
// <copyright file="DtoTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.Pedidos.Test.Dto
{
    using AutoFixture;
    using NUnit.Framework;
    using Omicron.Pedidos.Dtos.Models;
    using Omicron.Pedidos.Dtos.User;

    /// <summary>
    /// Class for tests entities.
    /// </summary>
    [TestFixture]
    public class DtoTest
    {
        /// <summary>
        /// TypeCases.
        /// </summary>
        private static readonly object[] TypeCases =
        {
            new AssignBatchDto(),
            new AutomaticAssingDto(),
            new BatchesConfigurationDto(),
            new CloseProductionOrderDto(),
            new CompleteDetalleFormulaDto(),
            new ComponentCustomComponentListDto(),
            new CreateIsolatedFabOrderDto(),
            new CustomComponentListDto(),
            new FinishOrderDto(),
            new ManualAssignDto(),
            new OrderIdDto(),
            new ProcessByOrderDto(),
            new ProcessOrderDto(),
            new ResultDto(),
            new UpdateFormulaDto(),
            new UpdateOrderCommentsDto(),
            new UpdateOrderSignatureDto(),
            new UpdateStatusOrderDto(),
            new UserActionDto<CustomComponentListDto>(),
            new UserDto(),
        };

        /// <summary>
        /// The fixture.
        /// </summary>
        private readonly Fixture fixture = new Fixture();

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
            // Arrange
            instance = this.fixture.Create<T>();

            // Assert
            Assert.IsTrue(IsValid(instance));
        }

        private static bool IsValid<T>(T instance)
        {
            Assert.IsNotNull(instance);
            foreach (var prop in instance.GetType().GetProperties())
            {
                Assert.IsNotNull(GetPropValue(instance, prop.Name));
            }

            return true;
        }

        private static object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }
    }
}
