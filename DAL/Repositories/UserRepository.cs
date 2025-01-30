using DAL.Entities;
using DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class UserRepository: IRepository<UserEntity>
    {
        PlanBoardContext _context;

        public UserRepository(PlanBoardContext context)
        {
            _context = context;
        }

        #region Public
        public async Task Delete(UserEntity value)
        {
            _context.Users.Remove(value);
            _context.SaveChanges();
        }

        public async Task<IEnumerable<UserEntity>> GetAll()
        {
            return await Task.Run(() => _context.Users.ToList());
        }

        public async Task Update(params UserEntity[] table)
        {
            foreach (var X in table)
            {
                if (IsUserValid(X))
                {
                    if (_context.Users.Where(Y => Y.Username == X.Username).Count() == 1)
                    {
                        UserEntity R = _context.Users.Where(Y => Y.Username == X.Username).First();
                        R.Password = X.Password;
                    }
                    else
                        _context.Users.Add(X);
                }
                else throw new Exception("The row was not valid, make sure all columns are correct!");
            }
            await _context.SaveChangesAsync();
        }

        public bool IsUserValid(UserEntity Row)
        {
            if (Row != null &&
                !String.IsNullOrEmpty(Row.Username) &&
                !String.IsNullOrEmpty(Row.Password))
            {
                return true;
            }
            return false;
        }
        #endregion
    }
}
