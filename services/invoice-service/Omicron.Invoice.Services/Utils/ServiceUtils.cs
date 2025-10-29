// <summary>
// <copyright file="ServiceUtils.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Invoice.Services.Utils
{
    /// <summary>
    /// The class for the Service Utils.
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
        /// <param name="comments">the comments.</param>
        /// <returns>the resultModel.</returns>
        public static ResultDto CreateResult(bool success, int code, string userError, object responseObj, string exceptionMessage, object comments)
        {
            return new ResultDto
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
        ///    test.
        /// </summary>
        /// <typeparam name="T">the generic object.</typeparam>
        /// <param name="obj">the object.</param>
        /// <param name="name">name of param.</param>
        /// <returns>ca.</returns>
        public static T ThrowIfNull<T>(this T obj, string name)
        {
            return obj ?? throw new ArgumentNullException(name);
        }
    }
}
