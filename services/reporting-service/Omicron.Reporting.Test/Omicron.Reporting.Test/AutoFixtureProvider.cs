// <summary>
// <copyright file="AutoFixtureProvider.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Reporting.Test
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using AutoFixture;
    using Omicron.Reporting.Dtos.Model;
    using Omicron.Reporting.Entities.Model;

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
            var fixture = GetAutoFixtureInstance();
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
            var fixture = GetAutoFixtureInstance();
            return fixture.CreateMany<T>(numberOfITems).ToList();
        }

        private static Fixture GetAutoFixtureInstance()
        {
            var fixture = new Fixture();
            fixture.Customize<FileResultModel>(c => c.With(p => p.FileStream, new MemoryStream()));
            fixture.Customize<FileResultDto>(c => c.With(p => p.FileStream, new MemoryStream()));
            return fixture;
        }
    }
}
