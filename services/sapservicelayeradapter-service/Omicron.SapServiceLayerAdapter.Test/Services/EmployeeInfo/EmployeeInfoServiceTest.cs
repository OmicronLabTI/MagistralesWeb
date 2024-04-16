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
                Assert.IsFalse(response.Success);
                Assert.AreEqual(400, response.Code);
                Assert.AreEqual("El identificador del asesor no es valido", response.UserError);
                Assert.AreEqual("El identificador del asesor no es valido", response.Response);
            }
            else if (isSuccesfully)
            {
                Assert.IsTrue(response.Success);
                Assert.AreEqual(200, response.Code);
                Assert.IsNull(response.UserError);
                Assert.IsNull(response.Response);
            }
            else
            {
                Assert.IsFalse(response.Success);
                Assert.AreEqual(400, response.Code);
                Assert.AreEqual(userError, response.UserError);
                Assert.IsNull(response.Response);
            }

            Assert.IsNull(response.Comments);
        }
    }
}
