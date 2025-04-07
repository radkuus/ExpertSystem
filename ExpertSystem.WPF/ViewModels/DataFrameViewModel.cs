using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertSystem.WPF.ViewModels
{
    public class DataFrameViewModel : BaseViewModel
    {
        private DataView _dataFrame;

        public DataView DataFrame
        {
            get => _dataFrame;
            set
            {
                _dataFrame = value;
                OnPropertyChanged(nameof(DataView));
            }
        }

        public DataFrameViewModel(DataTable dataTable)
        {
            DataFrame = dataTable.DefaultView;
        }
    }
}
