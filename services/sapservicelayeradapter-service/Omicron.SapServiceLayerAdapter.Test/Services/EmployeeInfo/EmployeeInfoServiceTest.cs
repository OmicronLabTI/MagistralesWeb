// <summary>
// <copyright file="EmployeeInfoServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Test.Services.EmployeeInfo
{
    /// <summary>
    /// Class OrdersServiceTest.
    /// </summary>
    [TestFixture]
    public class EmployeeInfoServiceTest : BaseTest
    {
        /// <summary>
        /// Test for update adviser profile info.
        /// </summary>
        /// <param name="adviserId">Adviser Id.</param>
        /// <param name="isSuccesfully">Is succesfully.</param>
        /// <param name="responseCode">Response code.</param>
        /// <param name="userError">User error.</param>
        /// <returns>The data.</returns>
        [Test]
        [TestCase("INVALIDADVISERID", false, 400, "")]
        [TestCase("15", true, 200, "")]
        [TestCase("15", false, 400, "Error al actualizar el usuario.")]
        public async Task UpdateAdviserProfileInfo(
            string adviserId,
            bool isSuccesfully,
            int responseCode,
            string userError)
        {
            // arrange
            var mockServiceLayerClient = new Mock<IServiceLayerClient>();
            var mockLogger = new Mock<ILogger>();

            var adviserProfileInfoRequest = new AdviserProfileInfoDto
            {
                AdviserId = adviserId,
                BirthDate = DateTime.Now,
                PhoneNumber = string.Empty,
            };

            var responseUpdateInfo = GetGenericResponseModel(responseCode, isSuccesfully, null, userError);

            mockServiceLayerClient
                .Setup(ts => ts.PatchAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(responseUpdateInfo));

            var employeeInfoServiceMock = new EmployeeInfoService(mockServiceLayerClient.Object, mockLogger.Object);
            var response = await employeeInfoServiceMock.UpdateAdviserProfileInfo(adviserProfileInfoRequest);

            if (adviserId == "INVALIDADVISERID")
            {
                Assert.That(response.Success, Is.False);
                Assert.That(response.Code, Is.EqualTo(400));
                Assert.That(response.UserError, Is.EqualTo("El identificador del asesor no es valido"));
                Assert.That(response.Response, Is.EqualTo("El identificador del asesor no es valido"));
            }
            else if (isSuccesfully)
            {
                Assert.That(response.Success, Is.True);
                Assert.That(response.Code, Is.EqualTo(200));
                Assert.That(response.UserError, Is.Null);
                Assert.That(response.Response, Is.Null);
            }
            else
            {
                Assert.That(response.Success, Is.False);
                Assert.That(response.Code, Is.EqualTo(400));
                Assert.That(response.UserError, Is.EqualTo(userError));
                Assert.That(response.Response, Is.Null);
            }

            Assert.That(response.Comments, Is.Null);
        }
    }
}
