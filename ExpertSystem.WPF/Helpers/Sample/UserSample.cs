using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ExpertSystem.WPF.ViewModels;

namespace ExpertSystem.WPF.Helpers.Sample
{
    public class UserSample
    {
        public ObservableCollection<ObservableCollection<SampleEntry>> UserSamples { get; set; } = new();

        public void AddNewSample(List<string> columnNames)
        {
            var newSample = new ObservableCollection<SampleEntry>();
            foreach (var columnName in columnNames)
            {
                newSample.Add(new SampleEntry { ColumnName = columnName });
            }
            UserSamples.Add(newSample);
        }
    }
}
