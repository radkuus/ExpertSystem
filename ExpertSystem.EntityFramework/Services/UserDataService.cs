using ExpertSystem.Domain.Models;
using ExpertSystem.Domain.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertSystem.EntityFramework.Services
{
    public class UserDataService : GenericDataService<User>, IUserService
    {
        private readonly ExpertSystemDbContextFactory _contextFactory;

        public UserDataService(ExpertSystemDbContextFactory contextFactory) : base(contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<User> GetByEmail(string email)
        {
            using (ExpertSystemDbContext context = _contextFactory.CreateDbContext())
            {
                return await context.Users.FirstOrDefaultAsync(a => a.Email == email);
            }
        }

        public async Task<User> GetByNickname(string nickname)
        {
            using (ExpertSystemDbContext context = _contextFactory.CreateDbContext())
            {
                return await context.Users.FirstOrDefaultAsync(a => a.Nickname == nickname);
            }
        }
    }
}
