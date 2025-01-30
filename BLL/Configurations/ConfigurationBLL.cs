using DAL;
using DAL.Entities;
using DAL.Interfaces;
using DAL.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Configurations
{
    public class ConfigurationBLL
    {
        public static void ConfigureServices(ServiceCollection service)
        {
            service.AddTransient(typeof(IRepository<UserEntity>), typeof(UserRepository));

            service.AddDbContext<PlanBoardContext>();
        }
    }
}
