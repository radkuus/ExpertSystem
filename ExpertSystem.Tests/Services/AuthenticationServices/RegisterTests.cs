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
    public class RegisterTests
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
        public async Task Register_WithValidData_ReturnsSuccess()
        {
            // Arrange
            string nickname = "testuser";
            string password = "Password123";
            string confirmPassword = "Password123";
            string email = "test@example.com";

            _mockUserService.Setup(s => s.GetByNickname(nickname.ToLower())).ReturnsAsync((User)null);
            _mockUserService.Setup(s => s.GetByEmail(email.ToLower())).ReturnsAsync((User)null);
            _mockPasswordHasher.Setup(s => s.HashPassword(password)).Returns("hashed_password");

            // Act
            RegistrationResult result = await _authenticationService.Register(nickname, password, confirmPassword, email, false);

            // Assert
            Assert.That(result, Is.EqualTo(RegistrationResult.Success));
        }

        [Test]
        public async Task Register_WithValidData_CallsCreateWithCorrectUser()
        {
            // Arrange
            string nickname = "TestUser";
            string password = "Password123";
            string confirmPassword = "Password123";
            string email = "Test@Example.com";
            string hashedPassword = "hashed_password";

            _mockUserService.Setup(s => s.GetByNickname(nickname.ToLower())).ReturnsAsync((User)null);
            _mockUserService.Setup(s => s.GetByEmail(email.ToLower())).ReturnsAsync((User)null);
            _mockPasswordHasher.Setup(s => s.HashPassword(password)).Returns(hashedPassword);

            User capturedUser = null;
            _mockUserService.Setup(s => s.Create(It.IsAny<User>()))
                .Callback<User>(u => capturedUser = u)
                .ReturnsAsync((User u) => u);

            // Act
            await _authenticationService.Register(nickname, password, confirmPassword, email, true);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(capturedUser, Is.Not.Null);
                Assert.That(capturedUser.Nickname, Is.EqualTo("testuser")); // normalized to lowercase
                Assert.That(capturedUser.Email, Is.EqualTo("test@example.com")); // normalized to lowercase
                Assert.That(capturedUser.PasswordHashed, Is.EqualTo(hashedPassword));
                Assert.That(capturedUser.IsAdmin, Is.True);
            });
        }

        [Test]
        public async Task Register_WithIsAdminFalse_CreatesNonAdminUser()
        {
            // Arrange
            string nickname = "testuser";
            string password = "Password123";
            string email = "test@example.com";

            _mockUserService.Setup(s => s.GetByNickname(nickname.ToLower())).ReturnsAsync((User)null);
            _mockUserService.Setup(s => s.GetByEmail(email.ToLower())).ReturnsAsync((User)null);
            _mockPasswordHasher.Setup(s => s.HashPassword(password)).Returns("hash");

            User capturedUser = null;
            _mockUserService.Setup(s => s.Create(It.IsAny<User>()))
                .Callback<User>(u => capturedUser = u)
                .ReturnsAsync((User u) => u);

            // Act
            await _authenticationService.Register(nickname, password, password, email, false);

            // Assert
            Assert.That(capturedUser.IsAdmin, Is.False);
        }

        #endregion

        #region Nickname Validation

        [TestCase("ab", TestName = "Register_WithTooShortNickname_ReturnsInvalidNicknameFormat")]
        [TestCase("abc12345678901234", TestName = "Register_WithTooLongNickname_ReturnsInvalidNicknameFormat")]
        [TestCase("_testuser", TestName = "Register_WithNicknameStartingWithUnderscore_ReturnsInvalidNicknameFormat")]
        [TestCase("test@user!", TestName = "Register_WithNicknameContainingSpecialCharacters_ReturnsInvalidNicknameFormat")]
        [TestCase("test user", TestName = "Register_WithNicknameContainingSpace_ReturnsInvalidNicknameFormat")]
        [TestCase("", TestName = "Register_WithEmptyNickname_ReturnsInvalidNicknameFormat")]
        [TestCase(null, TestName = "Register_WithNullNickname_ReturnsInvalidNicknameFormat")]
        public async Task Register_WithInvalidNickname_ReturnsInvalidNicknameFormat(string? nickname)
        {
            // Arrange & Act
            RegistrationResult result = await _authenticationService.Register(
                nickname, "Password123", "Password123", "test@example.com", false);

            // Assert
            Assert.That(result, Is.EqualTo(RegistrationResult.InvalidNicknameFormat));
        }

        [Test]
        public async Task Register_WithValidNicknameContainingUnderscoreAndHyphen_ReturnsSuccess()
        {
            // Arrange
            string nickname = "test_user-1";
            _mockUserService.Setup(s => s.GetByNickname(nickname.ToLower())).ReturnsAsync((User)null);
            _mockUserService.Setup(s => s.GetByEmail(It.IsAny<string>())).ReturnsAsync((User)null);
            _mockPasswordHasher.Setup(s => s.HashPassword(It.IsAny<string>())).Returns("hash");

            // Act
            RegistrationResult result = await _authenticationService.Register(
                nickname, "Password123", "Password123", "test@example.com", false);

            // Assert
            Assert.That(result, Is.EqualTo(RegistrationResult.Success));
        }

        [Test]
        public async Task Register_WithNicknameAlreadyTaken_ReturnsNicknameAlreadyTaken()
        {
            // Arrange
            string nickname = "existinguser";
            _mockUserService.Setup(s => s.GetByNickname(nickname.ToLower())).ReturnsAsync(new User
            {
                Nickname = nickname
            });

            // Act
            RegistrationResult result = await _authenticationService.Register(
                nickname, "Password123", "Password123", "test@example.com", false);

            // Assert
            Assert.That(result, Is.EqualTo(RegistrationResult.NicknameAlreadyTaken));
        }

        #endregion

        #region Email Validation

        [TestCase("", TestName = "Register_WithEmptyEmail_ReturnsInvalidEmailFormat")]
        [TestCase(null, TestName = "Register_WithNullEmail_ReturnsInvalidEmailFormat")]
        [TestCase("testexample.com", TestName = "Register_WithEmailWithoutAtSymbol_ReturnsInvalidEmailFormat")]
        [TestCase("test@", TestName = "Register_WithEmailWithoutDomain_ReturnsInvalidEmailFormat")]
        [TestCase("test@example", TestName = "Register_WithEmailWithoutTld_ReturnsInvalidEmailFormat")]
        [TestCase("test@example.c", TestName = "Register_WithEmailWithShortTld_ReturnsInvalidEmailFormat")]
        [TestCase("@example.com", TestName = "Register_WithEmailWithoutLocalPart_ReturnsInvalidEmailFormat")]
        public async Task Register_WithInvalidEmail_ReturnsInvalidEmailFormat(string? email)
        {
            // Arrange & Act
            RegistrationResult result = await _authenticationService.Register(
                "testuser", "Password123", "Password123", email, false);

            // Assert
            Assert.That(result, Is.EqualTo(RegistrationResult.InvalidEmailFormat));
        }

        [Test]
        public async Task Register_WithEmailAlreadyTaken_ReturnsEmailAlreadyTaken()
        {
            // Arrange
            string email = "existing@example.com";
            _mockUserService.Setup(s => s.GetByNickname(It.IsAny<string>())).ReturnsAsync((User)null);
            _mockUserService.Setup(s => s.GetByEmail(email.ToLower())).ReturnsAsync(new User
            {
                Email = email
            });

            // Act
            RegistrationResult result = await _authenticationService.Register(
                "testuser", "Password123", "Password123", email, false);

            // Assert
            Assert.That(result, Is.EqualTo(RegistrationResult.EmailAlreadyTaken));
        }

        [TestCase("test@example.com")]
        [TestCase("test.name@example.com")]
        [TestCase("test+tag@example.com")]
        [TestCase("test@sub.example.com")]
        [TestCase("test123@example123.com")]
        public async Task Register_WithValidEmailFormats_ReturnsSuccess(string email)
        {
            // Arrange
            _mockUserService.Setup(s => s.GetByNickname(It.IsAny<string>())).ReturnsAsync((User)null);
            _mockUserService.Setup(s => s.GetByEmail(It.IsAny<string>())).ReturnsAsync((User)null);
            _mockPasswordHasher.Setup(s => s.HashPassword(It.IsAny<string>())).Returns("hash");

            // Act
            RegistrationResult result = await _authenticationService.Register(
                "testuser", "Password123", "Password123", email, false);

            // Assert
            Assert.That(result, Is.EqualTo(RegistrationResult.Success));
        }

        #endregion

        #region Password Validation

        [TestCase("", TestName = "Register_WithEmptyPassword_ReturnsInvalidPasswordFormat")]
        [TestCase(null, TestName = "Register_WithNullPassword_ReturnsInvalidPasswordFormat")]
        [TestCase("Pass1", TestName = "Register_WithTooShortPassword_ReturnsInvalidPasswordFormat")]
        [TestCase("Pass12345678901234567", TestName = "Register_WithTooLongPassword_ReturnsInvalidPasswordFormat")]
        [TestCase("password123", TestName = "Register_WithPasswordWithoutUppercase_ReturnsInvalidPasswordFormat")]
        [TestCase("Passworddd", TestName = "Register_WithPasswordWithoutDigit_ReturnsInvalidPasswordFormat")]
        public async Task Register_WithInvalidPassword_ReturnsInvalidPasswordFormat(string? password)
        {
            // Arrange & Act
            RegistrationResult result = await _authenticationService.Register(
                "testuser", password, password, "test@example.com", false);
            // Assert
            Assert.That(result, Is.EqualTo(RegistrationResult.InvalidPasswordFormat));
        }

        [Test]
        public async Task Register_WithNonMatchingPasswords_ReturnsPasswordsDoNotMatch()
        {
            // Arrange & Act
            RegistrationResult result = await _authenticationService.Register(
                "testuser", "Password123", "Password456", "test@example.com", false);

            // Assert
            Assert.That(result, Is.EqualTo(RegistrationResult.PasswordsDoNotMatch));
        }

        [TestCase("Pass1234", TestName = "Register_WithMinimumValidPassword_ReturnsSuccess")]
        [TestCase("Pass1234567890123456", TestName = "Register_WithMaximumValidPassword_ReturnsSuccess")]
        public async Task Register_ValidPassword_ReturnsSuccess(string password)
        {
            // Arrange
            _mockUserService.Setup(s => s.GetByNickname(It.IsAny<string>())).ReturnsAsync((User)null);
            _mockUserService.Setup(s => s.GetByEmail(It.IsAny<string>())).ReturnsAsync((User)null);
            _mockPasswordHasher.Setup(s => s.HashPassword(password)).Returns("hash");
            // Act
            RegistrationResult result = await _authenticationService.Register(
                "testuser", password, password, "test@example.com", false);
            // Assert
            Assert.That(result, Is.EqualTo(RegistrationResult.Success));
        }

        #endregion

        #region Normalization Tests

        [Test]
        public async Task Register_NormalizesNicknameToLowercase()
        {
            // Arrange
            string inputNickname = "TestUser";
            string normalizedNickname = "testuser";

            _mockUserService.Setup(s => s.GetByNickname(normalizedNickname)).ReturnsAsync((User)null);
            _mockUserService.Setup(s => s.GetByEmail(It.IsAny<string>())).ReturnsAsync((User)null);
            _mockPasswordHasher.Setup(s => s.HashPassword(It.IsAny<string>())).Returns("hash");

            // Act
            await _authenticationService.Register(inputNickname, "Password123", "Password123", "test@example.com", false);

            // Assert
            _mockUserService.Verify(s => s.GetByNickname(normalizedNickname), Times.Once);
            _mockUserService.Verify(s => s.Create(It.Is<User>(u => u.Nickname == normalizedNickname)), Times.Once);
        }

        [Test]
        public async Task Register_NormalizesEmailToLowercase()
        {
            // Arrange
            string inputEmail = "Test@Example.COM";
            string normalizedEmail = "test@example.com";

            _mockUserService.Setup(s => s.GetByNickname(It.IsAny<string>())).ReturnsAsync((User)null);
            _mockUserService.Setup(s => s.GetByEmail(normalizedEmail)).ReturnsAsync((User)null);
            _mockPasswordHasher.Setup(s => s.HashPassword(It.IsAny<string>())).Returns("hash");

            // Act
            await _authenticationService.Register("testuser", "Password123", "Password123", inputEmail, false);

            // Assert
            _mockUserService.Verify(s => s.GetByEmail(normalizedEmail), Times.Once);
            _mockUserService.Verify(s => s.Create(It.Is<User>(u => u.Email == normalizedEmail)), Times.Once);
        }

        #endregion
    }
}
