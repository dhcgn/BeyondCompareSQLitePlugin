using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace SQLiteComparer.Test
{
    [TestFixture]
    public class CompareTest : TestBase
    {
        [TestCase(SampleSqliteDiffTuple_1, SampleSqliteDiffTuple_1, Reason.None, false)]
        [TestCase(SampleSqliteDiffTuple_1, SampleSqliteDiffTuple_1, Reason.ListOfTable, false)]
        [TestCase(SampleSqliteDiffTuple_1, SampleSqliteDiffTuple_1, Reason.ListOfAttrbutes, false)]
        [TestCase(SampleSqliteDiffTuple_1, SampleSqliteDiffTuple_2, Reason.Data, true)]
        [TestCase(SampleSqliteDiffRows_1, SampleSqliteDiffRows_2, Reason.AmountRows, true)]
        public void CompareDataOnlyTest(String path1, String path2, Reason reason, bool hasDiff)
        {
            #region Arrange

            var comparer = new SQLiteComparer.Comparer();
            path1 = Path.Combine(TestContext.CurrentContext.TestDirectory, path1);
            path2 = Path.Combine(TestContext.CurrentContext.TestDirectory, path2);

            #endregion

            #region Act

            var compareDataOnly = comparer.CompareDataOnly(path1, path2, new Options());

            #endregion

            #region Assert

            Assert.That(compareDataOnly.HasDiff, Is.EqualTo(hasDiff), nameof(Result.HasDiff));
            Assert.That(compareDataOnly.Reason, Is.EqualTo(reason), nameof(Result.Reason));

            #endregion
        }
    }
}
