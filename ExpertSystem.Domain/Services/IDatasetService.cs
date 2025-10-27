using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpertSystem.Domain.Models;

namespace ExpertSystem.Domain.Services
{
    public interface IDatasetService
    {
        Task AddDataset(Dataset dataset);
        Task RemoveDataset(int datasetId);
        Task<List<Dataset>>GetUserDatasets(int userId);
        Task<List<Dataset>> GetAll();
        Task<Dataset> GetDatasetById(int datasetId);
        Task<DataTable> GetDatasetAsDataTable(int datasetId);
        Task<ObservableCollection<string>> GetDatasetColumnNames(int datasetId);
        Task<ObservableCollection<string>> GetDatasetNumericColumnNames(int datasetId);
        Task<ObservableCollection<string>> GetUniqueNamesFromClassifyingColumn(int datasetId, string columnName);
    }
}
