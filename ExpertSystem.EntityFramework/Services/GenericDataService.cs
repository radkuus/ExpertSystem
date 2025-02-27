using ExpertSystem.Domain.Models;
using ExpertSystem.Domain.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertSystem.EntityFramework.Services
{
    public class GenericDataService<T> : IDataService<T> where T : BaseObject
    {
        private readonly ExpertSystemDbContextFactory _contextFactory;

        public GenericDataService(ExpertSystemDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<T> Create(T entity)
        {
            using (ExpertSystemDbContext context = _contextFactory.CreateDbContext())
            {
                EntityEntry<T> entityEntry = await context.Set<T>().AddAsync(entity);
                await context.SaveChangesAsync();

                return entityEntry.Entity;
            }
        }

        public async Task<bool> Delete(int id)
        {
            using (ExpertSystemDbContext context = _contextFactory.CreateDbContext())
            {
                T? entity = await context.Set<T>().FirstOrDefaultAsync((e) => e.Id == id);
                if (entity == null)
                {
                    return false;
                }
                context.Set<T>().Remove(entity);
                await context.SaveChangesAsync();

                return true;
            }
        }

        public async Task<T> Get(int id)
        {
            using (ExpertSystemDbContext context = _contextFactory.CreateDbContext())
            {
                T? entity = await context.Set<T>().FirstOrDefaultAsync((e) => e.Id == id);
                if (entity != null)
                {
                    return entity;
                }

                throw new KeyNotFoundException($"Jednostka z id: {id} nie została znaleziona.");
            }
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            using (ExpertSystemDbContext context = _contextFactory.CreateDbContext())
            {
                IEnumerable<T> entities = await context.Set<T>().ToListAsync();
                return entities;
            }
        }

        public async Task<T> Update(int id, T entity)
        {
            using (ExpertSystemDbContext context = _contextFactory.CreateDbContext())
            {
                entity.Id = id;
                context.Set<T>().Update(entity);
                await context.SaveChangesAsync();

                return entity;
            }
        }
    }
}
