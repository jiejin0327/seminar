using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace VideoPlayerDemos
{
    public class CameraPreview : View
    {
        public static readonly BindableProperty CameraProperty = BindableProperty.Create(
        propertyName: "Camera",
        returnType: typeof(CameraOptions),
        declaringType: typeof(CameraPreview),
        defaultValue: CameraOptions.Rear);

        public string FilePath { get; set; }

        public CameraOptions Camera
        {
            get { return (CameraOptions)GetValue(CameraProperty); }
            set { SetValue(CameraProperty, value); }
        }
        public Action UploadEvent { get; set; }
    }
}
