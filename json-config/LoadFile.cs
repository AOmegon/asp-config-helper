using Newtonsoft.Json.Linq;
using System.IO;
using System.Text.Json.Nodes;
using System.Windows.Controls;

namespace json_config
{
    public class LoadFile
    {
        private string _filePath { get; set; }
        public LoadFile(string FilePath) {
            this._filePath = FilePath;
        }

        public TreeViewItem GetTreeNode()
        {
            string fileContent = File.ReadAllText(this._filePath);
            JObject jsonO = JObject.Parse(fileContent);

            var rootNode = new TreeViewItem();

            PopulateTreeView(jsonO, rootNode);

            return rootNode;
        }

        private void PopulateTreeView(JObject jsonObject, TreeViewItem parentNode)
        {
            foreach (var property in jsonObject.Properties())
            {
                var childNode = new TreeViewItem();
                childNode.Header = property.Name;

                // Recursively populate child nodes
                if (property.Value.Type == JTokenType.Object)
                {
                    PopulateTreeView((JObject)property.Value, childNode);
                }
                else if (property.Value.Type == JTokenType.Array)
                {
                    foreach (var item in property.Value)
                    {
                        var itemNode = new TreeViewItem();
                        itemNode.Header = "Item";
                        PopulateTreeView((JObject)item, itemNode);
                        childNode.Items.Add(itemNode);
                    }
                }

                parentNode.Items.Add(childNode);
            }
        }
    }
}
