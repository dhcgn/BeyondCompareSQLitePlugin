using System;
using System.Management.Automation;
using System.Text.RegularExpressions;
using BeyondCompareSqlLite.Model;

namespace PowershellSQLite
{
    [Cmdlet(VerbsData.Export, "SQLite")]
    public class ExportSQlite : PSCmdlet
    {
        private const string FormatRaw = "Raw";
        private const string FormatHumanReadable = "HumanReadable";

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
        private string _format = FormatRaw;

        protected override void ProcessRecord()
        {
            try
            {
                ProcessRecordInternal();
            }
            catch (Exception e)
            {
                WriteError(new ErrorRecord(e, "NotSpecified", ErrorCategory.NotSpecified, null));
            }

            base.ProcessRecord();
        }

        private void ProcessRecordInternal()
        {
            DatabaseContent databaseContent = DbContext.GetTableContent(Path);

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
        }
    }
}