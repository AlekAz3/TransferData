using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
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

            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddDbContext<DataContext>();
                    services.AddTransient<Worker>();

                })
                .UseSerilog()
                .Build();

            var svc = ActivatorUtilities.CreateInstance<Worker>(host.Services);
            svc.Run("table2");
        }
    }
}