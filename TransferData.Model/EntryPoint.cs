﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace TransferData.Model
{
    public class EntryPoint
    {
        private readonly DataContext _data;
        private readonly MetadataExtractor _metadataExtractor;
        private readonly IConfiguration _config;
        private readonly ILogger<EntryPoint> _log;
        private readonly ITransfer _transfer;

        public EntryPoint(DataContext data, MetadataExtractor metadataExtractor, IConfiguration config, ILogger<EntryPoint> log, ITransfer transfer)
        {
            _data = data;
            _metadataExtractor = metadataExtractor;
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

            var tempTableQueries = GetQueriesTempTable(tableName);
            var mergeQueries = GetMergeQuaries(tableName);

            string path = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\{tableName}_Transfer_{DateOnly.FromDateTime(DateTime.Now)}.txt";
            
            using (var file = new StreamWriter(path, false))
            {
                file.WriteLine($"--Program Output: table {tableName} from {_config.GetValue<DbType>("AppDbOptions:DbType")} to {dbType} ");
                file.WriteLine($"--Create temp table: ");
                foreach (var item in tempTableQueries)
                {
                    file.WriteLine(item);
                }

                file.WriteLine($"--Create merge query: ");

                foreach (var item in mergeQueries)
                {
                    file.WriteLine(item);
                }

            }
            _log.LogInformation($"File created!");

        }

        public List<string> GetQueriesTempTable(string tableName)
        {
            var tables = _metadataExtractor.GetTableDependencyTables(tableName);
            tables.Reverse();
            var result = new List<string>();
            foreach (var table in tables)
            {
                result.Add(_transfer.GenerateTempTableQuary(table));
            }
            return result;
        }

        public List<string> GetMergeQuaries(string tableName)
        {
            var tables = _metadataExtractor.GetTableDependencyTables(tableName);
            tables.Reverse();
            var result = new List<string>();
            foreach (var table in tables)
            {
                result.Add(_transfer.GenerateMergeQuery(table));
            }
            return result;
        }


    }
}