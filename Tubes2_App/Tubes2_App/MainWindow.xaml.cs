using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Msagl.Drawing;

namespace Tubes2_App
{
    public partial class MainWindow : Window
    {
        // Attributes
        string[] lines;
        SortedSet<string> uniqueAccounts = new SortedSet<string>();
        Bitmap graphBitmap;
        string currentAccount;
        string currentTargetFriend;
        int lastIndexCurrentAccount;
        int lastIndexCurrentTargetFriend;
        Dictionary<string, List<string>> adjacencyList;
        List<string> exploreRoute;
        TextBlock descriptionTextBlock;
        bool DFSSolution;
        string selectedRadio;

        // Constructor
        public MainWindow()
        {
            InitializeComponent();
            lastIndexCurrentAccount = -1;
            lastIndexCurrentTargetFriend = -1;
            exploreRoute = new List<string>();
            descriptionTextBlock = new TextBlock();
            selectedRadio = "";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            bool isExplorable;

            // Membersihkan canvas dan textBlock
            friendCanvas.Children.Clear();
            descriptionTextBlock.Inlines.Clear();

            Friend_Recommendation();

            if(selectedRadio == "")
            {
                System.Windows.Forms.MessageBox.Show("Harap memilih DFS atau BFS terlebih dahulu"
                    , "Error Title", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else { 
                if(selectedRadio == "DFS")
                {
                    isExplorable = DFS_Explore();
                }
                else
                {
                    isExplorable = BFS_Explore();
                }

                if (isExplorable)
                {
                    descriptionTextBlock.Inlines.Add(new Bold(new Run("\n\nNama akun : " + currentAccount + " dan " + currentTargetFriend)));
                    descriptionTextBlock.Inlines.Add(new Run("\n" + (exploreRoute.Count-2).ToString() + " degree connection"));
                    for (int i=0;i<exploreRoute.Count;i++)
                    {
                        if (i == 0)
                        {
                            descriptionTextBlock.Inlines.Add(new Run("\n" + exploreRoute[i]));
                        }
                        else
                        {
                            descriptionTextBlock.Inlines.Add(new Run(" -> " + exploreRoute[i]));
                        }
                    }
                }
                else
                {
                    descriptionTextBlock.Inlines.Add(new Bold(new Run("\n\nNama akun : " + currentAccount + " dan " + currentTargetFriend)));
                    descriptionTextBlock.Inlines.Add(new Run("\nTidak ada jalur koneksi yang tersedia"));
                    descriptionTextBlock.Inlines.Add(new Run("\nAnda harus memulai koneksi baru itu sendiri"));
                }

                // Merender ulang komponen XAML friendCanvas
                friendCanvas.Children.Add(descriptionTextBlock);
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            selectedRadio = "DFS";
        }

        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            selectedRadio = "BFS";
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
                System.Windows.Controls.Image myImage3 = new System.Windows.Controls.Image();
                myImage3.Source = null;

                MakeGraph();
                
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

                generateAdjacencyList();

                // handler untuk event ChangeComboBox
                handleUpdateComboBox();
            }
        }

        private void generateAdjacencyList()
        {
            int countLines = lines.Length;
            adjacencyList = new Dictionary<string, List<string>>();
            for (int i=1;i<countLines;i++)
            {
                string source = lines[i][0].ToString();
                string dest = lines[i][2].ToString();

                if (!adjacencyList.ContainsKey(source))
                {
                    adjacencyList[source] = new List<string>();
                }
                if (!adjacencyList.ContainsKey(dest))
                {
                    adjacencyList[dest] = new List<string>();
                }
                adjacencyList[source].Add(dest);
                adjacencyList[dest].Add(source);
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
                src.Attr.Shape = Shape.Circle;
                target.Attr.Shape = Shape.Circle;

                // Menambah akun unik ke uniqueAccounts
                uniqueAccounts.Add(source);
                uniqueAccounts.Add(dest);
            }

            // Create Graph Image
            Microsoft.Msagl.GraphViewerGdi.GraphRenderer renderer = new Microsoft.Msagl.GraphViewerGdi.GraphRenderer(graph);
            renderer.CalculateLayout();
            int width = 120;
            graphBitmap = new Bitmap(width, (int)(graph.Height *
            (width / graph.Width)), System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            renderer.Render(graphBitmap);

            Bitmap cloneBitmap = (Bitmap)graphBitmap.Clone();

            string outputFileName = "graph.png";

            cloneBitmap.Save(outputFileName);
            cloneBitmap.Dispose();
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
            // Logic untuk handleSelectionChange (event) Choose_Account
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
            // Logic untuk handleSelectionChange (event) Explore
            if (lastIndexCurrentAccount >= 0)
            {
                Choose_Account_ComboBox.Items.Insert(lastIndexCurrentAccount, currentTargetFriend);
            }
            currentTargetFriend = Explore_ComboBox.SelectedItem.ToString();
            lastIndexCurrentAccount = Choose_Account_ComboBox.Items.IndexOf(currentTargetFriend);
            Choose_Account_ComboBox.Items.Remove(currentTargetFriend);
        }

        private void Friend_Recommendation()
        {
            // Mencetak judul
            descriptionTextBlock.Inlines.Add(new Bold(new Run("Friend Recommendations for " + currentAccount)));

            HashSet<string> uniqueFriendRecommendations = new HashSet<string>();
            Dictionary<string, List<string>> mutualConnections = new Dictionary<string, List<string>>();

            // Pencarian Friend Recommendation berdasarkan mutual friends
            var currentNode = adjacencyList[currentAccount];
            for (int m = 0; m < currentNode.Count; m++)
            {
                var currentMutualNode = adjacencyList[currentNode[m]];
                for (int n = 0; n < currentMutualNode.Count; n++)
                {
                    var candidateFriend = currentMutualNode[n];
                    if(!mutualConnections.ContainsKey(candidateFriend))
                    {
                        List<string> temp = new List<string>();
                        mutualConnections[candidateFriend] = temp;
                    }
                    if (candidateFriend != currentAccount && !currentNode.Contains(candidateFriend))
                    {
                        uniqueFriendRecommendations.Add(candidateFriend);
                        if (!mutualConnections[candidateFriend].Contains(currentNode[m]))
                        {
                            mutualConnections[candidateFriend].Add(currentNode[m]);
                        }
                    }
                }
            }

            // Mencetak Friend Recommendation ke layar
            foreach (string friendRecommendation in uniqueFriendRecommendations)
            {
                descriptionTextBlock.Inlines.Add(new Run("\n\nNama akun : " + friendRecommendation));
                descriptionTextBlock.Inlines.Add(new Run("\n" + mutualConnections[friendRecommendation].Count + " mutual friends :"));
                for (int k = 0; k < mutualConnections[friendRecommendation].Count; k++)
                {
                    descriptionTextBlock.Inlines.Add(new Run("\n" + mutualConnections[friendRecommendation][k]));
                }
            }
        }

        private bool BFS_Explore()
        {
            // Inisiasi variabel
            Queue<string> BFSQueue = new Queue<string>();
            Dictionary<string, bool> visited = new Dictionary<string, bool>();
            bool solutionFound = false;
            List<string> expandNode;
            Dictionary<string, List<string>> Route = new Dictionary<string, List<string>>();
            string expandAccount;

            foreach (string node in uniqueAccounts)
            {
                visited[node] = false;
            }

            // BFS secara iteratif
            expandAccount = currentAccount;
            visited[expandAccount] = true;
            while (!solutionFound)
            {
                expandNode = adjacencyList[expandAccount];
                for (int i=0;i<expandNode.Count;i++)
                {
                    string currentFocusAccount = expandNode[i];
                    if (!visited[currentFocusAccount])
                    {
                        visited[currentFocusAccount] = true;
                        Route[currentFocusAccount] = new List<string>();
                        if (Route.ContainsKey(expandAccount))
                        {
                            Route[currentFocusAccount] = Route[currentFocusAccount].Concat(Route[expandAccount]).ToList();
                        }
                        Route[currentFocusAccount].Add(expandAccount);
                        BFSQueue.Enqueue(currentFocusAccount);
                        if (currentFocusAccount == currentTargetFriend)
                        {
                            Route[currentFocusAccount].Add(currentFocusAccount);
                            exploreRoute = Route[currentFocusAccount];
                            solutionFound = true;
                            break;
                        }
                    }
                }
                if(BFSQueue.Count != 0)
                {
                    expandAccount = BFSQueue.Dequeue();
                }
                else
                {
                    break;
                }
            }
            return solutionFound;
        }

        private bool DFS_Explore()
        {
            // Main untuk pencarian dengan DFS. Menginisiasi variabel dan memanggil
            // DFS_Recursion (DFS secara rekursif).
            Dictionary<string, bool> visited = new Dictionary<string, bool>();
            List<string> Route = new List<string>();
            DFSSolution = false;
            int num_of_visited;

            foreach (string node in uniqueAccounts)
            {
                visited[node] = false;
            }

            num_of_visited = visited.Count;

            DFS_Recursion(currentAccount, visited, num_of_visited, Route);
            return DFSSolution;
        }

        private void DFS_Recursion(string currentFocusAccount, Dictionary<string, bool> visited, int num_of_visited, List<string> Route)
        {
            if (currentFocusAccount == currentTargetFriend)
            {
                // Basis apabila telah ditemukan friend yang hendak di-explore.
                DFSSolution = true;
                Route.Add(currentTargetFriend);
                exploreRoute = Route;
            }
            else if (num_of_visited < 1 && currentFocusAccount != currentTargetFriend && !DFSSolution)
            {
                // Basis terminasi karena tidak ditemukan path menuju target.
            }
            else
            {
                // Rekurens
                Route.Add(currentFocusAccount);
                visited[currentFocusAccount] = true;
                num_of_visited--;
                List<string> expandNode = adjacencyList[currentFocusAccount];
                int i = 0;

                while (i<expandNode.Count && !DFSSolution)
                {
                    if(!visited[expandNode[i]])
                    {
                        DFS_Recursion(expandNode[i], visited, num_of_visited, Route);
                    }
                    i++;
                }
                if(!DFSSolution)
                {
                    Route.Remove(currentFocusAccount);
                }
            }
        }
    }
}
