namespace TransferData.Model.Models
{
    public record TableQuery
    {
        public string TableName { get; set; }
        public string MurgeQuery { get; set; }
        public string TempTableQuery { get; set; }

        public TableQuery(string tableName, string murgeQuery, string tempTableQuery)
        {
            TableName = tableName;
            MurgeQuery = murgeQuery;
            TempTableQuery = tempTableQuery;
        }
    }
}
