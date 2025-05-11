using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertSystem.Domain.Services
{
    public interface IApiService
    {
        Task<T> PostAsync<T>(string endpoint, object data);
    }
}
