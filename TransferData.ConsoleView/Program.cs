using CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using TransferData.Model;

namespace TransferData.ConsoleView
{
    public class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Build())
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            Log.Logger.Information("Application Starting");
            
            var result = Parser.Default.ParseArguments<Options>(args);
            result.WithParsed(options =>
            {
                Log.Logger.Information($"Table {options.tableName} to {options.type}");
                Start(options.tableName, options.type);
            }).WithNotParsed(opt => { Log.Logger.Error("ParsingError"); return; });

        }

        private static void Start(string tableName, DbType dbType)
        {
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddDbContext<DataContext>();
                    services.AddTransient<DbDataExtractor>();
                    services.AddTransient<MetadataExtractor>();
                    services.AddTransient<TransferMSSQL>();
                    services.AddTransient<TransferPostgreSQL>();
                    services.AddTransient<ITransfer>(serviceProvider =>
                    {
                        switch (dbType)
                        {
                            case DbType.MSSQL:
                                return serviceProvider.GetService<TransferMSSQL>();
                            case DbType.PostgreSQL:
                                return serviceProvider.GetService<TransferPostgreSQL>();
                            default:
                                throw new KeyNotFoundException();
                        }
                    });
                    services.AddTransient<Worker>();
                })
                .UseSerilog()
                .Build();

            var svc = ActivatorUtilities.CreateInstance<Worker>(host.Services);
            svc.Run(tableName, dbType);
        }
    }

    class Options
    {
        [Option('t',"table", Required = true, HelpText = "Название таблицы ")]
        public string tableName { get; set; }

        [Option('d',"database", Required = true, HelpText = "Название СУБД ")]
        public DbType type { get; set; }
    }

}