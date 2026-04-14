using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Windows;
using TaskForge.Models.Entities;
using TaskForge.Models.Repositories;
using TaskForge.Views;
using TaskForge.Views.AuthPages;
using TaskForge.Views.Dialogs;
using TaskForge.Views.Pages;

namespace TaskForge
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider serviceProvider { get; private set;  }

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
            services.AddTransient<TaskPage>();
            services.AddTransient<ProjectPage>();
            services.AddTransient<AchievementsPage>();
            services.AddTransient<TaskEditWindow>();

            services.AddSingleton<MainWindow>();
            services.AddSingleton<AuthWindow>();


            serviceProvider = services.BuildServiceProvider();

            var authWindow = serviceProvider.GetRequiredService<AuthWindow>();
            authWindow.Show();


       }
    }

}
