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
    public partial class MainWindow : Window
    {
        // Attributes
        string[] lines;
        HashSet<string> uniqueAccounts = new HashSet<string>();
        string currentAccount;
        string currentTargetFriend;
        int lastIndexCurrentAccount = -1;
        int lastIndexCurrentTargetFriend = -1;

        // Constructor
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TextBlock newFriendTextBlock = new TextBlock();
            newFriendTextBlock.Inlines.Add(new Bold(new Run("Friend Recommendations for " + currentAccount)));

            friendCanvas.Children.Clear();
            friendCanvas.Children.Add(newFriendTextBlock);
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

                handleUpdateComboBox();
            }
        }

        private void MakeGraph()
        {
            // Create Graph Object 
            Graph graph = new Graph("graph");

            // Create Graph Content
            for (int i = 1; i < lines.Length; i++)
            {
                string source = lines[i][0].ToString();
                string dest = lines[i][2].ToString();

                // Styling Graph
                var edge = graph.AddEdge(source, dest);
                edge.Attr.ArrowheadAtSource = ArrowStyle.None;
                edge.Attr.ArrowheadAtTarget = ArrowStyle.None;
                Node src = graph.FindNode(source);
                Node target = graph.FindNode(dest);
                src.Attr.Shape = Microsoft.Msagl.Drawing.Shape.Circle;
                target.Attr.Shape = Microsoft.Msagl.Drawing.Shape.Circle;

                // Menambah akun unik ke uniqueAccounts
                uniqueAccounts.Add(source);
                uniqueAccounts.Add(dest);
            }
            
            // Create Graph Image
            Microsoft.Msagl.GraphViewerGdi.GraphRenderer renderer = new Microsoft.Msagl.GraphViewerGdi.GraphRenderer(graph);
            renderer.CalculateLayout();
            int width = 120;
            Bitmap bitmap = new Bitmap(width, (int)(graph.Height *
            (width / graph.Width)), System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            renderer.Render(bitmap);
            bitmap.Save("graph.png");
        }

        private void handleUpdateComboBox()
        {
            foreach (string account in uniqueAccounts)
            {
                Choose_Account_ComboBox.Items.Add(account);
                Explore_ComboBox.Items.Add(account);
            }
        }

        private void Choose_Account_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(lastIndexCurrentTargetFriend >= 0)
            {
                Explore_ComboBox.Items.Insert(lastIndexCurrentTargetFriend, currentAccount);  
            }
            currentAccount = Choose_Account_ComboBox.SelectedItem.ToString();
            lastIndexCurrentTargetFriend = Explore_ComboBox.Items.IndexOf(currentAccount);
            Explore_ComboBox.Items.Remove(currentAccount);
        }

        private void Explore_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lastIndexCurrentAccount >= 0)
            {
                Choose_Account_ComboBox.Items.Insert(lastIndexCurrentAccount, currentTargetFriend);
            }
            currentTargetFriend = Explore_ComboBox.SelectedItem.ToString();
            lastIndexCurrentAccount = Choose_Account_ComboBox.Items.IndexOf(currentTargetFriend);
            Choose_Account_ComboBox.Items.Remove(currentTargetFriend);
        }
    }
}
