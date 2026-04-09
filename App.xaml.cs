using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Windows;
using TaskForge.Models.Entities;
using TaskForge.Models.Repositories;
using TaskForge.Views;
using TaskForge.Views.AuthPages;

namespace TaskForge
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider serviceProvider { get; private set;  }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var services = new ServiceCollection();

            services.AddDbContext<ApplicationDBContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")),
        contextLifetime: ServiceLifetime.Scoped);

            services.AddSingleton<IUserSession, UserSession>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddTransient<SignUpPage>();
            services.AddTransient<LogInPage>();
            services.AddSingleton<MainWindow>();
            services.AddSingleton<AuthWindow>();


            serviceProvider = services.BuildServiceProvider();

            var authWindow = serviceProvider.GetRequiredService<AuthWindow>();
            authWindow.Show();


       }
    }

}
