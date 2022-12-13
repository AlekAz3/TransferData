﻿namespace TransferData.Model.Models
{
    public class Constants
    {
        public static readonly List<string> WithoutQuotes = new List<string>()
        {
            "bigint",
            "int",
            "smallint",
            "tinyint",
            "decimal",
            "numeric",
            "money",
            "smallmoney",
            "float",
            "real",
            "binary",
            "varbinary",
            "integer",
            "double precision",
            "bytea",
            "double"
        };

        public static readonly string TableBaseName = "T_Base";
        public static readonly string TableSourceName = "T_Source";

        public static readonly string[] charData = new string[3] { "varchar", "nvarchar", "character varying" };

        public static DbType toDbType;
        public static DbType fromDbType;
    }
}
