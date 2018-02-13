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
        private const string Spacer = "##################################################################################################################";

        #region Public

        public static void CreateTextReport(DatabaseContent tablesContentList, string target, bool listContents = true)
        {
            var sb = CreateTextReportInternal(tablesContentList,  listContents);
            File.WriteAllText(target, sb, Encoding.UTF8);
        }

        private static string CreateTextReportInternal(DatabaseContent tablesContentList, bool listContents = true)
        {
            var sb = new StringBuilder();
            sb.AppendLine(Spacer);
            sb.AppendLine("DataBase overview");
            sb.AppendLine(Spacer);
            sb.AppendFormat("SchemaVersion: {0,10}{1}", tablesContentList.SchemaVersion, Environment.NewLine);
            sb.AppendFormat("UserVersion:   {0,10}{1}", tablesContentList.UserVersion, Environment.NewLine);

            sb.AppendLine(Spacer);
            sb.AppendLine("Tables overview");
            sb.AppendLine(Spacer);

            CreateTablesSummary(tablesContentList.TableContent, sb);

            //sb.AppendLine(Spacer);
            //sb.AppendLine("Schema");
            //sb.AppendLine(Spacer);

            // Todo Schema

            sb.AppendLine(Spacer);
            sb.AppendLine("Content");
            sb.AppendLine(Spacer);

            foreach (var tablesContent in tablesContentList.TableContent)
            {
                sb.AppendLine(tablesContent.GetReport(listContents));
            }

            sb.AppendLine("EOF");
            return sb.ToString();
        }

        #endregion

        #region Private

        private static void CreateTablesSummary(List<TableContent> tablesContentList, StringBuilder sb)
        {
            var tables = tablesContentList.Select(x => x.TableName);
            var maxWidth = tables.Select(x => x.Length).OrderByDescending(x => x).FirstOrDefault();

            sb.AppendLine(string.Format("{0}|{1,10}|{2,10}|{3,36}", "table".PadRight(maxWidth), "columns", "rows", "schema hash"));
            sb.AppendLine();

            foreach (var table in tables)
            {
                int rows = 0;
                int columns = 0;
                string schemaHash = null;

                var tableContent = tablesContentList.FirstOrDefault(x => x.TableName == table);
                if (tableContent != null)
                {
                    columns = tableContent.Data.GetLength(0);
                    rows = tableContent.Data.GetLength(1);
                    schemaHash = tableContent.SchemaHash;
                }

                sb.AppendLine(string.Format("{0}|{1,10}|{2,10}|{3,36}", table.PadRight(maxWidth), columns, rows, schemaHash));
            }
        }

        #endregion

        public static string[] CreateTextReport(DatabaseContent tablesContentList)
        {
            var report = CreateTextReportInternal(tablesContentList);
            string[] result = Regex.Split(report, "\r\n|\r|\n");
            return result;
        }
    }
}