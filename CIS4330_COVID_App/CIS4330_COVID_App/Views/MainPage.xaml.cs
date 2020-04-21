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
        static InfoDatabase database;

        public static InfoDatabase Database
        {
            get
            {
                if (database == null)
                {
                    database = new InfoDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Info.db3"));
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


            Task.Run(async () =>
            {
                try
                {

                    // MyStringProperty = await MakeGetRequest();
                    HomeLocation = await CheckHomeSet();
                    CurrentLocation = await GetCurrentLocation();
                    await CheckIfHome(HomeLocation, CurrentLocation);
                }
                catch (System.OperationCanceledException ex)
                {
                    Console.WriteLine($"Text load cancelled: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });

        }

        async void btnUpdate_Click(object sender, EventArgs e)
        {
            MyLocation myLocation = new MyLocation();
            var request = new GeolocationRequest(GeolocationAccuracy.Best, new TimeSpan(0, 0, 3));
            var geolocation = await Geolocation.GetLocationAsync(request);

            if (geolocation != null)
            {
                await Database.DeleteAll();

                myLocation.lat = geolocation.Latitude;
                myLocation.lng = geolocation.Longitude;

                await Database.SaveMyLocationAsync(myLocation);

                lblHomeLocation = ($"Home Location Updated! Latitude: {myLocation.lat}, Longitude: {myLocation.lng}");
            }
        }

        async void btnChangeLocation_Click(object sender, EventArgs e)
        {
            MyLocation CurrentLocation = new MyLocation();
            MyLocation HomeLocation = new MyLocation();

            CurrentLocation.lat = 39.9897171;
            CurrentLocation.lng = -75.1924763;


            HomeLocation = await CheckHomeSet();

            await CheckIfHome(HomeLocation, CurrentLocation);


        }

        public async Task<MyLocation> GetCurrentLocation()
        {

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

            return null;
        }

        public async Task<MyLocation> CheckHomeSet()
        {
            List<MyLocation> location = await Database.GetMyLocationsAsync();
            if (location.Count == 0)
            {
                bool answer = await DisplayAlert("Home not saved!", "Are you at home? We need to save your home location", "Yes", "No");
                if (answer)
                {


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
                    return null;
                }
            }
            else
            {
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

            double meters = 1000 * Location.CalculateDistance(Home, Current, DistanceUnits.Kilometers);
            lblDistance = "Distance From Home: " + Math.Round(meters).ToString();
            if (meters > 100)
            {
                await DisplayAlert("Not Home!", "You're not at home, dickwad. Now we have to put you on a heatmap...", "I'm the worst :(");
                HttpClient _client = new HttpClient();
                string URL = "https://www.tylerdoudrick.com/api/coordinates";
                    var location = CurrentLocation;
                var content = new StringContent(JsonConvert.SerializeObject(location), Encoding.UTF8, "application/json");
                   HttpResponseMessage result = await _client.PostAsync(URL, content);
                await DisplayAlert("", result.ToString(),"ok");
                
            }
        }
        //public async Task<string> MakeGetRequest()
        //{
        //    try
        //    {
        //        var request = new GeolocationRequest(GeolocationAccuracy.Best, new TimeSpan(0, 0, 3));
        //        var location = await Geolocation.GetLocationAsync(request);

        //        if (location != null)
        //        {
        //            MyLocation = ($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
        //        }
        //        else
        //        {
        //            MyLocation = "null location";
        //        }
        //    }
        //    catch (FeatureNotSupportedException fnsEx)
        //    {
        //        // Handle not supported on device exception
        //    }
        //    catch (FeatureNotEnabledException fneEx)
        //    {
        //        // Handle not enabled on device exception
        //    }
        //    catch (PermissionException pEx)
        //    {
        //        // Handle permission exception
        //    }
        //    catch (Exception ex)
        //    {
        //        // Unable to get location
        //    }


        //    var client = new HttpClient();

        //    try
        //    {
        //        var httpRequest = new HttpRequestMessage()
        //        {
        //            RequestUri = new Uri("https://tylerdoudrick.com/api/coordinates"),
        //            Method = HttpMethod.Get,
        //        };
        //        var response = await client.SendAsync(httpRequest);
        //        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        //        {
        //            var responseString = await response.Content.ReadAsStringAsync();
        //            return responseString;
        //        }
        //        else if (response.StatusCode == HttpStatusCode.Unauthorized)
        //        {
        //            // you need to maybe re-authenticate here
        //            return "auth error";
        //        }
        //        else
        //        {
        //            return "fail";
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }

        //}
    }
}
