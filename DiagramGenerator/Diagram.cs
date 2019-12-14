using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DiagramGenerator {
    public partial class MainWindow {
        
        // Settings
        double textMargin = 10.0;
        double axisMargin = 50.0;
        double tickSize = 10.0;


        // Calculated
        double xScale = 0.0;
        double yScale = 0.0;
        List<Point> points = new List<Point>();


        #region Draw Axes and Title

        public void AddTitle(string title) {

            // Add text
            TextBlock textBlock = new TextBlock();
            textBlock.Text = title;
            textBlock.Foreground = Brushes.Black;
            textBlock.FontSize = 16;
            textBlock.Margin = new Thickness(axisMargin, textMargin, axisMargin, 0);
            textBlock.Width = diagramCanvas.Width - axisMargin * 2;
            textBlock.TextAlignment = TextAlignment.Center;
            textBlock.FontWeight = FontWeights.Bold;
            textBlock.TextWrapping = TextWrapping.Wrap;
            Canvas.SetZIndex(textBlock, 12);
            diagramCanvas.Children.Add(textBlock);


        }



        /// <summary>
        /// Create and Draw Y-axis
        /// </summary>
        /// <param name="diagramCanvas"></param>
        /// <param name="divisions"></param>
        /// <param name="intervalValue"></param>
        public void CreateYAxis(double divisions, double intervalValue) {

            // Calculate distance and scale
            double axisLength = diagramCanvas.Height - axisMargin * 2;
            double intervalLength = axisLength / divisions;
            yScale = intervalLength / intervalValue;

            // Add axis
            Line line = new Line();
            line.X1 = axisMargin;
            line.X2 = axisMargin;
            line.Y1 = axisMargin;
            line.Y2 = diagramCanvas.Height - axisMargin;
            line.Name = "Yaxis";
            line.Stroke = Brushes.Black;
            line.StrokeThickness = 2;
            Canvas.SetZIndex(line, 11);
            diagramCanvas.Children.Add(line);

            for (int i = 0; i <= divisions; i++) {
                double intervalDistance = diagramCanvas.Height - axisMargin - i * intervalLength;

                // Add ticks
                line = new Line();
                line.X1 = axisMargin - tickSize / 2;
                line.X2 = axisMargin + tickSize / 2;
                line.Y1 = intervalDistance;
                line.Y2 = intervalDistance;
                line.Name = string.Concat("Y", i);
                line.Stroke = Brushes.Black;
                Canvas.SetZIndex(line, 12);
                diagramCanvas.Children.Add(line);

                // Add text
                TextBlock textBlock = new TextBlock();
                textBlock.Text = (i * intervalValue).ToString();
                textBlock.Foreground = Brushes.Black;
                textBlock.Margin = new Thickness(textMargin, intervalDistance - 10, 0, 0);
                textBlock.Width = axisMargin - textMargin * 2;
                textBlock.TextAlignment = TextAlignment.Right;
                Canvas.SetZIndex(textBlock, 12);
                diagramCanvas.Children.Add(textBlock);

            }
        }



        /// <summary>
        /// Create and Draw X-axis
        /// </summary>
        /// <param name="diagramCanvas"></param>
        /// <param name="divisions"></param>
        /// <param name="intervalValue"></param>
        public void CreateXAxis(double divisions, double intervalValue) {

            // Calculate distance and scale
            double axisLength = diagramCanvas.Width - axisMargin * 2;
            double intervalLength = axisLength / divisions;
            xScale = intervalLength / intervalValue;

            // Add axis
            Line line = new Line();
            line.X1 = axisMargin;
            line.X2 = diagramCanvas.Width - axisMargin;
            line.Y1 = diagramCanvas.Height - axisMargin;
            line.Y2 = diagramCanvas.Height - axisMargin;
            line.Name = "Xaxis";
            line.Stroke = Brushes.Black;
            line.StrokeThickness = 2;
            Canvas.SetZIndex(line, 12);
            diagramCanvas.Children.Add(line);

            for (int i = 0; i <= divisions; i++) {
                double intervalDistance = axisMargin + i * intervalLength;

                // Add ticks
                line = new Line();
                line.X1 = intervalDistance;
                line.X2 = intervalDistance;
                line.Y1 = diagramCanvas.Height - axisMargin - tickSize / 2;
                line.Y2 = diagramCanvas.Height - axisMargin + tickSize / 2;
                line.Name = string.Concat("X", i);
                line.Stroke = Brushes.Black;
                //line.StrokeMiterLimit = 0;
                Canvas.SetZIndex(line, 12);
                diagramCanvas.Children.Add(line);

                // Add text
                TextBlock textBlock = new TextBlock();
                textBlock.Text = (i * intervalValue).ToString();
                textBlock.Foreground = Brushes.Black;
                textBlock.Margin = new Thickness(intervalDistance - textMargin, diagramCanvas.Height - axisMargin + textMargin, 0, 0);
                textBlock.Width = textMargin * 2;
                textBlock.TextAlignment = TextAlignment.Center;
                Canvas.SetZIndex(textBlock, 11);
                diagramCanvas.Children.Add(textBlock);

            }
        }

        #endregion

        #region Points and Polyline

        /// <summary>
        /// Add point to list
        /// </summary>
        /// <param name="diagramCanvas"></param>
        /// <param name="xValue"></param>
        /// <param name="yValue"></param>
        public void AddNewPoint(double xValue, double yValue) {

            // Check that there is sclae
            if ((xScale > 0) && (yScale > 0)) {

                // Create point and check if already exist
                Point point = new Point(xValue, yValue);

                if (!points.Contains(point)) {
                    points.Add(point);
                    ReDrawPoints();
                }
                return;
            }
        }

        /// <summary>
        /// Redraw graph after node has been added
        /// </summary>
        private void ReDrawPoints() {

            // Create list of items to remove
            List<UIElement> removeUIElement = new List<UIElement>();

            // Find items to remove (i.e. not axes etc.)
            foreach (UIElement uIElement in diagramCanvas.Children) {

                if (uIElement is Ellipse) {
                    Ellipse uIItem = (Ellipse)uIElement;
                    if (string.Compare(uIItem.Name.Substring(0, Math.Min(4, uIItem.Name.Length)), "Node") == 0) {
                        removeUIElement.Add(uIElement);
                    }
                }

                if (uIElement is Polyline) {
                    Polyline uIItem = (Polyline)uIElement;
                    if (string.Compare(uIItem.Name.Substring(0, Math.Min(8, uIItem.Name.Length)), "PolyLine") == 0) {
                        removeUIElement.Add(uIElement);
                        break;
                    }
                }
            }

            // Remove items
            foreach (UIElement uIElement in removeUIElement) {
                diagramCanvas.Children.Remove(uIElement);
            }

            // Set poly line
            Polyline polyline = new Polyline();

            // Add all nodes to canvas and to polyline
            for (int i = 0; i < points.Count; i++) {
                Point point = points[i];

                double canvasX = point.X * xScale + axisMargin;
                double canvasY = diagramCanvas.Height - (point.Y * yScale + axisMargin);
                polyline.Points.Add(new Point(canvasX, canvasY));

                Ellipse ellipse = new Ellipse();
                ellipse.Stroke = new SolidColorBrush(Colors.Green);
                ellipse.StrokeThickness = 0.5;
                ellipse.Height = 6;
                ellipse.Width = 6;
                ellipse.Margin = new Thickness(canvasX - 3, canvasY - 3, 0, 0);
                ellipse.Name = string.Concat("Node_", i);
                ellipse.Fill = Brushes.White;
                ToolTip toolTip = new ToolTip();
                toolTip.Content = string.Concat("Point (", point.X.ToString(), ",", point.Y.ToString(), ")");
                ellipse.ToolTip = toolTip;
                ellipse.MouseEnter += DisplayToolTip;
                ellipse.MouseLeave += HideToolTip;
                Canvas.SetZIndex(ellipse, 21);
                diagramCanvas.Children.Add(ellipse);

            }

            // Create polyline
            if (polyline.Points.Count > 1) {
                polyline.Stroke = new SolidColorBrush(Colors.Green);
                polyline.StrokeThickness = 1;
                polyline.Name = "PolyLine";
                Canvas.SetZIndex(polyline, 22);
                diagramCanvas.Children.Add(polyline);
            }


        }

        #endregion
    }
}
