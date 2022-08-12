using NetCommom_Ting;
using Newtonsoft.Json;
using CameraView;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VideoPlayerDemos
{
    public partial class HomePage : ContentPage
    {
        public CameraPreview CameraPath { get; set; }
        public HomePage()
        {
            InitializeComponent();
        }

        public async void TakePhoto(object sender, EventArgs e)
        {
            CameraPath = new CameraPreview
            {
                Camera = CameraOptions.Rear,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                UploadEvent = async () =>
                {
                    if (CameraPath.FilePath != null)
                    {
                        var PhotoFileName = CameraPath.FilePath;
                        //var PhotoFileName = Photo.Path;
                        var Ret = await App.SrvClient.UpToRecognize(PhotoFileName);

                        if (Ret.IsSuccess)
                        {
                            await DisplayAlert("訊息", "辨識成功！", "ok");
                            //CleanTempImage(PhotoFileName);
                            var UserInfo = JsonConvert.DeserializeObject<RecUser>(Ret.Result.ToString());
                            await Navigation.PushAsync(new ResultSuccess(UserInfo, PhotoFileName));
                        }
                        else
                        {
                            await DisplayAlert("訊息", "上傳辨識失敗！Message：" + Ret.ErrorMsg, "ok");
                            await Navigation.PushAsync(new ResultFail());
                        }
                    }
                }
            };

            Content = new StackLayout
            {
                Children = {
                    new Label { Text = "Camera Preview:" },
                    CameraPath
                }
            };
        }
        public void CleanTempImage(string PathName)
        {
            try
            {
                var FilePath = new FileInfo(PathName).Directory;
                if (FilePath.GetFiles("*").Length > 0)
                    FilePath.Delete(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
