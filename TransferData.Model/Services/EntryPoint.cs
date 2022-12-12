using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TransferData.Model.Infrastructure;
using TransferData.Model.Models;
using TransferData.Model.Services.Transfer;

namespace TransferData.Model.Services
{
    public class EntryPoint
    {
        private readonly DataContext _dataContext;
        private readonly MetadataExtractor _metadataExtractor;
        private readonly IConfiguration _config;
        private readonly ILogger<EntryPoint> _log;
        private readonly ITransfer _transfer;

        public EntryPoint(DataContext dataContext, MetadataExtractor metadataExtractor, IConfiguration config, ILogger<EntryPoint> log, ITransfer transfer)
        {
            _dataContext = dataContext;
            _metadataExtractor = metadataExtractor;
            _config = config;
            _log = log;
            _transfer = transfer;
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

            var queries = GetQueries(tableName);

            string path = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\{tableName}_Transfer_{DateOnly.FromDateTime(DateTime.Now)}.txt";

            using (var file = new StreamWriter(path, false))
            {
                file.WriteLine($"--Program Output: table {tableName} from {Constants.fromDbType} to {Constants.toDbType} ");

                file.WriteLine("--Temp table query");

                foreach (var item in queries)
                {
                    file.WriteLine($"--======= Table: {item.TableName} =========");
                    file.WriteLine(item.TempTableQuery);
                }
                file.WriteLine("");
                file.WriteLine("--Merge query");
                foreach (var item in queries)
                {
                    file.WriteLine($"--======= Table: {item.TableName} =========");
                    file.WriteLine(item.MergeQuery);
                }
            }
            _log.LogInformation($"File created!");

        }

        internal List<TableQuery> GetQueries(string tableName)
        {
            var tables = _metadataExtractor.GetTableDependencyTables(tableName);
            tables.Reverse();
            var result = new List<TableQuery>();
            foreach (var table in tables)
            {
                result.Add(new TableQuery(table, _transfer.GenerateMergeQuery(table), _transfer.GenerateTempTableQuary(table)));
            }
            return result;
        }
    }
}
