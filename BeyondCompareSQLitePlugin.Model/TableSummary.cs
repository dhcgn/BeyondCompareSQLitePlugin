using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BeyondCompareSQLitePlugin.Model
{
    public class TableSummary
    {
        private static readonly String Spacer = new String('-', 114);

        #region .ctor

        public TableSummary(String table)
        {
            this.TableName = table;
        }

        #endregion

        #region Properties

        public String TableName { get; set; }

        public List<String> ColumnNames { get; set; }

        public String[,] Data { get; set; }
        public String SchemaHash { get; set; }

        #endregion

        #region Public

        public String GetTextOutput(Boolean printData = true)
        {
            if (!printData)
            {
                return $"Table: {this.TableName}";
            }

            var sb = new StringBuilder();
            sb.AppendLine(Spacer);
            sb.AppendLine("Table: " + this.TableName);
            sb.AppendLine(Spacer);

            List<Tuple<String, Int32>> colmnWidths = this.GetColumnMaxWidths();

            foreach (var columnName in this.ColumnNames)
            {
                var totalWidth = colmnWidths.First(x => x.Item1 == columnName).Item2;
                sb.Append(columnName.PadRight(totalWidth) + "| ");
            }

            sb.Append(Environment.NewLine);

            if (this.Data.Length == 0 || !printData) return sb.ToString();

            var i1 = this.Data.GetLength(1);
            for (Int32 i = 0; i < i1; i++)
            {
                sb.Append(Environment.NewLine);
                var colCnt = 0;
                foreach (var columnName in this.ColumnNames)
                {
                    var totalWidth = colmnWidths.First(x => x.Item1 == columnName).Item2;
                    var s = this.Data[colCnt, i];
                    if (String.IsNullOrWhiteSpace(s))
                        s = "-";
                    sb.Append(s.PadRight(totalWidth) + "| ");
                    colCnt++;
                }
            }

            return sb.ToString();
        }

        #endregion

        #region Private

        private List<Tuple<String, Int32>> GetColumnMaxWidths()
        {
            // Todo: Use names tuples!
            var result = new List<Tuple<String, Int32>>();

            Int32 colnCnt = 0;
            foreach (var columnName in this.ColumnNames)
            {
                var maxWidth = columnName.Length;
                for (Int32 i = 0; i < this.Data.GetLength(1); i++)
                {
                    var length = this.Data[colnCnt, i].Length;
                    if (maxWidth < length)
                        maxWidth = length;
                }

                maxWidth += 1;
                colnCnt++;

                result.Add(new Tuple<String, Int32>(columnName, maxWidth));
            }

            return result;
        }

        #endregion
    }
}