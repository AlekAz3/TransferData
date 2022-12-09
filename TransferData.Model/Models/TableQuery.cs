namespace TransferData.Model.Models
{
    public record TableQuery
    {
        public string TableName { get; set; }
        public string MergeQuery { get; set; }
        public string TempTableQuery { get; set; }

        public TableQuery(string tableName, string mergeQuery, string tempTableQuery)
        {
            TableName = tableName;
            MergeQuery = mergeQuery;
            TempTableQuery = tempTableQuery;
        }
    }
}
