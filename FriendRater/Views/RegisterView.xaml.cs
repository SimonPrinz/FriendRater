using System;
using System.Linq;
using System.Threading.Tasks;
using FriendRater.Api;
using FriendRater.Api.Models;
using Xamarin.Forms;

namespace FriendRater.Views
{
    public partial class RegisterView : ContentPage
    {
        public RegisterView()
        {
            InitializeComponent();
        }

        private async void OnBack(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void OnRegister(object sender, EventArgs e)
        {
            await ShowRegister(false);
            try
            {
                if (uiEntryPassword.Text.Length < 8)
                    throw new Exception("The password must be at least 8 characters in length.");
                if (uiEntryPassword.Text != uiEntryPasswordAgain.Text)
                    throw new Exception("The passwords are not equal.");
                if (uiEntryUsername.Text.Length < 3 || uiEntryUsername.Text.Length > 16)
                    throw new Exception("The username must be between 3 and 16 characters in length.");
                using ApiClient lClient = new ApiClient(App.API);
                bool lResponse = await lClient.Register(new RegisterRequest
                {
                    Email = uiEntryEmail.Text,
                    Firstname = uiEntryFirstname.Text,
                    Lastname = uiEntryLastname.Text,
                    Password = uiEntryPassword.Text,
                    PhoneNumber = uiEntryPhoneNumber.Text,
                    Username = uiEntryUsername.Text,
                });
                if (!lResponse)
                    throw new Exception("The account could not be created. Please try again later.");
                await this.ShowAlert("The account was created. Please check your emails to activate it.", "Ok");
                await Navigation.PopAsync();
            }
            catch (Exception lException)
            {
                if (lException is ApiException lApiException)
                    if (lApiException.Errors.Any())
                        await this.ShowAlert(string.Join(". ", lApiException.Errors.Select(pError => pError.Message)), "Ok");
                    else
                        await this.ShowAlert("The account could not be created. Please check your credentials.", "Ok");
                else
                    await this.ShowAlert(lException.Message, "Ok");
                await ShowRegister(true);
            }
        }

        private async Task ShowRegister(bool flag = true)
        {
            if (flag) await uiFrameRegister.TranslateTo(0, 0, 500, Easing.SinOut);
            else await uiFrameRegister.TranslateTo(0, Height, 500, Easing.SinOut);
        }
    }
}
