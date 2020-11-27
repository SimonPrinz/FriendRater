using FriendRater.Api.Models;
using FriendRater.Services;
using Xamarin.Forms;

namespace FriendRater.Views
{
    public partial class UserView : ContentPage
    {
        private readonly IApiService _ApiService;
        private readonly UserProfile _Profile;

        public UserProfile Profile => _Profile;

        public UserView(IApiService pApiService, UserProfile pProfile)
        {
            _ApiService = pApiService;
            _Profile = pProfile;

            BindingContext = Profile;

            InitializeComponent();
        }
    }
}
