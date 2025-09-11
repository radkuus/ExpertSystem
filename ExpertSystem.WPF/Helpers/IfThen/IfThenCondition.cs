using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpertSystem.WPF.Core;
using ExpertSystem.WPF.ViewModels;

namespace ExpertSystem.WPF.Helpers.IfThen
{
    public class IfThenCondition :ObservableObject
    {
        private string? _selectedColumn;
        public string? SelectedColumn
        {
            get => _selectedColumn;
            set
            {
                _selectedColumn = value;
                OnPropertyChanged(nameof(SelectedColumn));
            }
        }
        private string? _selectedOperator;
        public string? SelectedOperator
        {
            get => _selectedOperator;
            set
            {
                _selectedOperator = value;
                OnPropertyChanged(nameof(SelectedOperator));
            }
        }
        private string? _selectedValue;
        public string? SelectedValue
        {
            get => _selectedValue;
            set
            {
                _selectedValue = value;
                OnPropertyChanged(nameof(SelectedValue));
            }
        }
        private string? _selectedType;
        public string? SelectedType
        {
            get => _selectedType;
            set
            {
                _selectedType = value;
                OnPropertyChanged(nameof(SelectedType));
            }
        }
        private string? _selectedClass;
        public string? SelectedClass
        {
            get => _selectedClass;
            set
            {
                _selectedClass = value;
                OnPropertyChanged(nameof(SelectedClass));
            }
        }
    }
}
