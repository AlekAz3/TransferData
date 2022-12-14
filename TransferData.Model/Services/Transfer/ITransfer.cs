namespace TransferData.Model.Services.Transfer
{
    public interface ITransfer
    {
        List<string> GenerateTempTableQuary(string tableName);
        List<string> GenerateMergeQuery(string tableName);
    }
}
