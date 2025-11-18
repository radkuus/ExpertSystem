using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using ExpertSystem.Domain.Models;
using ExpertSystem.Domain.Services;
using ExpertSystem.EntityFramework;
using ExpertSystem.WPF.State.Authenticators;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace ExpertSystem.EntityFramework.Services
{

    public class DatasetService : IDatasetService
    {
        private readonly IDbContextFactory<ExpertSystemDbContext> _contextFactory;
        private readonly IAuthenticator _authenticator;

        public DatasetService(IDbContextFactory<ExpertSystemDbContext> contextFactory, IAuthenticator authenticator)
        {
            _contextFactory = contextFactory;
            _authenticator = authenticator;
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
                return await context.Datasets
                    .Include(d => d.User)
                    .ToListAsync();
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
            var dataset = await GetDatasetById(datasetId);
            if (dataset != null)
            {
                var baseDir = AppDomain.CurrentDomain.BaseDirectory;
                var projectDir = Directory.GetParent(baseDir).Parent.Parent.Parent.Parent.FullName;
                var currentUserName = _authenticator.CurrentUser.Nickname;
                var datasetName = dataset.Name;
                var datasetPath = Path.Combine(projectDir, "Datasets", currentUserName, datasetName);
                if (File.Exists(datasetPath)) 
                {
                    return await Task.Run(() =>
                        {
                            var dataTable = new DataTable();
                            using (var reader = new StreamReader(datasetPath))
                            using (var csv = new CsvHelper.CsvReader(reader, CultureInfo.InvariantCulture))
                            using (var dr = new CsvHelper.CsvDataReader(csv))
                            {
                                dataTable.Load(dr);
                            }

                            return dataTable;
                        } );
                        
                }
                else
                {
                    throw new FileNotFoundException("CSV file not found.", datasetPath);
                }
            }
            else
            {
                throw new FileNotFoundException("Dataset not found.");
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

            return await Task.Run(() =>
            {
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
            });
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
    