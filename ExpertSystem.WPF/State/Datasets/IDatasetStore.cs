using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpertSystem.Domain.Models;

namespace ExpertSystem.WPF.State.Datasets
{
    public interface IDatasetStore
    {
        ObservableCollection<Dataset> UserDatasets { get; }
    }
}
