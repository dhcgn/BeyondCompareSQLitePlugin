using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace BeyondCompareSqlLite.Model
{
    public class DbContext
    {
        public static List<TableContent> GetTableContent(string source)
        {
            List<TableContent> tablesContent;
            using (var connection = new SQLiteConnection(String.Format("Data Source={0};Version=3;Read Only=True;", source)))
            {
                connection.Open();

                var tables = GetTables(connection);
                tablesContent = GetTableContent(tables, connection);
            }

            return tablesContent;
        }

        private static List<string> GetTables(SQLiteConnection dbConnection)
        {
            string sql = "SELECT name FROM sqlite_master WHERE type='table';";
            var command = new SQLiteCommand(sql, dbConnection);
            var reader = command.ExecuteReader();

            var tables = new List<string>();

            while (reader.Read())
            {
                tables.Add(reader.GetString(0));
            }

            return tables.OrderBy(x => x).ToList();
        }

        private static List<TableContent> GetTableContent(List<string> tables, SQLiteConnection dbConnection)
        {
            var result = new List<TableContent>();

            foreach (var table in tables)
            {
                var tableContent = new TableContent(table);

                tableContent.ColumnNames = GetColumnName(table, dbConnection);
                tableContent.Data = GetTableData(table, tableContent.ColumnNames, dbConnection);

                result.Add(tableContent);
            }

            return result;
        }


        private static string[,] GetData(string table, SQLiteConnection dbConnection, int width, int hight)
        {
            var result = new string[width, hight];

            string sql = String.Format("SELECT * FROM {0};", table);
            var command = new SQLiteCommand(sql, dbConnection);
            var reader = command.ExecuteReader();

            var rowCnt = 0;
            while (reader.Read())
            {
                for (int columnCnt = 0; columnCnt < width; columnCnt++)
                {
                    result[columnCnt, rowCnt] = reader.GetValue(columnCnt).ToString();
                }

                rowCnt++;
            }
            return result;
        }

        private static int RowCount(string table, SQLiteConnection dbConnection)
        {
            string sql = String.Format("SELECT Count(*) FROM {0};", table);
            var command = new SQLiteCommand(sql, dbConnection);
            var reader = command.ExecuteReader();

            int result = 0;
            while (reader.Read())
            {
                result = reader.GetInt32(0);
            }
            return result;
        }


        private static string[,] GetTableData(string table, List<string> columnNames, SQLiteConnection dbConnection)
        {
            var width = columnNames.Count;
            var heigth = RowCount(table, dbConnection);

            if (heigth == 0) return new string[0, 0];

            var result = GetData(table, dbConnection, width, heigth);

            return result;
        }

        private static List<string> GetColumnName(string table, SQLiteConnection dbConnection)
        {
            string sql = String.Format("PRAGMA table_info({0});", table);
           
            var command = new SQLiteCommand(sql, dbConnection);
            var reader = command.ExecuteReader();
            
            var tables = new List<string>();
            while (reader.Read())
            {
                tables.Add(reader.GetString(1));
            }
            return tables.ToList();
        }
    }
}