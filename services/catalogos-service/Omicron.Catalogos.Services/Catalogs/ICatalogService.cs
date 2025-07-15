// <summary>
// <copyright file="ICatalogService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Catalogos.Services.Catalogs
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Omicron.Catalogos.Dtos.User;
    using Omicron.Catalogos.Entities.Model;

    /// <summary>
    /// Interface for the catalogServicer.
    /// </summary>
    public interface ICatalogService
    {
        /// <summary>
        /// Gets all the roles.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<ResultModel> GetRoles();

        /// <summary>
        /// The values in the dictionary.
        /// </summary>
        /// <param name="parameters">the parameters.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> GetParamsContains(Dictionary<string, string> parameters);

        /// <summary>
        /// Get classification qfb.
        /// </summary>
        /// <returns>Classification qfb.</returns>
        Task<ResultModel> GetActiveClassificationQfb();

        /// <summary>
        /// Get classification qfb.
        /// </summary>
        /// <returns>Classification qfb.</returns>
        Task<ResultModel> GetActiveAllClassificationQfb();

        /// <summary>
        /// Import of valid warehouses through the Excel file.
        /// </summary>
        /// <returns> A <see cref="Task{TResult}"/> representing the result of the asynchronous operation. </returns>
        Task<ResultModel> UploadWarehouseFromExcel();

        /// <summary>
        /// Gets the values from parameters based in the dict.
        /// </summary>
        /// <param name="products">the dictionary.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> GetActivesWarehouses(List<ActiveWarehouseDto> products);

        /// <summary>
        /// Asynchronously retrieves classification data based on the provided parameters.
        /// </summary>
        /// <returns>A <see cref="Task{ResultModel}"/> containing the classification data.</returns>
        Task<ResultModel> GetClassifications();

        /// <summary>
        /// Import of valid classifications through the Excel file.
        /// </summary>
        /// <returns> A <see cref="Task{TResult}"/> representing the result of the asynchronous operation. </returns>
        Task<ResultModel> UploadConfigurationRouteFromExcel();

        /// <summary>
        /// GetActiveRouteConfigurationsForProducts.
        /// </summary>
        /// <returns> A <see cref="Task{TResult}"/> representing the result of the asynchronous operation. </returns>
        Task<ResultModel> GetActiveRouteConfigurationsForProducts();

        /// <summary>
        /// GetActiveRouteConfigurationsForProducts.
        /// </summary>
        /// <returns> A <see cref="Task{TResult}"/> representing the result of the asynchronous operation. </returns>
        Task<ResultModel> UploadProductTypeColorsFromExcel();

        /// <summary>
        /// GetProductsColors.
        /// </summary>
        /// <param name="themesIds">the dictionary.</param>
        /// <returns> A <see cref="Task{TResult}"/> representing the result of the asynchronous operation. </returns>
        Task<ResultModel> GetProductsColors(List<string> themesIds);

        /// <summary>
        /// PostConfigWarehouses.
        /// </summary>
        /// <returns> A <see cref="Task{TResult}"/> representing the result of the asynchronous operation. </returns>
        Task<ResultModel> PostConfigWarehouses();

        /// <summary>
        /// GetProductsColors.
        /// </summary>
        /// <param name="itemCode">the dictionary.</param>
        /// <returns> A <see cref="Task{TResult}"/> representing the result of the asynchronous operation. </returns>
        Task<ResultModel> GetWarehouses(string itemCode);
    }
}
