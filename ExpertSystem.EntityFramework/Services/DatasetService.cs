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

        public async Task AddDataset(Dataset dataset)
        {
            using (ExpertSystemDbContext context = _contextFactory.CreateDbContext())
            {
                context.Datasets.Add(dataset);
                await context.SaveChangesAsync();
            }
        }

        public async Task<Dataset> GetDatasetById(int id)
        {
            using(ExpertSystemDbContext context = _contextFactory.CreateDbContext())
            {
                var dataset = await context.Datasets.SingleOrDefaultAsync(x => x.Id == id);
                return dataset;
            }
        }

        public async Task<List<Dataset>> GetAll()
        {
            using (ExpertSystemDbContext context = _contextFactory.CreateDbContext())
            {
                return await context.Datasets.ToListAsync();
            }
        }

        public async Task RemoveDataset(int id)
        {
            using (ExpertSystemDbContext context = _contextFactory.CreateDbContext())
            {
                var dataset = await GetDatasetById(id);
                if (dataset != null)
                {
                    context.Datasets.Remove(dataset);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
