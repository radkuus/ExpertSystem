using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertSystem.Domain.Services
{
    public interface IDataService<T>
    {
        Task<T> Create(T entity);

        Task<IEnumerable<T>> GetAll();

        Task<T> Get(int id);

        Task<T> Update(int id, T entity);

        Task<bool> Delete(int id);

        Task<IEnumerable<T>> GetAllByUserId(int userId);

        Task<bool> Any();
    }
}
