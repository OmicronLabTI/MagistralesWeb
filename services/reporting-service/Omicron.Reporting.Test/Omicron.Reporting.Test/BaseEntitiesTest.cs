// <summary>
// <copyright file="BaseEntitiesTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.Reporting.Test
{
    /// <summary>
    /// Class for tests entities.
    /// </summary>
    public abstract class BaseEntitiesTest
    {
        /// <summary>
        /// Validate instance for type.
        /// </summary>
        /// <typeparam name="T">Generic type.</typeparam>
        /// <param name="instance">Type.</param>
        public void TestTypes<T>(T instance)
            where T : class
        {
            // Arrange
            instance = AutoFixtureProvider.Create<T>();

            // Assert
            Assert.That(IsValid(instance), Is.True);
        }

        /// <summary>
        /// Validate all properties.
        /// </summary>
        /// <typeparam name="T">Entity type.</typeparam>
        /// <param name="instance">Instance of entity.</param>
        /// <returns>Assertion result.</returns>
        internal static bool IsValid<T>(T instance)
        {
            Assert.That(instance, Is.Not.Null);
            foreach (var prop in instance.GetType().GetProperties())
            {
                Assert.That(GetPropValue(instance, prop.Name), Is.Not.Null);
            }

            return true;
        }

        /// <summary>
        /// Get property value in the object.
        /// </summary>
        /// <param name="src">Object.</param>
        /// <param name="propName">Property name.</param>
        /// <returns>Property value.</returns>
        internal static object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }
    }
}
