using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.Common;
using TransferData.Model.Infrastructure;
using TransferData.Model.Models;

namespace TransferData.Model.Services
{
    public class DbDataExtractor
    {
        private readonly DataContext _dataContext;
        private readonly MetadataExtractor _metadataExtractor;

        public DbDataExtractor(DataContext dataContext, MetadataExtractor metadataExtractor)
        {
            _dataContext = dataContext;
            _metadataExtractor = metadataExtractor;
        }

        private string GenerateSelectQuary(SchemaInfo schema) => $"select * from {schema.TableName}";

        private DataTable GetDataTable(string tableName)
        {
            var schema = _metadataExtractor.GetTableSchema(tableName);

            string sqlQuery = GenerateSelectQuary(schema);

            var dbFactory = DbProviderFactories.GetFactory(_dataContext.Database.GetDbConnection());

            using (var cmd = dbFactory.CreateCommand())
            {
                cmd.Connection = _dataContext.Database.GetDbConnection();
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
            var schema = _metadataExtractor.GetTableSchema(tableName);
            var dataTable = GetDataTable(tableName);

            var resultList = new List<List<string>>();
            foreach (DataRow row in dataTable.Rows)
            {
                object[] cells = row.ItemArray;
                var cellsList = new List<string>();
                for (int i = 0; i < cells.Length; i++)
                {
                    object cell = cells[i];
                    if (cell is null)
                        cellsList.Add("null");
                    else if (schema.Fields[i].FieldType != "geography")
                        cellsList.Add(cell.ToString().Replace(',', '.'));
                    else
                        cellsList.Add(cell.ToString());
                }
                resultList.Add(cellsList);
            }
            return resultList;
        }

    }
}
