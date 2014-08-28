using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using BeyondCompareSqlLite.CLI;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BeyondCompareSqlLite.Test
{
    [TestClass]
    public class UnitTest1
    {
        private const string SampleSqlite = "Chinook_Sqlite.sqlite";
        private const string SampleSqliteSecond = "Chinook_Sqlite_second.sqlite";

        [TestInitialize]
        public void Setup()
        {
            var name= Assembly.GetExecutingAssembly().GetManifestResourceNames().FirstOrDefault(x => x.EndsWith(SampleSqlite));
            using (Stream input = Assembly.GetExecutingAssembly().GetManifestResourceStream(name))
            using (Stream output = File.Create(SampleSqlite))
            {
                CopyStream(input, output);
            }
        }

        [TestMethod]
        public void StartAndOpen()
        {
            Program.Main(new[] { SampleSqlite, "sampleSqlite.txt" });
            Process.Start("sampleSqlite.txt");
        }

        public static void CopyStream(Stream input, Stream output)
        {
            // Insert null checking here for production
            byte[] buffer = new byte[8192];

            int bytesRead;
            while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, bytesRead);
            }
        }


    }
}
