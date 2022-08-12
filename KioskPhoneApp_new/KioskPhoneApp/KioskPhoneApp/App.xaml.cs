using KioskPhoneApp.NetCommom_Ting;
using Rugal.XTool;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using KioskPhoneApp.Pages;

namespace KioskPhoneApp
{
    public partial class App : Application
    {
        public static SrvClient SrvClient { get; set; }
        public static GdColorInfo BgInfo = new GdColorInfo(200, 200, GradientPath.ToBottomRight, "0.12 0.86", "#fbebd2", "#a3cbf6");
        public static bool IsNotNeedLogin = false;
        public static string SrvHost => "http://120.126.151.103/KioskService/api/Kiosk/";
        public App()
        {
            InitializeComponent();

            SrvClient = new SrvClient(SrvHost);
            MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

        public static void Invoker(Action action)
        {
            Device.BeginInvokeOnMainThread(action);
        }
    }
}
