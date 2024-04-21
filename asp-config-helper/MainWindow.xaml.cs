using asp_config_helper.Model;
using Microsoft.Win32;
using System.CodeDom;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace asp_config_helper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _cfgFullPath = string.Empty;
        public MainWindow()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.Height = SystemParameters.PrimaryScreenHeight * 0.5;
            this.Width = SystemParameters.PrimaryScreenWidth * 0.5;
            this.Title = "ASP Config";
        }

        private void OpenCfgFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Web.config (*.config)|*.config|Appsettings.json(*.json)|*.json"; 
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); 


            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                // need test if file type match config file
                this._cfgFullPath = openFileDialog.FileName;
                LoadFile(this._cfgFullPath);
            }
        }

        private void LoadFile(string FullPath)
        {
            string fileExt = Path.GetExtension(FullPath).ToLower();
            switch (fileExt) {
                case ".json":
                    this.loadJsonCfgFile();
                    break;
                default:
                    MessageBox.Show("The selected file is not a configuration file", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.ResetData();
                    break;
            }
        }

        private void ResetData()
        {
            this._cfgFullPath = string.Empty;
            CfgTree.Items.Clear();
            ShowJson.Text = null;
            NodesInputs.Children.Clear();
        }

        private void loadJsonCfgFile()
        {
            CfgTree.Items.Clear();
            ShowJson.Text = null;
            NodesInputs.Children.Clear();

            var manager = new json_config.LoadFile(this._cfgFullPath);
            var data = manager.GetTreeNode();
            data.Header = Path.GetFileName(this._cfgFullPath);
            CfgTree.Items.Add(data);
        }

        private void CfgTree_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var selected = FindAncestor<TreeViewItem>((DependencyObject)e.OriginalSource);
            this._clickedTreeViewItem = selected != null ? (TreeViewItem)selected : null;
        }
        private TreeViewItem _clickedTreeViewItem { get; set; }
        private object FindAncestor<TreeViewItem>(DependencyObject current)
        {
            while (current != null)
            {
                if (current is TreeViewItem treeViewItem)
                {
                    return treeViewItem;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            return null;
        }
        private string GetItemPath(TreeViewItem treeViewItem)
        {
            string itemPath = treeViewItem.Header.ToString();
            DependencyObject parent = VisualTreeHelper.GetParent(treeViewItem);

            while (parent != null)
            {
                if (parent is TreeViewItem)
                {
                    TreeViewItem parentTreeViewItem = parent as TreeViewItem;
                    itemPath = parentTreeViewItem.Header.ToString() + "\\" + itemPath;
                }
                parent = VisualTreeHelper.GetParent(parent);
            }
            return itemPath;
        }

        private void ContextMenu_Tree_GetPath(object sender, RoutedEventArgs e)
        {
            if (_clickedTreeViewItem != null)
            {
                string itemPath = GetItemPath(_clickedTreeViewItem);

                MessageBox.Show("Item Path: " + itemPath, "Item Path", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }


        private void ContextMenu_Tree_ShowPathValue(object sender, RoutedEventArgs e)
        {
            if (_clickedTreeViewItem != null)
            {
                string itemPath = GetItemPath(_clickedTreeViewItem);

                var itemPathArr = itemPath.Split("\\");

                var list = itemPathArr.ToList();
                list.RemoveAt(0);

                var path = string.Join(".", list);

                var manager = new json_config.LoadFile(this._cfgFullPath);
                var data = manager.GetPathValue(path);

                MessageBox.Show("Item Path: " + data, "Item Path", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }

        private void CfgTree_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var selected = FindAncestor<TreeViewItem>((DependencyObject)e.OriginalSource);
            this._clickedTreeViewItem = selected != null ? (TreeViewItem)selected : null;
            NodesInputs.Children.Clear();

            if (_clickedTreeViewItem != null)
            {
                string itemPath = GetItemPath(_clickedTreeViewItem);

                var itemPathArr = itemPath.Split("\\");

                var list = itemPathArr.ToList();
                list.RemoveAt(0);

                var path = string.Join(".", list);

                var manager = new json_config.LoadFile(this._cfgFullPath);
                var data = manager.GetPathValue(path);
                ShowJson.Text = data.ToString();

                var nodePathsAndValues = new List<KeyValuePair<string, object>>();

                manager.ExtractNodePathsAndValues(data, string.Empty, nodePathsAndValues);

                var nodeUi = nodePathsAndValues.Select( node =>
                        new NodeUIModel
                        {
                            NodePath = node.Key,
                            Value = node.Value != null ? node.Value.ToString() : string.Empty
                        }
                    );


                foreach (var item in nodeUi)
                {
                    StackPanel itemPanel = new StackPanel
                    {
                        Orientation = Orientation.Vertical,
                        Margin = new Thickness(0, 5, 0, 5),
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                    };

                    Label label = new Label
                    {
                        Content = $"{item.Label} :",
                        HorizontalAlignment = HorizontalAlignment.Left,
                    };

                    TextBox textBox = new TextBox
                    {
                        HorizontalAlignment = HorizontalAlignment.Stretch
                    };

                    textBox.SetBinding(TextBox.TextProperty, new Binding("Input")
                    {
                        Source = item,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                    });

                    itemPanel.Children.Add(label);
                    itemPanel.Children.Add(textBox);

                    NodesInputs.Children.Add(itemPanel);

                }

            }

            
        }
    }
}