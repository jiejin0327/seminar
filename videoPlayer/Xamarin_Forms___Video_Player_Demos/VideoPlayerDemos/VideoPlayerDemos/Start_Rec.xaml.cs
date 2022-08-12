using System;
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
    public partial class Start_Rec : ContentPage
    {
        public Start_Rec()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }
        async void success(object sender, EventArgs args)
        {
            //await Navigation.PushAsync(new ResultSuccess(UserInfo));
        }
        async void fail(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new ResultFail());
        }
        
        
    }
}