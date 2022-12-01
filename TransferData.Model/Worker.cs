using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;

namespace TransferData.Model
{
    public class Worker
    {
        private readonly DataContext _data;
        private readonly IConfiguration _config;
        private readonly ILogger<Worker> _log;

        public Worker(ILogger<Worker> log, IConfiguration config, DataContext data)
        {
            _data = data;
            _config = config;
            _log = log;
        }

        public void Run(string tableName, DbType type)
        {
            if (!_data.Database.CanConnect())
            {
                _log.LogError("Can't connect to db");
                return;
            }

            var schema = new DbSchemaExtractor(_data).GetTableSchema(tableName).Result;

            var transfer = new Transfer(_data, schema, type);

            var a = transfer.GenerateTempTableQuary();
            _log.LogInformation(a);

            _log.LogInformation(" ");

            _log.LogInformation(transfer.GenerateMergeQuary());
        }
    }
}
