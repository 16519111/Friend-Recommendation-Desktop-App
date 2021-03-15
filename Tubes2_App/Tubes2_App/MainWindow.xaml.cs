using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Msagl.WpfGraphControl;
using System.Windows.Controls.Primitives;
using Microsoft.Msagl.Core.Routing;
using Microsoft.Msagl.Drawing;
using Microsoft.Win32;
using Color = Microsoft.Msagl.Drawing.Color;
using ModifierKeys = System.Windows.Input.ModifierKeys;
using Size = System.Windows.Size;

namespace Tubes2_App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string[] lines;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void Browse_File_Button(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog fileDialog = new System.Windows.Forms.OpenFileDialog();
            fileDialog.DefaultExt = ".txt"; // Required file extension 
            fileDialog.Filter = "Text documents (.txt)|*.txt"; // Optional file extensions
            fileDialog.Multiselect = false;

            if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // Get directory file .txt yang dipilih
                string sFileName = fileDialog.FileName;

                // Baca line per line kemudian dimasukkan ke array of string lines
                lines = System.IO.File.ReadAllLines(@sFileName);

                // Memunculkan dialog untuk testing
                // System.Windows.Forms.MessageBox.Show(lines[0], "Error Title", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                // SetupToolbar();
                MakeGraph();
                System.Windows.Controls.Image myImage3 = new System.Windows.Controls.Image();
                string path = Environment.CurrentDirectory;
                BitmapImage bi3 = new BitmapImage();
                bi3.BeginInit();
                bi3.UriSource = new Uri(@path + "/graph.png", UriKind.Absolute);
                bi3.EndInit();
                myImage3.Stretch = Stretch.None;
                myImage3.Source = bi3;
                myImage3.Width = 200;
                myImage3.Height = 200;

                graphCanvas.Children.Add(myImage3);
            }
        }

        private void MakeGraph()
        {
            //create a viewer object 
            Microsoft.Msagl.GraphViewerGdi.GViewer viewer = new Microsoft.Msagl.GraphViewerGdi.GViewer();
            //create a graph object 
            Microsoft.Msagl.Drawing.Graph graph = new Microsoft.Msagl.Drawing.Graph("graph");
            //create the graph content
            for (int i = 1; i < lines.Length; i++)
            {
                var edge = graph.AddEdge(lines[i][0].ToString(), lines[i][2].ToString());
                edge.Attr.ArrowheadAtSource = ArrowStyle.None;
                edge.Attr.ArrowheadAtTarget = ArrowStyle.None;
            }
            
            // Creating Graph Image
            Microsoft.Msagl.GraphViewerGdi.GraphRenderer renderer = new Microsoft.Msagl.GraphViewerGdi.GraphRenderer(graph);
            renderer.CalculateLayout();
            int width = 100;
            Bitmap bitmap = new Bitmap(width, (int)(graph.Height *
            (width / graph.Width)), System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            renderer.Render(bitmap);
            bitmap.Save("graph.png");
        }
    }
}
