// <summary>
// <copyright file="ICatalogFacade.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Catalogos.Facade.Catalogs
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Omicron.Catalogos.Dtos.Models;
    using Omicron.Catalogos.Dtos.User;

    /// <summary>
    /// The interface for catalogs.
    /// </summary>
    public interface ICatalogFacade
    {
        /// <summary>
        /// Get roles.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<ResultDto> GetRoles();

        /// <summary>
        /// Gets the values from parameters based in the dict.
        /// </summary>
        /// <param name="parameters">the dictionary.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> GetParamsContains(Dictionary<string, string> parameters);

        /// <summary>
        /// Get classification qfb.
        /// </summary>
        /// <returns>Classification qfb.</returns>
        Task<ResultDto> GetActiveClassificationQfb();

        /// <summary>
        /// Import of valid warehouses through the Excel file.
        /// </summary>
        /// <returns> A <see cref="Task{TResult}"/> representing the result of the asynchronous operation. </returns>
        Task<ResultDto> UploadWarehouseFromExcel();

        /// <summary>
        /// Gets the values from parameters based in the dict.
        /// </summary>
        /// <param name="products">the dictionary.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> GetActivesWarehouses(List<ActiveWarehouseDto> products);

        /// <summary>
        /// Asynchronously retrieves classification data based on the provided parameters.
        /// </summary>
        /// <returns>A <see cref="Task{ResultModel}"/> containing the classification data.</returns>
        Task<ResultDto> GetClassifications();

        /// <summary>
        /// Import of valid classifications through the Excel file.
        /// </summary>
        /// <returns> A <see cref="Task{TResult}"/> representing the result of the asynchronous operation. </returns>
        Task<ResultDto> UploadSortingRouteFromExcel();
    }
}
