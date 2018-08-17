using System;
using System.IO;
using NUnit.Framework;

namespace BeyondCompareSQLitePlugin.Model.Test
{
    [TestFixture]
    public class ReportTest : TestBase
    {
        [Test]
        [TestCase(SampleSqlite, 15_688, true)]
        [TestCase(SampleSqliteSecond, 15_689, true)]
        [TestCase(SampleSqliteEscapeNeeded, 15_688, true)]

        [TestCase(SampleSqlite, 37, false)]
        [TestCase(SampleSqliteSecond, 37, false)]
        [TestCase(SampleSqliteEscapeNeeded, 37, false)]
        public void CreateSummary_CreateTextReport(String name, Int32 lines, bool containsData)
        {
            #region Arrange

            String path = Path.Combine(TestContext.CurrentContext.TestDirectory, name);

            #endregion

            #region Act

            var databaseContent = DbReader.CreateSummary(path);
            var stringContent = Report.CreateTextReport(databaseContent, containsData);

            #endregion

            #region Assert

            Assert.That(stringContent.Split('\n'), Has.Length.EqualTo(lines));

            if (containsData)
            {
                Assert.That(stringContent, Does.Contain("For Those About To Rock We Salute You"));
            }
            else
            {
                Assert.That(stringContent, Does.Not.Contain("For Those About To Rock We Salute You"));
            }

            #endregion
        }

        [Test]
        [TestCase(EmptySqlite, 15, false)]
        public void CreateSummary_CreateTextReport_EmptyDb(String name, Int32 lines, bool containsData)
        {
            #region Arrange

            String path = Path.Combine(TestContext.CurrentContext.TestDirectory, name);

            #endregion

            #region Act

            var databaseContent = DbReader.CreateSummary(path);
            var stringContent = Report.CreateTextReport(databaseContent, containsData);

            #endregion

            #region Assert

            Assert.That(stringContent.Split('\n'), Has.Length.EqualTo(lines));

            #endregion
        }

        [Test]
        [TestCase(SampleSqlite, 1_939_856)]
        [TestCase(SampleSqliteSecond, 1_933_464)]
        [TestCase(EmptySqlite, 871)]
        [TestCase(SampleSqliteEscapeNeeded, 1_957_773)]
        public void CreateSummary_WriteTextReportToFile(String name, Int32 length)
        {
            #region Arrange

            String path = Path.Combine(TestContext.CurrentContext.TestDirectory, name);

            #endregion

            #region Act

            var databaseContent = DbReader.CreateSummary(path);
            Report.WriteTextReportToFile(databaseContent, this.TestFile1);

            #endregion

            #region Assert

            Assert.That(new FileInfo(this.TestFile1), Has.Length.EqualTo(length));

            #endregion
        }
    }
}
