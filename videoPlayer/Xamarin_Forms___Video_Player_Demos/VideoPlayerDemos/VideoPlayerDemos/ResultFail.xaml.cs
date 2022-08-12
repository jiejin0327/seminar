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
    public partial class ResultFail : ContentPage
    {
        public ResultFail()
        {
            InitializeComponent();
        }
        async void Back(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new HomePage());
        }
    }
}