namespace TransferData.Model
{
    public static class DbDataHelper
    {

        public static string FieldsWithQuotes(List<string> input, SchemaInfo schema, DbType dbType)
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

            var columns = schema.Fields;
            string result = String.Empty;

            for (int i = 0; i < input.Count; i++)
                if (!quetes[(int)dbType].Contains(columns[i].FieldType) && input[i] != "null")
                    result += $"'{input[i]}' as {columns[i].FieldName}, ";
                else
                    result += $"{input[i]} as {columns[i].FieldName}, ";

            return result.Remove(result.Length - 2, 2);

        }
    }
}