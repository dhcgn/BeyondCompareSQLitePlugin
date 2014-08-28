using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BeyondCompareSqlLite.Model
{
    public class TableContent
    {
        private const string Spacer = "------------------------------------------------------------------------------------------------------------------";

        public TableContent(string table)
        {
            TableName = table;
        }

        public string TableName { get; set; }

        public List<string> ColumnNames { get; set; }
        public string[,] Data { get; set; }

        public string GetReport()
        {
            var sb = new StringBuilder();
            sb.AppendLine(Spacer);
            sb.AppendLine("Table: " + TableName);
            sb.AppendLine(Spacer);

            List<Tuple<string, int>> colmnWidths = GetColumnWidths();

            foreach (var columnName in this.ColumnNames)
            {
                var totalWidth = colmnWidths.First(x => x.Item1 == columnName).Item2;
                sb.Append(columnName.PadRight(totalWidth) + "| ");
            }

            sb.Append(Environment.NewLine);

            if (Data.Length == 0) return sb.ToString();

            var i1 = Data.GetLength(1);
            for (int i = 0; i < i1; i++)
            {
                sb.Append(Environment.NewLine);
                var colCnt = 0;
                foreach (var columnName in ColumnNames)
                {
                    var totalWidth = colmnWidths.First(x => x.Item1 == columnName).Item2;
                    var s = Data[colCnt, i];
                    if (String.IsNullOrWhiteSpace(s))
                        s = "-";
                    sb.Append(s.PadRight(totalWidth) + "| ");
                    colCnt++;
                }
            }

            return sb.ToString();
        }

        private List<Tuple<string, int>> GetColumnWidths()
        {
            var result = new List<Tuple<string, int>>();

            int colnCnt = 0;
            foreach (var columnName in this.ColumnNames)
            {
                var maxWidth = columnName.Length;
                for (int i = 0; i < Data.GetLength(1); i++)
                {
                    var length = Data[colnCnt, i].Length;
                    if (maxWidth < length)
                        maxWidth = length;
                }
                maxWidth += 1;
                colnCnt++;

                result.Add(new Tuple<string, int>(columnName, maxWidth));
            }

            return result;
        }
    }
}