using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text.RegularExpressions;
using BeyondCompareSqlLite.Model;

namespace PowershellSQLite
{
    [Cmdlet(VerbsCommon.Select, "SQLite")]
    public class SelectSQlite : PSCmdlet, IDynamicParameters
    {
        private const string FormatRaw = "Raw";
        private const string FormatHumanReadable = "HumanReadable";
        private static readonly Dictionary<string, List<string>> TablesCache = new Dictionary<string, List<string>>();
        private static RuntimeDefinedParameterDictionary _staticStroage;
        private string _format = FormatRaw;

        [Parameter(
            Mandatory = true,
            ValueFromPipelineByPropertyName = true,
            ValueFromPipeline = true,
            Position = 0,
            HelpMessage = "Path to an SQLite file"
            )]
        [ValidateNotNullOrEmpty]
        public FileInfo Path { get; set; }

        [Parameter(
            Mandatory = false,
            Position = 1,
            HelpMessage = "For Select-String use \"Raw\" for better readablility use \"HumanReadable\""
            )]
        [ValidateSet(new[] {FormatHumanReadable, FormatRaw}, IgnoreCase = true)]
        public string Format
        {
            get { return _format; }
            set { _format = value; }
        }


        public object GetDynamicParameters()
        {
            if (Path == null || !Path.Exists) return GetRuntimeDefinedParameterDictionary(null);

            var tableNames = new List<string>();
            if (TablesCache.ContainsKey(Path.FullName))
            {
                tableNames = TablesCache[Path.FullName];
            }
            else
            {
                try
                {
                    tableNames = DbContext.GetTableNamesContent(Path.FullName);
                    tableNames.Add("All");
                    TablesCache.Add(Path.FullName, tableNames);
                }
                catch (Exception e)
                {
                }
            }

            _staticStroage = GetRuntimeDefinedParameterDictionary(tableNames);
            return _staticStroage;
        }

        private static RuntimeDefinedParameterDictionary GetRuntimeDefinedParameterDictionary(List<string> tableNames)
        {
            var runtimeDefinedParameterDictionary = new RuntimeDefinedParameterDictionary();
            var attributes = new Collection<Attribute>
            {
                new ParameterAttribute
                {
                    Position = 2,
                    HelpMessage = "Table names"
                },
            };

            if (tableNames != null && tableNames.Any())
            {
                attributes.Add(new ValidateSetAttribute(tableNames.ToArray()));
            }
            
            runtimeDefinedParameterDictionary.Add("Tables", new RuntimeDefinedParameter("Tables", typeof (string[]), attributes));
            return runtimeDefinedParameterDictionary;
        }

        protected override void BeginProcessing()
        {
            WriteObject("BeginProcessing");

            if (!Path.Exists)
            {
                WriteError(new ErrorRecord(new FileNotFoundException("File not found", Path.FullName), "PowershellSQLite_Error:001", ErrorCategory.ObjectNotFound, Path));
            }

            base.BeginProcessing();
        }

        protected override void ProcessRecord()
        {
            var tables = new List<string>();

            KeyValuePair<string, RuntimeDefinedParameter> runtimeDefinedParameterTables = _staticStroage.FirstOrDefault(x => x.Key == "Tables");
            if (runtimeDefinedParameterTables.Value != null)
            {
                tables = new List<string>(runtimeDefinedParameterTables.Value.Value as string[]);
            }
            WriteProgress(new ProgressRecord(1, "Reading SQLite database.", "File: " + Path.FullName));

            DatabaseContent databaseContent = DbContext.GetTableContent(Path.FullName, tables);

            string[] output = new string[0];

            switch (Format)
            {
                case FormatRaw:
                    output = Report.CreateTextReportPowershell(databaseContent, Path.FullName);
                    break;
                case FormatHumanReadable:
                    output = Report.CreateTextReport(databaseContent);
                    break;
            }
            WriteProgress(new ProgressRecord(1, "Output SQLite content.", "Lines: " + output.Length));
            WriteObject(output, true);

            base.ProcessRecord();
        }

        protected override void EndProcessing()
        {
            base.EndProcessing();
        }
    }
}