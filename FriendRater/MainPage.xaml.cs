using System;
using FriendRater.Api;
using FriendRater.Api.Models;
using FriendRater.Data;
using Xamarin.Forms;

namespace FriendRater
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnAppearing(object sender, EventArgs e)
        {
            Credentials lCredentials = await App.Database.GetOptionAsync<Credentials>(nameof(Credentials));
            if (lCredentials != null)
            {
                SetLoginEnabled(false);
                try
                {
                    using ApiClient lClient = new ApiClient(ApiEnvironment.v1Staging);
                    await lClient.Login(lCredentials.Username, lCredentials.Password);
                    await DisplayAlert("Friend Rater", $"Hi there, {lCredentials.Name}", "Hello :)");
                }
                catch (Exception)
                {
                    uiUsername.Text = lCredentials.Username;
                }
                finally
                {
                    SetLoginEnabled(true);
                }
            }
        }

        private async void OnLogin(object sender, EventArgs e)
        {
            try
            {
                SetLoginEnabled(false);
                uiLogin.Text = "Loading...";
                using ApiClient lClient = new ApiClient(ApiEnvironment.v1Staging);
                ApiWrapper<LoginResponse> lResponse = await lClient.Login(uiUsername.Text, uiPassword.Text);
                await DisplayAlert("Friend Rater", $"Hi there, {lResponse.Data.Name}", "Hello :)");
                if (uiRememberMe.IsChecked)
                    await App.Database.SetOptionAsync(nameof(Credentials), new Credentials
                    {
                        Username = uiUsername.Text,
                        Password = uiPassword.Text,
                        Name = lResponse.Data.Name,
                    });
                else
                    await App.Database.DeleteOptionAsync(nameof(Credentials));
            }
            catch (Exception lException)
            {
                await DisplayAlert("Friend Rater", lException.Message, "Ok");
            }
            finally
            {
                uiLogin.Text = "Login";
                SetLoginEnabled(true);
            }
        }

        private void SetLoginEnabled(bool pFlag)
        {
            uiUsername.IsEnabled =
                uiPassword.IsEnabled =
                uiLogin.IsEnabled = pFlag;
        }
    }
}
