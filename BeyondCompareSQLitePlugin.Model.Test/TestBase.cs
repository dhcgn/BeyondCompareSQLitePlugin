using System;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace BeyondCompareSQLitePlugin.Model.Test
{
    public class TestBase
    {
        internal const String SampleSqlite = "Chinook_Sqlite.sqlite";
        internal const String SampleSqliteSecond = "Chinook_Sqlite_second.sqlite";
        internal const String SampleSqliteEscapeNeeded = "Chinook_Sqlite_escape_needed.sqlite";
        internal const String EmptySqlite = "empty.sqlite";

        private String SampleSqlitePath => Path.Combine(TestContext.CurrentContext.TestDirectory, SampleSqlite);

        private String SampleSqliteSecondPath =>
            Path.Combine(TestContext.CurrentContext.TestDirectory, SampleSqliteSecond);

        [OneTimeSetUp]
        public void Setup()
        {
            WriteSqlLiteDatabaseToDisk(SampleSqlite);
            WriteSqlLiteDatabaseToDisk(SampleSqliteSecond);
            WriteSqlLiteDatabaseToDisk(SampleSqliteEscapeNeeded);
            WriteSqlLiteDatabaseToDisk(EmptySqlite);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            this.DeleteFromDisk(SampleSqlite);
            this.DeleteFromDisk(SampleSqliteSecond);
            this.DeleteFromDisk(SampleSqliteEscapeNeeded);
            this.DeleteFromDisk(EmptySqlite);
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
            var sqlFilePath = Path.Combine(TestContext.CurrentContext.TestDirectory, filename);

            if (File.Exists(sqlFilePath))
                return;

            using (Stream input = Assembly.GetExecutingAssembly().GetManifestResourceStream(name))
            using (Stream output = File.Create(sqlFilePath))
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