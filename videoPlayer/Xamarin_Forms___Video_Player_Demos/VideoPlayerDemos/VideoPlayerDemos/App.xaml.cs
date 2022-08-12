using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using NetCommom_Ting;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace VideoPlayerDemos
{
    public partial class App : Application
    {
        public static SrvClient SrvClient { get; set; }
        public static string SrvHost => "http://120.126.151.103/KioskService/api/Kiosk/";
        public App()
        {
            InitializeComponent();

            SrvClient = new SrvClient(SrvHost);
            MainPage = new NavigationPage(new HomePage());
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
        public static void Invoker(Action action)
        {
            Device.BeginInvokeOnMainThread(action);
        }
    }
}
