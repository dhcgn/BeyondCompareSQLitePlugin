using System;
using System.IO;
using BeyondCompareSQLitePlugin.Model;

namespace BeyondCompareSQLitePlugin
{
    public class Program
    {
        internal const int Ok = 0x0;
        internal const int ErrorBadArguments = -0x1;
        internal const int ErrorFileDoesntExists = -0x2;
        internal const int ErrorUnknown = -0x3;

        public static int Main(string[] args)
        {
            EmbeddedLibsResolver.Init();
            return MainInternal(args);
        }

        private static int MainInternal(string[] args)
        {
            ConsoleHelper.PrintIntro();
            if (args == null || args.Length != 2)
            {
                ConsoleHelper.PrintHelp();
                return ErrorBadArguments;
            }

            string source = args[0];
            string target = args[1];

            if (!File.Exists(source))
            {
                ConsoleHelper.PrintFileDoesntExists(source);
                return ErrorFileDoesntExists;
            }

            try
            {
                var databaseContent = DbContext.GetTableContent(source);
                Report.CreateTextReport(databaseContent, target);
                return Ok;
            }
            catch (Exception e)
            {
                File.WriteAllText(target, e.ToString());
                Console.Out.WriteLine("Error ocurred: " + e);
                return ErrorUnknown;
            }
        }
    }
}