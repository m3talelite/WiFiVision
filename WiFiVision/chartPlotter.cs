using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        int chartBoxLineThickness = 3;
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

        private void drawBox()
        {
            drawLine(new Point(x, y), new Point(x + this.width, y), this.chartBoxLineThickness, this.chartboxLineColor);
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
    }
}
