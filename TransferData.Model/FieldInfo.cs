namespace TransferData.Model
{
    public record FieldInfo
    {
        public string FieldName { get; init; }
        public string FieldType { get; init; }

        public FieldInfo(string fn, string ft)
        {
            FieldName = fn;
            FieldType = ft;
        }

    }
}
