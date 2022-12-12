using Microsoft.IdentityModel.Tokens;
using System.Text;
using TransferData.Model.Infrastructure;

namespace TransferData.Model.Services.Transfer
{
    public class TransferPostgreSQL : ITransfer
    {
        private readonly DbDataExtractor _dataExtractor;
        private readonly MetadataExtractor _metadataExtractor;
        private readonly DataContext _dataContext;

        public TransferPostgreSQL(DbDataExtractor dataExtractor, MetadataExtractor metadataExtractor, DataContext dataContext)
        {
            _dataExtractor = dataExtractor;
            _metadataExtractor = metadataExtractor;
            _dataContext = dataContext;
        }

        public string GenerateMergeQuery(string tableName)
        {
            var schema = _metadataExtractor.GetTableSchema(tableName);
            var sqlQueryString = new StringBuilder();
            var columns = schema.Fields.Select(x => x.FieldNameWithEscape()).ToList();
            string primaryKey = $"\"{_metadataExtractor.GetPrimaryKeyColumn(tableName)}\"";
            string columsJoin = string.Join(", ", columns);

            sqlQueryString.AppendLine($"with upsert({primaryKey}) as");
            sqlQueryString.AppendLine($"(");
            sqlQueryString.AppendLine($"insert into {schema.TableName} ({columsJoin})");
            sqlQueryString.AppendLine($"select {columsJoin} from Temp{schema.TableName}");
            sqlQueryString.AppendLine($"on conflict ({primaryKey}) do update set ");
            for (int i = 0; i < columns.Count() - 1; i++)
            {
                sqlQueryString.AppendLine($"{columns[i]} = excluded.{columns[i]},");
            }
            sqlQueryString.AppendLine($"{columns[columns.Count() - 1]} = excluded.{columns[columns.Count() - 1]}");

            sqlQueryString.AppendLine($"returning {primaryKey} )");
            sqlQueryString.AppendLine($"delete from {schema.TableName} where {primaryKey} not in (select {primaryKey} from upsert);");
            return sqlQueryString.ToString();
        }


        public string GenerateTempTableQuary(string tableName)
        {
            var schema = _metadataExtractor.GetTableSchema(tableName);
            string columnsJoin = string.Join(", ", schema.Fields.Select(x => x.FieldNameWithEscape()));
            var tableData = _dataExtractor.ConvertDataTableToList(tableName);

            var sqlQueryString = new StringBuilder();
            sqlQueryString.AppendLine($"select {columnsJoin} into temp table Temp{schema.TableName} from");
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
