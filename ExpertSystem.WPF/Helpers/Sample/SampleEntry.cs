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

        private string? _value;
        public string? Value
        {
            get => _value;
            set
            {
                _value = value;
                OnPropertyChanged(nameof(Value));
                OnValueChanged?.Invoke();
            }
        }
        public Action? OnValueChanged { get; set; }
    }
}
