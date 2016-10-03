using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSVParser.App
{
    public enum OutputAlign
    {
        None,
        BasedOnlyOnColumnName,
        BasedOnCells
    }
    public class CommandLineOptions
    {
        [ValueOption(0)]
        public string FilePath { get; set; }
        [Option(HelpText ="File encoding code page name")]
        public string Encoding { get; set; }
        [Option(HelpText ="Column name for filter")]
        public string Column { get; set; }
        [Option(HelpText ="Value for filter")]
        public string Value { get; set; }
        [Option(HelpText ="Enable pagination for output", DefaultValue = false)]
        public bool EnablePagination { get; set; }
        [Option(HelpText ="Page size for pagination")]
        public int? PageSize { get; set; }
        [Option(HelpText ="Delimiter for columns")]
        public string ColumnDelimiter { get; set; }
        [Option(HelpText = "Delimiter for rows")]
        public string RowDelimiter { get; set; }
        [Option(HelpText ="Type of output align: BasedOnlyOnColumnName - algin will be calculate using column text length, BasedOnCells - align will be calculate using length of cells values", DefaultValue =OutputAlign.None)]
        public OutputAlign OutputAlign { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            var help = new HelpText
            {
                Heading = new HeadingInfo("CSVParser"),
                Copyright = new CopyrightInfo("Rafał Kaszczuk", 2016),
                AdditionalNewLineAfterOption = true,
                AddDashesToOption = true
            };
            help.AddOptions(this);
            return help;
        }
    }
}
