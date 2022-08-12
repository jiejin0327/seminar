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
    public partial class Register : ContentPage
    {
        public Register()
        {
            InitializeComponent();

            //this.SetBackGroundImage(App.BgInfo);

            NavigationPage.SetHasNavigationBar(this, false);

        }
        public async void BtnPopPage(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
        public async void BtnRegister(object sender, EventArgs e)
        {
            if (!await ChkTxt())
                return;
            var UserId = txtId.Text;
            var UserPw = txtPw.Text;
            var UserName = txtName.Text;
            var UserPhone = txtPhone.Text;
            var UserEmail = txtEmail.Text;
            var Ret = await App.SrvClient.Register(UserId, UserPw, UserName, UserPhone, UserEmail);
            if (Ret.IsSuccess)
            {
                await DisplayAlert("訊息", "註冊成功！", "ok");
                await Navigation.PopAsync();
            }
            else
                await DisplayAlert("訊息", "註冊失敗！訊息：" + Ret.ErrorMsg, "ok");
        }
        public async Task<bool> ChkTxt()
        {
            var ChkObj = new List<string> { txtId.Text, txtPw.Text, txtName.Text, txtPhone.Text, txtEmail.Text };
            foreach (var Item in ChkObj)
            {
                if (Item == null || Item == "")
                {
                    await DisplayAlert("訊息", "請確實填寫", "ok");
                    return false;
                }
            }
            return true;
        }
    }
}