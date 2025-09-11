using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertSystem.WPF.Helpers.IfThen
{
    public static class IfThenOperators
    {
        public static ObservableCollection<string> Operators { get; } = new ObservableCollection<string> { ">", "=", "<" };
    }
}
