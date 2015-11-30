using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiFiVision.Model;
using Windows.Devices.WiFi;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace WiFiVision
{
    class ChartPlotter
    {
        Canvas activeCanvas;

        double x, y, width, height;

        int chartBoxLineThickness = 2;
        Color chartboxLineColor = Colors.Black;

        public ChartPlotter(Canvas activeCanvas, double x, double y, double width, double height)
        {
            this.activeCanvas = activeCanvas;
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;

            drawBox();
        }

        public void draw(List<WifiDataModel> networks)
        {
            foreach (WifiDataModel network in networks)
            {
                drawCurve(network.Ssid, network.getChannelId(), network.AvailableNetwork.NetworkRssiInDecibelMilliwatts);
            }
        }

        private void drawBox()
        {
            int lt = this.chartBoxLineThickness;
            Color color = this.chartboxLineColor;

            // top line of the box
            drawLine(new Point(x, y), new Point(x + this.width, y), lt, color);
            // and we folow the box drawing lines clockwise
            drawLine(new Point(x + this.width, y), new Point(x + this.width, y + this.height), lt, color);
            drawLine(new Point(x + this.width, y + this.height), new Point(x, y + this.height), lt, color);
            drawLine(new Point(x, y + this.height), new Point(x, y), lt, color);
        }

        private void drawLine(Point point1, Point point2, int lineThickness, Color strokeColor)
        {
            LineGeometry line = new LineGeometry();
            Path linePath = new Path();
            line.StartPoint = point1;
            line.EndPoint = point2;
            linePath.Data = line;
            linePath.Stroke = new SolidColorBrush(strokeColor);
            linePath.StrokeThickness = lineThickness;

            activeCanvas.Children.Add(linePath);
        }

        private void drawCurve(String name, int channel, double amp)
        {
            if (-1 == channel) //don't draw networks that are not on the band
                return;

            double curveWidth = 100;
            double channelWidth = curveWidth / 3;

            double y = this.y + this.height;
            double x = this.x;

            x += channel * channelWidth;

            System.Diagnostics.Debug.WriteLine("drawingCurve with CHannel:" + channel + "on x: " + x);

            Path path = new Path();
            PathFigure figure = new PathFigure();
            BezierSegment myBs = new BezierSegment();

            myBs.Point1 = new Point(x, y);
            myBs.Point2 = new Point(x + curveWidth / 2, y + amp*10);
            myBs.Point3 = new Point(x + curveWidth, y);

            figure.Segments.Add(myBs);
            figure.StartPoint = new Point(x, y); //WUT?!

            PathGeometry myPath = new PathGeometry();
            myPath.Figures.Add(figure);
            path.Data = myPath;
            path.Stroke = new SolidColorBrush(Colors.Red);
            path.StrokeThickness = 2;

            this.activeCanvas.Children.Add(path);
        }
    }
}
