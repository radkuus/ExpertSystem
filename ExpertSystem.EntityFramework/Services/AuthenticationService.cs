using ExpertSystem.Domain.Exceptions;
using ExpertSystem.Domain.Models;
using ExpertSystem.Domain.Services;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ExpertSystem.EntityFramework.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserService _userService;
        private readonly IPasswordHasher _passwordhasher;

        public AuthenticationService(IUserService userService, IPasswordHasher passwordHasher)
        {
            _userService = userService;
            _passwordhasher = passwordHasher;
        }

        public async Task<User> Login(string nickname, string password)
        {
            User storedUser = await _userService.GetByNickname(nickname);

            if (storedUser == null)
            {
                throw new UserNotFoundException(nickname);
            }

            PasswordVerificationResult passwordsResult = _passwordhasher.VerifyHashedPassword(storedUser.PasswordHashed, password);

            if(passwordsResult != PasswordVerificationResult.Success)
            {
                throw new InvalidPasswordException(nickname,password);
            }
            return storedUser;
        }

        public async Task<RegistrationResult> Register(string nickname, string password, string confirmPassword, string email, bool isAdmin)
        {
            RegistrationResult result = RegistrationResult.Success;

            if (password != confirmPassword) 
            {
                result = RegistrationResult.PasswordsDoNotMatch;
            }

            User emailUser = await _userService.GetByEmail(email);
            if(emailUser != null)
            {
                result = RegistrationResult.EmailAlreadyTaken;
            }

            User nicknameUser = await _userService.GetByNickname(nickname);
            if (nicknameUser != null)
            {
                result = RegistrationResult.NicknameAlreadyTaken;
            }

            if(result==RegistrationResult.Success)
            {
                string passwordHashed = _passwordhasher.HashPassword(password);

                User user = new User()
                {
                    Nickname = nickname,
                    PasswordHashed = passwordHashed,
                    Email = email,
                    IsAdmin = isAdmin
                };

                await _userService.Create(user);
            }

            return result;
        }

        public async Task<EditResult> Edit(int id, string nickname, string password, string confirmPassword, string email)
        {
            EditResult result = EditResult.Success;

            User existingUser = await _userService.GetById(id);
            if (existingUser == null)
            {
                return EditResult.UserNotFound;
            }

            if ((password != null && confirmPassword == null) || (password == null && confirmPassword != null))
            {
                result = EditResult.OnlyOnePasswordEntered;
            }

            if (password != null && confirmPassword != null)
            {
                if (password != confirmPassword)
                {
                    result = EditResult.PasswordsDoNotMatch;
                }
            }

            if (nickname != null && nickname != existingUser.Nickname)
            {
                User nicknameUser = await _userService.GetByNickname(nickname);
                if (nicknameUser != null && nicknameUser.Id != id)
                {
                    result = EditResult.NicknameAlreadyTaken;
                }
            }

            if (email != null && email != existingUser.Email)
            {
                User emailUser = await _userService.GetByEmail(email);
                if (emailUser != null && emailUser.Id != id)
                {
                    result = EditResult.EmailAlreadyTaken;
                }
            }

            if (email == existingUser.Email && nickname == existingUser.Nickname && 
                password == null && confirmPassword == null)
            {
                result = EditResult.NoChangesDetected;
            }

            if (result == EditResult.Success)
            {
                User updatedUser = new User
                {
                    Id = existingUser.Id,
                    Nickname = existingUser.Nickname,
                    Email = existingUser.Email,
                    PasswordHashed = existingUser.PasswordHashed,
                    IsAdmin = existingUser.IsAdmin
                };

                if (nickname != null)
                {
                    updatedUser.Nickname = nickname;
                }

                if (email != null)
                {
                    updatedUser.Email = email;
                }

                if (password != null)
                {
                    updatedUser.PasswordHashed = _passwordhasher.HashPassword(password);
                }

                await _userService.Update(id, updatedUser);
            }

            return result;
        }
    }
}
