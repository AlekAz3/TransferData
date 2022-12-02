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
        private readonly Transfer _transfer;
        private readonly IConfiguration _config;
        private readonly ILogger<Worker> _log;

        public Worker(ILogger<Worker> log, IConfiguration config, DataContext data, Transfer transfer)
        {
            _data = data;
            _transfer = transfer;
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


            var tempTableQuary = _transfer.GenerateTempTableQuary(tableName, type);
            var mergeQuary = _transfer.GenerateMergeQuary(tableName, type);

            var dateTime = DateTime.Now;

            string path = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\{tableName}_Transfer_{DateOnly.FromDateTime(dateTime)}.txt";
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
