﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ExpertSystem.Domain.Models;

namespace ExpertSystem.EntityFramework
{
    public class ExpertSystemDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Dataset> Datasets { get; set; }
        public DbSet<Experiment> Experiments { get; set; }
        public DbSet<ModelConfiguration> ModelConfigurations { get; set; }
        public DbSet<ModelResult> ModelResults { get; set; }
        public DbSet<Plot> Plots { get; set; }
        public DbSet<DecisionRule> DecisionRules { get; set; }

        public ExpertSystemDbContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // converter
            var modelTypeConverter = new ValueConverter<ModelType, string>(
                v => v.ToString(),
                v => Enum.Parse<ModelType>(v)
            );

            modelBuilder.HasPostgresEnum<ModelType>();
            modelBuilder.Entity<ModelConfiguration>()
                .Property(e => e.ModelType)
                .HasConversion(modelTypeConverter);

            // converter
            var setTypeConverter = new ValueConverter<SetType, string>(
                v => v.ToString(),
                v => Enum.Parse<SetType>(v)
            );

            modelBuilder.HasPostgresEnum<SetType>();
            modelBuilder.Entity<ModelResult>()
                .Property(mr => mr.SetType)
                .HasConversion(setTypeConverter);

            
            var plotTypeConverter = new ValueConverter<PlotType, string>(
                v => v.ToString(),
                v => Enum.Parse<PlotType>(v)
            );

            modelBuilder.HasPostgresEnum<PlotType>();
            modelBuilder.Entity<Plot>()
                .Property(p => p.PlotType)
                .HasConversion(plotTypeConverter);

            var metricConverter = new ValueConverter<Metric, string>(
                v => v.ToString(),
                v => Enum.Parse<Metric>(v)
            );

            modelBuilder.HasPostgresEnum<Metric>();
            modelBuilder.Entity<DecisionRule>()
                .Property(dr => dr.Metric)
                .HasConversion(metricConverter);

            var operatorConverter = new ValueConverter<Operator, string>(
                v => v.ToString(),
                v => Enum.Parse<Operator>(v)
            );

            modelBuilder.HasPostgresEnum<Operator>();
            modelBuilder.Entity<DecisionRule>()
                .Property(dr => dr.Operator)
                .HasConversion(operatorConverter);

            var logicOperatorConverter = new ValueConverter<LogicOperator, string>(
                v => v.ToString(),
                v => Enum.Parse<LogicOperator>(v)
            );

            modelBuilder.HasPostgresEnum<LogicOperator>();
            modelBuilder.Entity<DecisionRule>()
                .Property(dr => dr.LogicOperator)
                .HasConversion(logicOperatorConverter);


            modelBuilder.Entity<ModelResult>()
                .Property(mr => mr.CreatedAt)
                .HasColumnType("timestamp with time zone");

            modelBuilder.Entity<ModelConfiguration>()
                .Property(mc => mc.Hyperparameters)
                .HasColumnType("jsonb");

            modelBuilder.Entity<User>().Property(u => u.Id).HasColumnName("UserId");
            modelBuilder.Entity<Dataset>().Property(d => d.Id).HasColumnName("DatasetId");
            modelBuilder.Entity<Experiment>().Property(e => e.Id).HasColumnName("ExperimentId");
            modelBuilder.Entity<ModelConfiguration>().Property(mc => mc.Id).HasColumnName("ConfigId");
            modelBuilder.Entity<ModelResult>().Property(mr => mr.Id).HasColumnName("ResultId");
            modelBuilder.Entity<Plot>().Property(p => p.Id).HasColumnName("PlotId");
            modelBuilder.Entity<DecisionRule>().Property(dr => dr.Id).HasColumnName("RuleId");

            modelBuilder.Entity<Dataset>()
                .HasOne(d => d.User)
                .WithMany(u => u.Datasets)
                .HasForeignKey(d => d.UserId);

            modelBuilder.Entity<Experiment>()
                .HasOne(e => e.User)
                .WithMany(u => u.Experiments)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<Experiment>()
                .HasOne(e => e.Dataset)
                .WithMany(d => d.Experiments)
                .HasForeignKey(e => e.DatasetID);

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