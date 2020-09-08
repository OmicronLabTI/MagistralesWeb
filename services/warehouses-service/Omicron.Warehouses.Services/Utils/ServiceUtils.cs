// <summary>
// <copyright file="ServiceUtils.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Warehouses.Services.Utils
{
    using Omicron.Warehouses.Entities.Model;

    /// <summary>
    /// the class for utils.
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
        /// <param name="comments">The comments.</param>
        /// <returns>the resultModel.</returns>
        public static ResultModel CreateResult(bool success, int code, string userError, object responseObj, string exceptionMessage, string comments = null)
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
    }
}
