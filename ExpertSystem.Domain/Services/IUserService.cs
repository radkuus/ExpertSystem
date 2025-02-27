using ExpertSystem.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertSystem.Domain.Services
{
    public interface IUserService : IDataService<User>
    {
        Task<User> GetByNickname(string nickname);
        Task<User> GetByEmail(string email);
    }
}
