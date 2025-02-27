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
    }
}
