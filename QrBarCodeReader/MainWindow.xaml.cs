using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WebEye.Controls.Wpf;
using ZXing;

namespace QrBarCodeReader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WebCameraId _webcamDevice = null;

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += _This_Loaded;
            this.Unloaded += _This_Unloaded;
            this.SizeChanged += _This_SizeChanged;
            this.CbWebCams.SelectionChanged += _CbWebCams_SelectionChanged;
            this.BtnRetake.Click += _BtnRetake_Click;
        }

        private async void _BtnRetake_Click(object sender, RoutedEventArgs e)
        {
            await _StartCapture(this.CbWebCams.SelectedValue.ToString());
        }

        private async void _CbWebCams_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_webcamDevice == null)
                return;

            await _StartCapture(this.CbWebCams.SelectedValue.ToString());
        }

        private async void _This_Loaded(object sender, RoutedEventArgs e)
        {
            await _StartCapture();
        }

        private void _This_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.WebEyeWebCameraControl.StopCapture();
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
            }
        }

        private void _This_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _ResizeCameraImgControl();
        }

        private void _ResizeCameraImgControl()
        {
            try
            {
                var bm = this.WebEyeWebCameraControl.GetCurrentImage();
                this.WebEyeWebCameraControl.Height = this.WebEyeWebCameraControl.ActualWidth * bm.Height / bm.Width;
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
            }
        }

        private DateTime _takeStartTime;

        private async Task _StartCapture(string webCamName = null)
        {
            this.TbResult.Text = "";
            this.ImgSnapshot.Source = null;
            _takeStartTime = DateTime.Now;

            try
            {
                this.WebEyeWebCameraControl.StopCapture();
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
            }

            var videoDeviceList = this.WebEyeWebCameraControl.GetVideoCaptureDevices();

            if (!videoDeviceList.Any())
                return;

            var webCamNameList = videoDeviceList.Select(d => d.Name);
            Trace.TraceInformation(@"webCamNameList : " + JsonConvert.SerializeObject(webCamNameList, Formatting.Indented));
            this.CbWebCams.ItemsSource = webCamNameList;

            if (string.IsNullOrWhiteSpace(webCamName))
            {
                this.CbWebCams.SelectedIndex = 0;
                _webcamDevice = videoDeviceList.First();
            }
            else
            {
                _webcamDevice = videoDeviceList.FirstOrDefault(d => d.Name == webCamName);
            }

            this.WebEyeWebCameraControl.StartCapture(_webcamDevice);
            _ResizeCameraImgControl();

            var barcodeReader = new BarcodeReader();

            while (true)
            {
                await TaskEx.Delay(1000);
                var bm = this.WebEyeWebCameraControl.GetCurrentImage();
                var barcodeResult = barcodeReader.Decode(bm);

                if (barcodeResult != null)
                {
                    var elapsedTime = DateTime.Now - _takeStartTime;
                    this.TbResult.Text = $"걸린시간 : {elapsedTime.TotalMilliseconds.ToString("0")}ms \n결과 : {barcodeResult.ToString()}";
                    System.Windows.Forms.SendKeys.SendWait(barcodeResult.ToString());

                    using (var ms = new MemoryStream())
                    {
                        bm.Save(ms, ImageFormat.Bmp);
                        this.ImgSnapshot.Source = BitmapFrame.Create(ms, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                    }

                    break;
                }
            }
        }

    }
}
