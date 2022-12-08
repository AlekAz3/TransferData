using System.Text;

namespace TransferData.Model
{
    public class TransferMSSQL : ITransfer
    {
        private readonly DbDataExtractor _dataExtractor;
        private readonly MetadataExtractor _metadataExtractor;
        private readonly DataContext _data;

        public TransferMSSQL(DbDataExtractor dataExtractor, MetadataExtractor metadataExtractor, DataContext data)
        {
            _dataExtractor = dataExtractor;
            _metadataExtractor = metadataExtractor;
            _data = data;
        }

        public string GenerateMergeQuery(string tableName)
        {
            var schema = _metadataExtractor.GetTableSchema(tableName);
            var command = new StringBuilder();
            var columns = schema.Fields.Select(x => x.FieldName).ToList();
            string firsColumn = columns[0];
            columns.RemoveAt(0);
            string columsJoin = String.Join(", ", columns);

            command.AppendLine($"merge {schema.TableName} AS T_Base ");
            command.AppendLine($"using #Temp{schema.TableName} AS T_Source ");
            command.AppendLine($"on (T_Base.{schema.Fields[0].FieldName} = T_Source.{schema.Fields[0].FieldName}) ");
            command.AppendLine($"when matched then ");
            command.AppendLine($"update set {schema.SetValuesSubQuery()} ");
            command.AppendLine($"when not matched then ");
            command.AppendLine($"insert ({String.Join(", ", columns)}) ");
            command.AppendLine($"values ({schema.ColumnsWithTableName()}) ");
            command.AppendLine($";");
            command.AppendLine($"--when not matched by source then delete");
            
            return command.ToString();

        }


        public string GenerateTempTableQuary(string tableName)
        {
            var schema = _metadataExtractor.GetTableSchema(tableName);
            string columnsJoin = String.Join(", ", schema.Fields.Select(x => x.FieldName));
            var tableData = _dataExtractor.ConvertDataTableToList(tableName);

            var command = new StringBuilder();
            command.AppendLine($"select {columnsJoin} into #Temp{schema.TableName} from");
            command.AppendLine("( ");
            for (int i = 0; i < tableData.Count - 1; i++)
                command.AppendLine($"select {schema.FieldsWithQuotes(tableData[i], _data.type)} union all");
            command.AppendLine($"select {schema.FieldsWithQuotes(tableData[tableData.Count - 1], _data.type)}");

            command.AppendLine(") as dt;");

            return command.ToString();
        }
    }
}