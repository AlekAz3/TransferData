
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.Common;

namespace TransferData.Model
{
    public class DbDataExtractor
    {
        private readonly DataContext _data;

        public DbDataExtractor(DataContext data)
        {
            _data = data;
        }

        public DataTable GetDataTable(string sqlQuery)
        {
            var dbFactory = DbProviderFactories.GetFactory(_data.Database.GetDbConnection());

            using (var cmd = dbFactory.CreateCommand())
            {
                cmd.Connection = _data.Database.GetDbConnection();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sqlQuery;
                using (var adapter = dbFactory.CreateDataAdapter())
                {
                    adapter.SelectCommand = cmd;
                    var dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    return dataTable;
                }
            }
        }

        public List<List<string>> ConvertDataTableToList(DataTable dt)
        {
            var resultList = new List<List<string>>();
            foreach (DataRow row in dt.Rows)
            {
                object[] cells = row.ItemArray;
                var cellsList = new List<string>();
                foreach (object cell in cells)
                {
                    if (cell is null)
                        cellsList.Add("null");
                    else
                        cellsList.Add(cell.ToString().Replace(',', '.'));
                }
                resultList.Add(cellsList);
            }
            return resultList;
        }

    }
}
