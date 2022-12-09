namespace TransferData.Model.Models
{
    public record FieldInfo
    {
        public string FieldName { get; init; }
        public string FieldType { get; init; }

        private readonly List<List<string>> quetes = new List<List<string>>()
            {
                //new List<string>() //PostgreSQL
                //{
                //    "character varying",
                //    "varchar",
                //    "char",
                //    "character",
                //    "text",
                //    "timestamp",
                //    "date",
                //    "time",
                //    "interval"
                //},
                //new List<string>() //MSSQL
                //{
                //    "varchar",
                //    "date",
                //    "datetime",
                //    "datetime2",
                //    "datetimeoffset",
                //    "smalldatetime",
                //    "time",
                //    "timestamp",
                //    "char",
                //    "varchar",
                //    "text",
                //    "nvarchar",
                //    "nchar",
                //    "ntext",
                //}

                new List<string>() //PostgreSQL
                {
                    "smallint",
                    "integer",
                    "bigint",
                    "decimal",
                    "numeric",
                    "real",
                    "double precision",
                    "bytea"

                },
                new List<string>() //MSSQL
                {
                    "bigint",
                    "int",
                    "smallint",
                    "tinyint",
                    "bit",
                    "decimal",
                    "numeric",
                    "money",
                    "smallmoney",
                    "float",
                    "real",
                    "binary",
                    "varbinary"

                }
            };

        public FieldInfo(string fieldName, string fieldType)
        {
            FieldName = fieldName;
            FieldType = fieldType;
        }

        internal string dataCheckQuotes(string value, DbType dbType)
        {
            if (dbType == DbType.PostgreSQL && FieldType == "date")
                return $"to_date('{value}', 'DD-MM-YYYY H:MI:SS')";

            if (!quetes[(int)dbType].Contains(FieldType) && value != "null")
                return $"'{value}'";
            
            return $"{value}";

        }
    }
}
