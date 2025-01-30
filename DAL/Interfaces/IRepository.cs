using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task Update(params T[] table);
        Task Delete(T value);
        Task<IEnumerable<T>> GetAll();
    }
}
