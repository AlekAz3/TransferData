namespace TransferData.Model.Services.Transfer
{
    public interface ITransfer
    {
        string GenerateTempTableQuary(string tableName);
        string GenerateMergeQuery(string tableName);
    }
}
