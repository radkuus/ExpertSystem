using ExpertSystem.Domain.Exceptions;
using ExpertSystem.Domain.Models;
using ExpertSystem.Domain.Services;
using Microsoft.AspNet.Identity;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ExpertSystem.EntityFramework.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserService _userService;
        private readonly IPasswordHasher _passwordHasher;

        public AuthenticationService(IUserService userService, IPasswordHasher passwordHasher)
        {
            _userService = userService;
            _passwordHasher = passwordHasher;
        }

        private (bool IsValid, RegistrationResult Result) ValidateInput(string email, string nickname, string password, string confirmPassword) // private () X () => return tuple 
        {
            if (string.IsNullOrEmpty(nickname))
                return (false, RegistrationResult.InvalidNicknameFormat);

            if (string.IsNullOrEmpty(email))
                return (false, RegistrationResult.InvalidEmailFormat);


            if (string.IsNullOrEmpty(password))
                return (false, RegistrationResult.InvalidPasswordFormat);

            var nicknameRegex = new Regex(@"^[a-zA-Z0-9][a-zA-Z0-9_-]{2,15}$");  // first character must be a letter or number, the rest must have min. 2 characters (max. 15)
            if (!nicknameRegex.IsMatch(nickname))
                return (false, RegistrationResult.InvalidNicknameFormat);

            var emailRegex = new Regex(@"^[a-zA-Z0-9._%+-]+@(?:[a-zA-Z0-9-]+\.)+[a-zA-Z0-9]{2,}$");     // smth@smth.smth (after comma min. 2 characters)
            if (!emailRegex.IsMatch(email))
                return (false, RegistrationResult.InvalidEmailFormat);


            var passwordRegex = new Regex(@"^(?=.*[A-Z])(?=.*\d).{8,20}$"); // min. 8 characters, max. 20 characters., 1 capital letter and 1 number min.!
            if (!passwordRegex.IsMatch(password))
                return (false, RegistrationResult.InvalidPasswordFormat);

            if (password != confirmPassword)
                return (false, RegistrationResult.PasswordsDoNotMatch);

            return (true, RegistrationResult.Success);

        }

        public async Task<User> Login(string nickname, string password)
        {
            User storedUser = await _userService.GetByNickname(nickname);

            if (storedUser == null)
            {
                throw new UserNotFoundException(nickname);
            }

            PasswordVerificationResult passwordsResult = _passwordHasher.VerifyHashedPassword(storedUser.PasswordHashed, password);

            if (passwordsResult != PasswordVerificationResult.Success)
            {
                throw new InvalidPasswordException(nickname, password);
            }
            return storedUser;
        }

        public async Task<RegistrationResult> Register(string nickname, string password, string confirmPassword, string email, bool isAdmin)
        {

            // validation
            var (isValid, validationResult) = ValidateInput(email, nickname, password, confirmPassword);
            if (!isValid)
                return validationResult;

            string normalizedNickname = nickname.ToLower();
            string normalizedEmail = email.ToLower();

            // check for duplicates 
            User nicknameUser = await _userService.GetByNickname(normalizedNickname);
            if (nicknameUser != null)
            {
                if (string.Equals(nicknameUser.Nickname, nickname, StringComparison.OrdinalIgnoreCase))
                    return RegistrationResult.NicknameAlreadyTaken;
            }

            User emailUser = await _userService.GetByEmail(normalizedEmail);
            if (emailUser != null)
            {
                if (string.Equals(emailUser.Email, email, StringComparison.OrdinalIgnoreCase))
                    return RegistrationResult.EmailAlreadyTaken;
            }


            string passwordHashed = _passwordHasher.HashPassword(password);

            // if everything good -> create new account
            User user = new User()
            {
                Nickname = normalizedNickname,
                PasswordHashed = passwordHashed,
                Email = normalizedEmail,
                IsAdmin = isAdmin
            };

            await _userService.Create(user);
            return RegistrationResult.Success;
        }

        public async Task<EditResult> Edit(int id, string? nickname, string? password, string? confirmPassword, string? email)
        {
            User existingUser = await _userService.GetById(id);
            if (existingUser == null)
                return EditResult.UserNotFound;

            // if nickname is NOT empty/null -> validation 
            if (!string.IsNullOrEmpty(nickname))
            {
                var nicknameRegex = new Regex(@"^[a-zA-Z0-9][a-zA-Z0-9_-]{2,15}$");
                if (!nicknameRegex.IsMatch(nickname))  // if not fit --> return 
                    return EditResult.InvalidNicknameFormat;

                // checks whether such a user exist
                User nicknameCheck = await _userService.GetByNickname(nickname.ToLower());
                if (nicknameCheck != null && nicknameCheck.Id != id) // if nickname is taken by another user --> return
                    return EditResult.NicknameAlreadyTaken;
            }

            // if email is NOT empty/null -> validation (the same us nickname)
            if (!string.IsNullOrEmpty(email))
            {
                var emailRegex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
                if (!emailRegex.IsMatch(email))
                    return EditResult.InvalidEmailFormat;

                User emailCheck = await _userService.GetByEmail(email.ToLower());
                if (emailCheck != null && emailCheck.Id != id)
                    return EditResult.EmailAlreadyTaken;
            }

            // if password is NOT empty/null OR confirmPassword is NOT empty/null
            if (!string.IsNullOrEmpty(password) || !string.IsNullOrEmpty(confirmPassword))
            {
                if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword)) // if only password/confirmPassword is given ---> return
                    return EditResult.OnlyOnePasswordEntered;

                // validation
                var (isValid, validationResult) = ValidateInput(
                    email ?? existingUser.Email,
                    nickname ?? existingUser.Nickname,
                    password,
                    confirmPassword);

                if (!isValid)
                    return (EditResult)validationResult;
            }

            // if nothing changes 
            if (string.IsNullOrEmpty(nickname) && string.IsNullOrEmpty(email) && string.IsNullOrEmpty(password))
                return EditResult.NoChangesDetected;

            // if given data is the same --> return
            if (string.Equals(nickname, existingUser.Nickname, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(email, existingUser.Email, StringComparison.OrdinalIgnoreCase) &&
                string.IsNullOrEmpty(password))
                return EditResult.NoChangesDetected;

            // update user
            User updatedUser = new User
            {
                Id = id,
                Nickname = nickname ?? existingUser.Nickname,
                Email = email ?? existingUser.Email,
                PasswordHashed = password != null ? _passwordHasher.HashPassword(password) : existingUser.PasswordHashed,
                IsAdmin = existingUser.IsAdmin
            };

            await _userService.Update(id, updatedUser);
            return EditResult.Success;
        }
    }
}