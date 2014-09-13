using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using BeyondCompareSqlLite.Model;

namespace PowershellSQLite
{
    [Cmdlet(VerbsCommon.Select, "SQLite")]
    public class SelectSQlite : PSCmdlet , IDynamicParameters
    {
        private static readonly Dictionary<string, List<string>> TablesCache = new Dictionary<string, List<string>>();

        [Parameter(
            Mandatory = true,
            ValueFromPipelineByPropertyName = true,
            ValueFromPipeline = true,
            Position = 0,
            HelpMessage = "Path to an SQLite file"
            )]
        [ValidateNotNullOrEmpty]
        public string Path { get; set; }


        

        public string[] Tables { get; set; }

   //RuntimeDefinedParameterDictionary

        protected override void BeginProcessing()
        {
            WriteObject("BeginProcessing");

            if (!File.Exists(Path))
            {
                WriteError(new ErrorRecord(new FileNotFoundException("File not found", Path), "PowershellSQLite_Error:001", ErrorCategory.ObjectNotFound, Path));
            }

            base.BeginProcessing();
        }

        protected override void ProcessRecord()
        {
            WriteObject("ProcessRecord");
            base.ProcessRecord();
        }

        protected override void EndProcessing()
        {
            WriteObject("EndProcessing");
            base.EndProcessing();
        }


        public object GetDynamicParameters()
        {
            if (!File.Exists(Path)) return null;

            var tableNames = new List<string>();
            if (TablesCache.ContainsKey(Path))
            {
                tableNames = TablesCache[Path];
            }
            else
            {
                try
                {
                    tableNames = DbContext.GetTableNamesContent(Path);
                    tableNames.Add("All");
                    TablesCache.Add(Path, tableNames);
                }
                catch (Exception e)
                {

                }
            }


            var tableDynamicParameters = new TableDynamicParameters();
            var typeDescriptionProvider = TypeDescriptor.AddAttributes(tableDynamicParameters, new ValidateSetAttribute("Table1_New", "Table2_New"));

            return tableDynamicParameters;
        }
    }

    public class TableDynamicParameters
    {
        [Parameter]
        [ValidateSet("Table1", "Table2")]
        public string[] Tables { get; set; }
    }
}
