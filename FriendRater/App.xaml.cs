using System;
using System.IO;
using FriendRater.Data;
using Xamarin.Forms;

namespace FriendRater
{
    public partial class App : Application
    {
        private static Database _Database;

        public static Database Database =>
            _Database ??= _Database = new Database(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FriendRater.db3"));

        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
            
        }

        protected override void OnResume()
        {
        }
    }
}
