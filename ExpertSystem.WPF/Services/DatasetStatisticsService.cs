using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ExpertSystem.Domain.Services;

namespace ExpertSystem.WPF.Services
{
    public class DatasetStatisticsService : IDatasetStatisticsService
    {
        public DataTable CalculateDatasetStatistics(DataTable dataTable)
        {
            var table = new DataTable();
            table.Columns.Add("Statistic");

            var numericColumns = dataTable.Columns.Cast<DataColumn>()
                .Where(IsNumericColumn)
                .ToList();

            if (!numericColumns.Any())
                return table;

            foreach(DataColumn column in numericColumns)
            {
                table.Columns.Add(column.ColumnName);
            }

            double[] GetColumnValues(DataColumn col)
            {
                return dataTable.AsEnumerable()
                    .Where(row => row[col] != DBNull.Value)
                    .Select(row => ParseDouble(row[col].ToString()))
                    .Where(d => !double.IsNaN(d))
                    .ToArray();
            }

            table.Rows.Add(CreateRow(table, "count", numericColumns, col => GetColumnValues(col).Length.ToString()));
            table.Rows.Add(CreateRow(table, "mean", numericColumns, col => GetColumnValues(col).Average().ToString("F2")));
            table.Rows.Add(CreateRow(table, "std", numericColumns, col =>
            {
                var values = GetColumnValues(col);
                var mean = values.Average();
                var std = Math.Sqrt(values.Sum(v => Math.Pow(v - mean, 2)) / values.Length);
                return std.ToString("F2");
            }));
            table.Rows.Add(CreateRow(table, "min", numericColumns, col => GetColumnValues(col).Min().ToString("F2")));
            table.Rows.Add(CreateRow(table, "25%", numericColumns, col => Percentile(GetColumnValues(col), 25).ToString("F2")));
            table.Rows.Add(CreateRow(table, "50%", numericColumns, col => Percentile(GetColumnValues(col), 50).ToString("F2")));
            table.Rows.Add(CreateRow(table, "75%", numericColumns, col => Percentile(GetColumnValues(col), 75).ToString("F2")));
            table.Rows.Add(CreateRow(table, "max", numericColumns, col => GetColumnValues(col).Max().ToString("F2")));

            return table;
        }

        private bool IsNumericColumn(DataColumn column)
        {
            foreach (DataRow row in column.Table.Rows)
            {
                if (row.IsNull(column)) continue;

                var valueStr = row[column].ToString();
                if (!double.TryParse(valueStr, NumberStyles.Any, CultureInfo.InvariantCulture, out _))
                {
                    return false;
                }
            }
            return true;
        }

        private double ParseDouble(string value)
        {
            if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
                return result;
            return double.NaN;
        }

        private DataRow CreateRow(DataTable table, string statisticName, List<DataColumn> columns, Func<DataColumn, string> valueSelector)
        {
            var row = table.NewRow();
            row[0] = statisticName;

            for (int i = 0; i < columns.Count; i++)
            {
                row[i + 1] = valueSelector(columns[i]);
            }

            return row;
        }


        private double Percentile(double[] sequence, double percentile)
        {
            if (sequence == null || sequence.Length == 0)
                return double.NaN;

            Array.Sort(sequence);

            double realIndex = percentile / 100.0 * (sequence.Length - 1);
            int index = (int)realIndex;
            double frac = realIndex - index;

            if (index + 1 < sequence.Length)
                return sequence[index] * (1 - frac) + sequence[index + 1] * frac;
            else
                return sequence[index];
        }
    }
}
