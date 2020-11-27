using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using FriendRater.Api.Models;
using FriendRater.Services;
using Xamarin.Forms;

namespace FriendRater.Views
{
    public partial class MainView : ContentPage
    {
        private readonly IApiService _ApiService;
        
        public string Banner => $"Welcome {_ApiService.GetName() ?? ""}";
        public ObservableCollection<User> Users { get; }
        public bool IsLoading { get; internal set; }
        public bool IsLoadingUser { get; internal set; }

        private int _CurrentTask = 0;
        private readonly List<User> _Contacts = null;
        
        public MainView(IApiService pApiService)
        {
            _ApiService = pApiService;
            
            InitializeComponent();

            Users = new ObservableCollection<User>();
            BindingContext = this;

            Appearing += async (s, e) =>
            {
                if (Users.Any())
                    return;
                await ShowContacts();
            };
        }

        private void SetIsLoading(bool pFlag)
        {
            IsLoading = pFlag;
            OnPropertyChanged(nameof(IsLoading));
        }

        private void SetIsLoadingUser(bool pFlag)
        {
            IsLoadingUser = pFlag;
            OnPropertyChanged(nameof(IsLoadingUser));
        }
        
        public async void OnSearch(object sender, EventArgs e)
        {
            bool lText = e is TextChangedEventArgs;
            TextChangedEventArgs te = lText ? (TextChangedEventArgs)e : null;

            if (lText && te.NewTextValue == null)
            {
                await ShowContacts();
                return;
            }
            if (lText && te.NewTextValue.Length < 3)
            {
                return;
            }

            _CurrentTask++;
            await SearchOnline(lText ? te.NewTextValue : uiSearchbarSearch.Text, _CurrentTask);
        }

        public async void OnUnfocused(object sender, EventArgs e)
        {
            if (uiSearchbarSearch.Text == "")
                await ShowContacts();
        }

        private async Task ShowContacts()
        {
            if (_Contacts != null)
            {
                Users.Clear();
                foreach (User lUser in _Contacts) Users.Add(lUser);
                return;
            }
            int lCurrentTask = ++_CurrentTask;
            SetIsLoading(true);
            List<User> lUsersInContacts = await _ApiService.GetUsersInContacts();
            if (lCurrentTask != _CurrentTask)
                return;
            Users.Clear();
            foreach (User lUser in lUsersInContacts)
                Users.Add(lUser);
            SetIsLoading(false);
        }

        private async Task SearchOnline(string pSearch, int pTask)
        {
            if (pSearch.Equals("logout", StringComparison.OrdinalIgnoreCase))
            {
                await _ApiService.LogoutAsync();
                await Navigation.PushAsync(App.Container.Resolve<LoginView>());
                Navigation.RemovePage(this);
                return;
            }
            
            SetIsLoading(true);
            List<User> lFoundUsers = await _ApiService.Search(pSearch);
            if (pTask != _CurrentTask)
                return;
            Users.Clear();
            foreach (User lUser in lFoundUsers)
                Users.Add(lUser);
            SetIsLoading(false);
        }

        private async void OnUserClick(object sender, EventArgs e)
        {
            if (!(sender is ViewCell lCell))
                return;

            SetIsLoadingUser(true);
            try
            {
                if (!Guid.TryParse(lCell.ClassId, out Guid lId))
                    throw new Exception();

                UserProfile lProfile = await _ApiService.LoadUserProfile(lId);
                
                await Navigation.PushAsync(App.Container.Resolve(typeof(UserView), lProfile) as UserView);
                
                SetIsLoadingUser(false);
            }
            catch (Exception lException)
            {
                await this.ShowAlert("Apparently this User has some problems while loading.", "That's sad :(");
                SetIsLoadingUser(false);
            }
        }

        // private async Task AddRecent(User pUser)
        // {
        //     List<User> lRecents = await GetRecent();
        //
        //     if (lRecents.Any(i => i.Id == pUser.Id))
        //         lRecents.Remove(pUser);
        //     lRecents.Add(pUser);
        //
        //     await App.Database.SetOptionAsync<List<User>>("Recent", lRecents);
        // }
        //
        // private async Task<List<User>> GetRecent()
        // {
        //     return await App.Database.GetOptionAsync<List<User>>("Recent", new List<User>());
        // }
    }
}
