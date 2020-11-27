using System;
using System.Linq;
using System.Threading.Tasks;
using FriendRater.Api;
using FriendRater.Api.Models;
using FriendRater.Services;
using Xamarin.Forms;

namespace FriendRater.Views
{
    public partial class RegisterView : ContentPage
    {
        private readonly IApiService _ApiService;

        public RegisterView(IApiService pApiService)
        {
            _ApiService = pApiService;
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
                if (uiEntryPassword.Text == null || uiEntryPassword.Text.Length < 8)
                    throw new Exception("The password must be at least 8 characters in length.");
                if (uiEntryPasswordAgain.Text == null || uiEntryPassword.Text != uiEntryPasswordAgain.Text)
                    throw new Exception("The passwords are not equal.");
                if (uiEntryUsername.Text == null || uiEntryUsername.Text.Length < 3 || uiEntryUsername.Text.Length > 16)
                    throw new Exception("The username must be between 3 and 16 characters in length.");
                (bool lSuccess, Exception lError) = await _ApiService.RegisterAsync(new RegisterRequest
                {
                    Email = uiEntryEmail.Text,
                    Firstname = uiEntryFirstname.Text,
                    Lastname = uiEntryLastname.Text,
                    Password = uiEntryPassword.Text,
                    PhoneNumber = uiEntryPhoneNumber.Text,
                    Username = uiEntryUsername.Text,
                });
                if (!lSuccess)
                    throw lError;    
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
                await ShowRegister();
            }
        }

        private async Task ShowRegister(bool pFlag = true)
        {
            if (pFlag) await uiFrameRegister.TranslateTo(0, 0, 500, Easing.SinOut);
            else await uiFrameRegister.TranslateTo(0, Height, 500, Easing.SinOut);
        }
    }
}
