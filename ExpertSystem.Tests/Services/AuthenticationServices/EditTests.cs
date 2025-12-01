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
    public class EditTests
    {
        private Mock<IUserService> _mockUserService;
        private Mock<IPasswordHasher> _mockPasswordHasher;
        private AuthenticationService _authenticationService;
        private User _existingUser;

        [SetUp]
        public void SetUp()
        {
            _mockUserService = new Mock<IUserService>();
            _mockPasswordHasher = new Mock<IPasswordHasher>();
            _authenticationService = new AuthenticationService(_mockUserService.Object, _mockPasswordHasher.Object);

            _existingUser = new User
            {
                Id = 1,
                Nickname = "existinguser",
                Email = "existing@example.com",
                PasswordHashed = "old_hashed_password",
                IsAdmin = false
            };
        }

        #region Happy Path

        [Test]
        public async Task Edit_WithAllValidFields_ReturnsSuccess()
        {
            // Arrange
            _mockUserService.Setup(s => s.GetById(1)).ReturnsAsync(_existingUser);
            _mockUserService.Setup(s => s.GetByNickname("newnickname")).ReturnsAsync((User)null);
            _mockUserService.Setup(s => s.GetByEmail("new@example.com")).ReturnsAsync((User)null);
            _mockPasswordHasher.Setup(s => s.HashPassword("NewPassword123")).Returns("new_hashed_password");

            // Act
            EditResult result = await _authenticationService.Edit(1, "newnickname", "NewPassword123", "NewPassword123", "new@example.com");

            // Assert
            Assert.That(result, Is.EqualTo(EditResult.Success));
        }

        [Test]
        public async Task Edit_WithValidData_CallsUpdateWithCorrectUser()
        {
            // Arrange
            _mockUserService.Setup(s => s.GetById(1)).ReturnsAsync(_existingUser);
            _mockUserService.Setup(s => s.GetByNickname("newnickname")).ReturnsAsync((User)null);
            _mockUserService.Setup(s => s.GetByEmail("new@example.com")).ReturnsAsync((User)null);
            _mockPasswordHasher.Setup(s => s.HashPassword("NewPassword123")).Returns("new_hashed_password");

            User capturedUser = null;
            _mockUserService.Setup(s => s.Update(1, It.IsAny<User>()))
                .Callback<int, User>((id, u) => capturedUser = u)
                .ReturnsAsync((int id, User u) => u);

            // Act
            await _authenticationService.Edit(1, "NewNickname", "NewPassword123", "NewPassword123", "New@Example.com");

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(capturedUser, Is.Not.Null);
                Assert.That(capturedUser.Id, Is.EqualTo(1));
                Assert.That(capturedUser.Nickname, Is.EqualTo("newnickname")); // normalized to lowercase
                Assert.That(capturedUser.Email, Is.EqualTo("new@example.com")); // normalized to lowercase
                Assert.That(capturedUser.PasswordHashed, Is.EqualTo("new_hashed_password"));
                Assert.That(capturedUser.IsAdmin, Is.EqualTo(_existingUser.IsAdmin));
            });
        }

        [Test]
        public async Task Edit_OnlyNickname_CallsUpdateWithCorrectId()
        {
            // Arrange
            _mockUserService.Setup(s => s.GetById(1)).ReturnsAsync(_existingUser);
            _mockUserService.Setup(s => s.GetByNickname("newnickname")).ReturnsAsync((User)null);

            // Act
            await _authenticationService.Edit(1, "newnickname", null, null, null);

            // Assert
            _mockUserService.Verify(s => s.Update(1, It.IsAny<User>()), Times.Once);
        }

        [Test]
        public async Task Edit_OnlyNickname_PreservesOtherFields()
        {
            // Arrange
            _mockUserService.Setup(s => s.GetById(1)).ReturnsAsync(_existingUser);
            _mockUserService.Setup(s => s.GetByNickname("newnickname")).ReturnsAsync((User)null);

            User capturedUser = null;
            _mockUserService.Setup(s => s.Update(1, It.IsAny<User>()))
                .Callback<int, User>((id, u) => capturedUser = u)
                .ReturnsAsync((int id, User u) => u);

            // Act
            await _authenticationService.Edit(1, "newnickname", null, null, null);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(capturedUser.Nickname, Is.EqualTo("newnickname"));
                Assert.That(capturedUser.Email, Is.EqualTo(_existingUser.Email));
                Assert.That(capturedUser.PasswordHashed, Is.EqualTo(_existingUser.PasswordHashed));
                Assert.That(capturedUser.IsAdmin, Is.EqualTo(_existingUser.IsAdmin));
            });
        }

        [Test]
        public async Task Edit_OnlyEmail_PreservesOtherFields()
        {
            // Arrange
            _mockUserService.Setup(s => s.GetById(1)).ReturnsAsync(_existingUser);
            _mockUserService.Setup(s => s.GetByEmail("new@example.com")).ReturnsAsync((User)null);

            User capturedUser = null;
            _mockUserService.Setup(s => s.Update(1, It.IsAny<User>()))
                .Callback<int, User>((id, u) => capturedUser = u)
                .ReturnsAsync((int id, User u) => u);

            // Act
            await _authenticationService.Edit(1, null, null, null, "new@example.com");

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(capturedUser.Nickname, Is.EqualTo(_existingUser.Nickname));
                Assert.That(capturedUser.Email, Is.EqualTo("new@example.com"));
                Assert.That(capturedUser.PasswordHashed, Is.EqualTo(_existingUser.PasswordHashed));
                Assert.That(capturedUser.IsAdmin, Is.EqualTo(_existingUser.IsAdmin));
            });
        }

        [Test]
        public async Task Edit_OnlyPassword_PreservesOtherFields()
        {
            // Arrange
            _mockUserService.Setup(s => s.GetById(1)).ReturnsAsync(_existingUser);
            _mockPasswordHasher.Setup(s => s.HashPassword("NewPassword123")).Returns("new_hashed_password");

            User capturedUser = null;
            _mockUserService.Setup(s => s.Update(1, It.IsAny<User>()))
                .Callback<int, User>((id, u) => capturedUser = u)
                .ReturnsAsync((int id, User u) => u);

            // Act
            await _authenticationService.Edit(1, null, "NewPassword123", "NewPassword123", null);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(capturedUser.Nickname, Is.EqualTo(_existingUser.Nickname));
                Assert.That(capturedUser.Email, Is.EqualTo(_existingUser.Email));
                Assert.That(capturedUser.PasswordHashed, Is.EqualTo("new_hashed_password"));
                Assert.That(capturedUser.IsAdmin, Is.EqualTo(_existingUser.IsAdmin));
            });
        }

        #endregion

        #region User Not Found

        [Test]
        public async Task Edit_WithNonExistentUser_ReturnsUserNotFound()
        {
            // Arrange
            _mockUserService.Setup(s => s.GetById(999)).ReturnsAsync((User)null);

            // Act
            EditResult result = await _authenticationService.Edit(999, "newnickname", null, null, null);

            // Assert
            Assert.That(result, Is.EqualTo(EditResult.UserNotFound));
        }

        [Test]
        public async Task Edit_WithNonExistentUser_DoesNotCallUpdate()
        {
            // Arrange
            _mockUserService.Setup(s => s.GetById(999)).ReturnsAsync((User)null);

            // Act
            await _authenticationService.Edit(999, "newnickname", null, null, null);

            // Assert
            _mockUserService.Verify(s => s.Update(It.IsAny<int>(), It.IsAny<User>()), Times.Never);
        }

        #endregion

        #region Nickname Validation

        [TestCase("ab", TestName = "Edit_WithTooShortNickname_ReturnsInvalidNicknameFormat")]
        [TestCase("abc12345678901234", TestName = "Edit_WithTooLongNickname_ReturnsInvalidNicknameFormat")]
        [TestCase("_testuser", TestName = "Edit_WithNicknameStartingWithUnderscore_ReturnsInvalidNicknameFormat")]
        [TestCase("test@user!", TestName = "Edit_WithNicknameContainingSpecialCharacters_ReturnsInvalidNicknameFormat")]
        [TestCase("test user", TestName = "Edit_WithNicknameContainingSpace_ReturnsInvalidNicknameFormat")]
        public async Task Edit_WithInvalidNickname_ReturnsInvalidNicknameFormat(string nickname)
        {
            // Arrange
            _mockUserService.Setup(s => s.GetById(1)).ReturnsAsync(_existingUser);

            // Act
            EditResult result = await _authenticationService.Edit(1, nickname, null, null, null);

            // Assert
            Assert.That(result, Is.EqualTo(EditResult.InvalidNicknameFormat));
        }

        [Test]
        public async Task Edit_WithNicknameAlreadyTakenByAnotherUser_ReturnsNicknameAlreadyTaken()
        {
            // Arrange
            User anotherUser = new User { Id = 2, Nickname = "takennickname" };
            _mockUserService.Setup(s => s.GetById(1)).ReturnsAsync(_existingUser);
            _mockUserService.Setup(s => s.GetByNickname("takennickname")).ReturnsAsync(anotherUser);

            // Act
            EditResult result = await _authenticationService.Edit(1, "takennickname", null, null, null);

            // Assert
            Assert.That(result, Is.EqualTo(EditResult.NicknameAlreadyTaken));
        }

        [Test]
        public async Task Edit_WithOwnCurrentNickname_ReturnsNoChangesDetected()
        {
            // Arrange
            _mockUserService.Setup(s => s.GetById(1)).ReturnsAsync(_existingUser);
            _mockUserService.Setup(s => s.GetByNickname("existinguser")).ReturnsAsync(_existingUser);

            // Act
            EditResult result = await _authenticationService.Edit(1, "existinguser", null, null, null);

            // Assert
            Assert.That(result, Is.EqualTo(EditResult.NoChangesDetected));
        }

        [TestCase("test_user-1")]
        [TestCase("User123")]
        [TestCase("abc")]
        [TestCase("a123456789012345")] // 16 characters starting with letter
        public async Task Edit_WithValidNicknameFormats_ReturnsSuccess(string nickname)
        {
            // Arrange
            _mockUserService.Setup(s => s.GetById(1)).ReturnsAsync(_existingUser);
            _mockUserService.Setup(s => s.GetByNickname(nickname.ToLower())).ReturnsAsync((User)null);

            // Act
            EditResult result = await _authenticationService.Edit(1, nickname, null, null, null);

            // Assert
            Assert.That(result, Is.EqualTo(EditResult.Success));
        }

        #endregion

        #region Email Validation

        [TestCase("testexample.com", TestName = "Edit_WithEmailWithoutAtSymbol_ReturnsInvalidEmailFormat")]
        [TestCase("test@", TestName = "Edit_WithEmailWithoutDomain_ReturnsInvalidEmailFormat")]
        [TestCase("test@example", TestName = "Edit_WithEmailWithoutTld_ReturnsInvalidEmailFormat")]
        [TestCase("test@example.c", TestName = "Edit_WithEmailWithShortTld_ReturnsInvalidEmailFormat")]
        [TestCase("@example.com", TestName = "Edit_WithEmailWithoutLocalPart_ReturnsInvalidEmailFormat")]
        public async Task Edit_WithInvalidEmail_ReturnsInvalidEmailFormat(string email)
        {
            // Arrange
            _mockUserService.Setup(s => s.GetById(1)).ReturnsAsync(_existingUser);

            // Act
            EditResult result = await _authenticationService.Edit(1, null, null, null, email);

            // Assert
            Assert.That(result, Is.EqualTo(EditResult.InvalidEmailFormat));
        }

        [Test]
        public async Task Edit_WithEmailAlreadyTakenByAnotherUser_ReturnsEmailAlreadyTaken()
        {
            // Arrange
            User anotherUser = new User { Id = 2, Email = "taken@example.com" };
            _mockUserService.Setup(s => s.GetById(1)).ReturnsAsync(_existingUser);
            _mockUserService.Setup(s => s.GetByEmail("taken@example.com")).ReturnsAsync(anotherUser);

            // Act
            EditResult result = await _authenticationService.Edit(1, null, null, null, "taken@example.com");

            // Assert
            Assert.That(result, Is.EqualTo(EditResult.EmailAlreadyTaken));
        }

        [Test]
        public async Task Edit_WithOwnCurrentEmail_ReturnsNoChangesDetected()
        {
            // Arrange
            _mockUserService.Setup(s => s.GetById(1)).ReturnsAsync(_existingUser);
            _mockUserService.Setup(s => s.GetByEmail("existing@example.com")).ReturnsAsync(_existingUser);

            // Act
            EditResult result = await _authenticationService.Edit(1, null, null, null, "existing@example.com");

            // Assert
            Assert.That(result, Is.EqualTo(EditResult.NoChangesDetected));
        }

        [TestCase("test@example.com")]
        [TestCase("test.name@example.com")]
        [TestCase("test+tag@example.com")]
        [TestCase("test@sub.example.com")]
        [TestCase("test123@example123.com")]
        public async Task Edit_WithValidEmailFormats_ReturnsSuccess(string email)
        {
            // Arrange
            _mockUserService.Setup(s => s.GetById(1)).ReturnsAsync(_existingUser);
            _mockUserService.Setup(s => s.GetByEmail(email.ToLower())).ReturnsAsync((User)null);

            // Act
            EditResult result = await _authenticationService.Edit(1, null, null, null, email);

            // Assert
            Assert.That(result, Is.EqualTo(EditResult.Success));
        }

        #endregion

        #region Password Validation

        [Test]
        public async Task Edit_WithOnlyPasswordProvided_ReturnsOnlyOnePasswordEntered()
        {
            // Arrange
            _mockUserService.Setup(s => s.GetById(1)).ReturnsAsync(_existingUser);

            // Act
            EditResult result = await _authenticationService.Edit(1, null, "NewPassword123", null, null);

            // Assert
            Assert.That(result, Is.EqualTo(EditResult.OnlyOnePasswordEntered));
        }

        [Test]
        public async Task Edit_WithOnlyConfirmPasswordProvided_ReturnsOnlyOnePasswordEntered()
        {
            // Arrange
            _mockUserService.Setup(s => s.GetById(1)).ReturnsAsync(_existingUser);

            // Act
            EditResult result = await _authenticationService.Edit(1, null, null, "NewPassword123", null);

            // Assert
            Assert.That(result, Is.EqualTo(EditResult.OnlyOnePasswordEntered));
        }

        [Test]
        public async Task Edit_WithEmptyPasswordAndFilledConfirmPassword_ReturnsOnlyOnePasswordEntered()
        {
            // Arrange
            _mockUserService.Setup(s => s.GetById(1)).ReturnsAsync(_existingUser);

            // Act
            EditResult result = await _authenticationService.Edit(1, null, "", "NewPassword123", null);

            // Assert
            Assert.That(result, Is.EqualTo(EditResult.OnlyOnePasswordEntered));
        }

        [Test]
        public async Task Edit_WithNonMatchingPasswords_ReturnsPasswordsDoNotMatch()
        {
            // Arrange
            _mockUserService.Setup(s => s.GetById(1)).ReturnsAsync(_existingUser);

            // Act
            EditResult result = await _authenticationService.Edit(1, null, "NewPassword123", "DifferentPassword123", null);

            // Assert
            Assert.That(result, Is.EqualTo((EditResult)RegistrationResult.PasswordsDoNotMatch));
        }

        [TestCase("Pass1", TestName = "Edit_WithTooShortPassword_ReturnsInvalidPasswordFormat")]
        [TestCase("Pass12345678901234567", TestName = "Edit_WithTooLongPassword_ReturnsInvalidPasswordFormat")]
        [TestCase("password123", TestName = "Edit_WithPasswordWithoutUppercase_ReturnsInvalidPasswordFormat")]
        [TestCase("Passworddd", TestName = "Edit_WithPasswordWithoutDigit_ReturnsInvalidPasswordFormat")]
        public async Task Edit_WithInvalidPassword_ReturnsInvalidPasswordFormat(string password)
        {
            // Arrange
            _mockUserService.Setup(s => s.GetById(1)).ReturnsAsync(_existingUser);

            // Act
            EditResult result = await _authenticationService.Edit(1, null, password, password, null);

            // Assert
            Assert.That(result, Is.EqualTo((EditResult)RegistrationResult.InvalidPasswordFormat));
        }

        [TestCase("Pass1234", TestName = "Edit_WithMinimumValidPassword_ReturnsSuccess")]
        [TestCase("Pass1234567890123456", TestName = "Edit_WithMaximumValidPassword_ReturnsSuccess")]
        [TestCase("NewPassword123", TestName = "Edit_WithTypicalValidPassword_ReturnsSuccess")]
        public async Task Edit_WithValidPassword_ReturnsSuccess(string password)
        {
            // Arrange
            _mockUserService.Setup(s => s.GetById(1)).ReturnsAsync(_existingUser);
            _mockPasswordHasher.Setup(s => s.HashPassword(password)).Returns("new_hashed_password");

            // Act
            EditResult result = await _authenticationService.Edit(1, null, password, password, null);

            // Assert
            Assert.That(result, Is.EqualTo(EditResult.Success));
        }

        [Test]
        public async Task Edit_WithValidPassword_HashesNewPassword()
        {
            // Arrange
            string newPassword = "NewPassword123";
            _mockUserService.Setup(s => s.GetById(1)).ReturnsAsync(_existingUser);
            _mockPasswordHasher.Setup(s => s.HashPassword(newPassword)).Returns("new_hashed_password");

            User capturedUser = null;
            _mockUserService.Setup(s => s.Update(1, It.IsAny<User>()))
                .Callback<int, User>((id, u) => capturedUser = u)
                .ReturnsAsync((int id, User u) => u);

            // Act
            await _authenticationService.Edit(1, null, newPassword, newPassword, null);

            // Assert
            _mockPasswordHasher.Verify(s => s.HashPassword(newPassword), Times.Once);
            Assert.That(capturedUser.PasswordHashed, Is.EqualTo("new_hashed_password"));
        }

        [Test]
        public async Task Edit_WithoutPassword_PreservesExistingPasswordHash()
        {
            // Arrange
            _mockUserService.Setup(s => s.GetById(1)).ReturnsAsync(_existingUser);
            _mockUserService.Setup(s => s.GetByNickname("newnickname")).ReturnsAsync((User)null);

            User capturedUser = null;
            _mockUserService.Setup(s => s.Update(1, It.IsAny<User>()))
                .Callback<int, User>((id, u) => capturedUser = u)
                .ReturnsAsync((int id, User u) => u);

            // Act
            await _authenticationService.Edit(1, "newnickname", null, null, null);

            // Assert
            _mockPasswordHasher.Verify(s => s.HashPassword(It.IsAny<string>()), Times.Never);
            Assert.That(capturedUser.PasswordHashed, Is.EqualTo(_existingUser.PasswordHashed));
        }

        #endregion

        #region No Changes Detected

        [Test]
        public async Task Edit_WithAllNullFields_ReturnsNoChangesDetected()
        {
            // Arrange
            _mockUserService.Setup(s => s.GetById(1)).ReturnsAsync(_existingUser);

            // Act
            EditResult result = await _authenticationService.Edit(1, null, null, null, null);

            // Assert
            Assert.That(result, Is.EqualTo(EditResult.NoChangesDetected));
        }

        [Test]
        public async Task Edit_WithAllEmptyFields_ReturnsNoChangesDetected()
        {
            // Arrange
            _mockUserService.Setup(s => s.GetById(1)).ReturnsAsync(_existingUser);

            // Act
            EditResult result = await _authenticationService.Edit(1, "", "", "", "");

            // Assert
            Assert.That(result, Is.EqualTo(EditResult.NoChangesDetected));
        }

        [Test]
        public async Task Edit_WithSameNicknameAndEmail_ReturnsNoChangesDetected()
        {
            // Arrange
            _mockUserService.Setup(s => s.GetById(1)).ReturnsAsync(_existingUser);
            _mockUserService.Setup(s => s.GetByNickname("existinguser")).ReturnsAsync(_existingUser);
            _mockUserService.Setup(s => s.GetByEmail("existing@example.com")).ReturnsAsync(_existingUser);

            // Act
            EditResult result = await _authenticationService.Edit(1, "existinguser", null, null, "existing@example.com");

            // Assert
            Assert.That(result, Is.EqualTo(EditResult.NoChangesDetected));
        }

        [Test]
        public async Task Edit_WithSameDataDifferentCase_ReturnsNoChangesDetected()
        {
            // Arrange
            _mockUserService.Setup(s => s.GetById(1)).ReturnsAsync(_existingUser);
            _mockUserService.Setup(s => s.GetByNickname("existinguser")).ReturnsAsync(_existingUser);
            _mockUserService.Setup(s => s.GetByEmail("existing@example.com")).ReturnsAsync(_existingUser);

            // Act
            EditResult result = await _authenticationService.Edit(1, "ExistingUser", null, null, "EXISTING@EXAMPLE.COM");

            // Assert
            Assert.That(result, Is.EqualTo(EditResult.NoChangesDetected));
        }

        [Test]
        public async Task Edit_WithNoChanges_DoesNotCallUpdate()
        {
            // Arrange
            _mockUserService.Setup(s => s.GetById(1)).ReturnsAsync(_existingUser);

            // Act
            await _authenticationService.Edit(1, null, null, null, null);

            // Assert
            _mockUserService.Verify(s => s.Update(It.IsAny<int>(), It.IsAny<User>()), Times.Never);
        }

        #endregion

        #region Normalization Tests

        [Test]
        public async Task Edit_NormalizesNicknameToLowercase()
        {
            // Arrange
            string inputNickname = "NewNickName";
            string normalizedNickname = "newnickname";

            _mockUserService.Setup(s => s.GetById(1)).ReturnsAsync(_existingUser);
            _mockUserService.Setup(s => s.GetByNickname(normalizedNickname)).ReturnsAsync((User)null);

            User capturedUser = null;
            _mockUserService.Setup(s => s.Update(1, It.IsAny<User>()))
                .Callback<int, User>((id, u) => capturedUser = u)
                .ReturnsAsync((int id, User u) => u);

            // Act
            await _authenticationService.Edit(1, inputNickname, null, null, null);

            // Assert
            _mockUserService.Verify(s => s.GetByNickname(normalizedNickname), Times.Once);
            Assert.That(capturedUser.Nickname, Is.EqualTo(normalizedNickname));
        }

        [Test]
        public async Task Edit_NormalizesEmailToLowercase()
        {
            // Arrange
            string inputEmail = "New@EXAMPLE.COM";
            string normalizedEmail = "new@example.com";

            _mockUserService.Setup(s => s.GetById(1)).ReturnsAsync(_existingUser);
            _mockUserService.Setup(s => s.GetByEmail(normalizedEmail)).ReturnsAsync((User)null);

            User capturedUser = null;
            _mockUserService.Setup(s => s.Update(1, It.IsAny<User>()))
                .Callback<int, User>((id, u) => capturedUser = u)
                .ReturnsAsync((int id, User u) => u);

            // Act
            await _authenticationService.Edit(1, null, null, null, inputEmail);

            // Assert
            _mockUserService.Verify(s => s.GetByEmail(normalizedEmail), Times.Once);
            Assert.That(capturedUser.Email, Is.EqualTo(normalizedEmail));
        }

        #endregion

    }
}
