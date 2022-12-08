namespace TransferData.Model
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
            return String.Join(", ",
                this.Fields.Select(x => $"{x.FieldName} = T_Source.{x.FieldName}"));
        }

        internal string ColumnsWithTableName()
        {
            return String.Join(", ",
                this.Fields.Select(x => $"T_Source.{x.FieldName}"));
        }

    }
}
