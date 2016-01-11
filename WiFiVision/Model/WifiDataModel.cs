using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.WiFi;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace WiFiVision.Model
{
    public class WifiDataModel
    {
        private WiFiAdapter adapter;
        public String Ssid => string.Format("{0} ", availableNetwork.Ssid);
        public String Bssid => string.Format("{0} ", availableNetwork.Bssid);
        public String ChannelCenterFrequency => string.Format("{0}kHz ", availableNetwork.ChannelCenterFrequencyInKilohertz);
        public String Rssi => string.Format("Strength: {0} dBm ", availableNetwork.NetworkRssiInDecibelMilliwatts);
        public String SecuritySettings => string.Format("Auth: {0}", availableNetwork.SecuritySettings.NetworkAuthenticationType, availableNetwork.SecuritySettings.NetworkEncryptionType);
        public String ConnectivityLevel { get; set; }
        public BitmapImage WiFiImage { get; set; }
        public String WifiImagePath { get; set; }
        public String SignalStrength => string.Format("{0} bars ", availableNetwork.SignalBars);
        public String WifiChannel => string.Format("Channel: {0} ", getChannel());
        public Symbol BarsInSymbol
        {
            get {
                switch (availableNetwork.SignalBars)
                {
                    case 0: return Symbol.ZeroBars;
                    case 1: return Symbol.OneBar;
                    case 2: return Symbol.TwoBars;
                    case 3: return Symbol.ThreeBars;
                    case 4: return Symbol.FourBars;
                    default: return (availableNetwork.SignalBars > 4) ? Symbol.FourBars : Symbol.ZeroBars;
                }
            }
            set { BarsInSymbol = value; }
        }

        public bool isCurrentNetwork = false;

        private WiFiAvailableNetwork availableNetwork;
        public WiFiAvailableNetwork AvailableNetwork
        { get { return availableNetwork; }
            private set { availableNetwork = value; }
        }

        public int getChannel()
        {
            int freq = Int32.Parse(ChannelCenterFrequency.Substring(0, 4));

            if (freq >= 2412 && freq <= 2484)
            {
                return (freq - 2412) / 5 + 1;
            }
            else if (freq >= 5170 && freq <= 5825)
            {
                return (freq - 5170) / 5 + 34;
            }
            else
            {
                return -1;
            }
    }

        public WifiDataModel(WiFiAvailableNetwork avNetwork, WiFiAdapter adapter)
        {
            AvailableNetwork = avNetwork;
            this.adapter = adapter;
        }

        public async Task UpdateConnectivity()
        {
            string connectionLevel = "Not connected";
            string connectedSsid = null;

            var connectedProfile = await adapter.NetworkAdapter.GetConnectedProfileAsync();
            if (connectedProfile != null &&
                connectedProfile.IsWlanConnectionProfile &&
                connectedProfile.WlanConnectionProfileDetails != null)
            {
                connectedSsid = connectedProfile.WlanConnectionProfileDetails.GetConnectedSsid();
            }
            if (!string.IsNullOrEmpty(connectedSsid))
            {
                if (connectedSsid.Equals(AvailableNetwork.Ssid))
                {
                    connectionLevel = connectedProfile.GetNetworkConnectivityLevel().ToString();
                    isCurrentNetwork = true;
                }
            }
            ConnectivityLevel = connectionLevel;
        }

    }
}
