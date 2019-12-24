using GalaSoft.MvvmLight;
using System;
using System.Windows.Media.Imaging;
using AForge.Video;
using AForge.Video.DirectShow;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Drawing;
using System.Windows.Threading;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;

namespace QR
{
    internal class ModelView : ObservableObject
    {
        #region Private fields
        private BitmapImage _image;
        private IVideoSource _videoSource;
        private FilterInfo _currentDevice;
        private DispatcherTimer timer;
        #endregion

        #region Constructor
        public ModelView()
        {
            VideoDevices = new ObservableCollection<FilterInfo>();
            GetVideoDevices();
            StartCamera();
            timer = new DispatcherTimer();
            timer.Tick += timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();
        }

        #endregion


        #region Properties

        public ObservableCollection<FilterInfo> VideoDevices { get; set; }
        public BitmapImage Image
        {
            get { return _image; }
            set { Set(ref _image, value); }
        }
        public FilterInfo CurrentDevice
        {
            get { return _currentDevice; }
            set { Set(ref _currentDevice, value); }
        }


        #endregion

        private void timer_Tick(object sender, EventArgs e)
        {
            string qrdecode = QRScan.QRDecode(Image);
            if (qrdecode != "")
            {
                MessageBox.Show(qrdecode);
            }
        }
        private void GetVideoDevices()
        {
            var devices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo device in devices)
            {
                VideoDevices.Add(device);
            }
            if (VideoDevices.Any())
            {
                CurrentDevice = VideoDevices[0];
            }
            else
            {
                MessageBox.Show("No webcam found");
            }
        }
        private void StartCamera()
        {
            if (CurrentDevice != null)
            {
                _videoSource = new VideoCaptureDevice(CurrentDevice.MonikerString);
                _videoSource.NewFrame += video_NewFrame;
                _videoSource.Start();
            }
            else
            {
                MessageBox.Show("Current device can't be null");
            }
        }

        private void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                using (var bitmap = (Bitmap)eventArgs.Frame.Clone())
                {
                    var bi = bitmap.Bitmap2BitmapImage();
                    bi.Freeze();
                    Dispatcher.CurrentDispatcher.Invoke(() => Image = bi);
                    //string qrdecode = QRScan.QRDecode(bi);
                    //if (qrdecode != "")
                    //{
                    //    MessageBox.Show(qrdecode);
                    //}
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show("Error on _videoSource_NewFrame:\n" + exc.Message, "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                StopCamera();
            }
        }
        private void StopCamera()
        {
            if (_videoSource != null && _videoSource.IsRunning)
            {
                _videoSource.SignalToStop();
                _videoSource.NewFrame -= video_NewFrame;
            }
            Image = null;
        }
        public void Dispose()
        {
            if (_videoSource != null && _videoSource.IsRunning)
            {
                _videoSource.SignalToStop();
            }
        }
    }
}

