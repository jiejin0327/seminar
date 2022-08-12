using NetCommom_Ting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VideoPlayerDemos
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ResultSuccess : ContentPage
    {
        public RecUser UserInfo { get; set; }
        public string PhotoFileName { get; set; }

        public ResultSuccess(RecUser _UserInfo, string _PhotoFileName)
        {
            InitializeComponent();

            UserInfo = _UserInfo;
            PhotoFileName = _PhotoFileName;
            UserInfo.StarName = UserInfo.StarId.Split(':')[1].Split('\"')[1].Split(',')[0].Split('\"')[0];

            UserInfo.StarScore = UserInfo.StarId.Split('r')[1].Split(':')[1].Split('\"')[1].Split(',')[0].Split('\"')[0];
            Lb_name.Text = "歡迎你，" + UserInfo.UserName + "\n明星臉偵測 : " + UserInfo.StarName + "\n相似度 : " + UserInfo.StarScore + "%";
            var DefUrl = "http://120.126.151.103/KioskService/api/Kiosk/GetImage";
            var FullStarUrl = $"{DefUrl}?AsUseType=Star&ImageId={UserInfo.StarName}&Token={HttpUtility.UrlEncode(UserInfo.Token)}";
            Img_Star.Source = ImageSource.FromUri(new Uri(FullStarUrl));
            var FullUserUrl = $"{DefUrl}?AsUseType=User&ImageId={UserInfo.UserId}&Token={HttpUtility.UrlEncode(UserInfo.Token)}";
            Img_User.Source = ImageSource.FromUri(new Uri(FullUserUrl));
        }
        async void Complete(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new HomePage());
        }
    }
}