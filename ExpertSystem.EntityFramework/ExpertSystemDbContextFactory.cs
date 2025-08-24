using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertSystem.EntityFramework
{
    public class ExpertSystemDbContextFactory : IDesignTimeDbContextFactory<ExpertSystemDbContext>
    {
        public ExpertSystemDbContext CreateDbContext(string[] args = null)
        {
            var options = new DbContextOptionsBuilder<ExpertSystemDbContext>();
            options.UseNpgsql(@"Host=localhost;Username=postgres;Password=123;Database=ExpertSystemDB");

            return new ExpertSystemDbContext(options.Options);
        }
    }
}
