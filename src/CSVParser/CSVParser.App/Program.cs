using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSVParser.App
{
    class Program
    {
        static void Main(string[] args)
        {
            var commandLineParams = new CommandLineOptions();
            //Convert single dash to double dash to pass specification
            CommandLine.Parser.Default.ParseArguments(args.Select(x=>x.Replace("-","--")).ToArray(), commandLineParams);

            IDataParser parser;

            //Optional delimiters
            if(!String.IsNullOrEmpty(commandLineParams.ColumnDelimiter) || !String.IsNullOrEmpty(commandLineParams.RowDelimiter))
            {
                parser = new CSVDataParser(commandLineParams.ColumnDelimiter, commandLineParams.RowDelimiter);
            }
            else
            {
                parser = new CSVDataParser();
            }
            
            //Filter
            //Accept empty strings as filter value
            if (!String.IsNullOrWhiteSpace(commandLineParams.Column))
            {
                parser.AddFilter(commandLineParams.Column, commandLineParams.Value);
            }


            DataTable result;
            //Encoding
            if (!String.IsNullOrWhiteSpace(commandLineParams.Encoding))
            {
                var encoding = Encoding.GetEncoding(commandLineParams.Encoding);
                result = parser.ParseFromFile(commandLineParams.FilePath, encoding);
            }
            else
            {
                result = parser.ParseFromFile(commandLineParams.FilePath);
            }


            //Pagination
            var printer = new CSVConsolePrintHelper(result, commandLineParams.OutputAlign);
            if (commandLineParams.EnablePagination)
            {
                var pageNumber = 0;
                var pageSize = commandLineParams.PageSize ?? 10; 
                while(printer.HasNextPage(pageNumber, pageSize))
                {
                    pageNumber++;
                    printer.PrintPage(pageNumber, pageSize);
                    Console.WriteLine("Page: " + pageNumber);
                    Console.WriteLine("Press any key to view next page...");
                    Console.ReadKey();
                    Console.Clear();                  
                }
            }
            else
            {
                printer.PrintAll();
            }

            Console.ReadKey();
        }
    }
}
