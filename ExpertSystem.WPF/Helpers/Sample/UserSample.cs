﻿using System;
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

        public void AddNewSample(ObservableCollection<string> columnNames, Action onValueChangedCallback)
        {
            var newSample = new ObservableCollection<SampleEntry>();
            foreach (var columnName in columnNames)
            {
                newSample.Add(new SampleEntry
                {
                    ColumnName = columnName,
                    OnValueChanged = onValueChangedCallback
                });
            }

            UserSamples.Add(newSample);
        }

    }
}
