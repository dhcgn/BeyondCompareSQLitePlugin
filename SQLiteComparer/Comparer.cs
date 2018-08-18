using System;
using System.Collections.Generic;
using System.Linq;
using BeyondCompareSQLitePlugin.Model;

namespace SQLiteComparer
{
    public class Comparer
    {
        public Result CompareDataOnly(String path1, String path2, Options options)
        {
            var databaseSummary1 = DbReader.CreateSummary(path1);
            var databaseSummary2 = DbReader.CreateSummary(path2);

            IEnumerable<String> OrderTableNames(DatabaseSummary databaseSummary)
            {
                return databaseSummary.Tables.OrderBy(summary => summary.TableName).Select(summary => summary.TableName);
            }

            var sequenceEqual = OrderTableNames(databaseSummary1).SequenceEqual(OrderTableNames(databaseSummary2));
            if (!sequenceEqual)
            {
                return new Result
                {
                    HasDiff = true,
                    Reason = Reason.ListOfTable
                };
            }

            foreach (var tables1 in databaseSummary1.Tables.OrderBy(summary => summary.TableName))
            {
                var tables2 = databaseSummary2.Tables.Single(summary => summary.TableName == tables1.TableName);

                var equal = tables1.ColumnNames.OrderBy(s => s).SequenceEqual(tables2.ColumnNames.OrderBy(s => s));
                if(!equal)
                {
                    return new Result
                    {
                        HasDiff = true,
                        Reason = Reason.ListOfAttrbutes
                    };
                }
            }

            foreach (var table1 in databaseSummary1.Tables.OrderBy(summary => summary.TableName))
            {
                var table2 = databaseSummary2.Tables.Single(summary => summary.TableName == table1.TableName);

                var rowDimension = 1;
                if (table1.Data.GetLength(rowDimension) != table2.Data.GetLength(rowDimension))
                {
                    return new Result
                    {
                        HasDiff = true,
                        Reason = Reason.AmountRows
                    };
                }

                var equal = table1.Data.Cast<string>().SequenceEqual(table2.Data.Cast<string>());
                if(!equal)
                {
                    return new Result
                    {
                        HasDiff = true,
                        Reason = Reason.Data
                    };
                }
            }

            return new Result();
        }

        public Result CompareSchemaOnly(String path1, String path2, Options options)
        {
            return new Result();
        }
    }

    public enum Reason
    {
        None = 0, 
        ListOfTable,
        Data,
        ListOfAttrbutes,
        AmountRows
    }
}