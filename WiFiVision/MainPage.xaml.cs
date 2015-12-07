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
using Windows.UI;
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

        public MainPage()
        {
            this.InitializeComponent();

            WifiCollection = new ObservableCollection<WifiDataModel>();

            ScanForWifi();

            cp = new ChartPlotter(myCanvas, 100, 100, 300, 300);
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
            WifiCollection.Clear();
            for(int i = 0; i < networkReport.AvailableNetworks.Count; ++i)
            {
                var networkModel = new WifiDataModel(networkReport.AvailableNetworks[i], Adapter);
                await networkModel.UpdateConnectivity();
                WifiCollection.Add(networkModel);
            }

            cp.draw(WifiCollection.ToList());
        }
    }
}
