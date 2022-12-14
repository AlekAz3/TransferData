using Microsoft.IdentityModel.Tokens;
using TransferData.Model.Models;

namespace TransferData.Model.Services.Transfer
{
    public class TransferMSSQL : ITransfer
    {
        private readonly DbDataExtractor _dataExtractor;
        private readonly MetadataExtractor _metadataExtractor;

        public TransferMSSQL(DbDataExtractor dataExtractor, MetadataExtractor metadataExtractor)
        {
            _dataExtractor = dataExtractor;
            _metadataExtractor = metadataExtractor;
        }

        public List<string> GenerateMergeQuery(string tableName)
        {
            var schema = _metadataExtractor.GetTableSchema(tableName);
            var sqlQuery = new List<string>();
            var columnsJoin = string.Join(", ", schema.Fields.Select(x => x.FieldNameWithEscape()));
            
            string primaryKey = $"[{_metadataExtractor.GetPrimaryKeyColumn(tableName)}]";

            sqlQuery.Add($"merge {schema.TableName} AS {Constants.TableBaseName} ");
            sqlQuery.Add($"using #Temp{schema.TableName} AS T_Source ");
            sqlQuery.Add($"on ({Constants.TableBaseName}.{primaryKey} = {Constants.TableSourceName}.{primaryKey}) ");
            sqlQuery.Add($"when matched then ");
            sqlQuery.Add($"update set {schema.SetValuesSubQuery()} ");
            sqlQuery.Add($"when not matched then ");
            sqlQuery.Add($"insert ({columnsJoin}) ");
            sqlQuery.Add($"values ({schema.ColumnsWithTableName()}) ");
            sqlQuery.Add($";--when not matched by source then delete;");

            return sqlQuery;

        }

        public List<string> GenerateTempTableQuary(string tableName)
        {
            var schema = _metadataExtractor.GetTableSchema(tableName);
            string columnsJoin = string.Join(", ", schema.Fields.Select(x => x.FieldNameWithEscape()));
            var tableData = _dataExtractor.ConvertDataTableToList(tableName);

            var sqlQuery = new List<string>
            {
                $"select {columnsJoin} into #Temp{schema.TableName} from",
                "( "
            };

            if (tableData.IsNullOrEmpty())
            {
                sqlQuery.Add(") as dt;");
                return sqlQuery;
            }

            if (tableData.Count == 1)
            {
                sqlQuery.Add($"select {schema.FieldsWithQuotes(tableData[0])})");
                sqlQuery.Add($"as dt;");
                return sqlQuery;
            }

            for (int i = 0; i < tableData.Count; i++)
                sqlQuery.Add($"select {schema.FieldsWithQuotes(tableData[i])} union all");

            sqlQuery[sqlQuery.Count - 1].Remove(sqlQuery.Count - 10, 10);

            sqlQuery.Add(") as dt;");

            return sqlQuery;
        }
    }
}