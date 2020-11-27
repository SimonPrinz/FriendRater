using System.Collections.Generic;
using Newtonsoft.Json;

namespace FriendRater.Api
{
    public class ApiWrapper<TResponse> where TResponse : new()
    {
        [JsonProperty("ok")]
        public bool Ok { get; set; }

        [JsonProperty("data")]
        public TResponse Data { get; set; }

        [JsonProperty("errors")]
        public List<Error> Errors { get; set; }
    }

    public class Error
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
