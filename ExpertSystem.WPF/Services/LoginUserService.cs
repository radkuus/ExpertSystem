using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpertSystem.Domain.Models;
using ExpertSystem.Domain.Services;
using ExpertSystem.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace ExpertSystem.WPF.Services
{
    class LoginUserService : ILoginUserService
    {
        private readonly ExpertSystemDbContextFactory _contextFactory;

        public LoginUserService(ExpertSystemDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task AddDatabase(Database database)
        {
            using (ExpertSystemDbContext context = _contextFactory.CreateDbContext())
            {
                context.Databases.Add(database);
                await context.SaveChangesAsync();
            }
        }
    }
}
