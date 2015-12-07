using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WiFiVision
{
    public sealed partial class PivotHeader : UserControl
    {
        public string TabText {
            get { return _TB.Text; }
            set { _TB.Text = value; }
        }

        public Symbol TabSymbol
        {
            get { return _SI.Symbol; }
            set { _SI.Symbol = value; }
        }

        public PivotHeader()
        {
            this.InitializeComponent();
        }
    }
}
