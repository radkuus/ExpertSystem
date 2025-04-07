using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpertSystem.Domain.Services;
using ExpertSystem.WPF.Views;

namespace ExpertSystem.WPF.Services
{
    public class DataFrameDialogService : IDataFrameDialogService
    {
        public void ShowDataFrame(DataTable table)
        {
            var dataFrameView = new DataFrameView(table);
            dataFrameView.Show();
        }
    }
}
