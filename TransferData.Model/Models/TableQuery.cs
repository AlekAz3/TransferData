namespace TransferData.Model.Models
{
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
