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
        Task AddDatabase(Database database);
        Task RemoveDatabase(int id);
        Task<List<Database>> GetAll();
        Task<Database> GetDatabaseById(int id);
    }
}
