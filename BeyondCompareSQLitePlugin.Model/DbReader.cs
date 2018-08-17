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
    public class DbReader
    {
        // Need this reference at runtime!
        // ReSharper disable once UnusedMember.Local
        private static volatile Type dependencyProperty = typeof(SQLiteConnection);

        #region Public

        public static List<String> GetTableNamesContent(String source)
        {
            List<String> tables;
            using (var connection = new SQLiteConnection(String.Format("Data Source={0};Version=3;Read Only=True;", source)))
            {
                connection.Open();
                tables = ReadTables(connection);
            }
            return tables;
        }

        public static DatabaseSummary CreateSummary(String source)
        {
            PrepareInterop();

            var result = new DatabaseSummary();
            using (var connection = new SQLiteConnection(String.Format("Data Source={0};Version=3;Read Only=True;", source)))
            {
                connection.Open();

                var tableNames = ReadTables(connection);
                
                result.Tables = CreateTableSummary(tableNames, connection);
                result.SchemaVersion = ReadSchemaVersion(connection);
                result.UserVersion = ReadUserVersion(connection);

                connection.Close();
            }

            return result;
        }

        [System.Runtime.InteropServices.DllImport("kernel32.dll",
            CharSet = System.Runtime.InteropServices.CharSet.Unicode, SetLastError = true)]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        static extern Boolean SetDllDirectory(String lpPathName);
        
        private static void PrepareInterop()
        {
            var path = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
            var dir = Path.GetDirectoryName(path);

            var ressources = Assembly.GetExecutingAssembly().GetManifestResourceNames();

            var processorRegisters = new[] { "x64", "x86"};

            foreach (var processorRegister in processorRegisters)
            {
                Directory.CreateDirectory(Path.Combine(dir, processorRegister));
            }

            foreach (var processorRegister in processorRegisters)
            {
                var name = ressources.First(s => s.EndsWith($"{processorRegister}_SQLite.Interop.dll"));
                var fullPath = Path.Combine(dir, processorRegister, "SQLite.Interop.dll");
                if (!File.Exists(fullPath))
                    using (var fileStream = File.OpenWrite(fullPath))
                    {
                        Assembly.GetExecutingAssembly().GetManifestResourceStream(name).CopyTo(fileStream);
                    }
            }

            Int32 wsize = IntPtr.Size;
            String libdir = (wsize == 4) ? "x86" : "x64";
            String appPath = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            SetDllDirectory(Path.Combine(appPath, libdir));
        }

        #endregion

        #region Private

        private static List<String> ReadTables(SQLiteConnection dbConnection)
        {
            String sql = "SELECT name FROM sqlite_master WHERE type='table';";
            var command = new SQLiteCommand(sql, dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();

            var tables = new List<String>();

            while (reader.Read())
            {
                tables.Add(reader.GetString(0));
            }

            return tables.OrderBy(x => x).ToList();
        }

        private static Int32 ReadSchemaVersion(SQLiteConnection dbConnection)
        {
            String sql = "PRAGMA schema_version;";
            var command = new SQLiteCommand(sql, dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();

            Int32 result = 0;
            while (reader.Read())
            {
                result = reader.GetInt32(0);
            }
            return result;
        }

        private static Int32 ReadUserVersion(SQLiteConnection dbConnection)
        {
            String sql = "PRAGMA user_version;";
            var command = new SQLiteCommand(sql, dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();

            Int32 result = 0;
            while (reader.Read())
            {
                result = reader.GetInt32(0);
            }
            return result;
        }

        private static List<TableSummary> CreateTableSummary(IEnumerable<String> tables, SQLiteConnection dbConnection)
        {
            var result = new List<TableSummary>();

            foreach (String table in tables)
            {
                var tableContent = new TableSummary(table)
                {
                    ColumnNames = ReadColumns(table, dbConnection),
                    SchemaHash = ReadSchemaHash(table, dbConnection),
                    Data = ReadTableData(table, dbConnection)
                };

                tableContent.Data = ReadTableData(table, dbConnection);
                
                result.Add(tableContent);
            }

            return result;
        }

        private static String ReadSchemaHash(String table, SQLiteConnection dbConnection)
        {
            String sql = String.Format("PRAGMA table_info('{0}');", table);

            var command = new SQLiteCommand(sql, dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();

            var sb = new StringBuilder();

            while (reader.Read())
            {
                for (Int32 i = 0; i < reader.FieldCount; i++)
                {
                    sb.AppendLine(reader.GetValue(i).ToString());
                }
            }

            Byte[] hash = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(sb.ToString()));

            return Convert.ToBase64String(hash);
        }


        private static String[,] GetData(String table, SQLiteConnection dbConnection, Int32 width, Int32 hight)
        {
            var result = new String[width, hight];

            String sql = String.Format("SELECT * FROM '{0}' ORDER BY {1};",
                table,
                Enumerable.Range(1, width).Select(i => i.ToString()).Aggregate((a, b) => a + ", " + b));

            var command = new SQLiteCommand(sql, dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();

            Int32 rowCnt = 0;
            while (reader.Read())
            {
                for (Int32 columnCnt = 0; columnCnt < width; columnCnt++)
                {
                    result[columnCnt, rowCnt] = reader.GetValue(columnCnt).ToString();
                }

                rowCnt++;
            }
            return result;
        }

        private static Int32 ReadCount(String table, SQLiteConnection dbConnection)
        {
            String sql = String.Format("SELECT Count(*) FROM '{0}';", table);
            var command = new SQLiteCommand(sql, dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();

            Int32 result = 0;
            while (reader.Read())
            {
                result = reader.GetInt32(0);
            }
            return result;
        }


        private static String[,] ReadTableData(String table, SQLiteConnection dbConnection)
        {
            var columnNames = ReadColumns(table, dbConnection);
            Int32 width = columnNames.Count;
            Int32 heigth = ReadCount(table, dbConnection);

            if (heigth == 0) return new String[0, 0];

            String[,] result = GetData(table, dbConnection, width, heigth);

            return result;
        }

        private static List<String> ReadColumns(String table, SQLiteConnection dbConnection)
        {
            var sql = String.Format("PRAGMA table_info('{0}');", table);

            var command = new SQLiteCommand(sql, dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();

            var columns = new List<String>();
            while (reader.Read())
            {
                columns.Add(reader.GetString(1));
            }
            return columns.ToList();
        }

        #endregion

        public static DatabaseSummary CreateSummary(String source, List<String> tables)
        {
            DatabaseSummary databaseSummary = CreateSummary(source);
            List<String> tablesToRemove = databaseSummary.Tables.Select(x => x.TableName).ToList();

            // What is this for?
            if (tables.Contains("All")) return databaseSummary;

            foreach (String table in tablesToRemove)
            {
                TableSummary tableSummary = databaseSummary.Tables.FirstOrDefault(x => x.TableName == table);
                if (tableSummary == null) continue;
                if (tables.Contains(tableSummary.TableName)) continue;

                databaseSummary.Tables.Remove(tableSummary);
            }

            return databaseSummary;
        }
    }

    public class DatabaseSummary
    {
        public List<TableSummary> Tables { get; set; }
        public Int32 SchemaVersion { get; set; }
        public Int32 UserVersion { get; set; }
    }
}