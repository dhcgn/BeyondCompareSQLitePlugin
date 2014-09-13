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
        public string Path { get; set; }

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

        public string[] Tables { get; set; }

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

            var runtimeDefinedParameterDictionary = new RuntimeDefinedParameterDictionary();
            runtimeDefinedParameterDictionary.Add("Tables",
                new RuntimeDefinedParameter("Tables", typeof (string[]),
                    new Collection<Attribute> {new ParameterAttribute(), new ValidateSetAttribute(tableNames.ToArray())}));


            _staticStroage = runtimeDefinedParameterDictionary;


            return runtimeDefinedParameterDictionary;
        }

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
            var tables = new List<string>();

            KeyValuePair<string, RuntimeDefinedParameter> runtimeDefinedParameterTables = _staticStroage.FirstOrDefault(x => x.Key == "Tables");
            if (runtimeDefinedParameterTables.Value != null)
            {
                tables = new List<string>(runtimeDefinedParameterTables.Value.Value as string[]);
            }

            DatabaseContent databaseContent = DbContext.GetTableContent(Path, tables);

            string output = String.Empty;

            switch (Format)
            {
                case FormatRaw:
                    output = Report.CreateTextReportPowershell(databaseContent);
                    break;
                case FormatHumanReadable:
                    output = Report.CreateTextReport(databaseContent);
                    break;
            }

            string[] result = Regex.Split(output, "\r\n|\r|\n");

            WriteObject(result, true);

            base.ProcessRecord();
        }

        protected override void EndProcessing()
        {
            base.EndProcessing();
        }
    }
}