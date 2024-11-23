// <summary>
// <copyright file="DoctorServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Test.Services
{
    /// <summary>
    /// Class DoctorServiceTest.
    /// </summary>
    public class DoctorServiceTest : BaseTest
    {
        private Mock<ILogger> mockLogger;
        private IDoctorService doctorService;

        /// <summary>
        /// Init configuration.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            var mockServiceLayerClient = new Mock<IServiceLayerClient>();
            this.mockLogger = new Mock<ILogger>();
            this.doctorService = new DoctorService(mockServiceLayerClient.Object, this.mockLogger.Object);
        }

        /// <summary>
        /// Method to crud delivery direction.
        /// </summary>
        /// <param name="success">Is Response Invoice Success.</param>
        /// <param name="userError">User error.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        [TestCase(true, null)]
        [TestCase(false, "No existe el usuario")]
        public async Task UpdateDoctorDeliveryAddressTest(bool success, string userError)
        {
            var direction1 = new DoctorDeliveryAddressDto
            {
                AddressId = "Dir-1",
                ZipCode = "91000",
                State = "VER",
                City = "XALAPA",
                Neighborhood = "XALAPA ENRÍQUEZ CENTRO",
                Street = "CALLE",
                Number = "NUMERO",
                Phone = "2222222222",
                Action = "i",
                Contact = "PACIENTE",
                DoctorId = "C03865",
            };

            var direction2 = new DoctorDeliveryAddressDto
            {
                AddressId = "Dir-2",
                ZipCode = "91000",
                State = "VER",
                City = "XALAPA",
                Neighborhood = "XALAPA ENRÍQUEZ CENTRO",
                Street = "CALLE",
                Number = "NUMERO",
                Phone = "2222222222",
                Action = "u",
                Contact = "PACIENTE",
                DoctorId = "C03865",
            };

            var direction3 = new DoctorDeliveryAddressDto
            {
                AddressId = "Dir-3",
                ZipCode = "91000",
                State = "VER",
                City = "XALAPA",
                Neighborhood = "XALAPA ENRÍQUEZ CENTRO",
                Street = "CALLE",
                Number = "NUMERO",
                Phone = "2222222222",
                Action = "d",
                Contact = "PACIENTE",
                DoctorId = "C03865",
            };

            var mockServiceLayerClient = new Mock<IServiceLayerClient>();
            var doctorSap = new DoctorDto();
            doctorSap.Addresses = new List<DoctorAddressDto>();
            var doctorDirection1 = new DoctorAddressDto
            {
                AddressName = "Dir-3",
                AddressType = ServiceConstants.DeliveryAddress,
            };
            var doctorDirection2 = new DoctorAddressDto
            {
                AddressName = "Dir-2",
                AddressType = ServiceConstants.DeliveryAddress,
            };

            doctorSap.Addresses.Add(doctorDirection1);
            doctorSap.Addresses.Add(doctorDirection2);

            var data = new List<DoctorDeliveryAddressDto>();
            data.Add(direction1);
            data.Add(direction2);
            data.Add(direction3);

            mockServiceLayerClient
                .Setup(x => x.GetAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(GetResult(doctorSap, success, userError)));

            var created = new ResultModel
            {
                Success = true,
                Code = 200,
            };

            mockServiceLayerClient
                .Setup(x => x.PutAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(created));

            var mockLogger = new Mock<ILogger>();
            var service = new DoctorService(mockServiceLayerClient.Object, mockLogger.Object);

            var result = await service.UpdateDoctorDeliveryAddress(data);
            if (success)
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Code, Is.EqualTo(200));
            }
            else
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Code, Is.EqualTo(400));
            }
        }

        /// <summary>
        /// Method to update invoice direction.
        /// </summary>
        /// <param name="success">Is Response Invoice Success.</param>
        /// <param name="userError">User error.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        [TestCase(true, null)]
        [TestCase(false, "No existe el usuario")]
        public async Task UpdateDoctorDeliveryInvoiceTest(bool success, string userError)
        {
            var direction1 = new DoctorInvoiceAddressDto
            {
                NickName = "Dir-1",
                ZipCode = "91000",
                State = "VER",
                City = "XALAPA",
                Street = "CALLE",
                Number = "NUMERO",
                Action = "i",
                DoctorId = "C03865",
            };

            var direction2 = new DoctorInvoiceAddressDto
            {
                NickName = "Dir-2",
                ZipCode = "91000",
                State = "VER",
                City = "XALAPA",
                Street = "CALLE",
                Number = "NUMERO",
                Action = "u",
                DoctorId = "C03865",
            };

            var direction3 = new DoctorInvoiceAddressDto
            {
                NickName = "Dir-3",
                ZipCode = "91000",
                State = "VER",
                City = "XALAPA",
                Street = "CALLE",
                Number = "NUMERO",
                Action = "d",
                DoctorId = "C03865",
            };

            var mockServiceLayerClient = new Mock<IServiceLayerClient>();
            var doctorSap = new DoctorDto();
            doctorSap.Addresses = new List<DoctorAddressDto>();
            var doctorDirection1 = new DoctorAddressDto
            {
                AddressName = "Dir-3",
                AddressType = ServiceConstants.InvoiceAddress,
            };
            var doctorDirection2 = new DoctorAddressDto
            {
                AddressName = "Dir-2",
                AddressType = ServiceConstants.InvoiceAddress,
            };

            doctorSap.Addresses.Add(doctorDirection1);
            doctorSap.Addresses.Add(doctorDirection2);

            var data = new List<DoctorInvoiceAddressDto>();
            data.Add(direction1);
            data.Add(direction2);
            data.Add(direction3);

            mockServiceLayerClient
                .Setup(x => x.GetAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(GetResult(doctorSap, success, userError)));

            var created = new ResultModel
            {
                Success = true,
                Code = 200,
            };

            mockServiceLayerClient
                .Setup(x => x.PutAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(created));

            var mockLogger = new Mock<ILogger>();
            var service = new DoctorService(mockServiceLayerClient.Object, mockLogger.Object);

            var result = await service.UpdateDoctorDeliveryAddress(data);
            if (success)
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Code, Is.EqualTo(200));
            }
            else
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Code, Is.EqualTo(400));
            }
        }

        /// <summary>
        /// Test for Update Doctor Profile Info.
        /// </summary>
        /// <param name="isSuccess">Is Success.</param>
        /// <param name="code">Response code.</param>
        /// <param name="userError">User error.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        [TestCase(true, 200, null)]
        [TestCase(false, 400, "No existe el usuario")]
        public async Task UpdateDoctorProfileInfo(bool isSuccess, int code, string userError)
        {
            var request = new DoctorProfileInfoDto
            {
                DoctorId = "C00001",
                BirthDate = DateTime.Now.AddYears(-25),
                PhoneNumber = "5566778899",
            };

            var mockServiceLayerClient = new Mock<IServiceLayerClient>();
            var resultUpdateServiceLayer = GetGenericResponseModel(code, isSuccess, null, userError);

            mockServiceLayerClient
                .Setup(ts => ts.PatchAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(resultUpdateServiceLayer));

            var mockDoctorService = new DoctorService(mockServiceLayerClient.Object, this.mockLogger.Object);
            var result = await mockDoctorService.UpdateDoctorProfileInfo(request);

            if (isSuccess)
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Code, Is.EqualTo(200));
                Assert.That(result.UserError, Is.Null);
            }
            else
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Code, Is.EqualTo(400));
                Assert.That(userError, Is.EqualTo(result.UserError));
            }

            Assert.That(result.Response, Is.Null);
            Assert.That(result.ExceptionMessage, Is.Null);
        }

        /// <summary>
        /// Test for Update Doctor Profile Info.
        /// </summary>
        /// <param name="isDoctorFound">Is doctor found.</param>
        /// <param name="isAddressUpdateSuccess">Is Address Update Success.</param>
        /// <param name="userError">User Error.</param>
        /// <param name="addressType">Address Type.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        [TestCase(false, true, "No se encontró el médico", "B")]
        [TestCase(true, false, "Error al actualizar la dirección", "B")]
        [TestCase(true, true, null, "B")]
        [TestCase(true, true, null, "S")]
        public async Task UpdateDoctorDefaultAddress(
            bool isDoctorFound,
            bool isAddressUpdateSuccess,
            string userError,
            string addressType)
        {
            var request = new DoctorDefaultAddressDto
            {
                DoctorId = "C00001",
                AddressName = "Address Test",
                Type = addressType,
            };

            var sapServiceLayerResult = new BusinessPartnerDefaultAddressDto
            {
                BillToDefault = "Bill Default Address",
                ShipToDefault = "Ship Default Address",
            };

            var mockServiceLayerClient = new Mock<IServiceLayerClient>();
            var getDoctorCode = isDoctorFound ? 200 : 400;
            var resultGetDoctor = GetGenericResponseModel(getDoctorCode, isDoctorFound, sapServiceLayerResult, userError);

            mockServiceLayerClient
                .Setup(ts => ts.GetAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(resultGetDoctor));

            var updateDoctorAddressCode = isAddressUpdateSuccess ? 200 : 400;
            var resultUpdateAddressDoctor = GetGenericResponseModel(updateDoctorAddressCode, isAddressUpdateSuccess, null, userError);

            mockServiceLayerClient
                .Setup(ts => ts.PatchAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(resultUpdateAddressDoctor));

            var mockDoctorService = new DoctorService(mockServiceLayerClient.Object, this.mockLogger.Object);
            var result = await mockDoctorService.UpdateDoctorDefaultAddress(request);

            if (!isDoctorFound)
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Code, Is.EqualTo(400));
                Assert.That(result.UserError, Is.EqualTo("No se encontró el médico"));
            }
            else if (!isAddressUpdateSuccess)
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Code, Is.EqualTo(400));
                Assert.That(result.UserError, Is.EqualTo("Error al actualizar la dirección"));
            }
            else
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Code, Is.EqualTo(200));
                Assert.That(result.UserError, Is.Null);
            }

            Assert.That(result.Response, Is.Null);
            Assert.That(result.ExceptionMessage, Is.Null);
        }

        private static ResultModel GetResult(object data, bool success, string userError)
        {
            return new ResultModel()
            {
                Success = success,
                UserError = success ? string.Empty : userError,
                Response = JsonConvert.SerializeObject(data),
                Code = 200,
            };
        }
    }
}