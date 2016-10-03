using System.Data;
using System.Text;

namespace CSVParser
{
    public interface IDataParser
    {
        DataTable ParseFromFile(string path, Encoding encoding);
        DataTable ParseFromFile(string path);
        void AddFilter(string column, string value);
    }
}