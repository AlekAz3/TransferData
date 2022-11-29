namespace TransferData.Model
{
    public record SchemaInfo
    {
        public string TableName { get; init; }
        public List<FieldInfo> Fields { get; init; }

        public SchemaInfo(string tName, List<FieldInfo> fields)
        {
            TableName = tName;
            Fields = fields;
        }
    }
}
