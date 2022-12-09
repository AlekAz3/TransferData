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

        internal string DataCheckQuotes(string value, DbType dbType)
        {
            if (dbType == DbType.PostgreSQL && FieldType == "date")
                return $"to_date('{value}', 'DD-MM-YYYY H:MI:SS')";

            if (value.IsNullOrEmpty())
            {
                return "null";
            }

            if (Constants.WithoutQuotes.Contains(FieldType))
                return $"{value}";
            
            return $"'{value}'";

        }
    }
}
