using System.Diagnostics;
using System.IO;
using NUnit.Framework;

namespace BeyondCompareSQLitePlugin.Test
{
    [TestFixture]
    public class ProgramTest : TestBase
    {
        [Test]
        public void Program_Main()
        {
            #region Act

            var returnCode = Program.Main(new[] { this.SampleSqlitePath, this.TestFile1 });

            #endregion

            #region Assert

            Assert.That(returnCode, Is.EqualTo(Program.Ok), "Return Code");
            Assert.That(File.ReadAllText(this.TestFile1), Does.Not.Contain("Exception"));

            // Process.Start("sampleSqlite.txt");

            #endregion
        }

        [Test]
        public void Program_Main_OnlySchema()
        {
            #region Act

            var returnCode = Program.Main(new[] { this.SampleSqlitePath, this.TestFile1, "/schema" });

            #endregion

            #region Assert

            Assert.That(returnCode, Is.EqualTo(Program.Ok), "Return Code");
            Assert.That(File.ReadAllText(this.TestFile1), Does.Not.Contain("Exception"));

            // Process.Start("sampleSqlite.txt");

            #endregion
        }

        [Test]
        public void Program_Main_WrongHeader()
        {
            #region Act

            var returnCode = Program.Main(new[] { this.SampleTextFilePath, this.TestFile1, "/schema" });

            #endregion

            #region Assert

            Assert.That(returnCode, Is.EqualTo(Program.ErrorNoSqlLiteHeader), "Return Code");
            Assert.That(File.Exists(this.TestFile1), Is.False, "File exists");

            // Process.Start("sampleSqlite.txt");

            #endregion
        }
    }
}