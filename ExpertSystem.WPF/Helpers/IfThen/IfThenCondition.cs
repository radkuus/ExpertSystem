using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpertSystem.WPF.ViewModels;

namespace ExpertSystem.WPF.Helpers.IfThen
{
    public class IfThenCondition
    {
        public string? SelectedColumn { get; set; }
        public string? SelectedOperator { get; set; }
        public string? SelectedValue { get; set; }
        public string? Type { get; set; }
        public string? SelectedClass { get; set; }
    }
}
