using System;
using System.Threading.Tasks;
using FriendRater.Api;
using FriendRater.Api.Models;
using FriendRater.Data;
using Xamarin.Forms;

namespace FriendRater.Views
{
    public partial class LoginView : ContentPage
    {
        private bool _LoginChecked = false;

        public LoginView()
        {
            InitializeComponent();

            Appearing += OnAppearing;
        }

        private async void OnAppearing(object sender, EventArgs e)
        {
            if (_LoginChecked)
                return;
            _LoginChecked = true;

            await ShowLogin(false);
            uiFrameLogin.IsVisible = true;
            Credentials lCredentials = await App.Database.GetOptionAsync<Credentials>(nameof(Credentials));
            if (lCredentials != null)
            {
                try
                {
                    using ApiClient lClient = new ApiClient(App.API);
                    await lClient.Login(lCredentials.Username, lCredentials.Password);
                    await OpenMainView(lCredentials);
                }
                catch (Exception)
                {
                    uiEntryUsername.Text = lCredentials.Username;
                    await ShowLogin();
                }
            }
            else
            {
                await ShowLogin();
            }
        }

        private async Task ShowLogin(bool flag = true)
        {
            if (flag) await uiFrameLogin.TranslateTo(0, 0, 500, Easing.SinOut);
            else await uiFrameLogin.TranslateTo(0, Height, 500, Easing.SinOut);
        }

        private async Task OpenMainView(Credentials pCredentials)
        {
            await Navigation.PushAsync(new MainView(pCredentials));
            Navigation.RemovePage(this);
        }

        private async void OnRegister(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RegisterView());
        }

        public async void OnLogin(object sender, EventArgs e)
        {
            await ShowLogin(false);

            string lUaername = uiEntryUsername.Text;
            string lPassword = uiEntryPassword.Text;
            try
            {
                using ApiClient lClient = new ApiClient(App.API);
                LoginResponse lResponse = await lClient.Login(lUaername, lPassword);
                Credentials lCredentials = new Credentials
                {
                    Username = lUaername,
                    Password = lPassword,
                    Name = lResponse.Name,
                };
                if (uiCheckboxRememberMe.IsChecked)
                    await App.Database.SetOptionAsync<Credentials>(nameof(Credentials), lCredentials);
                else
                    await App.Database.DeleteOptionAsync(nameof(Credentials));
                await OpenMainView(lCredentials);
            }
            catch (Exception)
            {
                await this.ShowAlert("Login was not successful. Please check your credentials.", "Ok");
                await ShowLogin(true);
            }
        }
    }
}
