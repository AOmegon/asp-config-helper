using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace asp_config_helper.Model
{
    public class NodeUIModel : INotifyPropertyChanged
    {
        public string NodePath { get; set; }
        public string Label { 
            get{
                return NodePath.Replace('.', '\\');
            } 
        }
        public string? Value { get; set; }
        public string Input
        {
            get => Value;
            set
            {
                if (Value != value)
                {
                    Value = value;
                    OnPropertyChanged(nameof(Input));
                }
            }
        }

        // Implement the INotifyPropertyChanged interface
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
