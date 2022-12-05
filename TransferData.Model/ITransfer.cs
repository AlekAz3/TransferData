namespace TransferData.Model
{
    public interface ITransfer
    {
        string GenerateTempTableQuary(string tableName);
        string GenerateMergeQuery(string tableName);
    }
}
