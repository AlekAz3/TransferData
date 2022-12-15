namespace TransferData.Model.Models
{
    /// <summary>
    /// Класс в котором содержится Мёрдж запрос и создание временной таблицы
    /// </summary>
    public record TableQuery
    {
        public string TableName { get; set; }
        public List<string> MergeQuery { get; set; }
        public List<string> TempTableQuery { get; set; }

        public TableQuery(string tableName, List<string> mergeQuery, List<string> tempTableQuery)
        {
            TableName = tableName;
            MergeQuery = mergeQuery;
            TempTableQuery = tempTableQuery;
        }
    }
}
