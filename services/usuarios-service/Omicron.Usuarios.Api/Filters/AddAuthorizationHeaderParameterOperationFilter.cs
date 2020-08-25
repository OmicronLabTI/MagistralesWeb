// <summary>
// <copyright file="AddAuthorizationHeaderParameterOperationFilter.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Usuarios.Api.Filters
{
    using Microsoft.OpenApi.Models;
    using Swashbuckle.AspNetCore.SwaggerGen;

    /// <summary>
    /// Class for the autorization.
    /// </summary>
    public class AddAuthorizationHeaderParameterOperationFilter : IOperationFilter
    {
        /// <summary>
        /// the apply autorization.
        /// </summary>
        /// <param name="operation">the operation.</param>
        /// <param name="context">the context.</param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters != null)
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "Authorization",
                    Description = "access token",
                    Required = false,
                    In = ParameterLocation.Header,
                });
            }
        }
    }
}
