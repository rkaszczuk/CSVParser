# CSVParser
Recruiting exercise

###Command line parameters
CSVParser.App.exe <csv_file_path> *options*

* -Encoding - File encoding code page name
* -Column - Column name for filter
* -Value - Value for filter
* -EnablePagination - Enable pagination for output (default = false)
* -PageSize - Optional page size for pagination (default = 10)
* -ColumnDelimiter - Custom delimiter for columns
* -RowDelimiter - Custom delimiter for rows
* -OutputAlign - Type of output align: 
 * BasedOnlyOnColumnName - algin will be calculate using column text length 
 * BasedOnCells - align will be calculate using length of cells values