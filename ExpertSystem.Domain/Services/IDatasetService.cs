using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpertSystem.Domain.Models;

namespace ExpertSystem.Domain.Services
{
    public interface IDatasetService
    {
        Task AddDataset(Dataset dataset);
        Task RemoveDataset(int id);
        Task<List<Dataset>> GetAll();
        Task<Dataset> GetDatasetById(int id);
    }
}
