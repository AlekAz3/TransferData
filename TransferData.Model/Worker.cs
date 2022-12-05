using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;
using static System.Net.Mime.MediaTypeNames;
using System.IO;

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

            var dataExtractor = new DbDataExtractor(_data);

            var transfer = new Transfer(_data, schema, dataExtractor, type);

            var tempTableQuary = transfer.GenerateTempTableQuary();
            var mergeQuary = transfer.GenerateMergeQuary();

            var path = $@"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\{tableName}_Transfer.txt";
            using (StreamWriter file = new StreamWriter(path, false))
            {
                file.WriteLine($"Program Output: table {tableName} from {_config.GetValue<DbType>("AppDbOptions:DbType")} to {type} ");
                file.WriteLine($"Create temp table: ");
                file.WriteLine(tempTableQuary);
                file.WriteLine($"Create merge query: ");
                file.WriteLine(mergeQuary);
            }
            _log.LogInformation($"File created!");

        }
    }
}
