using ExpertSystem.Domain.Exceptions;
using ExpertSystem.Domain.Models;
using ExpertSystem.Domain.Services;
using ExpertSystem.EntityFramework.Services;
using Microsoft.AspNet.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertSystem.Tests.Services.AuthenticationServices
{
    [TestFixture]
    public class LoginTests
    {
        private Mock<IUserService> _mockUserService;
        private Mock<IPasswordHasher> _mockPasswordHasher;
        private AuthenticationService _authenticationService;

        [SetUp]
        public void SetUp()
        {
            _mockUserService = new Mock<IUserService>();
            _mockPasswordHasher = new Mock<IPasswordHasher>();
            _authenticationService = new AuthenticationService(_mockUserService.Object, _mockPasswordHasher.Object);
        }

        #region Happy Path

        [Test]
        public async Task Login_WithValidCredentials_ReturnsUserWithCorrectProperties()
        {
            // Arrange
            var expectedUser = new User
            {
                Id = 1,
                Nickname = "testuser",
                Email = "test@example.com",
                PasswordHashed = "hashedpassword",
                IsAdmin = true
            };

            _mockUserService.Setup(s => s.GetByNickname("testuser")).ReturnsAsync(expectedUser);
            _mockPasswordHasher
                .Setup(s => s.VerifyHashedPassword(expectedUser.PasswordHashed, "Password123"))
                .Returns(PasswordVerificationResult.Success);

            // Act
            User result = await _authenticationService.Login("testuser", "Password123");

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Id, Is.EqualTo(expectedUser.Id));
                Assert.That(result.Nickname, Is.EqualTo(expectedUser.Nickname));
                Assert.That(result.Email, Is.EqualTo(expectedUser.Email));
                Assert.That(result.IsAdmin, Is.EqualTo(expectedUser.IsAdmin));
            });
        }

        # endregion

        #region Invalid Credentials

        [Test]
        public void Login_WithExistingNicknameAndIncorrectPassword_ThrowsInvalidPasswordException()
        {
            // Arrange
            string nickname = "testuser";
            string passwordHash = "hashedPassword";
            string wrongPassword = "WrongPass1";
            _mockUserService.Setup(s => s.GetByNickname(nickname)).ReturnsAsync(new User
            {
                Nickname = nickname,
                PasswordHashed = passwordHash
            });
            _mockPasswordHasher.Setup(s => s.VerifyHashedPassword(passwordHash, wrongPassword)).Returns(PasswordVerificationResult.Failed);

            // Act and assert
            Assert.ThrowsAsync<InvalidPasswordException>(async () => await _authenticationService.Login(nickname, wrongPassword));
        }

        [Test]
        public void Login_WithNonExistentNickname_ThrowsUserNotFoundException()
        {
            // Arrange
            string nickname = "nonexistent";
            string password = "Password123";
            _mockUserService.Setup(s => s.GetByNickname(nickname)).ReturnsAsync((User)null);

            // Act and Assert (
            Assert.ThrowsAsync<UserNotFoundException>(
                async () => await _authenticationService.Login(nickname, password));
        }

        [Test]
        public void Login_WithEmptyPassword_ThrowsInvalidPasswordException()
        {
            // Arrange
            string nickname = "testuser";
            string passwordHash = "hashedPassword";

            _mockUserService.Setup(s => s.GetByNickname(nickname)).ReturnsAsync(new User
            {
                Nickname = nickname,
                PasswordHashed = passwordHash
            });
            _mockPasswordHasher
                .Setup(s => s.VerifyHashedPassword(passwordHash, string.Empty))
                .Returns(PasswordVerificationResult.Failed);

            // Act & Assert
            Assert.ThrowsAsync<InvalidPasswordException>(
                async () => await _authenticationService.Login(nickname, string.Empty));
        }

        [Test]
        public void Login_WithEmptyNickname_ThrowsUserNotFoundException()
        {
            // Arrange
            _mockUserService.Setup(s => s.GetByNickname(string.Empty)).ReturnsAsync((User)null);

            // Act & Assert
            Assert.ThrowsAsync<UserNotFoundException>(
                async () => await _authenticationService.Login(string.Empty, "Password123"));
        }

        #endregion

        #region Normalization Tests

        [Test]
        public async Task Login_WithUppercaseNickname_NormalizesToLowercaseAndReturnsUser()
        {
            // Arrange
            string inputNickname = "TestUser";
            string normalizedNickname = "testuser";
            string password = "Password123";
            string passwordHash = "hashedPassword";

            var expectedUser = new User
            {
                Id = 1,
                Nickname = normalizedNickname,
                PasswordHashed = passwordHash
            };

            _mockUserService.Setup(s => s.GetByNickname(normalizedNickname)).ReturnsAsync(expectedUser);
            _mockPasswordHasher
                .Setup(s => s.VerifyHashedPassword(passwordHash, password))
                .Returns(PasswordVerificationResult.Success);

            // Act
            User result = await _authenticationService.Login(inputNickname, password);

            // Assert
            _mockUserService.Verify(s => s.GetByNickname(normalizedNickname), Times.Once);
            Assert.That(result, Is.EqualTo(expectedUser));
        }

        #endregion

    }
}
