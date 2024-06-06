using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Northrend.Alodi.EventExtensions;
using Northrend.ViewModels;
using Northrend.Views;
using System;
using System.Reflection;
using System.Runtime;

namespace Northrend
{
    public partial class App : Application
    {
        private IHost mCurrentHost;
        public IServiceProvider Services => mCurrentHost.Services;
        MainWindowViewModel mMainWindowViewModel;


        public App()
        {
            InitializeApplicationServices();
        }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            mMainWindowViewModel = Services.GetRequiredService<MainWindowViewModel>();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var mainView = Services.GetRequiredService<MainWindow>();
                mainView.DataContext = mMainWindowViewModel;
                desktop.MainWindow = mainView;                
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void InitializeApplicationServices()
        {
            try
            {   
                mCurrentHost = CreateHostBuilder()
                               .Build();

                var eventAggregator = mCurrentHost.Services.GetRequiredService<IEventAggregator>();

            }
            catch (Exception ex)
            {
               
            }
        }

        /// <summary>
        /// Создаёт строителя, обеспечивающего регистрацию, построение 
        /// и слежение за временем жизни всех компонент приложения.
        /// </summary>
        /// <param name="settingsFileName"></param>
        /// <param name="appSettings"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        internal static IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    _ = services
                    .AddSingleton<IEventAggregator, EventAggregator>()
                    .AddSingleton<MainWindowViewModel>()
                    .AddSingleton<MainWindow>();
                });               
        }
    }
}