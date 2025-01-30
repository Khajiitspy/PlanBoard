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

namespace BLL.Services
{
    public class UserService
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
        public async Task Delete(int ID)
        {
            await _repository.Delete(_repository.GetAll().Result.Where(X => ID == X.ID).First());
        }

        public async Task<IEnumerable<UserModel>> GetAll(params Func<UserModel, bool>[] Filters)
        {
            return _users = _repository.GetAll().Result.Select(X => _mapper.Map<UserEntity,UserModel>(X)).Where(X => {
                bool Accept = true;
                Filters.ToList().ForEach(M => Accept = M(X));
                return Accept;
            }).ToList();
        }

        public async Task Update(params UserModel[] table)
        {
            if (table.Length == 0 && _users != null)
                await _repository.Update(_users.Select(X => _mapper.Map<UserModel,UserEntity>(X)).ToArray());
            else if (table.Length != 0)
                await _repository.Update(table.Select(X => _mapper.Map<UserModel,UserEntity>(X)).ToArray());
        }
        #endregion

        #region Private
        #endregion
    }
}
