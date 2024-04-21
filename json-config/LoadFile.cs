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

        public JToken? GetPathValue(string Path)
        {
            string fileContent = File.ReadAllText(this._filePath);
            JObject jsonO = JObject.Parse(fileContent);
            return jsonO.SelectToken(Path);
        }

        public void ExtractNodePathsAndValues(JToken token, string currentPath, List<KeyValuePair<string, object>> result)
        {
            // Check the type of the current JToken
            if (token is JValue)
            {
                // If it's a value, add the current path and value to the result list
                result.Add(new KeyValuePair<string, object>(currentPath, token));
            }
            else if (token is JObject)
            {
                // If it's an object, iterate through its properties
                foreach (var property in token.Children<JProperty>())
                {
                    // Create the new path for the property
                    string newPath = string.IsNullOrEmpty(currentPath) ? property.Name : $"{currentPath}.{property.Name}";
                    // Recursively extract the node paths and values
                    ExtractNodePathsAndValues(property.Value, newPath, result);
                }
            }
            else if (token is JArray)
            {
                // If it's an array, iterate through its elements
                for (int i = 0; i < token.Count(); i++)
                {
                    // Create the new path for the array element
                    string newPath = $"{currentPath}[{i}]";
                    // Recursively extract the node paths and values
                    ExtractNodePathsAndValues(token[i], newPath, result);
                }
            }
        }
    }
}
