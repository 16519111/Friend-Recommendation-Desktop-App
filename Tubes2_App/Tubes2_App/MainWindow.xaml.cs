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

namespace Tubes2_App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.DefaultExt = ".txt"; // Required file extension 
            fileDialog.Filter = "Text documents (.txt)|*.txt"; // Optional file extensions
            fileDialog.Multiselect = false;

            if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // Get directory file .txt yang dipilih
                string sFileName = fileDialog.FileName;

                // Baca line per line kemudian dimasukkan ke array of string lines
                string[] lines = System.IO.File.ReadAllLines(@sFileName);

                // Memunculkan dialog untuk testing
                System.Windows.Forms.MessageBox.Show(lines[0], "Error Title", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                // ------------------------ GRAPH TESTER --------------------------- //
                System.Windows.Forms.Form form = new System.Windows.Forms.Form();
                //create a viewer object 
                Microsoft.Msagl.GraphViewerGdi.GViewer viewer = new Microsoft.Msagl.GraphViewerGdi.GViewer();
                //create a graph object 
                Microsoft.Msagl.Drawing.Graph graph = new Microsoft.Msagl.Drawing.Graph("graph");
                //create the graph content
                for (int i=1;i<lines.Length;i++)
                {
                    graph.AddEdge(lines[i][0].ToString(), lines[i][2].ToString());
                }
                //graph.AddEdge("A", "B");
                //graph.AddEdge("B", "C");
                //graph.AddEdge("A", "C").Attr.Color = Microsoft.Msagl.Drawing.Color.Green;
                //graph.FindNode("A").Attr.FillColor = Microsoft.Msagl.Drawing.Color.Magenta;
                //graph.FindNode("B").Attr.FillColor = Microsoft.Msagl.Drawing.Color.MistyRose;
                //Microsoft.Msagl.Drawing.Node c = graph.FindNode("C");
                //c.Attr.FillColor = Microsoft.Msagl.Drawing.Color.PaleGreen;
                //c.Attr.Shape = Microsoft.Msagl.Drawing.Shape.Diamond;
                //bind the graph to the viewer 
                //viewer.Graph = graph;
                ////associate the viewer with the form 
                //form.SuspendLayout();
                //viewer.Dock = System.Windows.Forms.DockStyle.Fill;
                //this.graphBox.
                //form.Controls.Add(viewer);
                //form.ResumeLayout();
                ////show the form 
                //form.ShowDialog();
                Microsoft.Msagl.GraphViewerGdi.GraphRenderer renderer = new Microsoft.Msagl.GraphViewerGdi.GraphRenderer(graph);
                renderer.CalculateLayout();
                int width = 50;
                Bitmap bitmap = new Bitmap(width, (int)(graph.Height *
                (width / graph.Width)));
                renderer.Render(bitmap);
                bitmap.Save("test.png");
            }
            this.graphBox.Visibility = Visibility.Visible;
        }
    }
}
