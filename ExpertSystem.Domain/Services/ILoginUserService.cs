using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpertSystem.Domain.Models;

namespace ExpertSystem.Domain.Services
{
    public interface ILoginUserService
    {
        Task AddDatabase(Database database);
    }
}
