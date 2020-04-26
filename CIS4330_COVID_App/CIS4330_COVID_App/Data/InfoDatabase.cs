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
        //Simple database interaction
        //  Developed with help from Microsoft Xamarin documentation
        readonly SQLiteAsyncConnection _database;

        public InfoDatabase(string dbPath)
        {
            //In the constructor, it connects to the db and creates the table (if necessary)
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<MyLocation>().Wait();
        }

        public Task<List<MyLocation>> GetMyLocationsAsync()
        {
            //Get all locations into a list. There's only one: the home location
            return _database.Table<MyLocation>().ToListAsync();
        }



        public Task<int> SaveMyLocationAsync(MyLocation MyLocation)
        {
            //Save the home location
                return _database.InsertAsync(MyLocation);
        }

        async public Task DeleteAll()
        {

            //Laziest (and probably slowest) method to drop the home record.
            //  The table gets dropped and recreated
            await _database.DropTableAsync<MyLocation>();
            await _database.CreateTableAsync<MyLocation>();
        }
    }

}
