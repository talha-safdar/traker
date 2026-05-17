using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using Traker.Services;
using Traker.States;
using Traker.ViewModels;
using Traker.ViewModels.User;
using Traker.ViewModels.Edit;
using Traker.ViewModels.Add;

namespace Traker
{
    public class Bootstrapper : BootstrapperBase
    {
        private readonly SimpleContainer _container = new SimpleContainer();

        public Bootstrapper()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            //var splash = new SplashView();
            //splash.Show();
            var windowManager = IoC.Get<IWindowManager>();
            var splash = IoC.Get<SplashScreenViewModel>();
            windowManager.ShowWindowAsync(splash);
            //Task.Delay(2000); // simulate (replace with real init)
            DisplayRootViewForAsync<ShellViewModel>();
        }

        protected override void Configure()
        {
            _container.Instance(_container);

            _container.Singleton<IWindowManager, WindowManager>();
            _container.Singleton<IEventAggregator, EventAggregator>();
            _container.Singleton<ShellViewModel>();
            _container.Singleton<SetupViewModel>();
            _container.Singleton<AppState>();
            _container.Singleton<DataService>();
            _container.Singleton<SortJobsViewModel>();
            _container.Singleton<FilterJobsViewModel>();

            /// handling how ViewModels will connect to the Views
            // this is run Once at the beginning of the application
            GetType().Assembly.GetTypes()
                .Where(type => type.IsClass)
                .Where(type => type.Name.EndsWith("ViewModel"))
                .ToList()
                .ForEach(viewModelType => _container.RegisterPerRequest(
                    viewModelType, viewModelType.ToString(), viewModelType));

            ViewLocator.AddNamespaceMapping("Traker.ViewModels.*", "Traker.Views.*");
            // LogManager.GetLog = type => new DebugLog(type); // uncomment this line for Caliburn debug messages
        }

        protected override object GetInstance(Type service, string key)
        {
            return _container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }
    }
}
