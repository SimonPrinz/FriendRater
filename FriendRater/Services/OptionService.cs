using System.Threading.Tasks;
using FriendRater.Data;
using FriendRater.Data.Models;
using Newtonsoft.Json;

namespace FriendRater.Services
{
    public interface IOptionService
    {
        public Task<TType> GetOptionAsync<TType>(string pKey, TType pDefault = default);

        public Task<bool> SetOptionAsync<TType>(string pKey, TType pValue);

        public Task DeleteOptionAsync(string pKey);

        public Task ClearOptionsAsync();
    }
    
    public class OptionService : IOptionService
    {
        private readonly IDatabase _Database;
        
        public OptionService(IDatabase pDatabase)
        {
            _Database = pDatabase;
        }
        
        public async Task<TType> GetOptionAsync<TType>(string pKey, TType pDefault = default)
        {
            Option lOption = await _Database.Connection.Table<Option>().FirstOrDefaultAsync(pOption => pOption.Key.Equals(pKey));
            return lOption != null ? JsonConvert.DeserializeObject<TType>(lOption.Value) : pDefault;
        }

        public async Task<bool> SetOptionAsync<TType>(string pKey, TType pValue)
        {
            return await _Database.Connection.InsertOrReplaceAsync(new Option
            {
                Key = pKey,
                Value = JsonConvert.SerializeObject(pValue),
            }) > 0;
        }

        public async Task DeleteOptionAsync(string pKey)
        {
            await _Database.Connection.DeleteAsync<Option>(pKey);
        }

        public async Task ClearOptionsAsync()
        {
            await _Database.Connection.DeleteAllAsync<Option>();
        }
    }
}