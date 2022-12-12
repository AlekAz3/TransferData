using Microsoft.IdentityModel.Tokens;
using System.Text;
using TransferData.Model.Infrastructure;
using TransferData.Model.Models;

namespace TransferData.Model.Services.Transfer
{
    public class TransferMSSQL : ITransfer
    {
        private readonly DbDataExtractor _dataExtractor;
        private readonly MetadataExtractor _metadataExtractor;
        private readonly DataContext _dataContext;

        public TransferMSSQL(DbDataExtractor dataExtractor, MetadataExtractor metadataExtractor, DataContext dataContext)
        {
            _dataExtractor = dataExtractor;
            _metadataExtractor = metadataExtractor;
            _dataContext = dataContext;
        }

        public string GenerateMergeQuery(string tableName)
        {
            var schema = _metadataExtractor.GetTableSchema(tableName);
            var sqlQueryString = new StringBuilder();
            var columnsJoin = string.Join(", ", schema.Fields.Select(x => x.FieldNameWithEscape()));
            
            string primaryKey = $"[{_metadataExtractor.GetPrimaryKeyColumn(tableName)}]";

            sqlQueryString.AppendLine($"merge {schema.TableName} AS {Constants.TableBaseName} ");
            sqlQueryString.AppendLine($"using #Temp{schema.TableName} AS T_Source ");
            sqlQueryString.AppendLine($"on ({Constants.TableBaseName}.{primaryKey} = {Constants.TableSourceName}.{primaryKey}) ");
            sqlQueryString.AppendLine($"when matched then ");
            sqlQueryString.AppendLine($"update set {schema.SetValuesSubQuery()} ");
            sqlQueryString.AppendLine($"when not matched then ");
            sqlQueryString.AppendLine($"insert ({columnsJoin}) ");
            sqlQueryString.AppendLine($"values ({schema.ColumnsWithTableName()}) ");
            sqlQueryString.AppendLine($"when not matched by source then delete;");

            return sqlQueryString.ToString();

        }


        public string GenerateTempTableQuary(string tableName)
        {
            var schema = _metadataExtractor.GetTableSchema(tableName);
            string columnsJoin = string.Join(", ", schema.Fields.Select(x => x.FieldNameWithEscape()));
            var tableData = _dataExtractor.ConvertDataTableToList(tableName);

            var sqlQueryString = new StringBuilder();
            sqlQueryString.AppendLine($"select {columnsJoin} into #Temp{schema.TableName} from");
            sqlQueryString.AppendLine("( ");
            if (tableData.IsNullOrEmpty())
                return "";

            if (tableData.Count == 1)
            {
                sqlQueryString.AppendLine($"select {schema.FieldsWithQuotes(tableData[0])} ) as dt;");
                return sqlQueryString.ToString();
            }

            for (int i = 0; i < tableData.Count - 1; i++)
                sqlQueryString.AppendLine($"select {schema.FieldsWithQuotes(tableData[i])} union all");
            sqlQueryString.AppendLine($"select {schema.FieldsWithQuotes(tableData[tableData.Count - 1])}");

            sqlQueryString.AppendLine(") as dt;");

            return sqlQueryString.ToString();
        }
    }
}