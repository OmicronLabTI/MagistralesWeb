// <summary>
// <copyright file="ConverterBase64ToByteArray.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Warehouses.Services.Mapping
{
    using AutoMapper;

    /// <summary>
    /// Custom converter form string to byte[].
    /// </summary>
    public class ConverterBase64ToByteArray : IValueConverter<string, byte[]>
    {
        /// <summary>
        /// Conversion.
        /// </summary>
        /// <param name="sourceMember">String source.</param>
        /// <param name="context">Context.</param>
        /// <returns>Converted value.</returns>
        public byte[] Convert(string sourceMember, ResolutionContext context)
        {
            if (string.IsNullOrEmpty(sourceMember))
            {
                return new byte[0];
            }

            return System.Convert.FromBase64String(sourceMember);
        }
    }
}
