namespace TransferData.Model
{
    public static class DbDataHelper
    {

        public static string CompareColumns(SchemaInfo schema)
        {
            string line = String.Empty;
            for (int i = 1; i < schema.Fields.Count; i++)
            {
                line += $"{schema.Fields[i].FieldName} = T_Source.{schema.Fields[i].FieldName}, ";
            }

            return line.Remove(line.Length - 2, 2);
        }

        public static string ColumnsWithTableName(string tableName, SchemaInfo schema)
        {
            string line = String.Empty;

            for (int i = 1; i < schema.Fields.Count; i++)
            {
                line += $"{tableName}.{schema.Fields[i].FieldName}, ";
            }

            return line.Remove(line.Length - 2, 2);
        }

        public static string FieldsWithQuotes(List<string> input, SchemaInfo schema, DbType dbType)
        {
            List<List<string>> quetes = new List<List<string>>()
            {
                new List<string>() //PostgreSQL
                {
                    "character varying",
                    "date",
                    "varchar"
                },
                new List<string>() //MSSQL
                {
                    "varchar",
                    "date",
                    "datetime",
                    "datetime2",
                    "datetimeoffset",
                    "smalldatetime",
                    "time",
                    "char",
                    "varchar",
                    "text",
                    "nvarchar",
                    "nchar",
                    "ntext",
                }
            };

            var columns = schema.Fields;
            string result = String.Empty;

            for (int i = 0; i < input.Count; i++)
                if (quetes[(int)dbType].Contains(columns[i].FieldType) && input[i] != "null")
                    result += $"'{input[i]}' as {columns[i].FieldName}, ";
                else
                    result += $"{input[i]} as {columns[i].FieldName}, ";

            return result.Remove(result.Length - 2, 2);

        }
    }
}