// <summary>
// <copyright file="ServiceUtils.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Usuarios.Services.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Omicron.Usuarios.Entities.Model;
    using Omicron.Usuarios.Services.Constants;

    /// <summary>
    /// Class for common logic.
    /// </summary>
    public static class ServiceUtils
    {
        /// <summary>
        /// creates the result.
        /// </summary>
        /// <param name="success">if it was successful.</param>
        /// <param name="code">the code.</param>
        /// <param name="userError">the user error.</param>
        /// <param name="responseObj">the responseobj.</param>
        /// <param name="exceptionMessage">the exception message.</param>
        /// <param name="comments">Extra comments.</param>
        /// <returns>the resultModel.</returns>
        public static ResultModel CreateResult(bool success, int code, string userError, object responseObj, string exceptionMessage, object comments)
        {
            return new ResultModel
            {
                Success = success,
                Response = responseObj,
                UserError = userError,
                ExceptionMessage = exceptionMessage,
                Code = code,
                Comments = comments,
            };
        }

        /// <summary>
        /// Filters the users.
        /// </summary>
        /// <param name="users">the complete list of users.</param>
        /// <param name="criteria">the criteria.</param>
        /// <returns>the data.</returns>
        public static List<UserModel> GetUsersToReturn(List<UserModel> users, Dictionary<string, string> criteria)
        {
            users = criteria.ContainsKey(ServiceConstants.UserName) ? users.Where(x => x.UserName.ToLower().Contains(criteria[ServiceConstants.UserName].ToLower())).ToList() : users;
            users = criteria.ContainsKey(ServiceConstants.FirstName) ? users.Where(x => x.FirstName.ToLower().Contains(criteria[ServiceConstants.FirstName].ToLower())).ToList() : users;
            users = criteria.ContainsKey(ServiceConstants.LastName) ? users.Where(x => x.LastName.ToLower().Contains(criteria[ServiceConstants.LastName].ToLower())).ToList() : users;
            users = criteria.ContainsKey(ServiceConstants.Role) ? users.Where(x => x.Role.ToString() == criteria[ServiceConstants.Role]).ToList() : users;
            users = criteria.ContainsKey(ServiceConstants.Assignable) ? users.Where(x => x.Asignable.ToString() == criteria[ServiceConstants.Assignable]).ToList() : users;
            users = criteria.ContainsKey(ServiceConstants.Status) ? users.Where(x => x.Activo.ToString() == criteria[ServiceConstants.Status]).ToList() : users;
            return users;
        }
    }
}
