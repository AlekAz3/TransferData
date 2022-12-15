namespace TransferData.Model.Models
{
    /// <summary>
    /// Класс описание констант
    /// </summary>
    public class Constants
    {
        /// <summary>
        /// Типы данных где не нужны апострофы
        /// </summary>
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
        /// <summary>
        /// Название базовой таблицы 
        /// </summary>
        public static readonly string TableBaseName = "T_Base";

        /// <summary>
        /// Название таблицы источника 
        /// </summary>
        public static readonly string TableSourceName = "T_Source";

        /// <summary>
        /// Максимальное количество строк в одном запросе 
        /// </summary>
        public static readonly int MaxRow = 5000;

        /// <summary>
        /// Строковые типы данных
        /// </summary>
        public static readonly string[] charData = new string[3] { "varchar", "nvarchar", "character varying" };

        /// <summary>
        /// Тип субд В который надо перенести данный 
        /// </summary>
        public static DbType toDbType;

        /// <summary>
        /// Тип субд ИЗ которой надо перенести данный 
        /// </summary>
        public static DbType fromDbType;
    }
}
