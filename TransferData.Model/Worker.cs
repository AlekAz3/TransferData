using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;

namespace TransferData.Model
{
    public class Worker
    {
        private readonly DataContext _data;
        private readonly IConfiguration _config;
        private readonly ILogger<Worker> _log;

        public Worker(ILogger<Worker> log, IConfiguration config, DataContext data)
        {
            _data = data;
            _config = config;
            _log = log;
        }

        public void Run(string table_name)
        {
            //if (_data.type == DbType.MSSQL)
            //    return;

            //string a = "select table_name, column_name, data_type from information_schema.columns where table_name = 'table1' or table_name = 'table2'";

            //string a = "select table_name, column_name, data_type from information_schema.columns where table_name = 'table1' or table_name = 'table2'";

            //SqlDataAdapter adapter = new SqlDataAdapter(a, _data);

            //var b = _data.schema.FromSqlRaw($"select table_schema,table_name, column_name, data_type from information_schema.columns").ToList();

            //var b = _data.schema.Where(x => x.table_schema == "public");

            //var c = _data.information_schema.Where(x => x.table_schema == "public");
            //var b = _data.schema.FromSqlRaw($"select table_name, column_name, data_type from information_schema.columns where table_schema = 'public'");


            //foreach (var item in b)
            //{
            //    _log.LogInformation($"{item.table_name}, {item.column_name}, {item.data_type}");
            //}

            //DbSchemaExtractor schema = new DbSchemaExtractor(_data);

            //Task<SchemaInfo> info = schema.GetTableSchema(table_name);

            //info.Wait();

            //var res = info.Result;

            //_log.LogInformation($"{res.TableName} ");
            //_log.LogInformation("FieldName : FieldType");
            //foreach (var item in res.Fields)
            //{
            //    _log.LogInformation($"{item.FieldName}:{item.FieldType}");
            //}

            var schema = new DbSchemaExtractor(_data).GetTableSchema(table_name).Result;

            var transfer = new Transfer(_data, schema, DbType.Postgres);


            //var values = transfer.ConvertDataTableToList(transfer.GetDataTable(transfer.GenerateSelectQuary()));

            //foreach (var ItemList in values)
            //{
            //    foreach (var item in ItemList)
            //    {
            //        _log.LogInformation(item);
            //    }
            //    _log.LogInformation(" ");
            //}

            //foreach (var item in schema.Fields)
            //{
            //    _log.LogInformation($"{item.FieldName} : {item.FieldType}");
            //}

            var a = transfer.GenerateTempTableQuary();
            _log.LogInformation(a);

            _log.LogInformation(" ");

            _log.LogInformation(transfer.GenerateMergeQuary());
        }
    }
}
