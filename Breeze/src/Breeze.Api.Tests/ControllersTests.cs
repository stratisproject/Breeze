using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Moq;
using Breeze.Wallet.Wrappers;
using Breeze.Wallet;
using Breeze.Wallet.Controllers;
using Breeze.Wallet.Errors;
using Breeze.Wallet.Models;

namespace Breeze.Api.Tests
{
    public class ControllersTests
    {
        [Fact]
        public void CreateWalletSuccessfullyReturnsMnemonic()
        {
            var mockWalletCreate = new Mock<IWalletWrapper>();
            mockWalletCreate.Setup(wallet => wallet.Create(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns("mnemonic");

            var controller = new WalletController(mockWalletCreate.Object);

            // Act
            var result = controller.Create(new WalletCreationRequest
            {
                Name = "myName",
                FolderPath = "",
                Password = "",
                Network = ""
            });

            // Assert
            mockWalletCreate.VerifyAll();
            var viewResult = Assert.IsType<JsonResult>(result);
            Assert.Equal("mnemonic", viewResult.Value);
            Assert.NotNull(result);
        }

        [Fact]
        public void LoadWalletNoException()
        {
            var mockWalletWrapper = new Mock<IWalletWrapper>();
            mockWalletWrapper.Setup(wallet => wallet.Recover(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            var controller = new WalletController(mockWalletWrapper.Object);

            // Act
            var result = controller.Recover(new WalletRecoveryRequest
            {
                Name = "myName",
                FolderPath = "",
                Password = "",
                Network = "",
                Mnemonic = "mnemonic"
            });

            // Assert
            mockWalletWrapper.VerifyAll();
            var viewResult = Assert.IsType<JsonResult>(result);
            Assert.NotNull(viewResult.Value);

            var model = viewResult.Value as SuccessModel;
            Assert.Equal(true, model.Success);
        }

        [Fact]
        public void RecoverWalletNoException()
        {
            var mockWalletWrapper = new Mock<IWalletWrapper>();
            mockWalletWrapper.Setup(wallet => wallet.Load(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            var controller = new WalletController(mockWalletWrapper.Object);

            // Act
            var result = controller.Load(new WalletLoadRequest
            {
                Name = "myName",
                FolderPath = "",
                Password = ""
            });

            // Assert
            mockWalletWrapper.VerifyAll();
            var viewResult = Assert.IsType<JsonResult>(result);
            Assert.NotNull(viewResult.Value);

            var model = viewResult.Value as SuccessModel;
            Assert.Equal(true, model.Success);
        }

        [Fact]
        public void FileNotFoundExceptionandReturns404()
        {
            var mockWalletWrapper = new Mock<IWalletWrapper>();
            mockWalletWrapper.Setup(wallet => wallet.Load(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Throws<FileNotFoundException>();
            
            var controller = new WalletController(mockWalletWrapper.Object);

            // Act
            var result = controller.Load(new WalletLoadRequest
            {
                Name = "myName",
                FolderPath = "",
                Password = ""
            });

            // Assert
            mockWalletWrapper.VerifyAll();
            var viewResult = Assert.IsType<ErrorResult>(result);
            Assert.NotNull(viewResult);		
            Assert.Equal(404, viewResult.StatusCode);
        }

    }
}
