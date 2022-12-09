namespace TransferData.Model.Models
{
    public class Constants
    {
        public static readonly List<List<string>> Quotes = new List<List<string>>()
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

        public static readonly string TableBaseName = "T_Base";
        public static readonly string TableSourceName = "T_Source";

    }
}
