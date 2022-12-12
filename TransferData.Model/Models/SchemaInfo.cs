namespace TransferData.Model.Models
{
    public record SchemaInfo
    {
        public string TableName { get; init; }
        public List<FieldInfo> Fields { get; init; }

        public SchemaInfo(string tableName, List<FieldInfo> fields)
        {
            TableName = tableName;
            Fields = fields;
        }

        internal string SetValuesSubQuery(DbType dbType)
        {
            return string.Join(", ",
                Fields.Select(x => $"{x.FieldName} = {Constants.TableSourceName}.{x.FieldNameWithEscape(dbType)}"));
        }

        internal string ColumnsWithTableName(DbType dbType)
        {
            return string.Join(", ",
                Fields.Select(x => $"{Constants.TableSourceName}.{x.FieldNameWithEscape(dbType)}"));
        }

        internal string FieldsWithQuotes(List<string> input, DbType fromDbType, DbType toDbType)
        {
            List<string> result = new List<string>();

            for (int i = 0; i < input.Count; i++)
            {
                result.Add($"{Fields[i].DataCheckQuotes(input[i], fromDbType)} as {Fields[i].FieldNameWithEscape(toDbType)}");
            }

            return string.Join(", ", result);

        }

    }
}
