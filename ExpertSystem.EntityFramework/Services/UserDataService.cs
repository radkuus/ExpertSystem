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
        private readonly IDbContextFactory<ExpertSystemDbContext> _contextFactory;

        public UserDataService(IDbContextFactory<ExpertSystemDbContext> contextFactory) : base(contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<User> GetByEmail(string email)
        {
            await using (ExpertSystemDbContext context = _contextFactory.CreateDbContext())
            {
                return await context.Users.FirstOrDefaultAsync(a => a.Email == email);
            }
        }

        public async Task<User> GetById(int id)
        {
            await using (ExpertSystemDbContext context = _contextFactory.CreateDbContext())
            {
                return await context.Users.FirstOrDefaultAsync(a => a.Id == id);
            }
        }

        public async Task<User> GetByNickname(string nickname)
        {
            await using (ExpertSystemDbContext context = _contextFactory.CreateDbContext())
            {
                return await context.Users.FirstOrDefaultAsync(a => a.Nickname == nickname);
            }
        }
    }
}
