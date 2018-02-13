using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace BeyondCompareSQLitePlugin.Model.Test
{
    [TestFixture]
    public class DbContextTest : TestBase
    {
        [Test]
        [TestCase(SampleSqlite, 32, 0, 11, "Album", "fRwEEcT38Y5KiIDHcY0ucuRvhugvQWddyXMkX4Y8gDM=", 3)]
        [TestCase(SampleSqliteSecond, 54, 0, 11, "Album", "0ScP8ubkLHfxjIKhMaTGKeHrOFgZOCV6tHKaO3uFFHk=", 3)]
        public void GetTableContent(String name, Int32 schemaVersion, Int32 userVersion, Int32 tableContentCount,
            String firstTablename, String firstSchemaHash, Int32 firstColumnCount)
        {
            #region Arrange

            String path = Path.Combine(TestContext.CurrentContext.TestDirectory, name);

            #endregion

            #region Act

            var databaseContent = DbReader.CreateSummary(path);
            
            #endregion

            #region Assert

            Assert.That(databaseContent.SchemaVersion, Is.EqualTo(schemaVersion),
                nameof(databaseContent.SchemaVersion));
            Assert.That(databaseContent.UserVersion, Is.EqualTo(userVersion),
                nameof(databaseContent.UserVersion));
            Assert.That(databaseContent.Tables, Has.Count.EqualTo(tableContentCount),
                nameof(databaseContent.Tables));

            Assert.That(databaseContent.Tables[0].TableName, Is.EqualTo(firstTablename));
            Assert.That(databaseContent.Tables[0].SchemaHash, Is.EqualTo(firstSchemaHash));
            Assert.That(databaseContent.Tables[0].ColumnNames, Has.Count.EqualTo(firstColumnCount));

            #endregion
        }
    }
}