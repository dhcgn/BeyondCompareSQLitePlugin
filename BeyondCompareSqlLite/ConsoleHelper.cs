using System;

namespace BeyondCompareSqlLite.CLI
{
    public class ConsoleHelper
    {
        public static void PrintIntro()
        {
            Console.Out.WriteLine("This program dumps a SQLite file to a text file.");
            Console.Out.WriteLine("See wiki for instructions how to include this program in Beyond Compare 3 and 4.");
            Console.Out.WriteLine("Link to wiki https://github.com/dhcgn/Beyond-Compare-SQLite-Plugin/wiki");
        }

        public static void PrintHelp()
        {
            Console.Out.WriteLine("Please input two argumments.");
            Console.Out.WriteLine("1. argumment - source file (SQLite file");
            Console.Out.WriteLine("2. argumment - destination file (text file, will be overridden)");
        }

        public static void PrintFileDoesntExists(string source)
        {
            Console.Out.WriteLine("File \"{0}\" doens't exists.", source);
        }
    }
}