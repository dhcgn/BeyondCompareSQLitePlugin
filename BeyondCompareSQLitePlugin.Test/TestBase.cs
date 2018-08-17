using System;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace BeyondCompareSQLitePlugin.Test
{
    public class TestBase
    {
        internal const String SampleSqlite = "Chinook_Sqlite.sqlite";
        internal const String SampleSqliteSecond = "Chinook_Sqlite_second.sqlite";
        internal const String SampleTextFile = "JustATextFile.txt";

        internal String SampleSqlitePath => Path.Combine(TestContext.CurrentContext.TestDirectory, SampleSqlite);
        internal String SampleTextFilePath => Path.Combine(TestContext.CurrentContext.TestDirectory, SampleTextFile);

        internal String SampleSqliteSecondPath =>
            Path.Combine(TestContext.CurrentContext.TestDirectory, SampleSqliteSecond);

        [OneTimeSetUp]
        public void Setup()
        {
            WriteSqlLiteDatabaseToDisk(SampleSqlite);
            WriteSqlLiteDatabaseToDisk(SampleSqliteSecond);
            WriteSqlLiteDatabaseToDisk(SampleTextFile);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            this.DeleteFromDisk(SampleSqlite);
            this.DeleteFromDisk(SampleSqliteSecond);
            this.DeleteFromDisk(SampleTextFile);
        }

        public String TestFile1 => Path.Combine(TestContext.CurrentContext.TestDirectory, "testfile_1");

        [TearDown]
        public void TearDown()
        {
            if(File.Exists(this.TestFile1))
                File.Delete(this.TestFile1);
        }

        private void DeleteFromDisk(String filename)
        {
            if (File.Exists(filename))
                File.Delete(filename);
        }

        private static void WriteSqlLiteDatabaseToDisk(String filename)
        {
            var name = Assembly.GetExecutingAssembly().GetManifestResourceNames()
                .FirstOrDefault(x => x.EndsWith(filename));
            using (Stream input = Assembly.GetExecutingAssembly().GetManifestResourceStream(name))
            using (Stream output = File.Create(Path.Combine(TestContext.CurrentContext.TestDirectory, filename)))
            {
                CopyStream(input, output);
            }
        }

        public static void CopyStream(Stream input, Stream output)
        {
            // Insert null checking here for production
            Byte[] buffer = new Byte[8192];

            Int32 bytesRead;
            while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, bytesRead);
            }
        }
    }
}