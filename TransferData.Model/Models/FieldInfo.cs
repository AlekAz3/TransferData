using Microsoft.IdentityModel.Tokens;

namespace TransferData.Model.Models
{
    public record FieldInfo
    {
        public string FieldName { get; init; }
        public string FieldType { get; init; }

        public FieldInfo(string fieldName, string fieldType)
        {
            FieldName = fieldName;
            FieldType = fieldType;
        }

        internal string DataCheckQuotes(string value)
        {
            if (Constants.toDbType == DbType.PostgreSQL && FieldType == "date")
                return $"to_date('{value}', 'DD-MM-YYYY H:MI:SS')";

            if (value.IsNullOrEmpty())
                return "null";
            
            if (Constants.WithoutQuotes.Contains(FieldType))
                return $"{value}";
            
            return $"'{value}'";

        }

        internal string FieldNameWithEscape()
        {
            switch (Constants.toDbType)
            {
                case DbType.PostgreSQL:
                    return $"\"{FieldName}\"";
                case DbType.MSSQL:
                    return $"[{FieldName}]";
                default:
                    return FieldName;
            }
        }
    }
}
