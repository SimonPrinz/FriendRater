using System.Threading.Tasks;
using FriendRater.Data.Models;
using Newtonsoft.Json;
using SQLite;

namespace FriendRater.Data
{
    public class Database
    {
        public SQLiteAsyncConnection Impl { get; }

        public Database(string pDatabasePath)
        {
            Impl = new SQLiteAsyncConnection(pDatabasePath);
            Impl.CreateTableAsync<Option>().Wait();
        }

        public async Task<TType> GetOptionAsync<TType>(string pKey, TType pDefault = default)
        {
            Option lOption = await Impl.Table<Option>().FirstOrDefaultAsync(pOption => pOption.Key.Equals(pKey));
            return lOption != null ? JsonConvert.DeserializeObject<TType>(lOption.Value) : pDefault;
        }

        public async Task<bool> SetOptionAsync<TType>(string pKey, TType pValue)
        {
            return await Impl.InsertOrReplaceAsync(new Option
            {
                Key = pKey,
                Value = JsonConvert.SerializeObject(pValue),
            }) > 0;
        }

        public async Task DeleteOptionAsync(string pKey)
        {
            await Impl.DeleteAsync<Option>(pKey);
        }

        public async Task ClearOptionsAsync()
        {
            await Impl.DeleteAllAsync<Option>();
        }
    }
}
