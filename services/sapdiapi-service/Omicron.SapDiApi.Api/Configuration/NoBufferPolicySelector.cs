// ------------------------------------------------------------------------------------------------
// <copyright file="NoBufferPolicySelector.cs" company="Ann Inc">
//  Copyright (c) 2017 Ann Inc, All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace Omicron.SapDiApi.Api.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Web;
    using System.Web.Http.WebHost;

    /// <summary>
    /// The no buffer policy selector.
    /// </summary>
    public class NoBufferPolicySelector : WebHostBufferPolicySelector
    {
        /// <summary>
        /// The use buffered input stream.
        /// </summary>
        /// <param name="hostContext">
        /// The host context.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool UseBufferedInputStream(object hostContext)
        {
            return true;
        }

        /// <summary>
        /// The use buffered output stream.
        /// </summary>
        /// <param name="response">
        /// The response.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool UseBufferedOutputStream(HttpResponseMessage response)
        {
            return false;
        }
    }
}