using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media;
using System.Threading;
using Windows.System.Threading;

namespace VideoPlayerDemos.UWP
{
    class FaceDetection
    {
        private FaceTracker faceTracker;
        private ThreadPoolTimer frameProcessingTimer;
        private SemaphoreSlim frameProcessingSemaphore = new SemaphoreSlim(1);
        public static IAsyncOperation<FaceTracker> CreateAsync()
        {
            this.faceTracker = await FaceTracker.CreateAsync();
            TimeSpan timerInterval = TimeSpan.FromMilliseconds(66); // 15 fps
            this.frameProcessingTimer = Windows.System.Threading.ThreadPoolTimer.CreatePeriodicTimer(new Windows.System.Threading.TimerElapsedHandler(ProcessCurrentVideoFrame), timerInterval);
        }
        public async void ProcessCurrentVideoFrame(ThreadPoolTimer timer)
        {
            if (!frameProcessingSemaphore.Wait(0))
            {
                return;
            }

            VideoFrame currentFrame = await GetLatestFrame();

            // Use FaceDetector.GetSupportedBitmapPixelFormats and IsBitmapPixelFormatSupported to dynamically
            // determine supported formats
            const BitmapPixelFormat faceDetectionPixelFormat = BitmapPixelFormat.Nv12;

            if (currentFrame.SoftwareBitmap.BitmapPixelFormat != faceDetectionPixelFormat)
            {
                return;
            }

            try
            {
                IList<DetectedFace> detectedFaces = await faceTracker.ProcessNextFrameAsync(currentFrame);

                var previewFrameSize = new Windows.Foundation.Size(currentFrame.SoftwareBitmap.PixelWidth, currentFrame.SoftwareBitmap.PixelHeight);
                var ignored = this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    this.SetupVisualization(previewFrameSize, detectedFaces);
                });
            }
            catch (Exception e)
            {
                // Face tracking failed
            }
            finally
            {
                frameProcessingSemaphore.Release();
            }

            currentFrame.Dispose();
        }

    }
}
