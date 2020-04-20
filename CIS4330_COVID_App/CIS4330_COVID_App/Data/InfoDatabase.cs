using CIS4330_COVID_App.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CIS4330_COVID_App.Data
{
    public class InfoDatabase
    {
        readonly SQLiteAsyncConnection _database;

        public InfoDatabase(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<MyLocation>().Wait();
        }

        public Task<List<MyLocation>> GetMyLocationsAsync()
        {
            return _database.Table<MyLocation>().ToListAsync();
        }



        public Task<int> SaveMyLocationAsync(MyLocation MyLocation)
        {
                return _database.InsertAsync(MyLocation);
        }

        async public Task DeleteAll()
        {
            await _database.DropTableAsync<MyLocation>();
            await _database.CreateTableAsync<MyLocation>();
        }
    }

}
