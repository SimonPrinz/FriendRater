using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FriendRater
{
    public static class Extensions
    {
        public static Task ShowAlert(this Page @this, string message, string cancel)
        {
            return @this.DisplayAlert("Friend Rater", message, cancel);
        }

        public static Task ShowAlert(this Page @this, string message, string accept, string cancel)
        {
            return @this.DisplayAlert("Friend Rater", message, accept, cancel);
        }
    }
}
