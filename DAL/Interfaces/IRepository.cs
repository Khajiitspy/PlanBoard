using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IRepository<T> where T : class
    {
        void Update(params T[] table);
        void Delete(T value);
        IEnumerable<T> GetAll();
    }
}
