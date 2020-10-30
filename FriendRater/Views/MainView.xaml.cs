using System;
using System.Collections.Generic;
using FriendRater.Data;
using Xamarin.Forms;

namespace FriendRater.Views
{
    public partial class MainView : ContentPage
    {
        public MainView(Credentials pCredentials)
        {
            InitializeComponent();

            Button lButton = new Button
            {
                Text = "logout",
            };
            lButton.Clicked += async (s, e) =>
            {
                await App.Database.DeleteOptionAsync(nameof(Credentials));
                await Navigation.PushAsync(new LoginView());
                Navigation.RemovePage(this);
            };
            Content = lButton;
        }
    }
}
