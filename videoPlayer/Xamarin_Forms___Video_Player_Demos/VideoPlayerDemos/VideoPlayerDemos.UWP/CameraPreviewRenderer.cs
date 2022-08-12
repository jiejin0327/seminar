using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VideoPlayerDemos;
using VideoPlayerDemos.UWP;
using Windows.Devices.Enumeration;
using Windows.Graphics.Display;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System.Display;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(CameraPreview), typeof(CameraPreviewRenderer))]
namespace VideoPlayerDemos.UWP
{
    class CameraPreviewRenderer : ViewRenderer<CameraPreview, Windows.UI.Xaml.Controls.CaptureElement>
    {
        public string FilePath { get; set; }

        readonly DisplayInformation _displayInformation = DisplayInformation.GetForCurrentView();
        DisplayOrientations _displayOrientation = DisplayOrientations.Portrait;
        readonly DisplayRequest _displayRequest = new DisplayRequest();

        // Rotation metadata to apply to preview stream (https://msdn.microsoft.com/en-us/library/windows/apps/xaml/hh868174.aspx)
        static readonly Guid RotationKey = new Guid("C380465D-2271-428C-9B83-ECEA3B4A85C1"); // (MF_MT_VIDEO_ROTATION)

        static readonly SemaphoreSlim _mediaCaptureLifeLock = new SemaphoreSlim(1);

        MediaCapture _mediaCapture;
        CaptureElement _captureElement;
        bool _isInitialized;
        bool _isPreviewing;
        bool _externalCamera;
        bool _mirroringPreview;

        Application _app;

        //protected override void

        protected override void OnElementChanged(ElementChangedEventArgs<CameraPreview> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                Tapped -= GetPhoto;
            }
            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    _app = Application.Current;

                    _captureElement = new CaptureElement();
                    _captureElement.Stretch = Stretch.UniformToFill;

                    SetupCamera();
                    SetNativeControl(_captureElement);
                }
                Tapped += GetPhoto;
            }
        }

        async void SetupCamera()
        {
            _displayOrientation = _displayInformation.CurrentOrientation;
            await InitializeCameraAsync();
        }

        public async void GetPhoto(object sender, TappedRoutedEventArgs e)
        {
            _captureElement.Source = _mediaCapture;

            ImageEncodingProperties format = ImageEncodingProperties.CreateJpeg();

            //创建本地存储文件夹
            StorageFile Photo = await ApplicationData.Current.LocalFolder.CreateFileAsync(
                "Photo.jpg",
                CreationCollisionOption.ReplaceExisting);

            if (Photo != null)
            {
                FilePath = Photo.Path;
            }
            await _mediaCapture.CapturePhotoToStorageFileAsync(format, Photo);
            Element.FilePath = FilePath;
            Element.UploadEvent.Invoke();
        }

        #region Camera

        async Task InitializeCameraAsync()
        {
            await _mediaCaptureLifeLock.WaitAsync();

            if (_mediaCapture == null)
            {
                // Attempt to get the back camera, but use any camera if not
                var cameraDevice = await FindCameraDeviceByPanelAsync(Windows.Devices.Enumeration.Panel.Back);
                if (cameraDevice == null)
                {
                    Debug.WriteLine("No camera found");
                    return;
                }

                _mediaCapture = new MediaCapture();
                var settings = new MediaCaptureInitializationSettings { VideoDeviceId = cameraDevice.Id };
                try
                {
                    await _mediaCapture.InitializeAsync(settings);
                    _isInitialized = true;
                }

                catch (UnauthorizedAccessException)
                {
                    Debug.WriteLine("Camera access denied");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Exception initializing MediaCapture - {0}: {1}", cameraDevice.Id, ex.ToString());
                }
                finally
                {
                    _mediaCaptureLifeLock.Release();
                }

                if (_isInitialized)
                {
                    if (cameraDevice.EnclosureLocation == null || cameraDevice.EnclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Unknown)
                    {
                        _externalCamera = true;
                    }
                    else
                    {
                        // Camera is on device
                        _externalCamera = false;

                        // Mirror preview if camera is on front panel
                        _mirroringPreview = (cameraDevice.EnclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Front);
                    }
                    await StartPreviewAsync();
                }
            }
            else
            {
                _mediaCaptureLifeLock.Release();
            }
        }

        async Task StartPreviewAsync()
        {
            // Prevent the device from sleeping while the preview is running
            _displayRequest.RequestActive();

            // Setup preview source in UI and mirror if required
            _captureElement.Source = _mediaCapture;
            //_captureElement.FlowDirection = _mirroringPreview ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;

            // Start preview
            await _mediaCapture.StartPreviewAsync();
        }

        async Task StopPreviewAsync()
        {
            _isPreviewing = false;
            await _mediaCapture.StopPreviewAsync();

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                // Allow device screen to sleep now preview is stopped
                _displayRequest.RequestRelease();
            });
        }

        async Task CleanupCameraAsync()
        {
            await _mediaCaptureLifeLock.WaitAsync();

            if (_isInitialized)
            {
                if (_isPreviewing)
                {
                    await StopPreviewAsync();
                }
                _isInitialized = false;
            }
            if (_captureElement != null)
            {
                _captureElement.Source = null;
            }
            if (_mediaCapture != null)
            {
                _mediaCapture.Dispose();
                _mediaCapture = null;
            }
        }

        #endregion

        #region Helpers

        async Task<DeviceInformation> FindCameraDeviceByPanelAsync(Windows.Devices.Enumeration.Panel desiredPanel)
        {

            var allVideoDevices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
            var desiredDevice = allVideoDevices.FirstOrDefault(d => d.EnclosureLocation != null && d.EnclosureLocation.Panel == desiredPanel);
            return desiredDevice ?? allVideoDevices.FirstOrDefault();
        }
        #endregion
    }
}
