namespace TransferData.Model
{
    public record TableQuery
    {
        public string MurgeQuery { get; set; }
        public string TempTableQuery { get; set; }

        public TableQuery(string murgeQuery, string tempTableQuery)
        {
            MurgeQuery = murgeQuery;
            TempTableQuery = tempTableQuery;
        }
    }
}
