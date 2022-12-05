using System.Text;

namespace TransferData.Model
{
    public class TransferPostgreSQL : ITransfer
    {
        private readonly DbDataExtractor _dataExtractor;
        private readonly DbSchemaExtractor _schemaExtractor;
        private readonly DbDataHelper _dataHelper;
        private readonly DataContext _data;

        public TransferPostgreSQL(DbDataExtractor dataExtractor, DbSchemaExtractor schemaExtractor, DbDataHelper dataHelper, DataContext data)
        {
            _dataExtractor = dataExtractor;
            _schemaExtractor = schemaExtractor;
            _dataHelper = dataHelper;
            _data = data;
        }

        public string GenerateMergeQuary(string tableName)
        {
            var schema = _schemaExtractor.GetTableSchema(tableName).Result;
            var command = new StringBuilder();
            var columns = schema.Fields.Select(x => x.FieldName).ToList();
            string firsColumn = columns[0];
            columns.RemoveAt(0);
            string columsJoin = String.Join(", ", columns);

            command.AppendLine($"with upsert({firsColumn}) as");
            command.AppendLine($"(");
            command.AppendLine($"insert into {schema.TableName} ({columsJoin})");
            command.AppendLine($"select {columsJoin} from Temp{schema.TableName}");
            command.AppendLine($"on conflict ({firsColumn}) do update set ");
            for (int i = 0; i < columns.Count() - 1; i++)
            {
                command.AppendLine($"{columns[i]} = excluded.{columns[i]},");
            }

            command.AppendLine($"{columns[columns.Count() - 1]} = excluded.{columns[columns.Count() - 1]}");

            command.AppendLine($"returning {firsColumn} )");
            command.AppendLine($"");
            command.AppendLine($"--delete from {schema.TableName} where {firsColumn} not in (select {firsColumn} from upsert);");

            return command.ToString();
        }


        public string GenerateTempTableQuary(string tableName)
        {
            var schema = _schemaExtractor.GetTableSchema(tableName).Result;
            string columnsJoin = String.Join(", ", schema.Fields.Select(x => x.FieldName));
            var tableData = _dataExtractor.ConvertDataTableToList(tableName);

            var command = new StringBuilder();
            command.AppendLine($"select {columnsJoin} into temp table Temp{schema.TableName} from");
            command.AppendLine("( ");
            for (int i = 0; i < tableData.Count - 1; i++)
                command.AppendLine($"select {_dataHelper.JoinWithQuetes(tableData[i], schema, _data.type)} union all");
            command.AppendLine($"select {_dataHelper.JoinWithQuetes(tableData[tableData.Count - 1], schema, _data.type)}");

            command.AppendLine(") as dt");

            return command.ToString();
        }
    }
}
