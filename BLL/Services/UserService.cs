using AutoMapper;
using DAL.Entities;
using DAL.Interfaces;
using BLL.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.Models;
using BLL.Interfaces;

namespace BLL.Services
{
    public class UserService : IService<UserModel>
    {
        private IRepository<UserEntity> _repository;
        private IMapper _mapper;
        protected List<UserModel> _users;

        public UserService(IRepository<UserEntity> repository)
        {
            _repository = repository;

            var configuration = new MapperConfiguration(x =>
            {
                x.AddProfile<ProjectMapping>();
            });
            _mapper = configuration.CreateMapper();
        }

        #region Public
        public void Delete(int ID)
        {
            _repository.Delete(_repository.GetAll().Where(X => ID == X.ID).First());
        }

        public IEnumerable<UserModel> GetAll(params Func<UserModel, bool>[] Filters)
        {
            return _users = _repository.GetAll().Select(X => _mapper.Map<UserEntity,UserModel>(X)).Where(X => {
                bool Accept = true;
                Filters.ToList().ForEach(M => Accept = M(X));
                return Accept;
            }).ToList();
        }

        public void Update(params UserModel[] table)
        {
            if (table.Length == 0 && _users != null)
                _repository.Update(_users.Select(X => _mapper.Map<UserModel,UserEntity>(X)).ToArray());
            else if (table.Length != 0)
                _repository.Update(table.Select(X => _mapper.Map<UserModel,UserEntity>(X)).ToArray());
        }
        #endregion

        #region Private
        #endregion
    }
}
