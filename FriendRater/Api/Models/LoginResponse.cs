using Newtonsoft.Json;

namespace FriendRater.Api.Models
{
    public class LoginResponse
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
