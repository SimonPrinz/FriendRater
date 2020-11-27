using FriendRater.Data.Models;
using SQLite;

namespace FriendRater.Data
{
    public interface IDatabase
    {
        public SQLiteAsyncConnection Connection { get; }
    }
    
    public class Database : IDatabase
    {
        public SQLiteAsyncConnection Connection { get; }

        public Database(string pDatabasePath)
        {
            Connection = new SQLiteAsyncConnection(pDatabasePath);
            Connection.CreateTableAsync<Option>().Wait();
        }
    }
}
