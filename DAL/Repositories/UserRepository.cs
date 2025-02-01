using DAL.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
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
        public void Delete(UserEntity value)
        {
            _context.Users.Remove(value);
            _context.SaveChanges();
        }

        public IEnumerable<UserEntity> GetAll()
        {
            return _context.Users.Include(u => u.Boards).ToList();
        }

        public void Update(params UserEntity[] table)
        {
            foreach (var X in table)
            {
                if (IsUserValid(X))
                {
                    if (_context.Users.Where(Y => Y.Username == X.Username).Count() == 1)
                    {
                        UserEntity R = _context.Users.Where(Y => Y.Username == X.Username).First();
                        R.Password = X.Password;
                        BoardUpdate(R, X.Boards.ToArray());
                    }
                    else
                        _context.Users.Add(X);
                }
                else throw new Exception("The row was not valid, make sure all columns are correct!");
            }
            _context.SaveChanges();
        }

        private void BoardUpdate(UserEntity user, params BoardEntity[] table)
        {
            foreach (var X in table)
            {
                if (user.Boards.Where(Y => Y.Name == X.Name).Count() == 1)
                {
                    BoardEntity R = user.Boards.Where(Y => Y.Name == X.Name).First();
                    R.Content = X.Content;
                    R.Users = X.Users;
                }
                else
                {
                    if(X.ID == 0)
                    {
                        _context.Boards.Add(X);
                        X.Users.Add(user);
                        _context.SaveChanges();
                    }
                    else
                    {
                        user.Boards.Add(_context.Boards.Where(Y => Y.ID == X.ID).First());
                        _context.SaveChanges();
                    }

                    //user.Boards.Add(_context.Boards.Where(Y=>Y.ID == X.ID).First());
                    //_context.SaveChanges();
                }
            }
            _context.SaveChanges();
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
