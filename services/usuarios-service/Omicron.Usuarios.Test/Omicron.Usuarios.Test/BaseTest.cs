// <summary>
// <copyright file="BaseTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Usuarios.Test
{
    using System;
    using System.Collections.Generic;
    using Omicron.Usuarios.Dtos.User;
    using Omicron.Usuarios.Entities.Model;

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
                new UserModel { Id = "1", FirstName = "Alejandro", LastName = "Ojeda", UserName = "Alex", Password = "abc", Role = 1 },
                new UserModel { Id = "2", FirstName = "Jorge", LastName = "Morales", UserName = "George", Password = "abc", Role = 1 },
                new UserModel { Id = "3", FirstName = "Arturo", LastName = "Miranda", UserName = "Artuhr", Password = "abc", Role = 1 },
                new UserModel { Id = "4", FirstName = "Benjamin", LastName = "Galindo", UserName = "Benji", Password = "abc", Role = 1 },
            };
        }

        /// <summary>
        /// Gets user Dto.
        /// </summary>
        /// <returns>the user.</returns>
        public UserDto GetUserDto()
        {
            return new UserDto
            {
                Id = "10",
                FirstName = "Jorge",
                LastName = "Morales",
                UserName = "George",
                Password = "abc",
                Role = 1,
                Activo = 1,
            };
        }

        /// <summary>
        /// Returns a user model.
        /// </summary>
        /// <returns>the return model.</returns>
        public UserModel GetUserModel()
        {
            return new UserModel
            {
                Id = "10",
                FirstName = "Jorge",
                LastName = "Morales",
                UserName = "George",
                Password = "abc",
                Role = 1,
                Activo = 1,
            };
        }
    }
}
