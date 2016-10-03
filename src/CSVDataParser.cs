using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CSVParser
{
    public class CSVDataParser : IDataParser
    {
        //Default values for parser delimiters based on RFC 4180
        //Can be move to configuration class
        static readonly string DefaultColumnDelimiter = ",";
        static readonly string DefaultRowDelimiter = Environment.NewLine;
        static readonly Encoding DefaultEncoding = Encoding.UTF8;
        const char Quote = '"';


        private Dictionary<string, string> filters;
        private string columnDelimiter;
        private string rowDelimiter;
        /// <summary>
        /// Constructor
        /// </summary>
        public CSVDataParser() : this(DefaultColumnDelimiter, DefaultRowDelimiter) { }
        /// <summary>
        /// Optional constructor to change default values for delimiters
        /// </summary>
        /// <param name="columnDelimiter">Column delimiter</param>
        /// <param name="rowDelimiter">Row delimiter</param>
        public CSVDataParser(string columnDelimiter, string rowDelimiter)
        {
            this.columnDelimiter = columnDelimiter;
            this.rowDelimiter = rowDelimiter;
            filters = new Dictionary<string, string>();
        }
        /// <summary>
        /// Parse CSV data from file
        /// </summary>
        /// <param name="path">Path to CSV file</param>
        /// <returns></returns>
        public DataTable ParseFromFile(string path)
        {
            return ParseFromFile(path, DefaultEncoding);
        }
        /// <summary>
        /// Parse CSV data from file
        /// </summary>
        /// <param name="path">Path to CSV file</param>
        /// <param name="encoding">File encoding</param>
        /// <returns></returns>
        public DataTable ParseFromFile(string path, Encoding encoding)
        {
            return ParseFromStream(new FileStream(path,FileMode.Open, FileAccess.Read), encoding);
        }

        public DataTable ParseFromStream(Stream stream, Encoding encoding)
        {
            DataTable result = new DataTable();
            var rowsCollection = ReadRows(new StreamReader(stream, encoding)).ToList();

            if (!rowsCollection.Any())
            {
                throw new InvalidDataException("CSV file hasn't any rows");
            }
            string[] columns = ReadCellsFromRow(rowsCollection.First()).ToArray();
            result.Columns.AddRange(columns.Select(x => new DataColumn(x)).ToArray());

            foreach (var row in rowsCollection.Skip(1))
            {
                if (String.IsNullOrWhiteSpace(row))
                {
                    //Exception for empty rows in CSV file (can be deleted if we accept emtpy rowes)
                    throw new InvalidDataException("CSV file has an empty rows");
                }
                var rowValues = ReadCellsFromRow(row).ToArray();
                if (!filters.Any() || IfRowMatchFilters(columns, rowValues))
                {
                    result.Rows.Add(rowValues);
                }
            }

            return result;
        }
        /// <summary>
        /// Add new filter condition
        /// </summary>
        /// <param name="column">Filter column</param>
        /// <param name="value">Filter value</param>
        public void AddFilter(string column, string value)
        {
            filters.Add(column, value);
        }

        private bool IfRowMatchFilters(string[] columns, string[] values)
        {
            foreach(var filter in filters)
            {
                if (columns.Contains(filter.Key))
                {
                    if(values[Array.IndexOf(columns, filter.Key)] != filter.Value)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private IEnumerable<string> ReadRows(StreamReader reader)
        {
            StringBuilder line = new StringBuilder();
            var readBuffer = String.Empty;
            var valueInQuotes = false;
            while (reader.Peek() >= 0)
            {

                char currentChar = (char)reader.Read();
                    
                if (currentChar == Quote){
                    valueInQuotes = !valueInQuotes;                       
                }
                readBuffer += currentChar;

                //ignore row delimiter if value is in quotes
                if (rowDelimiter == readBuffer && !valueInQuotes)
                {
                    yield return line.ToString();
                    readBuffer = String.Empty;
                    line.Clear();
                }
                else if (rowDelimiter.StartsWith(readBuffer) && !valueInQuotes)
                {
                    //ignore part of delimiter - waiting for the next value
                    continue;
                }
                else
                {
                    line.Append(readBuffer);
                    readBuffer = String.Empty;
                }
                    
            }
            //return last line without row delimiter
            if (line.Length > 0)
            {
                yield return line.ToString();
            }
            
        }

        //TODO: Extract some duplicated logic from ReadRows and ReadCellsFromRow to another method 
        private IEnumerable<string> ReadCellsFromRow(string rowData)
        {
            bool valueInQuotes = false;
            StringBuilder cellValue = new StringBuilder();
            var readBuffer = String.Empty;
            for (int i = 0; i < rowData.Length; i++)
            {
                var currentChar = rowData[i];
                var escapeChar = false;
                if (currentChar == Quote)
                {
                    //if double quotes in quoted value
                    if(valueInQuotes && i + 1 < rowData.Length && rowData[i + 1] == Quote)
                    {
                        //escape first quote mark
                        i++;
                    }
                    else
                    {
                        //escape character if single quote
                        escapeChar = true;
                        valueInQuotes = !valueInQuotes;

                    }                    
                }
                if (!escapeChar)
                {
                    readBuffer += currentChar;
                }
                //ignore column delimiter if value is in quotes
                if (columnDelimiter == readBuffer && !valueInQuotes)
                {
                    yield return cellValue.ToString();
                    readBuffer = String.Empty;
                    cellValue.Clear();
                }
                else if (columnDelimiter.StartsWith(readBuffer) && !valueInQuotes)
                {
                    //ignore part of delimiter - waiting for the next value
                    continue;
                }
                else
                {
                    cellValue.Append(readBuffer);
                    readBuffer = String.Empty;
                }
            }
            //return last cell value
            yield return cellValue.ToString();
        }
    }
}
