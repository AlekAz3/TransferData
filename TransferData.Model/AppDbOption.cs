namespace TransferData.Model
{
    public class AppDbOptions
    {
        /// <summary>
        /// Тип БД
        /// </summary>
        public DbType DbType { set; get; }

        /// <summary>
        /// Флаг запуска миграций БД
        /// </summary>
        public bool IsMigrate { set; get; }
    }

    public enum DbType
    {
        /// <summary>
        /// БД Postgres
        /// </summary>
        Postgres,

        /// <summary>
        ///  БД MS SQL
        /// </summary>
        MSSQL
    }

}
