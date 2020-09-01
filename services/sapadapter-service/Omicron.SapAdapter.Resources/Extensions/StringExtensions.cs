// <summary>
// <copyright file="StringExtensions.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Resources.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// String extension methods.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Concat string N times.
        /// </summary>
        /// <param name="baseString">Original string.</param>
        /// <param name="stringToConcat">String to concat.</param>
        /// <param name="repeat">N times to concat.</param>
        /// <returns>New string.</returns>
        public static string Concat(this string baseString, string stringToConcat, int repeat = 1)
        {
            var newString = baseString;

            for (int i = 0; i < repeat; i++)
            {
                newString += stringToConcat;
            }

            return newString;
        }
    }
}
