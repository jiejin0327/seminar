using KioskPhoneApp.NetCommom_Ting;
using Newtonsoft.Json;
using Rugal.XTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KioskPhoneApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            //this.SetBackGroundImage(App.BgInfo);
            NavigationPage.SetHasNavigationBar(this, false);
        }

        public async void BtnRegister(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Register());
        }
        public async void BtnLogin(object sender, EventArgs e)
        {
            /*if (App.IsNotNeedLogin)
            {
                TxtUserPw.Text = "";
                await Navigation.PushAsync(new Upload());
                return;
            }
            */

            if (await ChkTxt())
            {
                var UserId = TxtUserId.Text;
                var UserPw = TxtUserPw.Text;
                var Ret = await App.SrvClient.GetToken(UserId, UserPw);

                if (Ret.IsSuccess)
                {
                    await DisplayAlert("訊息", "登入成功", "ok");
                    TxtUserPw.Text = "";
                    var UserName = (Ret.ErrorMsg.ToString());
                    await Navigation.PushAsync(new Upload(UserName));
                }
                else
                    await DisplayAlert("訊息", "登入失敗，Message：" + Ret.ErrorMsg, "ok");
            }

        }
        public async Task<bool> ChkTxt()
        {
            if (TxtUserId.Text != null && TxtUserId.Text != "" && TxtUserPw.Text != null && TxtUserPw.Text != "")
                return true;
            await DisplayAlert("訊息", "帳號、密碼不得留白", "ok");
            return false;
        }
    }
}