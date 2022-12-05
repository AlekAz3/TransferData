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
        private readonly ITransfer _transfer;

        public Worker(ILogger<Worker> log, IConfiguration config, DataContext data, ITransfer transfer)
        {
            _data = data;
            _config = config;
            _log = log;
            _transfer = transfer;
        }

        public void Run(string tableName, DbType dbType)
        {
            if (!_data.Database.CanConnect())
            {
                _log.LogError("Can't connect to db");
                return;
            }
            string tempTableQuary = _transfer.GenerateTempTableQuary(tableName);
            string mergeQuary = _transfer.GenerateMergeQuary(tableName);
            var dateTime = DateTime.Now;

            string path = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\{tableName}_Transfer_{DateOnly.FromDateTime(dateTime)}.txt";
            
            using (var file = new StreamWriter(path, false))
            {
                file.WriteLine($"--Program Output: table {tableName} from {_config.GetValue<DbType>("AppDbOptions:DbType")} to {dbType} ");
                file.WriteLine($"--Create temp table: ");
                file.WriteLine(tempTableQuary);
                file.WriteLine($"--Create merge query: ");
                file.WriteLine(mergeQuary);
            }
            _log.LogInformation($"File created!");

        }
    }
}
