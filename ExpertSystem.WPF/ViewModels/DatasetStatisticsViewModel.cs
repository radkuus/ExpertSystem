using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertSystem.WPF.ViewModels
{
    public class DatasetStatisticsViewModel
    {
        public DataTable StatisticsTable { get; }
        public DatasetStatisticsViewModel(DataTable statisticsTable)
        {
            StatisticsTable = statisticsTable;
        }
    }
}