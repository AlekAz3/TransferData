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

        internal string FieldsWithQuotes(List<string> input, DbType dbType)
        {
            List<List<string>> quetes = new List<List<string>>()
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

            List<string> result = new List<string>();

            for (int i = 0; i < input.Count; i++)
                if (!quetes[(int)dbType].Contains(Fields[i].FieldType) && input[i] != "null")
                    result.Add($"'{input[i]}' as {Fields[i].FieldName}");
                else
                    result.Add($"{input[i]} as {Fields[i].FieldName}");

            return string.Join(", ", result);

        }

    }
}
