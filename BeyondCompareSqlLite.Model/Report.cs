using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BeyondCompareSqlLite.Model
{
    public class Report
    {
        private const string Spacer = "##################################################################################################################";

        public static void CreateTextReport(List<TableContent> tablesContentList, string target)
        {
            var sb = new StringBuilder();
            sb.AppendLine(Spacer);
            sb.AppendLine("Tables");
            sb.AppendLine(Spacer);
            foreach (var table in tablesContentList.Select(x => x.TableName))
            {
                sb.AppendLine(table);
            }

            sb.AppendLine(Spacer);
            sb.AppendLine("Tables Summary");
            sb.AppendLine(Spacer);

            CreateTablesSummary(tablesContentList, sb);

            sb.AppendLine(Spacer);
            sb.AppendLine("Schema");
            sb.AppendLine(Spacer);

            sb.AppendLine(Spacer);
            sb.AppendLine("Content");
            sb.AppendLine(Spacer);

            foreach (var tablesContent in tablesContentList)
            {
                sb.AppendLine(tablesContent.GetReport());
            }

            sb.AppendLine("EOF");
            File.WriteAllText(target, sb.ToString(), Encoding.UTF8);
        }

        private static void CreateTablesSummary(List<TableContent> tablesContentList, StringBuilder sb)
        {
            var tables = tablesContentList.Select(x => x.TableName);
            var maxWidth = tables.Select(x => x.Length).OrderByDescending(x => x).First();

            sb.AppendLine(string.Format("{0},{1,10},{2,10}", "table".PadLeft(maxWidth), "columns", "rows"));

            foreach (var table in tables)
            {
                int rows = 0;
                int columns = 0;

                var temp = tablesContentList.FirstOrDefault(x => x.TableName == table);
                if (temp != null)
                {
                    columns = temp.Data.GetLength(0);
                    rows = temp.Data.GetLength(1);
                }

                sb.AppendLine(string.Format("{0," + maxWidth + "},{1,10},{2,10}", table, columns, rows));
            }
        }
    }
}