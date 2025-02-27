using ExpertSystem.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertSystem.EntityFramework
{
    public class ExpertSystemDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public ExpertSystemDbContext(DbContextOptions options) : base(options) { }
    }
}
