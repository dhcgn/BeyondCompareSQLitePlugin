using System;

namespace BeyondCompareSQLitePlugin
{
    public class ConsoleHelper
    {
        public static void PrintIntro()
        {
            Console.Out.WriteLine("This program dumps a SQLite file to a text file.");
            Console.Out.WriteLine("See wiki for instructions how to include this program in Beyond Compare 3 and 4.");
            Console.Out.WriteLine("https://github.com/dhcgn/BeyondCompareSQLitePlugin/wiki");
        }

        public static void PrintHelp()
        {
            Console.Out.WriteLine("Please input two (or three) arguments.");
            Console.Out.WriteLine("1. argument - source file (SQLite file");
            Console.Out.WriteLine("2. argument - destination file (text file, will be overwritten)");
            Console.Out.WriteLine("3. argument - optional switch '/schema' (only writes the schema, no data)");
        }

        public static void PrintFileDoesntExists(String source)
        {
            Console.Out.WriteLine("File \"{0}\" doesn't exist.", source);
        }

        public static void PrintFileNotSqlliteDatabase(String source)
        {
            Console.Out.WriteLine("File \"{0}\" doesn't contains the sqlite header. This file is not an sqlite databse!", source);
        }
    }
}