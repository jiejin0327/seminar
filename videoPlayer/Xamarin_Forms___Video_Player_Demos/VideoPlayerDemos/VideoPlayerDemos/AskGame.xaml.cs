using NetCommom_Ting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VideoPlayerDemos
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AskGame : ContentPage
    {
        public RecUser UserInfo { get; set; }
        public string PhotoFileName { get; set; }
        public AskGame(RecUser _UserInfo, string _PhotoFileName)
        {
            InitializeComponent();
            UserInfo = _UserInfo;
            PhotoFileName = _PhotoFileName;
        }
        async void Yes(object sender, EventArgs args)
        {
            var Ret = await App.SrvClient.UpToStarDetect(PhotoFileName);

            if (Ret.IsSuccess)
            {
                await DisplayAlert("訊息", "辨識成功！Message：", "ok");
                var UserInfo = JsonConvert.DeserializeObject<RecStar>(Ret.Result.ToString());
                await Navigation.PushAsync(new DetectStar(UserInfo));
            }
            else
                await DisplayAlert("訊息", "上傳辨識失敗！Message：" + Ret.ErrorMsg, "ok");
                
        }
        async void No(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new HomePage ());
        }
    }
}