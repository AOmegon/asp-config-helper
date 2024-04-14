using Microsoft.Win32;
using System.CodeDom;
using System.IO;
using System.Windows;
using System.Windows.Controls;
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
        }

        private void loadJsonCfgFile()
        {
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
                // Construct the path of the clicked item
                string itemPath = GetItemPath(_clickedTreeViewItem);

                // Display the item path
                MessageBox.Show("Item Path: " + itemPath, "Item Path", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }
    }
}