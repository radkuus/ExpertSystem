﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using ExpertSystem.Domain.Models;
using ExpertSystem.Domain.Services;
using ExpertSystem.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace ExpertSystem.EntityFramework.Services
{

    public class DatasetService : IDatasetService
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
                try
                {
                    var existingDataset = await context.Datasets
                        .FirstOrDefaultAsync(d => d.UserId == dataset.UserId && d.Name == dataset.Name);

                    if (existingDataset != null)
                    {
                        throw new InvalidOperationException($"The user with ID {dataset.UserId} already has a database named '{dataset.Name}'.");
                    }
                    context.Datasets.Add(dataset);
                    int affectedRows = await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw; 
                }
            }
        }

        public async Task<List<Dataset>> GetUserDatasets(int userId)
        {
            using(ExpertSystemDbContext context = _contextFactory.CreateDbContext())
            {
                var datasets = await context.Datasets.Where(x => x.UserId == userId).ToListAsync();
                return datasets;
            }
        }

        public async Task<Dataset> GetDatasetById(int datasetId)
        {
            using(ExpertSystemDbContext context = _contextFactory.CreateDbContext())
            {
                var dataset = await context.Datasets.SingleOrDefaultAsync(x => x.Id == datasetId);
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

        public async Task RemoveDataset(int datasetId)
        {
            using (ExpertSystemDbContext context = _contextFactory.CreateDbContext())
            {
                var dataset = await GetDatasetById(datasetId);
                if (dataset != null)
                {
                    context.Datasets.Remove(dataset);
                    await context.SaveChangesAsync();
                }
            }
        }

        public async Task<DataTable> GetDatasetAsDataTable(int datasetId)
        {
            using (ExpertSystemDbContext context = _contextFactory.CreateDbContext())
            {
                var dataset = await GetDatasetById(datasetId);
                if (dataset != null)
                {
                    var baseDir = AppDomain.CurrentDomain.BaseDirectory;
                    var projectDir = Directory.GetParent(baseDir).Parent.Parent.Parent.Parent.FullName;
                    var fileName = dataset.Name;
                    var filePath = Path.Combine(projectDir, "Datasets", fileName);
                    if (File.Exists(filePath)) 
                    {
                        var dataTable = new DataTable();
                        using (var reader = new StreamReader(filePath))
                        using (var csv = new CsvHelper.CsvReader(reader, CultureInfo.InvariantCulture))
                        using (var dr = new CsvHelper.CsvDataReader(csv))
                        {
                            dataTable.Load(dr);
                        }

                        return dataTable;
                    }
                    else
                    {
                        throw new FileNotFoundException("CSV file not found.", filePath);
                    }
                }
                else
                {
                    throw new FileNotFoundException("Dataset not found.");
                }
            }
        }

        public async Task<ObservableCollection<string>> GetDatasetColumnNames(int datasetId)
        {
            var dataTable = await GetDatasetAsDataTable(datasetId);
            return new ObservableCollection<string>(dataTable.Columns.Cast<DataColumn>().Select(col => col.ColumnName));
        }

        public async Task<ObservableCollection<string>> GetDatasetNumericColumnNames(int datasetId)
        {
            var dataTable = await GetDatasetAsDataTable(datasetId);

            bool IsNumericColumn(DataColumn column)
            {
                foreach (DataRow row in column.Table.Rows)
                {
                    if (row.IsNull(column)) continue;

                    var valueStr = row[column].ToString();
                    if (!double.TryParse(valueStr, NumberStyles.Any, CultureInfo.InvariantCulture, out _))
                    {
                        return false;
                    }
                }
                return true;
            }

            var numericColumns = dataTable.Columns.Cast<DataColumn>()
                .Where(IsNumericColumn)
                .Select(col => col.ColumnName);

            return new ObservableCollection<string>(numericColumns);
        }

        public async Task<ObservableCollection<string>> GetDatasetTextColumnNames(int datasetId)
        {
            var dataTable = await GetDatasetAsDataTable(datasetId);

            bool IsTextColumn(DataColumn column)
            {
                foreach (DataRow row in column.Table.Rows)
                {
                    if (row.IsNull(column)) continue;

                    var valueStr = row[column].ToString();
                    if (double.TryParse(valueStr, NumberStyles.Any, CultureInfo.InvariantCulture, out _))
                    {
                        return false;
                    }
                }
                return true;
            }

            var textColumns = dataTable.Columns.Cast<DataColumn>()
                .Where(IsTextColumn)
                .Select(col => col.ColumnName);

            return new ObservableCollection<string>(textColumns);
        }

        public async Task<ObservableCollection<string>> GetUniqueNamesFromClassifyingColumn(int datasetId, string columnName)
        {
            var dataTable = await GetDatasetAsDataTable(datasetId);

            var uniqueNames = dataTable
                .AsEnumerable()
                .Select(row => row[columnName]?.ToString())
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Distinct()
                .ToList();

            return new ObservableCollection<string>(uniqueNames);
        }
    }
}
    