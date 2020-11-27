using System;
using Newtonsoft.Json;

namespace FriendRater.Api.Models
{
    public class RegisterRequest
    {
        /// <summary>
        /// required: true; minLength: 3; maxLength: 16
        /// </summary>
        [JsonProperty("username")]
        public string Username { get; set; }

        /// <summary>
        /// required: true, minLength: 8
        /// </summary>
        [JsonProperty("password")]
        public string Password { get; set; }

        /// <summary>
        /// required: true; example: hello@example.com
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>
        /// required: true
        /// </summary>
        [JsonProperty("firstname")]
        public string Firstname { get; set; }

        /// <summary>
        /// required: true
        /// </summary>
        [JsonProperty("lastname")]
        public string Lastname { get; set; }

        /// <summary>
        /// required: false; example: +49123456789
        /// </summary>
        [JsonProperty("phoneNumber")]
#nullable enable
        public string? PhoneNumber { get; set; }
#nullable restore
    }
}
