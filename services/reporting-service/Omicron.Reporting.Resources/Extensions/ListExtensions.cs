// <summary>
// <copyright file="ListExtensions.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.Reporting.Resources.Extensions
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// List extension methods.
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// Split list.
        /// </summary>
        /// <typeparam name="T">Type of items.</typeparam>
        /// <param name="source">List source.</param>
        /// <param name="size">Sublist size.</param>
        /// <returns>Sublist.</returns>
        public static List<List<T>> Split<T>(this List<T> source, int size)
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / size)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }
    }
}
