using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.Common;

namespace TransferData.Model
{
    public class DbDataExtractor
    {
        private readonly DataContext _data;
        private readonly MetadataExtractor _metadataExtractor;

        public DbDataExtractor(DataContext data, MetadataExtractor metadataExtractor)
        {
            _data = data;
            _metadataExtractor = metadataExtractor;
        }

        private string GenerateSelectQuary(SchemaInfo schema) => $"select {String.Join(", ", schema.Fields.Select(x => x.FieldName).ToList())} from {schema.TableName}";

        private DataTable GetDataTable(string tableName)
        {
            var schema = _metadataExtractor.GetTableSchema(tableName);

            string sqlQuery = GenerateSelectQuary(schema);

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

        public List<List<string>> ConvertDataTableToList(string tableName)
        {

            var dataTable = GetDataTable(tableName);

            var resultList = new List<List<string>>();
            foreach (DataRow row in dataTable.Rows)
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
