using System;
using System.IO;
using FriendRater.Api;
using FriendRater.Data;
using FriendRater.Views;
using Xamarin.Forms;

namespace FriendRater
{
    public partial class App : Application
    {
#if DEBUG
        public static ApiEnvironment API = ApiEnvironment.v1Staging;
#else
        public static ApiEnvironment API = ApiEnvironment.v1Production;
#endif

        private static Database _Database;

        public static Database Database =>
            _Database ??= _Database = new Database(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FriendRater.db3"));

        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new LoginView());
        }
    }
}
