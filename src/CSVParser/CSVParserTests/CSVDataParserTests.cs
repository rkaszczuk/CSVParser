using Microsoft.VisualStudio.TestTools.UnitTesting;
using CSVParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using System.IO;
using System.Data;
using System.Text.RegularExpressions;

namespace CSVParser.Tests
{
    [TestClass()]
    public class CSVDataParserTests
    {
        private static MemoryStream csvStream;
        private static string[] csvHeaders;
        private static string[] csvRow1;
        private static string[] csvRow2;
        private static string[] csvRow3;

        [TestInitialize()]
        public void TestInit()
        {
            csvHeaders = new[] { "header1", "header2", "header3" };
            csvRow1 = new[] { "value11", "value12", "value13" };
            csvRow2 = new[] { @"""value21""", @"""value22""", @"""value23""" };
            csvRow3 = new[] { @"""value31,value31_b""", @"""value32,""""value32_b""", @"""value32"+Environment.NewLine + @"value33""" };

            StringBuilder CSVData = new StringBuilder();
            CSVData.Append(string.Join(",", csvHeaders) + Environment.NewLine);
            CSVData.Append(string.Join(",",csvRow1) + Environment.NewLine);
            CSVData.Append(string.Join(",",csvRow2) + Environment.NewLine);
            CSVData.Append(string.Join(",",csvRow3));
            byte[] byteArray = Encoding.UTF8.GetBytes(CSVData.ToString());
            csvStream = new MemoryStream(byteArray);
        }

        [TestMethod()]
        public void ParseFromStream_ParseHeaders_Pass()
        {
            var parser = new CSVDataParser();
            var result = parser.ParseFromStream(csvStream, Encoding.UTF8);

            result.Columns.Should().NotBeNullOrEmpty();
            result.Columns.Should().HaveSameCount(csvHeaders);
            result.Columns.Cast<DataColumn>().Select(x => x.ColumnName).Should().Equal(csvHeaders);

        }

        [TestMethod()]
        public void ParseFromStream_ParseSimpleRow_Pass()
        {
            var parser = new CSVDataParser();
            var result = parser.ParseFromStream(csvStream, Encoding.UTF8);

            result.Rows.Should().NotBeNullOrEmpty();
            result.Rows[0].ItemArray.Should().HaveSameCount(csvRow1);
            result.Rows[0].ItemArray.Should().Equal(csvRow1);
        }

        [TestMethod()]
        public void ParseFromStream_ParseRowStartAndEndWithQuotes_Pass()
        {
            var parser = new CSVDataParser();
            var result = parser.ParseFromStream(csvStream, Encoding.UTF8);

            result.Rows.Should().NotBeNullOrEmpty();
            result.Rows[1].ItemArray.Should().HaveSameCount(csvRow2);
            result.Rows[1].ItemArray.Should().Equal(csvRow2.Select(x=>x.Replace(@"""", "")).ToArray());
        }

        [TestMethod()]
        public void ParseFromStream_ParseRowWithDelimitersInQueotes_Pass()
        {
            var parser = new CSVDataParser();
            var result = parser.ParseFromStream(csvStream, Encoding.UTF8);

            result.Rows.Should().NotBeNullOrEmpty();
            result.Rows[2].ItemArray.Should().HaveSameCount(csvRow3);

            result.Rows[2].ItemArray.Should().Equal(csvRow3.Select(x => x.StartsWith(@"""") ? x.Remove(0,1) : x)
                .Select(x=>x.EndsWith(@"""") ? x.Remove(x.Length-1, 1) : x)
                .Select(x=>x.Replace(@"""""", @"""")).ToArray());
        }
    }
}