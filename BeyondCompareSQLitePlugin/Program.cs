using System;
using System.IO;
using System.Linq;
using System.Text;
using BeyondCompareSQLitePlugin.Model;

namespace BeyondCompareSQLitePlugin
{
    public class Program
    {
        internal const Int32 Ok = 0x0;
        internal const Int32 ErrorBadArguments = -0x1;
        internal const Int32 ErrorFileDoesntExist = -0x2;
        internal const Int32 ErrorNoSqlLiteHeader = -0x3;
        internal const Int32 ErrorUnknown = -0x4;

        /// <summary>
        /// Please input two (or three) arguments.
        /// 1. argument - source file (SQLite file)
        /// 2. argument - destination file (text file, will be overwritten)
        /// 3. argument - optional switch '/schema' (only writes the schema, no data)
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static Int32 Main(String[] args)
        {
            EmbeddedLibsResolver.Init();
            return MainInternal(args);
        }

        private static Int32 MainInternal(String[] args)
        {
            ConsoleHelper.PrintIntro();
            if (args == null || (args.Length != 2 && args.Length != 3))
            {
                ConsoleHelper.PrintHelp();
                return ErrorBadArguments;
            }

            var sourcePath = args[0];
            var targetPath = args[1];

            var containsData = true;

            if (args.Length > 2) containsData = args[2]?.ToLower().TrimStart('/') != "schema";

            if (!File.Exists(sourcePath))
            {
                ConsoleHelper.PrintFileDoesntExists(sourcePath);
                return ErrorFileDoesntExist;
            }

            if (!IsSqlLiteFile(sourcePath))
            {
                ConsoleHelper.PrintFileNotSqlliteDatabase(sourcePath);
                return ErrorNoSqlLiteHeader;
            }

            try
            {
                var databaseContent = DbReader.CreateSummary(sourcePath);
                Report.WriteTextReportToFile(databaseContent, targetPath, containsData);
                return Ok;
            }
            catch (Exception e)
            {
                File.WriteAllText(targetPath, e.ToString());
                Console.Out.WriteLine("Error ocurred: " + e);
                return ErrorUnknown;
            }
        }

        private static Boolean IsSqlLiteFile(String path)
        {
            byte[] buffer = new byte[16];

            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                fs.Read(buffer, 0, buffer.Length);
                fs.Close();
            }

            var fileHeader = Encoding.UTF8.GetString(buffer);
            return fileHeader == "SQLite format 3\0";
        }
    }
}