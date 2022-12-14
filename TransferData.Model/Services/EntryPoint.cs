using Microsoft.Extensions.Logging;
using TransferData.Model.Infrastructure;
using TransferData.Model.Models;
using TransferData.Model.Services.Transfer;

namespace TransferData.Model.Services
{
    public class EntryPoint
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<EntryPoint> _log;
        private readonly WriteToFile _writer;

        public EntryPoint(DataContext dataContext, ILogger<EntryPoint> log, WriteToFile writer)
        {
            _dataContext = dataContext;
            _log = log;
            _writer = writer;
        }

        public void CreateFile(string tableName, DbType dbType)
        {
            if (!_dataContext.Database.CanConnect())
            {
                _log.LogError("Can't connect to db");
                return;
            }

            Constants.fromDbType = _dataContext.Type;
            Constants.toDbType = dbType;

            _writer.Write(tableName);

            _log.LogInformation($"File created!");

        }
    }
}
