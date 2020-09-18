// <summary>
// <copyright file="StringExtensions.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Warehouses.Resources.Extensions
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// String extension methods.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Convert string to list of int values.
        /// </summary>
        /// <param name="baseString">Base string.</param>
        /// <param name="separator">Values separator.</param>
        /// <returns>List of ints.</returns>
        public static List<int> ToIntList(this string baseString, string separator = ",")
        {
            return baseString.Split(separator).Where(x => !string.IsNullOrEmpty(x)).Select(x => int.Parse(x)).ToList();
        }
    }
}
