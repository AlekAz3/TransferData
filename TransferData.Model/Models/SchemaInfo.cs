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

        internal string SetValuesSubQuery()
        {
            return string.Join(", ",
                Fields.Select(x => $"{x.FieldNameWithEscape()} = {Constants.TableSourceName}.{x.FieldNameWithEscape()}"));
        }

        internal string ColumnsWithTableName()
        {
            return string.Join(", ",
                Fields.Select(x => $"{Constants.TableSourceName}.{x.FieldNameWithEscape()}"));
        }

        internal string FieldsWithQuotes(List<string> input)
        {
            List<string> result = new List<string>();

            for (int i = 0; i < input.Count; i++)
            {
                result.Add($"{Fields[i].DataCheckQuotes(input[i])} as {Fields[i].FieldNameWithEscape()}");
            }

            return string.Join(", ", result);

        }

    }
}
