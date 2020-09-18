// <summary>
// <copyright file="AutoFixtureProvider.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Test
{
    using System.Collections.Generic;
    using System.Linq;
    using AutoFixture;

    /// <summary>
    /// Provider of autofixture.
    /// </summary>
    public static class AutoFixtureProvider
    {
        /// <summary>
        /// Create new mock instance.
        /// </summary>
        /// <typeparam name="T">Type of mock instance.</typeparam>
        /// <returns>Mock instance.</returns>
        public static T Create<T>()
            where T : class
        {
            var fixture = new Fixture();
            return fixture.Create<T>();
        }

        /// <summary>
        /// Create new list of mock instance.
        /// </summary>
        /// <typeparam name="T">Type of mock instance.</typeparam>
        /// <param name="numberOfITems">Number of items.</param>
        /// <returns>Mock instance.</returns>
        public static List<T> CreateList<T>(int numberOfITems)
            where T : class
        {
            var fixture = new Fixture();
            return fixture.CreateMany<T>(numberOfITems).ToList();
        }
    }
}
