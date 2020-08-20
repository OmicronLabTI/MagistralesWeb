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
    using System.Text;
    using Omicron.Usuarios.Entities.Model;

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
        /// Converts data to base64.
        /// </summary>
        /// <param name="data">the input.</param>
        /// <returns>the string in base64.</returns>
        public static string ConvertToBase64(string data)
        {
            var bytes = Encoding.ASCII.GetBytes(data);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Convert from Base64.
        /// </summary>
        /// <param name="data">the hash.</param>
        /// <returns>the data.</returns>
        public static string ConvertFromBase64(string data)
        {
            var bytes = Convert.FromBase64String(data);
            return ASCIIEncoding.ASCII.GetString(bytes);
        }
    }
}
