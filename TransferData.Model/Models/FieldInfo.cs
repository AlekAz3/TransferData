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
            switch (Constants.toDbType)
            {
                case DbType.PostgreSQL:
                    return QuotesPostgreSQL(value);
                case DbType.MSSQL:
                    return QuotesMSSQL(value);
                default:
                    return value;
            }
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

        private string QuotesMSSQL(string value)
        {
            if (value.IsNullOrEmpty())
                return "null";

            if (Constants.WithoutQuotes.Contains(FieldType))
                return $"{value}";

            return $"'{value}'";
        }

        private string QuotesPostgreSQL(string value)
        {
            if (value.IsNullOrEmpty())
                return "null";

            if (FieldType == "date")
                return $"to_date('{value}', 'DD-MM-YYYY H:MI:SS')";

            if (FieldType == "uniqueidentifier" || FieldType == "uuid")
                return $"'{value}'::uuid";

            if (FieldType == "boolean" || FieldType == "bool" || FieldType == "bit")
                return $"'{value}'::boolean";

            if (Constants.WithoutQuotes.Contains(FieldType))
                return $"{value}";

            return $"'{value}'";
        }

    }
}
