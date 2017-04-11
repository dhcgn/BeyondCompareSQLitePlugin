using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography;
using System.Text;

namespace BeyondCompareSQLitePlugin.Model
{
    public class DbContext
    {
        private static volatile Type dependencyProperty = typeof(SQLiteConnection);

        #region Public

        public static List<string> GetTableNamesContent(string source)
        {
            List<string> tables;
            using (var connection = new SQLiteConnection(String.Format("Data Source={0};Version=3;Read Only=True;", source)))
            {
                connection.Open();
                tables = GetTables(connection);
            }
            return tables;
        }

        public static DatabaseContent GetTableContent(string source)
        {
            Prepare();

            var result = new DatabaseContent();
            List<TableContent> tablesContent;
            using (var connection = new SQLiteConnection(String.Format("Data Source={0};Version=3;Read Only=True;", source)))
            {
                connection.Open();

                List<string> tables = GetTables(connection);
                tablesContent = GetTableContent(tables, connection);
                result.TableContent = tablesContent;

                result.SchemaVersion = GetSchemaVersion(connection);
                result.UserVersion = GetUserersion(connection);
            }

            return result;
        }

        [System.Runtime.InteropServices.DllImport("kernel32.dll",
            CharSet = System.Runtime.InteropServices.CharSet.Unicode, SetLastError = true)]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        static extern bool SetDllDirectory(string lpPathName);
        
        private static void Prepare()
        {
            var temp1 = Assembly.GetEntryAssembly()?.CodeBase;
            var temp2 = Assembly.GetExecutingAssembly()?.CodeBase;
            var temp3 = Assembly.GetCallingAssembly()?.CodeBase;

            var path = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
            var dir = Path.GetDirectoryName(path);

            var ressources = Assembly.GetExecutingAssembly().GetManifestResourceNames();

            var x64 = ressources.First(s => s.EndsWith("x64_SQLite.Interop.dll"));
            var x86 = ressources.First(s => s.EndsWith("x86_SQLite.Interop.dll"));

            Directory.CreateDirectory(Path.Combine(dir, "x64"));
            Directory.CreateDirectory(Path.Combine(dir, "x86"));

            using (var fileStream = File.OpenWrite(Path.Combine(dir, "x64", "SQLite.Interop.dll")))
            {
                Assembly.GetExecutingAssembly().GetManifestResourceStream(x64).CopyTo(fileStream);
            }
            using (var openWrite = File.OpenWrite(Path.Combine(dir, "x86", "SQLite.Interop.dll")))
            {
                Assembly.GetExecutingAssembly().GetManifestResourceStream(x86).CopyTo(openWrite);
            }

            int wsize = IntPtr.Size;
            string libdir = (wsize == 4) ? "x86" : "x64";
            string appPath = System.IO.Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            SetDllDirectory(System.IO.Path.Combine(appPath, libdir));
        }

        #endregion

        #region Private

        private static List<string> GetTables(SQLiteConnection dbConnection)
        {
            string sql = "SELECT name FROM sqlite_master WHERE type='table';";
            var command = new SQLiteCommand(sql, dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();

            var tables = new List<string>();

            while (reader.Read())
            {
                tables.Add(reader.GetString(0));
            }

            return tables.OrderBy(x => x).ToList();
        }

        private static int GetSchemaVersion(SQLiteConnection dbConnection)
        {
            string sql = "PRAGMA schema_version;";
            var command = new SQLiteCommand(sql, dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();

            int result = 0;
            while (reader.Read())
            {
                result = reader.GetInt32(0);
            }
            return result;
        }

        private static int GetUserersion(SQLiteConnection dbConnection)
        {
            string sql = "PRAGMA user_version;";
            var command = new SQLiteCommand(sql, dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();

            int result = 0;
            while (reader.Read())
            {
                result = reader.GetInt32(0);
            }
            return result;
        }

        private static List<TableContent> GetTableContent(List<string> tables, SQLiteConnection dbConnection)
        {
            var result = new List<TableContent>();

            foreach (string table in tables)
            {
                var tableContent = new TableContent(table);

                tableContent.ColumnNames = GetColumnName(table, dbConnection);
                tableContent.Data = GetTableData(table, tableContent.ColumnNames, dbConnection);
                tableContent.SchemaHash = GetTableSchemaHash(table, dbConnection);
                result.Add(tableContent);
            }

            return result;
        }

        private static string GetTableSchemaHash(string table, SQLiteConnection dbConnection)
        {
            string sql = String.Format("PRAGMA table_info({0});", table);

            var command = new SQLiteCommand(sql, dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();

            var sb = new StringBuilder();

            while (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    sb.AppendLine(reader.GetValue(i).ToString());
                }
            }

            byte[] hash = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(sb.ToString()));

            return BitConverter.ToString(hash).Replace("-", String.Empty).ToLower();
        }


        private static string[,] GetData(string table, SQLiteConnection dbConnection, int width, int hight)
        {
            var result = new string[width, hight];

            string sql = String.Format("SELECT * FROM {0} ORDER BY {1};",
                table,
                Enumerable.Range(1, width).Select(i => i.ToString()).Aggregate((a, b) => a + ", " + b));

            var command = new SQLiteCommand(sql, dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();

            int rowCnt = 0;
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
            SQLiteDataReader reader = command.ExecuteReader();

            int result = 0;
            while (reader.Read())
            {
                result = reader.GetInt32(0);
            }
            return result;
        }


        private static string[,] GetTableData(string table, List<string> columnNames, SQLiteConnection dbConnection)
        {
            int width = columnNames.Count;
            int heigth = RowCount(table, dbConnection);

            if (heigth == 0) return new string[0, 0];

            string[,] result = GetData(table, dbConnection, width, heigth);

            return result;
        }

        private static List<string> GetColumnName(string table, SQLiteConnection dbConnection)
        {
            string sql = String.Format("PRAGMA table_info({0});", table);

            var command = new SQLiteCommand(sql, dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();

            var tables = new List<string>();
            while (reader.Read())
            {
                tables.Add(reader.GetString(1));
            }
            return tables.ToList();
        }

        #endregion

        public static DatabaseContent GetTableContent(string source, List<string> tables)
        {
            DatabaseContent databaseContent = GetTableContent(source);
            List<string> tablesToRemove = databaseContent.TableContent.Select(x => x.TableName).ToList();

            if (tables.Contains("All")) return databaseContent;

            foreach (string table in tablesToRemove)
            {
                TableContent tableContent = databaseContent.TableContent.FirstOrDefault(x => x.TableName == table);
                if (tableContent == null) continue;
                if (tables.Contains(tableContent.TableName)) continue;

                databaseContent.TableContent.Remove(tableContent);
            }

            return databaseContent;
        }
    }

    public class DatabaseContent
    {
        public List<TableContent> TableContent { get; set; }
        public int SchemaVersion { get; set; }
        public int UserVersion { get; set; }
    }
}