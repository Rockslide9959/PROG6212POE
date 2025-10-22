using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PROG6212POE.Controllers;
using PROG6212POE.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace PROG6212POE.Tests
{
    [TestClass]
    public class ControllerTests
    {
        private string _testRoot;
        private Mock<IWebHostEnvironment> _envMock;

        [TestInitialize]
        public void Setup()
        {
            _testRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            Directory.CreateDirectory(Path.Combine(_testRoot, "data"));
            Directory.CreateDirectory(Path.Combine(_testRoot, "uploads"));

            _envMock = new Mock<IWebHostEnvironment>();
            _envMock.Setup(e => e.WebRootPath).Returns(_testRoot);
        }

        // ============ ClaimController Tests (3) ============

        [TestMethod]
        public async Task ClaimController_Create_InvalidClaim_ReturnsCreateView()
        {
            // Arrange
            var controller = new ClaimController(_envMock.Object);
            controller.ModelState.AddModelError("LecturerName", "Required");
            var model = new Claim();

            // Act
            var result = await controller.Create(model, null) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Create", result?.ViewName ?? "Create");
            Assert.IsFalse(controller.ModelState.IsValid);
        }



        [TestMethod]
        public async Task ClaimController_Create_InvalidModel_ReturnsView()
        {
            // Arrange
            var controller = new ClaimController(_envMock.Object);
            controller.ModelState.AddModelError("LecturerName", "Required");
            var model = new Claim();

            // Act
            var result = await controller.Create(model, null) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(controller.ModelState.IsValid);
        }

        [TestMethod]
        public async Task Create_InvalidClaim_ReturnsView()
        {
            // Arrange
            var envMock = new Mock<IWebHostEnvironment>();
            envMock.Setup(e => e.WebRootPath).Returns(Path.GetTempPath());
            var controller = new ClaimController(envMock.Object);
            controller.ModelState.AddModelError("LecturerName", "Required");

            var invalidClaim = new Claim();

            // Act
            var result = await controller.Create(invalidClaim, null);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        // ============ CoordinatorController Test (1) ============

        [TestMethod]
        public void Index_ReturnsPendingClaimsSortedByDate()
        {
            // Arrange
            var mockEnv = new Mock<IWebHostEnvironment>();
            var testRootPath = Path.GetTempPath();
            mockEnv.Setup(m => m.WebRootPath).Returns(testRootPath);

            // Ensure the 'data' directory exists and write claims.json to it
            var testDataFolder = Path.Combine(testRootPath, "data");
            Directory.CreateDirectory(testDataFolder);
            var testJsonPath = Path.Combine(testDataFolder, "claims.json");

            var claims = new List<Claim>
            {
                new Claim { ClaimId = "1", Status = "Pending", DateSubmitted = DateTime.Now.AddDays(-1) },
                new Claim { ClaimId = "2", Status = "Pending", DateSubmitted = DateTime.Now },
                new Claim { ClaimId = "3", Status = "Verified by Coordinator", DateSubmitted = DateTime.Now.AddDays(-2) }
            };
            File.WriteAllText(testJsonPath, JsonSerializer.Serialize(claims, new JsonSerializerOptions { WriteIndented = true }));

            var controller = new CoordinatorController(mockEnv.Object);

            // Act
            var result = controller.Index() as ViewResult;
            var model = result?.Model as List<Claim>;

            // Assert
            Assert.IsNotNull(result, "Result should not be null");
            Assert.IsNotNull(model, "Model should not be null");
            Assert.AreEqual(2, model.Count, "Should return 2 pending claims");
            Assert.IsTrue(model.All(c => c.Status == "Pending"), "All claims should be pending");
            Assert.IsTrue(model[0].DateSubmitted > model[1].DateSubmitted, "Claims should be sorted by date descending");

            // Cleanup
            if (File.Exists(testJsonPath))
                File.Delete(testJsonPath);
            if (Directory.Exists(testDataFolder))
                Directory.Delete(testDataFolder);
        }

        // ============ ManagerController Test (1) ============

        [TestMethod]
        public void Approve_ValidClaim_UpdatesStatusAndRedirects()
        {
            // Arrange
            var envMock = new Mock<IWebHostEnvironment>();
            var testRoot = Path.Combine(Path.GetTempPath(), "wwwroot");
            envMock.Setup(e => e.WebRootPath).Returns(testRoot);
            var testJsonPath = Path.Combine(testRoot, "data", $"claims_{Guid.NewGuid()}.json");
            Directory.CreateDirectory(Path.Combine(testRoot, "data"));

            var claims = new List<Claim>
            {
                new Claim { ClaimId = "1", Status = "Verified by Coordinator", DateSubmitted = DateTime.Now }
            };
            File.WriteAllText(testJsonPath, JsonSerializer.Serialize(claims, new JsonSerializerOptions { WriteIndented = true }));

            var controller = new ManagerController(envMock.Object);
            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            // Inject the private _jsonPath field
            var field = typeof(ManagerController).GetField("_jsonPath", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
                field.SetValue(controller, testJsonPath);

            // Act
            var result = controller.Approve("1") as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
            var updatedClaims = JsonSerializer.Deserialize<List<Claim>>(File.ReadAllText(testJsonPath));
            Assert.AreEqual("Approved", updatedClaims[0].Status);

            // Cleanup
            if (File.Exists(testJsonPath))
                File.Delete(testJsonPath);
        }

    }
}
