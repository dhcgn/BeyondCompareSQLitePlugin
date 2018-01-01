using System.Diagnostics;
using NUnit.Framework;

namespace BeyondCompareSQLitePlugin.Test
{
    [TestFixture]
    public class ProgramTest : TestBase
    {
        [Test]
        public void Process_Start()
        {
            Program.Main(new[] { base.SampleSqlitePath, "sampleSqlite.txt" });
            Process.Start("sampleSqlite.txt");
        }
    }
}
