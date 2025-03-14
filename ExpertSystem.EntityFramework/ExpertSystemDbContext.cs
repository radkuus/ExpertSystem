using ExpertSystem.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ExpertSystem.EntityFramework
{
    public class ExpertSystemDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Database> Databases { get; set; }

        public DbSet<Experiment> Experiments { get; set; }

        public DbSet<ModelConfiguration> ModelConfigurations { get; set; }

        public DbSet<ModelResult> ModelResults { get; set; }

        public DbSet<Plot> Plots { get; set; }

        public DbSet<DecisionRule> DecisionRules { get; set; }
        public ExpertSystemDbContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresEnum<ModelType>();

            modelBuilder.Entity<ModelConfiguration>()
                .Property(e => e.ModelType)
                .HasColumnType("model_type");

            modelBuilder.HasPostgresEnum<SetType>();

            modelBuilder.Entity<ModelResult>()
                .Property(mr => mr.SetType)
                .HasColumnType("set_type");

            modelBuilder.HasPostgresEnum<PlotType>();

            modelBuilder.Entity<Plot>()
                .Property(p => p.PlotType)
                .HasColumnType("plot_type");

            modelBuilder.HasPostgresEnum<Metric>();

            modelBuilder.Entity<DecisionRule>()
                .Property(dr => dr.Metric)
                .HasColumnType("metric");

            modelBuilder.HasPostgresEnum<Operator>();

            modelBuilder.Entity<DecisionRule>()
                .Property(dr => dr.Operator)
                .HasColumnType("operator");

            modelBuilder.HasPostgresEnum<LogicOperator>();

            modelBuilder.Entity<DecisionRule>()
                .Property(dr => dr.LogicOperator)
                .HasColumnType("logic_operator");


            modelBuilder.Entity<ModelConfiguration>()
                .Property(mc => mc.Hyperparameters)
                .HasColumnType("jsonb");

            modelBuilder.Entity<ModelResult>()
                .Property(mr => mr.CreatedAt)
                .HasColumnType("timestamp");


            modelBuilder.Entity<User>().Property(u => u.Id).HasColumnName("UserId");
            modelBuilder.Entity<Database>().Property(d => d.Id).HasColumnName("DatabaseId");
            modelBuilder.Entity<Experiment>().Property(e => e.Id).HasColumnName("ExperimentId");
            modelBuilder.Entity<ModelConfiguration>().Property(mc => mc.Id).HasColumnName("ConfigId");
            modelBuilder.Entity<ModelResult>().Property(mr => mr.Id).HasColumnName("ResultId");
            modelBuilder.Entity<Plot>().Property(p => p.Id).HasColumnName("PlotId");
            modelBuilder.Entity<DecisionRule>().Property(dr => dr.Id).HasColumnName("RuleId");

            // creating FK and relations 

            modelBuilder.Entity<Database>()
                .HasOne(d => d.User)
                .WithMany(u => u.Databases)
                .HasForeignKey(d => d.UserId);

            modelBuilder.Entity<Experiment>()
                .HasOne(e => e.User)
                .WithMany(u => u.Experiments)
                .HasForeignKey(e => e.UserId);

           modelBuilder.Entity<Experiment>()
                .HasOne(e => e.Database)
                .WithMany(d => d.Experiments)
                .HasForeignKey(e => e.DatabaseID);

           modelBuilder.Entity<ModelConfiguration>()
                .HasOne(mc => mc.Experiment)
                .WithMany(e => e.ModelConfigurations)
                .HasForeignKey(mc => mc.ExperimentId);

           modelBuilder.Entity<DecisionRule>()
                .HasOne(dr => dr.Experiment)
                .WithMany(e => e.DecisionRules)
                .HasForeignKey(dr => dr.ExperimentID);

           modelBuilder.Entity<ModelResult>()
                .HasOne(mr => mr.ModelConfiguration)
                .WithMany(mc => mc.ModelResults)
                .HasForeignKey(mr => mr.ConfigId);

           modelBuilder.Entity<Plot>()
                .HasOne(p => p.ModelResult)
                .WithMany(mr => mr.Plots)
                .HasForeignKey(p => p.ResultId);
        }
    } 
}
