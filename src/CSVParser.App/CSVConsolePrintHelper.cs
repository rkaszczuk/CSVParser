using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSVParser.App
{
    public class CSVConsolePrintHelper
    {
        private DataTable dataSource;
        private Dictionary<int, int> columnsTextSize;
        private int currentRow = 0;
        const string CellSeparationSymbol = "|";
        public CSVConsolePrintHelper(DataTable dataSource, OutputAlign outputAlign = OutputAlign.None)
        {
            columnsTextSize = new Dictionary<int, int>();
            this.dataSource = dataSource;
            switch (outputAlign)
            {
                case OutputAlign.BasedOnCells:
                    SetColumnsSizeBasedOnCellsTextInColumn(dataSource);
                    break;
                case OutputAlign.BasedOnlyOnColumnName:
                    SetColumnsSizeBasedOnColumnLabelText(dataSource.Columns);
                    break;
            }


        }
        private void SetColumnsSizeBasedOnColumnLabelText(DataColumnCollection columns)
        {

            for (int i = 0; i < columns.Count; i++)
            {
                columnsTextSize.Add(i, columns[i].ColumnName.Length);
            }
        }
        //Possible performance problems with big data sources
        private void SetColumnsSizeBasedOnCellsTextInColumn(DataTable data)
        {
            columnsTextSize = Enumerable.Range(0, dataSource.Columns.Count)
             .Select(column => 
             new KeyValuePair<int, int>(
                 column, 
                 Math.Max(dataSource.AsEnumerable().Select(row => row[column] as string).Max(cell => cell.Length), dataSource.Columns[column].ColumnName.Length))).ToDictionary(x => x.Key, x => x.Value);
        }


        private void SetCursorAlign(int columnStartPosition)
        {
            Console.SetCursorPosition(columnStartPosition % Console.BufferWidth, Console.CursorTop);
        }


        private void AlignToColumn(int columnNumber)
        {
            if (columnsTextSize.ContainsKey(columnNumber))
            {
                var columnStartPosition = columnsTextSize.Where(x => x.Key < columnNumber).Sum(x => x.Value);
                SetCursorAlign(columnStartPosition);
            }
            
        }

        private void PrintRowEnd()
        {
            if (columnsTextSize.Any())
            {
                SetCursorAlign(columnsTextSize.Sum(x => x.Value));
            }
            Console.Write("|");
            Console.WriteLine();
        }

        public void PrintCSVHeaders()
        {          
            for (int i = 0; i < dataSource.Columns.Count; i++)
            {
                AlignToColumn(i);
                Console.Write("|"+dataSource.Columns[i]);
            }
            PrintRowEnd();
        }
        public bool PrintNextCSVRow()
        {
            PrintRow(currentRow);
            currentRow++;
            return dataSource.Rows.Count > currentRow;
        }


        public bool PrintRow(int rowNumber)
        {
            if(dataSource.Rows.Count <= rowNumber)
            {
                return false;
            }
            for (int i = 0; i < dataSource.Rows[rowNumber].ItemArray.Length; i++)
            {
                AlignToColumn(i);
                Console.Write("|" + dataSource.Rows[rowNumber].ItemArray[i]);
            }
            PrintRowEnd();
            return true;
        }
        public void PrintAll()
        {
            PrintCSVHeaders();
            while (PrintNextCSVRow()) { }
        }

        public void PrintPage(int pageNumber, int pageSize = 10)
        {
            PrintCSVHeaders();
            for (int i = 0; i < pageSize; i++)
            {
                if(!PrintRow((pageNumber-1) * pageSize + i))
                {
                    return;
                }
            }
        }

        public bool HasNextPage(int currentPageName, int pageSize = 10)
        {
            return dataSource.Rows.Count >= (currentPageName - 1) * pageSize + pageSize;
        }
    }
}
