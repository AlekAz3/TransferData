using Microsoft.IdentityModel.Tokens;
using NetTopologySuite.Operation.Valid;

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
            if ((FieldType == "varchar" || FieldType == "character varying" || FieldType == "nvarchar") && value.Contains('\''))
            {
                return $"'{value.Insert(value.IndexOf('\''), "\'")}'";
            }

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
                return $"'{value}'::date";

            if (FieldType == "time")
                return $"'{value}'::time";

            if (FieldType == "smalldatetime" || FieldType == "datetime" || FieldType == "datetime2" || FieldType == "timestamp without time zone")
                return $"to_timestamp('{value}', 'DD.MM.YYYY HH24:MI:SS')::timestamp";

            if (FieldType == "datetimeoffset" || FieldType == "timestamp with time zone")
                return $"'{value}'::TimestampTz";

            if (FieldType == "uniqueidentifier" || FieldType == "uuid")
                return $"'{value}'::uuid";

            if (FieldType == "boolean" || FieldType == "bool" || FieldType == "bit")
                return $"'{value}'::boolean";

            if (FieldType == "geography")
                return $"'{value}'::geometry";
            
            if (Constants.WithoutQuotes.Contains(FieldType))
                return $"{value}";

            if (FieldType == "varchar" || FieldType == "character varying")
                return $"\"{value}\"";


            return $"'{value}'";
        }

    }
}
