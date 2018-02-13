using System;

namespace BeyondCompareSQLitePlugin
{
    public class ConsoleHelper
    {
        public static void PrintIntro()
        {
            Console.Out.WriteLine("This program dumps a SQLite file to a text file.");
            Console.Out.WriteLine("See wiki for instructions how to include this program in Beyond Compare 3 and 4.");
            Console.Out.WriteLine("Link to wiki https://github.com/dhcgn/BeyondCompareSQLitePlugin/wiki");
        }

        public static void PrintHelp()
        {
            Console.Out.WriteLine("Please input two (or three) arguments.");
            Console.Out.WriteLine("1. argument - source file (SQLite file");
            Console.Out.WriteLine("2. argument - destination file (text file, will be overwritten)");
            Console.Out.WriteLine("3. argument - schema (only writes the schema, no data)");
        }

        public static void PrintFileDoesntExists(string source)
        {
            Console.Out.WriteLine("File \"{0}\" doesn't exist.", source);
        }
    }
}