using TransferData.Model.Models;

namespace TransferData.Model.Services.Transfer
{
    public interface ITransfer
    { 
        List<TableQuery> GetTableQueries(string tableName);
        List<string> GenerateTempTableQuary(string tableName);
        List<string> GenerateMergeQuery(string tableName);
    }
}
