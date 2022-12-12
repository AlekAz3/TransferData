namespace TransferData.Model.Models
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

    }
}
