using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertSystem.Domain.Services
{
    public interface IDialogService
    {
        void ShowDataFrameDialog(DataTable table);
        void ShowResultsDialog();
    }
}
