using CIS4330_COVID_App.Data;
using CIS4330_COVID_App.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CIS4330_COVID_App.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        //
        //
        //BEGIN Property getters/setters for labels
        //
        //For each: Sets the string and alerts the application that the property was changed
        //So that the label updates
        //
        //
        private string homeLocation;
        private string currentLocation;
        private string distance;
        public string lblHomeLocation
        {
            get { return homeLocation; }
            set
            {
                homeLocation = value;
                OnPropertyChanged(nameof(lblHomeLocation)); // Notify that there was a change on this property
            }
        }
        public string lblCurrentLocation
        {
            get { return currentLocation; }
            set
            {
                currentLocation = value;
                OnPropertyChanged(nameof(lblCurrentLocation)); // Notify that there was a change on this property
            }
        }
        public string lblDistance
        {
            get { return distance; }
            set
            {
                distance = value;
                OnPropertyChanged(nameof(lblDistance)); // Notify that there was a change on this property
            }
        }

        //SQLite DB functionality
        static InfoDatabase database;

        public static InfoDatabase Database
        {
            get
            {
                if (database == null)
                {
                    database = new InfoDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Locations.db3"));
                }
                return database;
            }
        }

        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;

            MyLocation HomeLocation = new MyLocation();
            MyLocation CurrentLocation = new MyLocation();

            //
            //Run asyncronous tasks so that we don't rush ahead of the rest of the app
            //
            Task.Run(async () =>
            {
                try
                {
                    //Wait for the home location to be retrieved from the database
                    HomeLocation = await CheckHomeSet();

                    //Set the home location label
                    lblHomeLocation = ($"Home: Latitude: {HomeLocation.lat}, Longitude: {HomeLocation.lng}");

                    //Wait for the current location to be retrived from the Geolocator
                    CurrentLocation = await GetCurrentLocation();

                    //Set the current location label
                    lblCurrentLocation = ($"Current Location: Latitude: {CurrentLocation.lat}, Longitude: {CurrentLocation.lng}");

                    //Wait for the result of checking if home
                    await CheckIfHome(HomeLocation, CurrentLocation);
                }
                catch (Exception ex)
                {
                    //If there's an error, display it in the home location label for debugging
                    lblHomeLocation = (ex.ToString());
                    throw ex;
                }
            });

        }

        async void btnUpdate_Click(object sender, EventArgs e)
        {
            //
            //This button is used to manually update the home location in the event it becomes messed up
            //
            MyLocation myLocation = new MyLocation();

            //Grab the geolocation with 100 meter accuracy and a 3 second timeout
            var request = new GeolocationRequest(GeolocationAccuracy.Best, new TimeSpan(0, 0, 3));
            var geolocation = await Geolocation.GetLocationAsync(request);

            if (geolocation != null)
            {
                //If we found the location, update the database with the new home location
                //  The database just uses a single row in a single table that contains the home location info
                //  In production, this could store all locations that have been posted to the server

                await Database.DeleteAll();

                myLocation.lat = geolocation.Latitude;
                myLocation.lng = geolocation.Longitude;

                await Database.SaveMyLocationAsync(myLocation);

                //Update the home location label
                lblHomeLocation = ($"Home Location Updated! Latitude: {myLocation.lat}, Longitude: {myLocation.lng}");
            }
        }

        async void btnChangeLocation_Click(object sender, EventArgs e)
        {
            //
            //This is a button that was used specifically for development
            //
            //Due to Philadelphia's measures on sheltering in place, testing the application is difficult
            //  This button allows data to be entered manually.
            //
            MyLocation CurrentLocation = new MyLocation();
            MyLocation HomeLocation = new MyLocation();

            CurrentLocation.lat = 39.973122;
            CurrentLocation.lng = -75.116068;


            HomeLocation = await CheckHomeSet();

            await CheckIfHome(HomeLocation, CurrentLocation);


        }

        public async Task<MyLocation> GetCurrentLocation()
        {
            //Wait for the current location from Geolocator and set return it as a MyLocation type
            MyLocation myLocation = new MyLocation();
            var request = new GeolocationRequest(GeolocationAccuracy.Best, new TimeSpan(0, 0, 5));
            var geolocation = await Geolocation.GetLocationAsync(request);

            if (geolocation != null)
            {
                myLocation.lat = geolocation.Latitude;
                myLocation.lng = geolocation.Longitude;

                return myLocation;

            }

            return null;
        }

        public async Task<MyLocation> CheckHomeSet()
        {
            //Attempt to grab the location record from the database
            List<MyLocation> location = new List<MyLocation>();
            location = await Database.GetMyLocationsAsync();
            if (location.Count == 0)
            {
                //If the location isn't saved, ask the user if they are home
                bool answer = await DisplayAlert("Home not saved!", "Are you at home? We need to save your home location", "Yes", "No");
                if (answer)
                {

                    //If they're home, grab the location from the geolocator and save it in the database record
                    MyLocation myLocation = new MyLocation();
                    var request = new GeolocationRequest(GeolocationAccuracy.Best, new TimeSpan(0, 0, 3));
                    var geolocation = await Geolocation.GetLocationAsync(request);

                    if (geolocation != null)
                    {
                        myLocation.lat = geolocation.Latitude;
                        myLocation.lng = geolocation.Longitude;
                        await Database.SaveMyLocationAsync(myLocation);
                        return myLocation;

                    }
                }
                else
                {
                    //If they aren't home, do nothing
                    //
                    //Ideally, this would close the application so that the user could open it when they are home
                    //
                    //  A push notification would assist in that
                    return null;
                }
            }
            else
            {
                //If the home location is stored already, return it.
                await DisplayAlert("Found home", "Home location already stored!", "OK");

                return location[0];

            }
            return null;
        }


        async public Task CheckIfHome(MyLocation HomeLocation, MyLocation CurrentLocation)
        {

            Location Home = new Location(HomeLocation.lat, HomeLocation.lng);
            Location Current = new Location(CurrentLocation.lat, CurrentLocation.lng);
            lblHomeLocation = ($"Home: Latitude: {HomeLocation.lat}, Longitude: {HomeLocation.lng}");
            lblCurrentLocation = ($"Current Location: Latitude: {CurrentLocation.lat}, Longitude: {CurrentLocation.lng}");

            //Use the calculate distance method of Location. It returns the distance in kilometers, so it's converted to meters
            //  Then it gets shown on the label/
            double meters = 1000 * Location.CalculateDistance(Home, Current, DistanceUnits.Kilometers);
            lblDistance = "Distance From Home: " + Math.Round(meters).ToString() + " meters";
            if (meters > 100)
            {
                //If the user is more than 100 meters away, we can POST it to the api
                //
                //100 meters is completely arbitrary, it was chosen because I could trigger updates while I walk to work (Tyler)
                //
                //The MyLocation object is serialized into Json using the getters and setters. 
                //  The Node side can convert it into a js object when it receives it.
                //
                HttpClient _client = new HttpClient();
                string URL = "https://www.tylerdoudrick.com/api/coordinates";
                    var location = CurrentLocation;
                var content = new StringContent(JsonConvert.SerializeObject(location), Encoding.UTF8, "application/json");
                   HttpResponseMessage result = await _client.PostAsync(URL, content);
            }
        }
     
    }
}
