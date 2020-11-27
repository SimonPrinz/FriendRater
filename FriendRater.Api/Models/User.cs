using System;
using Newtonsoft.Json;

namespace FriendRater.Api.Models
{
    public class User : IComparable<User>
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        public int CompareTo(User other)
        {
            return Id.CompareTo(other.Id);
        }
    }

    public class UserProfile
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("picture")]
        public string Picture { get; set; }

        [JsonProperty("ratings")]
        public Ratings Ratings {get;set;}
    }

    public class Ratings
    {
        [JsonProperty("up")]
        public int Up { get; set; }

        [JsonProperty("down")]
        public int Down { get; set; }

        [JsonProperty("like")]
        public int Like { get; set; }
    }

    public class UserComment
    {
        [JsonProperty("comment")]
        public string Comment { get; set; }

        [JsonProperty("user")]
        public User User { get; set; }
    }
}
