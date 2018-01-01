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
        [TestCase(SampleSqlite, 32, 0, 11, "Album", "da49fafbba60b375030169ab9bc1ce05", 3)]
        [TestCase(SampleSqliteSecond, 54, 0, 11, "Album", "d25b1472de06f98e260f74d63e1f6c41", 3)]
        public void GetTableContent(string name, int schemaVersion, int userVersion, int tableContentCount,
            string firstTablename, string firstSchemaHash, int firstColumnCount)
        {
            #region Arrange

            string path = Path.Combine(TestContext.CurrentContext.TestDirectory, name);

            #endregion

            #region Act

            var databaseContent = DbContext.GetTableContent(path);
            

            #endregion

            #region Assert

            Assert.That(databaseContent.SchemaVersion, Is.EqualTo(schemaVersion),
                nameof(databaseContent.SchemaVersion));
            Assert.That(databaseContent.UserVersion, Is.EqualTo(userVersion),
                nameof(databaseContent.UserVersion));
            Assert.That(databaseContent.TableContent, Has.Count.EqualTo(tableContentCount),
                nameof(databaseContent.TableContent));

            Assert.That(databaseContent.TableContent[0].TableName, Is.EqualTo(firstTablename));
            Assert.That(databaseContent.TableContent[0].SchemaHash, Is.EqualTo(firstSchemaHash));
            Assert.That(databaseContent.TableContent[0].ColumnNames, Has.Count.EqualTo(firstColumnCount));

            #endregion
        }
    }
}