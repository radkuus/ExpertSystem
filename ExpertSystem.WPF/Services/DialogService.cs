using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpertSystem.Domain.Services;
using ExpertSystem.WPF.ViewModels;
using ExpertSystem.WPF.Views;
using ExpertSystem.Domain.Models;

namespace ExpertSystem.WPF.Services
{
    public class DialogService : IDialogService
    {
        public void ShowDataFrameDialog(DataTable table)
        {
            var dataFrameView = new DataFrameView(table);
            dataFrameView.Show();
        }
    }
}
