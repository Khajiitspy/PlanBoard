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

        public void Update(params UserEntity[] table) // Update users
        {
            foreach (var X in table)
            {
                if (IsUserValid(X))
                {
                    if (_context.Users.Where(Y => Y.Username == X.Username).Count() == 1) // If the user exists / UPDATE
                    {
                        // Gets the corresponding user from the context, which avois an error
                        UserEntity R = _context.Users.Where(Y => Y.Username == X.Username).First();
                        R.Password = X.Password;
                        BoardUpdate(R, X.Boards.ToArray());
                    }
                    else // If the the user does not exist / ADD
                        _context.Users.Add(X);
                }
                else throw new Exception("The row was not valid, make sure all columns are correct!");
            }
            _context.SaveChanges();
        }

        private void BoardUpdate(UserEntity user, params BoardEntity[] table)
        {
            user.Boards.Clear();

            foreach (var X in table)
            {
                // Since this method is private and will only get a users whole board list I decided to clear the _context user's boards (first line of this method) and then add the params Boards to be able to delete boards without having to create a seperate service and repository.

                if (X.ID == -1)
                {
                    _context.Boards.Add(X);
                    X.Users.Add(user);
                }
                else // Board already exists but so you just grant the user access. and updates the contents of the board
                {
                    BoardEntity R = _context.Boards.Where(Y => Y.ID == X.ID).First();
                    R.Content = X.Content;
                    user.Boards.Add(R);
                }
            }

            _context.SaveChanges();

            List<BoardEntity> BL = _context.Boards.Include(u => u.Users).ToList();
            foreach (var Board in BL) { // Search for boards that have been deleted by all users that owned it to delete them.
                if (Board.Users.Count() == 0) {
                    _context.Boards.Remove(Board);
                }
            }

            _context.SaveChanges();
        }

        public bool IsUserValid(UserEntity Row) // Checks whether the user is able to be added to the database.
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
