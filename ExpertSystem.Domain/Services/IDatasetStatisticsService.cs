﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertSystem.Domain.Services
{
    public interface IDatasetStatisticsService
    {
        DataTable CalculateDatasetStatistics(DataTable dataTable);
    }
}
