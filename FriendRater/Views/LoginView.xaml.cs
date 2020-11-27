using System;
using System.Threading.Tasks;
using FriendRater.Services;
using Xamarin.Forms;

namespace FriendRater.Views
{
    public partial class LoginView : ContentPage
    {
        private readonly IApiService _ApiService;

        private bool _LoginChecked;
        
        public bool IsLoading { get; internal set; }

        public LoginView(IApiService pApiService)
        {
            _ApiService = pApiService;

            BindingContext = this;
            
            InitializeComponent();
        }

        private void SetIsLoading(bool pFlag)
        {
            IsLoading = pFlag;
            OnPropertyChanged(nameof(IsLoading));
        }

        private async void OnAppearing(object sender, EventArgs e)
        {
            if (_LoginChecked)
                return;
            _LoginChecked = true;

            await ShowLogin(false);
            uiFrameLogin.IsVisible = true;
            if (await _ApiService.IsLoggedInAsync())
                await OpenMainView();
            else
                await ShowLogin();
        }

        private async Task ShowLogin(bool pFlag = true)
        {
            SetIsLoading(!pFlag);
            if (pFlag) await uiFrameLogin.TranslateTo(0, 0, 500, Easing.SinOut);
            else await uiFrameLogin.TranslateTo(0, Height, 500, Easing.SinOut);
        }

        private async Task OpenMainView()
        {
            await Navigation.PushAsync(App.Container.Resolve<MainView>());
            Navigation.RemovePage(this);
        }

        private async void OnRegister(object sender, EventArgs e)
        {
            await Navigation.PushAsync(App.Container.Resolve<RegisterView>());
        }

        public async void OnLogin(object sender, EventArgs e)
        {
            await ShowLogin(false);

            if (await _ApiService.LoginAsync(
                uiEntryUsername.Text,
                uiEntryPassword.Text,
                uiCheckboxRememberMe.IsChecked))
            {
                await OpenMainView();
            }
            else
            {
                await this.ShowAlert("Login was not successful. Please check your credentials.", "Ok");
                await ShowLogin();
            }
        }
    }
}
