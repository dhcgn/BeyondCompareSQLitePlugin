using System;
using System.IO;
using BeyondCompareSqlLite.Model;

namespace BeyondCompareSqlLite.CLI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args == null || args.Length != 2)
            {
                Console.Out.WriteLine("Please input two argumments.");
                Console.Out.WriteLine("1. argumment - source file");
                Console.Out.WriteLine("2. argumment - destination file");
                return;
            }

            var source = args[0];
            var target = args[1];

            try
            {
                var tableContentList = DbContext.GetTableContent(source);
                Report.CreateTextReport(tableContentList, target);
            }
            catch (Exception e)
            {
                File.WriteAllText(target, e.ToString());
                Console.Out.WriteLine("Error ocurred: " + e);
            }
        }
    }
}