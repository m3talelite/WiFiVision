using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
        public MainPage()
        {
            this.InitializeComponent();

            //drawWifiGraph();

            ChartPlotter cp = new ChartPlotter(myCanvas, 100, 100, 400, 400);
            
        }

        private void drawWifiGraph()
        {
            EllipseGeometry ellipse = new EllipseGeometry();
            Path circle = new Path();
            ellipse.Center = new Point(90, 300);
            ellipse.RadiusX = 30;
            ellipse.RadiusY = 90;
            circle.Data = ellipse;
            circle.Stroke = new SolidColorBrush(Colors.Blue);
            circle.StrokeThickness = 5;
            circle.Margin = new Thickness(10);

            myCanvas.Children.Add(circle);

            // Drawing a cruve

            LineGeometry line = new LineGeometry();
            Path linePath = new Path();
            line.StartPoint = new Point(200, 200);
            line.EndPoint = new Point(600, 200);
            linePath.Data = line;
            linePath.Stroke = new SolidColorBrush(Colors.Black);
            linePath.StrokeThickness = 1;

            myCanvas.Children.Add(linePath);

            //drawCurve(500,500, 400);
            //drawCurve(540, 500, 400);
            //drawCurve(550, 500, 700);
        }
    }
}
