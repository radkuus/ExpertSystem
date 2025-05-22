using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpertSystem.WPF.Core;

namespace ExpertSystem.WPF.Helpers.Sample
{
    public class SampleEntry :ObservableObject
    {
        public string ColumnName { get; set; }

        private double? _value;
        public double? Value
        {
            get => _value;
            set
            {
                _value = value;
                OnPropertyChanged(nameof(Value));
            }
        }
    }
}
