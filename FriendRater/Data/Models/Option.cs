using Newtonsoft.Json;
using SQLite;

namespace FriendRater.Data.Models
{
    public class Option
    {
        [PrimaryKey, Unique]
        public string Key { get; set; }

        public string Value { get; set; }
    }
}
