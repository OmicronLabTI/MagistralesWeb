// <summary>
// <copyright file="BaseTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Catalogos.Test
{
    using System.Collections.Generic;
    using Omicron.Catalogos.Entities.Model;

    /// <summary>
    /// Class Base Test.
    /// </summary>
    public abstract class BaseTest
    {
        /// <summary>
        /// List of Users.
        /// </summary>
        /// <returns>IEnumerable Users.</returns>
        public IEnumerable<UserModel> GetAllUsers()
        {
            return new List<UserModel>()
            {
                new UserModel { Id = 1, FirstName = "Alejandro", LastName = "Ojeda", Email = "alejandro.ojeda@axity.com" },
                new UserModel { Id = 2, FirstName = "Jorge", LastName = "Morales", Email = "jorge.morales@axity.com" },
                new UserModel { Id = 3, FirstName = "Arturo", LastName = "Miranda", Email = "arturo.miranda@axity.com" },
                new UserModel { Id = 4, FirstName = "Benjamin", LastName = "Galindo", Email = "benjamin.galindo@axity.com" },
            };
        }
    }
}
