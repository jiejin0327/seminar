using KioskPhoneApp.NetCommom_Ting;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Rugal.XTool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KioskPhoneApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Upload : ContentPage
    {
        public string PhotoFileName { get; set; }
        public string Name { get; }
        public Upload(string _UserName)
        {
            InitializeComponent();
            Name = _UserName;
            //this.SetBackGroundImage(App.BgInfo);

            UserName.Text = "Hi, " + Name;
            NavigationPage.SetHasNavigationBar(this, false);

            EventInit();
        }
        private void EventInit()
        {
            var PickT = new TapGestureRecognizer();
            PickT.Tapped += PickPhoto;
            PhotoPick.GestureRecognizers.Add(PickT);

            var TakeT = new TapGestureRecognizer();
            TakeT.Tapped += TakePhoto;
            PhotoTake.GestureRecognizers.Add(TakeT);

            var UploadT = new TapGestureRecognizer();
            UploadT.Tapped += UploadImage;
            PhotoUpload.GestureRecognizers.Add(UploadT);
        }
        public async void TakePhoto(object sender, EventArgs e)
        {
            var Photo = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions() { });
            if (Photo != null)
            {
                PhotoFileName = Photo.Path;
                PhotoContent.Source = ImageSource.FromStream(() => { return Photo.GetStream(); });
            }
        }
        public async void PickPhoto(object sender, EventArgs e)
        {
            var Photo = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions() { });
            if (Photo != null)
            {
                PhotoFileName = Photo.Path;
                PhotoContent.Source = ImageSource.FromStream(() => { return Photo.GetStream(); });
            }
        }
        public async void LogOut(object sender, EventArgs e)
        {
            App.SrvClient.LogOut();
            await Navigation.PopToRootAsync();
        }
        public async void UploadImage(object sender, EventArgs e)
        {
            if (PhotoFileName != null && PhotoFileName != "")
            {
                var Ret = await App.SrvClient.UpToTraining(PhotoFileName);
                if (Ret.IsSuccess)
                {
                    await DisplayAlert("訊息", "上傳訓練成功！", "ok");
                    CleanTempImage();
                }
                else
                    await DisplayAlert("訊息", "上傳訓練失敗！Message：" + Ret.ErrorMsg, "ok");
            }
            else
                await DisplayAlert("訊息", "請先拍照或選擇照片", "ok");
        }
        public void CleanTempImage()
        {
            try
            {
                var FilePath = new FileInfo(PhotoFileName).Directory;
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