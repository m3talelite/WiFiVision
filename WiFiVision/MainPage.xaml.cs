using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using WiFiVision.Model;
using Windows.Devices.WiFi;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WiFiVision
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private WiFiAdapter Adapter;
        public ObservableCollection<WifiDataModel> WifiCollection { get; private set; }

        ChartPlotter cp;
        ChartPlotter cpDashboard;

        Size size;

        public MainPage()
        {
            this.InitializeComponent();

            //get display bounds
            var bounds = ApplicationView.GetForCurrentView().VisibleBounds;
            var scaleFactor = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
            this.size = new Size(bounds.Width * scaleFactor, bounds.Height * scaleFactor);

            DashboardWifiListView.MaxHeight = size.Height * 0.3 * 0.6;

            WifiCollection = new ObservableCollection<WifiDataModel>();
            
            ScanForWifi();
        }

        private async void ScanForWifi()
        {
            var access = await WiFiAdapter.RequestAccessAsync();
            if (access != WiFiAccessStatus.Allowed)
            {
                System.Diagnostics.Debug.WriteLine("Access denied");
                return;
            }
            DataContext = this;
            var result = await Windows.Devices.Enumeration.DeviceInformation.FindAllAsync(WiFiAdapter.GetDeviceSelector());
            if (result.Count > 0)
            {
                Adapter = await WiFiAdapter.FromIdAsync(result[0].Id);
                var connected = await Adapter.NetworkAdapter.GetConnectedProfileAsync();
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("No WiFiAdapters detected");
                return;
            }
            await Adapter.ScanAsync();
            
            FillWifiCollection(Adapter.NetworkReport);
        }

        private async void FillWifiCollection(WiFiNetworkReport networkReport)
        {
            List<WifiDataModel> wifiCollection = new List<WifiDataModel>();
            List<WifiDataModel> currentWifiCollection = new List<WifiDataModel>();
            for(int i = 0; i < networkReport.AvailableNetworks.Count; ++i)
            {
                var networkModel = new WifiDataModel(networkReport.AvailableNetworks[i], Adapter);
                await networkModel.UpdateConnectivity();
                if (networkModel.isCurrentNetwork)
                    currentWifiCollection.Add(networkModel);
                else
                    wifiCollection.Add(networkModel);
            }
            currentWifiCollection.Sort(delegate (WifiDataModel x, WifiDataModel y)
            {
                if (x.AvailableNetwork.NetworkRssiInDecibelMilliwatts == y.AvailableNetwork.NetworkRssiInDecibelMilliwatts) return 0;
                else if (x.AvailableNetwork.NetworkRssiInDecibelMilliwatts > y.AvailableNetwork.NetworkRssiInDecibelMilliwatts) return -1;
                else if (x.AvailableNetwork.NetworkRssiInDecibelMilliwatts < y.AvailableNetwork.NetworkRssiInDecibelMilliwatts) return 1;
                else return x.AvailableNetwork.NetworkRssiInDecibelMilliwatts.CompareTo(y.AvailableNetwork.NetworkRssiInDecibelMilliwatts);
            });

            foreach (WifiDataModel o in wifiCollection)
            {
                currentWifiCollection.Add(o);
            }
            foreach (WifiDataModel o in currentWifiCollection) {
                WifiCollection.Add(o);
            }
            drawGraphs(WifiCollection);
        }

        private async void drawGraphs(ObservableCollection<WifiDataModel> WifiCollection)
        {
            var bounds = ApplicationView.GetForCurrentView().VisibleBounds;
            var scaleFactor = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
            var size = new Size(bounds.Width * scaleFactor, bounds.Height * scaleFactor);

            cp = new ChartPlotter(myCanvas, 20, 15, size.Width * 0.7, size.Height * 0.65, true);
            cp.draw(WifiCollection.ToList());

            cpDashboard = new ChartPlotter(myLittleCanvas, 20, 15, size.Width * 0.7, size.Height * 0.3 * 0.6, false);
            cpDashboard.draw(WifiCollection.ToList());
        }

        private void TextBlockHeader_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if(sender.GetType() == typeof(TextBlock))
            {
                var block = sender as TextBlock;
                if (block.Text.Equals("Available networks"))
                {
                    pivot.SelectedIndex = 2;
                }
                else if (block.Text.Equals("Network charts"))
                {
                    pivot.SelectedIndex = 1;
                }
            }
        }
    }
}
