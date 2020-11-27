using System;
using System.IO;
using FriendRater.Api;
using FriendRater.Data;
using FriendRater.Services;
using FriendRater.Views;
using Microsoft.Extensions.DependencyInjection;
using NanoIoC;
using Xamarin.Forms;

namespace FriendRater
{
    public partial class App : Application
    {
        // if we are running in debug, we are using the staging environment
        // otherwise the app is being deployed and we are running live
#if DEBUG
        public static ApiEnvironment API = ApiEnvironment.v1Staging;
#else
        public static ApiEnvironment API = ApiEnvironment.v1Production;
#endif

        // container for di
        private static IContainer _Container;
        public static IContainer Container =>
            _Container ??= _Container = new Container();

        public App()
        {
            InitializeComponent();
            
            // register services
            Container.Register(typeof(IDatabase), _ => new Database(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FriendRater.db3")), ServiceLifetime.Singleton);
            Container.Register<IApiService, ApiService>();
            Container.Register<IOptionService, OptionService>();
            Container.Register<IPhonebookService, PhonebookService>();
            
            // register views
            Container.Register<LoginView>(ServiceLifetime.Transient);
            Container.Register<RegisterView>(ServiceLifetime.Transient);
            Container.Register<MainView>(ServiceLifetime.Transient);
            Container.Register<UserView>(ServiceLifetime.Transient);
            
            // open the LoginView everytime,
            // it handles the remember function aswell
            LoginView lMainPage = Container.Resolve<LoginView>();
            MainPage = new NavigationPage(lMainPage);
        }
    }
}
