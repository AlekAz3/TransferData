using Microsoft.IdentityModel.Tokens;

namespace TransferData.Model.Models
{
    /// <summary>
    /// Класс описание полей таблицы
    /// </summary>
    public record FieldInfo
    {
        public string FieldName { get; init; }
        public string FieldType { get; init; }

        public FieldInfo(string fieldName, string fieldType)
        {
            FieldName = fieldName;
            FieldType = fieldType;
        }

        /// <summary>
        /// Обрамление данных в апострофы данных 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Экранирование столбцов таблицы
        /// </summary>
        /// <returns></returns>
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

            if (Constants.charData.Contains(FieldType) && value.Contains('\''))
                return $"'{value.Insert(value.IndexOf('\''), "\'")}'";

            if (Constants.WithoutQuotes.Contains(FieldType))
                return $"{value}";

            return $"'{value}'";
        }

        private string QuotesPostgreSQL(string value)
        {
            if (value.IsNullOrEmpty())
                value = "null";
            
            if (Constants.charData.Contains(FieldType) && value.Contains('\''))
                return $"'{value.Insert(value.IndexOf('\''), "\'")}'";
            
            if (FieldType == "date" && value == "null")
                return $"null::date";
            else if(FieldType == "date")
                return $"'{value}'::date";

            if (FieldType == "time" && value == "null")
                return $"null::time";
            else if (FieldType == "time")
                return $"'{value}'::time";
            

            if ((FieldType == "smalldatetime" || FieldType == "datetime" || FieldType == "datetime2" || FieldType == "timestamp without time zone") && value == "null")
                return $"null::timestamp";
            else if (FieldType == "smalldatetime" || FieldType == "datetime" || FieldType == "datetime2" || FieldType == "timestamp without time zone")
                return $"to_timestamp('{value}', 'DD.MM.YYYY HH24:MI:SS')::timestamp";


            if ((FieldType == "datetimeoffset" || FieldType == "timestamp with time zone") && value == "null")
                return $"null::TimestampTz";
            else if (FieldType == "datetimeoffset" || FieldType == "timestamp with time zone")
                return $"'{value}'::TimestampTz";


            if ((FieldType == "uniqueidentifier" || FieldType == "uuid") && value == "null")
                return $"null::uuid";
            else if (FieldType == "uniqueidentifier" || FieldType == "uuid")
                return $"'{value}'::uuid";

            if ((FieldType == "boolean" || FieldType == "bool" || FieldType == "bit") && value == "null")
                return $"null::boolean";
            else if (FieldType == "boolean" || FieldType == "bool" || FieldType == "bit")
                return $"'{value}'::boolean";

            if (FieldType == "geography" && value == "null")
                return $"null::geometry";
            else if (FieldType == "geography")
                return $"'{value}'::geometry";

            if (value == "null")
                return "null";
            

            if (Constants.WithoutQuotes.Contains(FieldType))
                return $"{value}";

            return $"'{value}'";
        }

    }
}
