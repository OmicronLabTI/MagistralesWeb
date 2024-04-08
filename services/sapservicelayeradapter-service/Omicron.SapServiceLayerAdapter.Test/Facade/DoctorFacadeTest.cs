// <summary>
// <copyright file="DoctorFacadeTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Test.Facade
{
    /// <summary>
    /// Class ProductValidationsFacadeTest.
    /// </summary>
    [TestFixture]
    public class DoctorFacadeTest
    {
        private IDoctorFacade doctorFacade;

        private IMapper mapper;

        /// <summary>
        /// The init.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            var mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
            this.mapper = mapperConfiguration.CreateMapper();
            var doctorService = new Mock<IDoctorService>();

            var resultDto = new ResultModel()
            {
                Code = 200,
                Success = true,
                Response = "response",
            };

            doctorService.SetReturnsDefault(Task.FromResult(resultDto));
            this.doctorFacade = new DoctorFacade(this.mapper, doctorService.Object);
        }

        /// <summary>
        /// Test for selecting all models.
        /// </summary>
        /// <returns>nothing.</returns>
        [Test]
        public async Task UpdateDoctorAddressDelivery()
        {
            // Act
            var response = await this.doctorFacade.UpdateDoctorAddress(new List<DoctorDeliveryAddressDto>());

            // Assert
            this.AssertResponse(response);
        }

        /// <summary>
        /// Test for selecting all models.
        /// </summary>
        /// <returns>nothing.</returns>
        [Test]
        public async Task UpdateDoctorAddressInvoice()
        {
            // Act
            var response = await this.doctorFacade.UpdateDoctorAddress(new List<DoctorInvoiceAddressDto>());

            // Assert
            this.AssertResponse(response);
        }

        /// <summary>
        /// Assert response.
        /// </summary>
        /// <param name="response">Response to validate.</param>
        public void AssertResponse(ResultDto response)
        {
            Assert.IsTrue(response.Success);
            Assert.IsNotNull(response.Response);
            Assert.IsNull(response.ExceptionMessage);
            Assert.IsNull(response.UserError);
            Assert.AreEqual(200, response.Code);
        }
    }
}