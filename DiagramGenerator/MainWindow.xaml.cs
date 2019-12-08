using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DiagramGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        int xDivisions = 0;
        int yDivisions = 0;
        double xIntervalValue = 0.0;
        double yIntervalValue = 0.0;


        public MainWindow() {
            InitializeComponent();
            inputPointX.IsEnabled = false;
            inputPointY.IsEnabled = false;
            addPoint.IsEnabled = false;
            //Debug();
        }

        private void Debug() {
            inputTitle.Text = "Test Diagram";
            inputXDivisions.Text = "5";
            inputYDivisions.Text = "20";
            inputXIntervalValue.Text = "20";
            inputYIntervalValue.Text = "5";
            CreateNewDiagram();
            AddNewPoint(0, 0);
            AddNewPoint(20, 20);
            AddNewPoint(10, 20);
            AddNewPoint(50, 50);
        }

        /// <summary>
        /// Create new diagram and transpose from X to Y
        /// </summary>
        /// <returns></returns>
        private bool CreateNewDiagram() {

            diagramCanvas.Children.Clear();

            AddTitle(inputTitle.Text);

            if (!int.TryParse(inputXDivisions.Text, out xDivisions)) {
                MessageBox.Show("X-axis divisions needs to be a number", "Error!");
                return false;
            }
            if (!double.TryParse(inputXIntervalValue.Text, out xIntervalValue)) {
                MessageBox.Show("X-axis interval needs to be a number", "Error!");
                return false;
            }
            if (Math.Min(xDivisions, xIntervalValue) <= 0) {
                MessageBox.Show("X-axis numbers cannot be less or equal to zero", "Error!");
                diagramCanvas.Children.Clear();
                return false;
            }
            CreateYAxis(xDivisions, xIntervalValue);

            if (!int.TryParse(inputYDivisions.Text, out yDivisions)) {
                MessageBox.Show("Y-axis divisions needs to be a number", "Error!");
                return false;
            }
            if (!double.TryParse(inputYIntervalValue.Text, out yIntervalValue)) {
                MessageBox.Show("Y-axis interval needs to be a number", "Error!");
                return false;
            }
            if (Math.Min(yDivisions, yIntervalValue) <= 0) {
                MessageBox.Show("Y-axis numbers cannot be less or equal to zero", "Error!");
                diagramCanvas.Children.Clear();
                return false;
            }
            CreateXAxis(yDivisions, yIntervalValue);

            return true;
        }

        /// <summary>
        /// Create new diagram
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateDiagram_Click(object sender, RoutedEventArgs e) {
            if (CreateNewDiagram()) {
                inputTitle.IsEnabled = false;
                inputXDivisions.IsEnabled = false;
                inputYDivisions.IsEnabled = false;
                inputXIntervalValue.IsEnabled = false;
                inputYIntervalValue.IsEnabled = false;
                createDiagram.IsEnabled = false;
                inputPointX.IsEnabled = true;
                inputPointY.IsEnabled = true;
                addPoint.IsEnabled = true;
            }

        }

        /// <summary>
        /// Hide node coordinates
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HideToolTip(object sender, MouseEventArgs e) {
            if (sender is Ellipse) {
                Ellipse ellipse = (Ellipse)sender;
                ToolTip toolTip = (ToolTip)ellipse.ToolTip;
                toolTip.IsOpen = false;
            }
        }

        /// <summary>
        /// Show node coordinates
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DisplayToolTip(object sender, MouseEventArgs e) {
            if (sender is Ellipse) {
                Ellipse ellipse = (Ellipse)sender;
                ToolTip toolTip = (ToolTip)ellipse.ToolTip;
                toolTip.IsOpen = true;
            }
        }

        /// <summary>
        /// Add point
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddPoint_Click(object sender, RoutedEventArgs e) {

            double xValue;
            double yValue;

            if (!double.TryParse(inputPointX.Text, out xValue)) {
                MessageBox.Show("X-value needs to be a number", "Error!");
                return;
            }
            if (!double.TryParse(inputPointY.Text, out yValue)) {
                MessageBox.Show("Y-value needs to be a number", "Error!");
                return;
            }

            if (xValue > xIntervalValue * xDivisions) {
                MessageBox.Show("X-value is too high", "Error!");
                return;
            }

            if (yValue > yIntervalValue * yDivisions) {
                MessageBox.Show("Y-value is too high", "Error!");
                return;
            }
            if (Math.Min(yValue, xValue) < 0) {
                MessageBox.Show("Y-axis numbers cannot be less than zero", "Error!");
                return;
            }
            AddNewPoint(xValue, yValue);
            listPoints.Items.Clear();
            foreach (Point point in points) {
                listPoints.Items.Add(point.ToString());
            }
        }

        /// <summary>
        /// Clear diagram
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearDiagram_Click(object sender, RoutedEventArgs e) {
            diagramCanvas.Children.Clear();
            inputTitle.IsEnabled = true;
            inputXDivisions.IsEnabled = true;
            inputYDivisions.IsEnabled = true;
            inputXIntervalValue.IsEnabled = true;
            inputYIntervalValue.IsEnabled = true;
            createDiagram.IsEnabled = true;
            inputPointX.IsEnabled = false;
            inputPointY.IsEnabled = false;
            addPoint.IsEnabled = false;
            points = new List<Point>();
            listPoints.Items.Clear();

        }

        private void menuItemClose_Click(object sender, RoutedEventArgs e) {
            Environment.Exit(0);
        }

        private void menuItemSortX_Click(object sender, RoutedEventArgs e) {
            sortX = true;
            points = points.OrderBy(o => o.X).ToList();
            ReDrawPoints();
        }

        private void menuItemSortY_Click(object sender, RoutedEventArgs e) {
            sortX = false;
            points = points.OrderBy(o => o.Y).ToList();
            ReDrawPoints();
        }
    }
}
