using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IService<T> where T : class
    {
        void Delete(int ID);
        void Update(params T[] table); // Both add and update
        IEnumerable<T> GetAll(params Func<T, bool>[] Filter); // Get all with an optional built in filter
    }
}
