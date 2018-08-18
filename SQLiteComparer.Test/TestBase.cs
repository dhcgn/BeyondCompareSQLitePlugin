using System;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace SQLiteComparer.Test
{
    public class TestBase
    {
        internal const String SampleSqliteDiffTuple_1 = "Chinook_Sqlite_only_diff_data_part_1.sqlite";
        internal const String SampleSqliteDiffTuple_2 = "Chinook_Sqlite_only_diff_data_part_2.sqlite";
        internal const String SampleSqliteDiffRows_1 = "Chinook_Sqlite_only_diff_row_count_part_1.sqlite";
        internal const String SampleSqliteDiffRows_2 = "Chinook_Sqlite_only_diff_row_count_part_2.sqlite";

        internal static String SampleSqlitePathDiffTuple_1 => Path.Combine(TestContext.CurrentContext.TestDirectory, SampleSqliteDiffTuple_1);
        internal static String SampleSqlitePathDiffTuple_2 => Path.Combine(TestContext.CurrentContext.TestDirectory, SampleSqliteDiffTuple_2);

        [OneTimeSetUp]
        public void Setup()
        {
            WriteSqlLiteDatabaseToDisk(SampleSqliteDiffTuple_1);
            WriteSqlLiteDatabaseToDisk(SampleSqliteDiffTuple_2);
            WriteSqlLiteDatabaseToDisk(SampleSqliteDiffRows_1);
            WriteSqlLiteDatabaseToDisk(SampleSqliteDiffRows_2);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            this.DeleteFromDisk(SampleSqliteDiffTuple_1);
            this.DeleteFromDisk(SampleSqliteDiffTuple_2);
            this.DeleteFromDisk(SampleSqliteDiffRows_1);
            this.DeleteFromDisk(SampleSqliteDiffRows_2);
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