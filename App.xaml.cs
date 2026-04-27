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
        public static IServiceProvider serviceProvider { get; private set; }
        public static Window MainAppWindow { get; private set; }
        protected override void OnStartup(StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += (s, args) =>
            {
                var ex = args.ExceptionObject as Exception;
                MessageBox.Show($"Unhandled: {ex?.Message}\n{ex?.StackTrace}", "Fatal Error");
            };
            DispatcherUnhandledException += (s, args) =>
            {
                MessageBox.Show($"Dispatcher: {args.Exception.Message}\n{args.Exception.StackTrace}", "UI Error");
                args.Handled = true;
            };

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
            services.AddSingleton<ISessionStorage, SessionStorage>();

            services.AddTransient<SignUpPage>();
            services.AddTransient<LogInPage>();
            services.AddTransient<TaskPage>();
            services.AddTransient<ProjectPage>();
            services.AddTransient<UserPage>();
            services.AddTransient<AchievementsPage>();
            services.AddTransient<TaskEditWindow>();
            services.AddTransient<CompletedTasksPage>();

            services.AddTransient<AuthWindow>();
            services.AddSingleton<Func<AuthWindow>>(sp => () => sp.GetRequiredService<AuthWindow>());
            services.AddSingleton<MainWindow>();


            serviceProvider = services.BuildServiceProvider();

            var sessionStorage = serviceProvider.GetRequiredService<ISessionStorage>();
            var savedUserId = sessionStorage.LoadUserId();
            if (savedUserId.HasValue)
            {
                var userSession = serviceProvider.GetRequiredService<IUserSession>();
                userSession.CurrentUserId = savedUserId.Value;

                using (var scope = serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
                    var user = context.Users.Find(savedUserId.Value);
                    if (user != null)
                    {
                        userSession.CurrentUserName = user.Name;
                        userSession.CurrentUserEmail = user.Email;
                        userSession.CurrentUserLevel = user.Level;
                        userSession.CurrentUserTotalEx = user.Total_exp;
                    }
                    else
                    {
                        sessionStorage.Clear();
                        ShowAuthWindow();
                        return;
                    }

                    var mainWindow = serviceProvider.GetRequiredService<MainWindow>();
                    MainAppWindow = mainWindow;
                    mainWindow.Show();
                }

            }
            else
            {
                ShowAuthWindow();
            }
        }

        private void ShowAuthWindow()
        {
            var authWindow = serviceProvider.GetRequiredService<AuthWindow>();
            authWindow.Show();
        }
        public static void ShutdownApplication() { Current.Shutdown(); }

    }
}

