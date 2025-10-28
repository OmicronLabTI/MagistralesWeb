// <summary>
// <copyright file="InvoiceService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Invoice.Services.Invoice.Impl
{
    /// <summary>
    /// InvoiceService class.
    /// </summary>
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceDao invoiceDao;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceService"/> class.
        /// </summary>
        /// <param name="mapper">Mapper.</param>
        /// <param name="invoiceDao">Users dao.</param>
        public InvoiceService(IMapper mapper, IInvoiceDao invoiceDao)
            => (this.mapper, this.invoiceDao) = (mapper, invoiceDao);

        /// <inheritdoc/>
        public async Task<IEnumerable<UserDto>> GetAllAsync()
            => mapper.Map<IEnumerable<UserDto>>(
                await invoiceDao.GetAllAsync());

        /// <inheritdoc/>
        public async Task<UserDto> GetByIdAsync(int id)
        {
            var model = await invoiceDao.GetByIdAsync(id);
            model.ThrowExceptionIfNull<NotFoundException>(
                string.Format(ErrorMessages.NotFoundIdFormat, id));
            return mapper.Map<UserDto>(model);
        }

        /// <inheritdoc/>
        public async Task<UserDto> InsertAsync(string user, CreateUserDto userRequest)
        {
            var model = mapper.Map<UserModel>(userRequest);
            model.Active = true;
            await invoiceDao.InsertAsync(model);
            return mapper.Map<UserDto>(model);
        }

        /// <inheritdoc/>
        public async Task<UserDto> UpdateAsync(
            int id, string user, UpdateUserDto usersRequest)
        {
            var model = await invoiceDao.GetByIdAsync(id);
            model.ThrowExceptionIfNull<NotFoundException>(
                string.Format(ErrorMessages.NotFoundIdFormat, id));

            model.Name = usersRequest.Name;
            model.UserName = usersRequest.UserName;
            model.Email = usersRequest.Email;
            model.Active = usersRequest.Active;
            invoiceDao.Update(model);
            return mapper.Map<UserDto>(model);
        }

        /// <inheritdoc/>
        public async Task DeleteAsync(int id)
        {
            var model = await invoiceDao.GetByIdAsync(id);
            model.ThrowExceptionIfNull<NotFoundException>(
                string.Format(ErrorMessages.NotFoundIdFormat, id));
            invoiceDao.Delete(model);
            await invoiceDao.SaveChangesAsync();
        }
    }
}
