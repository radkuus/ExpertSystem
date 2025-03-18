using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpertSystem.Domain.Models;
using ExpertSystem.Domain.Services;
using ExpertSystem.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace ExpertSystem.WPF.Services
{
    class DatasetService : IDatasetService
    {
        private readonly ExpertSystemDbContextFactory _contextFactory;

        public DatasetService(ExpertSystemDbContextFactory contextFactory)
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

        public async Task<Database> GetDatabaseById(int id)
        {
            using(ExpertSystemDbContext context = _contextFactory.CreateDbContext())
            {
                var weather = await context.Databases.SingleOrDefaultAsync(x => x.Id == id);
                return weather;
            }
        }

        public async Task<List<Database>> GetAll()
        {
            using (ExpertSystemDbContext context = _contextFactory.CreateDbContext())
            {
                return await context.Databases.ToListAsync();
            }
        }

        public async Task RemoveDatabase(int id)
        {
            using (ExpertSystemDbContext context = _contextFactory.CreateDbContext())
            {
                var database = await GetDatabaseById(id);
                if (database != null)
                {
                    context.Databases.Remove(database);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
