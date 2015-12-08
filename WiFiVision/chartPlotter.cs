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

        int channelSpaces = 16;

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
            DrawLine();
            //drawCurve("2: -90", 2, -90);
            //drawCurve("4: -80", 4, -80);
            //drawCurve("6: -70", 6, -70);
            //drawCurve("8: -60", 8, -60);
            //drawCurve("10: -50", 10, -50);
            //drawCurve("12: -40", 12, -40);
            //drawCurve("3: -30", 3, -30);
            //drawCurve("7: -20", 7, -20);
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
                double lineY = y + this.height - ((amplitude + 100.0) / 80.0) * this.height;
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
            for (int i = 1; i < 14; i++)
            {
                double channelWidth = this.width / channelSpaces;

                double offset = 1 * channelWidth;

                //drawing of channel number numberline number
                Point startPoint = new Point(x + offset + (channelWidth) * i, y + this.height);
                Point endPoint = new Point(startPoint.X, startPoint.Y + 10);
                drawLine(startPoint, endPoint, 1, Colors.Black);

                //drawing of channel number
                Point channelNumberPoint = new Point(x + offset + (channelWidth) * i, y + this.height + 20);

                TextBlock textBlock = new TextBlock();
                textBlock.Text = i.ToString();
                double guestimatedLabelWidth = 8 * i.ToString().Length;
                Canvas.SetLeft(textBlock, channelNumberPoint.X - guestimatedLabelWidth / 2);
                Canvas.SetTop(textBlock, channelNumberPoint.Y - 10);

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

        private void DrawLine()
        {
            int amp = -90;
            double channelWidth = this.width / channelSpaces;
            double channelAmplitude = ((amp + 100) / 80) * this.height;

            Point[] points = new Point[3];
            points[0] = new Point(0, 20);
            points[1] = new Point(10, 0);
            points[2] = new Point(20, 20);

            int i;
            int count = (int)20;

            int xOffset = 500;
            for (i = -count; i < count - 1; i++)
            {
                double x1 = i;
                double y1 = (i - points[1].X) * (i - points[2].X) / (points[0].X - points[1].X) * (points[0].X - points[2].X) * points[0].Y +
                            (i - points[0].X) * (i - points[2].X) / (points[1].X - points[0].X) * (points[1].X - points[2].X) * points[1].Y +
                            (i - points[0].X) * (i - points[1].X) / (points[2].X - points[0].X) * (points[2].X - points[1].X) * points[2].Y;

                Point p1 = new Point(x1, y1);

                double x2 = i + 1;
                double y2 = (i + 1 - points[1].X) * (i + 1 - points[2].X) / (points[0].X - points[1].X) * (points[0].X - points[2].X) * points[0].Y +
                            (i + 1 - points[0].X) * (i + 1 - points[2].X) / (points[1].X - points[0].X) * (points[1].X - points[2].X) * points[1].Y +
                            (i + 1 - points[0].X) * (i + 1 - points[1].X) / (points[2].X - points[0].X) * (points[2].X - points[1].X) * points[2].Y;
                Point p2 = new Point(x2, y2);
                drawLine(p1, p2, 1, Colors.Blue);
            }
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

            double channelAmplitude = 1.12* ( ((amp + 100) / 80) * this.height );
            myBs.Point1 = new Point(x, y);
            Point curveTop = new Point(x + curveWidth / 2, y - channelAmplitude);
            //calculation of line heights on amplitude scale
            //                double lineY = y + this.height - ((amplitude + 100.0) / 80.0) * this.height;
            //P(t) = P0*t^2 + P1*2*t*(1-t) + P2*(1-t)^2 //http://stackoverflow.com/questions/6711707/draw-a-quadratic-b%C3%A9zier-curve-through-three-given-points
            double controlX = 2 * (x + curveWidth / 2) - x / 2 - (x + curveWidth) / 2;
            double controlY = 2 * (curveTop.Y) - y / 2 - y / 2;
            myBs.Point2 = new Point(controlX, controlY);
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
            Canvas.SetTop(textBlock, curveTop.Y);

            this.activeCanvas.Children.Add(textBlock);
        }
    }
}
