using System.Diagnostics;
using System.IO;
using NUnit.Framework;

namespace BeyondCompareSQLitePlugin.Test
{
    [TestFixture]
    public class ProgramTest : TestBase
    {
        [Test]
        public void Process_Start()
        {
            #region Act

            var returnCode = Program.Main(new[] { base.SampleSqlitePath, "sampleSqlite.txt" });

            #endregion

            #region Assert

            Assert.That(returnCode, Is.EqualTo(Program.Ok), "Return Code");
            Assert.That(File.ReadAllText("sampleSqlite.txt"), Does.Not.Contain("Exception"));

            // Process.Start("sampleSqlite.txt");

            #endregion
        }
    }
}
