// <summary>
// <copyright file="ServiceUtils.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Catalogos.Services.Utils
{
    using System;
    using System.Collections.Generic;
    using Omicron.Catalogos.Entities.Model;

    /// <summary>
    /// The static class for service utils.
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
        /// <returns>the resultModel.</returns>
        public static ResultModel CreateResult(bool success, int code, string userError, object responseObj, string exceptionMessage)
        {
            return new ResultModel
            {
                Success = success,
                Response = responseObj,
                UserError = userError,
                ExceptionMessage = exceptionMessage,
                Code = code,
            };
        }

        /// <summary>
        /// gets the distinc by.
        /// </summary>
        /// <typeparam name="Tsource">the list source.</typeparam>
        /// <typeparam name="TKey">the key to look.</typeparam>
        /// <param name="source">the sourec.</param>
        /// <param name="keyselector">the key.</param>
        /// <returns>the list distinc.</returns>
        public static IEnumerable<Tsource> DistinctBy<Tsource, TKey>(this IEnumerable<Tsource> source, Func<Tsource, TKey> keyselector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (Tsource element in source)
            {
                if (seenKeys.Add(keyselector(element)))
                {
                    yield return element;
                }
            }
        }
    }
}
