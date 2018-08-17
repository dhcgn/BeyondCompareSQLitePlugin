using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BeyondCompareSQLitePlugin.Model
{
    public class Report
    {
        private static readonly String Spacer = new String('#', 114);

        #region Public

        public static void WriteTextReportToFile(DatabaseSummary tablesSummaryList, String path, Boolean containsData = true)
        {
            // TODO Use Stream to relive memory
            var sb = CreateTextReportInternal(tablesSummaryList, containsData);
            File.WriteAllText(path, sb, Encoding.UTF8);
        }

        public static String CreateTextReport(DatabaseSummary tablesSummaryList, Boolean containsData = true)
        {
            var report = CreateTextReportInternal(tablesSummaryList, containsData);
            return report;
        }

        #endregion

        #region Private

        private static String CreateTextReportInternal(DatabaseSummary tablesSummaryList, Boolean containsData = true)
        {
            var sb = new StringBuilder();
            sb.AppendLine(Spacer);
            sb.AppendLine("DataBase overview");
            sb.AppendLine(Spacer);
            sb.AppendFormat("SchemaVersion: {0,10}{1}", tablesSummaryList.SchemaVersion, Environment.NewLine);
            sb.AppendFormat("UserVersion:   {0,10}{1}", tablesSummaryList.UserVersion, Environment.NewLine);

            sb.AppendLine(Spacer);
            sb.AppendLine("Tables overview");
            sb.AppendLine(Spacer);

            AppendTablesToStringBuilder(tablesSummaryList.Tables, sb);

            sb.AppendLine(Spacer);
            sb.AppendLine("Content");
            sb.AppendLine(Spacer);

            foreach (var tableSummary in tablesSummaryList.Tables)
            {
                sb.AppendLine(tableSummary.GetTextOutput(containsData));
            }

            sb.AppendLine("EOF");
            return sb.ToString();
        }

        private static void AppendTablesToStringBuilder(List<TableSummary> tableSummaries, StringBuilder sb)
        {
            var tables = tableSummaries.Select(ts => ts.TableName);
            var maxWidth = tables.Select(n => n.Length).OrderByDescending(l => l).FirstOrDefault();

            sb.AppendLine(String.Format("{0}|{1,10}|{2,10}|{3,36}", "table".PadRight(maxWidth), "columns", "rows", "schema hash"));
            sb.AppendLine();

            foreach (var table in tables)
            {
                Int32 rows = 0;
                Int32 columns = 0;
                String schemaHash = null;

                var tableContent = tableSummaries.FirstOrDefault(x => x.TableName == table);
                if (tableContent != null)
                {
                    columns = tableContent.Data.GetLength(0);
                    rows = tableContent.Data.GetLength(1);
                    schemaHash = tableContent.SchemaHash;
                }

                sb.AppendLine(String.Format("{0}|{1,10}|{2,10}|{3,36}", table.PadRight(maxWidth), columns, rows, schemaHash));
            }
        }

        #endregion

    }
}