using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IService<T> where T : class
    {
        Task Delete(int ID);
        Task Update(params T[] table);
        Task<IEnumerable<T>> GetAll(params Func<T, bool>[] Filter);
    }
}
