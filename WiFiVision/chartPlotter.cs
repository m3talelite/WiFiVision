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

        int channelSpaces = 15;

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
                drawCurve(network.Ssid, network.getChannel(), network.AvailableNetwork.NetworkRssiInDecibelMilliwatts);
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

            for (int i = 30; i < 100; i += 10)
            {
                //draw amplines
                double amplitude = -i;
                double lineY = y + ((amplitude + 100.0) / 80.0) * this.height;
                Point startPoint = new Point(x, lineY);
                Point endPoint = new Point(x + width, lineY);
                drawLine(startPoint,endPoint, 1, Colors.Gray);

                TextBlock textBlock = new TextBlock();
                textBlock.Text = amplitude.ToString();
                Canvas.SetLeft(textBlock, startPoint.X - 30);
                Canvas.SetTop(textBlock, startPoint.Y - 10);

                this.activeCanvas.Children.Add(textBlock);
            }

            //draw channel numbers
            for (int i = -1; i < 14; i++)
            {
                double channelWidth = this.width / channelSpaces;

                double offset = 2 * channelWidth;
                Point startPoint = new Point(x + offset + (channelWidth) * i, y + this.height + 20);

                TextBlock textBlock = new TextBlock();
                textBlock.Text = i.ToString();
                Canvas.SetLeft(textBlock, startPoint.X - 30);
                Canvas.SetTop(textBlock, startPoint.Y - 10);
                this.activeCanvas.Children.Add(textBlock);
            }
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

            double channelWidth = this.width / channelSpaces;
            double curveWidth = channelWidth * 4;

            double y = this.y + this.height;
            double x = this.x;

            x += (channel -1) * channelWidth;

            System.Diagnostics.Debug.WriteLine("drawingCurve with CHannel:" + channel + " on x: " + x);
            System.Diagnostics.Debug.WriteLine("With Amplitude: " + amp);

            Path path = new Path();
            PathFigure figure = new PathFigure();
            BezierSegment myBs = new BezierSegment();

            double channelAmplitude = ((amp - 100) / 80) * this.height;
            myBs.Point1 = new Point(x, y);
            Point curveTop = new Point(x + curveWidth / 2, y + channelAmplitude);
            myBs.Point2 = curveTop;
            myBs.Point3 = new Point(x + curveWidth, y);

            figure.Segments.Add(myBs);
            figure.StartPoint = new Point(x, y); //WUT?!

            PathGeometry myPath = new PathGeometry();
            myPath.Figures.Add(figure);
            path.Data = myPath;
            path.Stroke = new SolidColorBrush(Colors.Red);
            path.StrokeThickness = 2;

            this.activeCanvas.Children.Add(path);

            TextBlock textBlock = new TextBlock();
            if (null != name)
            {
                textBlock.Text = name;
                    }
            else
            {
                textBlock.Text = "NULL";
                    }

            Canvas.SetLeft(textBlock, curveTop.X);
            Canvas.SetTop(textBlock, y + curveTop.Y);

            this.activeCanvas.Children.Add(textBlock);
        }
    }
}
