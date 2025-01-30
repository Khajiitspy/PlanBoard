using BLL.Configurations;
using BLL.Interfaces;
using BLL.Models;
using BLL.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;

namespace PlanBoard
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IServiceProvider _serviceProvider;
        public App()
        {
            ServiceCollection services = new ServiceCollection();
            ConfigureServices(services);
        }

        private void ConfigureServices(ServiceCollection services)
        {
            services.AddTransient(typeof(IService<UserModel>), typeof(UserService));
            services.AddTransient(typeof(MainWindow));

            ConfigurationBLL.ConfigureServices(services);
        }

        private void OnStartUp(object sender, StartupEventArgs e)
        {
            var mainWind = _serviceProvider.GetService<MainWindow>();
            if (mainWind.IsLoaded)
            {
                mainWind.Show();
            }
        }
    }
}
