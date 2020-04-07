using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using CIS4330_COVID_App.Views;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace CIS4330_COVID_App
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();


            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
