using Microsoft.AspNetCore.Mvc;
using Xunit;
using Moq;
using Breeze.Api.Controllers;
using Breeze.Api.Models;
using Breeze.Api.Wrappers;

namespace Breeze.Api.Tests
{
    public class ControllersTests
    {
        [Fact]
        public void CreateSafesuccessfullyReturnsMnemonic()
        {
            var mockSafeCreate = new Mock<ISafeWrapper>();            
            mockSafeCreate.Setup(safe => safe.Create(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns("mnemonic");

            var controller = new SafeController(mockSafeCreate.Object);

            // Act
            var result = controller.Create(new SafeCreationModel
            {
                Name = "myName",
                FolderPath = "",
                Password = "",
                Network = ""
            });

            // Assert
            var viewResult = Assert.IsType<JsonResult>(result);
            Assert.Equal("mnemonic", viewResult.Value);
            Assert.NotNull(result);
        }
    }
}
