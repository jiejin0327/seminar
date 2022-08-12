using NetCommom_Ting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace VideoPlayerDemos
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DetectStar : ContentPage
    {
        public RecStar UserInfo { get; set; }
        public DetectStar(RecStar _UserInfo)
        {
            InitializeComponent();
            Lb_name.Text = "你長得很像，" + UserInfo.UserName;
            //var DefUrl = "http://120.126.151.103/KioskService/api/Kiosk/GetStarImage";
            //var FullUrl = $"{DefUrl}?UserId={UserInfo.UserId}&Token={HttpUtility.UrlEncode(UserInfo.Token)}";
            //Img_Star.Source = ImageSource.FromUri(new Uri(FullUrl));
        }
        async void Back(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new HomePage());
        }
        async void TakePhoto(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new HomePage());
        }
    }
}