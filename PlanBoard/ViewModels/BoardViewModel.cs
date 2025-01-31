using BLL.Interfaces;
using BLL.Models;
using DAL.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanBoard.ViewModels
{
    public class BoardViewModel : INotifyPropertyChanged
    {
        public IService<UserModel> UserService;
        public event PropertyChangedEventHandler? PropertyChanged;

        public List<UserModel> UserTable { get; set; }

        public BoardViewModel(IService<UserModel> UserService)
        {
            this.UserService = UserService;
            UserTable = UserService.GetAll().ToList();
        }
    }
}
