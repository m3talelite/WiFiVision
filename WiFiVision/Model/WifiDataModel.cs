using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.WiFi;
using Windows.UI.Xaml.Media.Imaging;

namespace WiFiVision.Model
{
    public class WifiDataModel
    {
        private WiFiAdapter adapter;
        public String Ssid { get; set; }
        public String Bssid { get; set; }
        public String ChannelCenterFrequency => string.Format("{0}kHz", availableNetwork.ChannelCenterFrequencyInKilohertz);
        public String Rssi => string.Format("{0}dBm", availableNetwork.NetworkRssiInDecibelMilliwatts);
        public String SecuritySettings => string.Format("Auth: {0}", availableNetwork.SecuritySettings.NetworkAuthenticationType, availableNetwork.SecuritySettings.NetworkEncryptionType);
        public String ConnectivityLevel { get; set; }
        public BitmapImage WiFiImage { get; set; }
        public String SignalStrength => string.Format("Bars: {0}", availableNetwork.SignalBars);

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
                }
            }
            ConnectivityLevel = connectionLevel;
        }

    }
}
