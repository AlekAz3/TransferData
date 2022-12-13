using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TransferData.Model.Models;

namespace TransferData.Model.Infrastructure
{
    public class DataContext : DbContext
    {
        private readonly IConfiguration _config;
        private readonly ILogger<DataContext> _logger;
        public DbSet<InformationSchema> Schema { get; init; }

        public DbType Type { get; private set; }

        public DataContext(ILogger<DataContext> log, IConfiguration config)
        {
            _config = config;
            _logger = log;
            _logger.LogInformation("DataContext created");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var DbOption = _config.GetSection(nameof(AppDbOptions)).Get<AppDbOptions>();
            Type = DbOption.DbType;
            switch (DbOption.DbType)
            {
                case DbType.PostgreSQL:
                    optionsBuilder.UseNpgsql(_config.GetConnectionString(typeof(DataContext).Name), o => o.UseNetTopologySuite());
                    _logger.LogInformation("PostgeSQL Connect");
                    break;
                case DbType.MSSQL:
                    optionsBuilder.UseSqlServer(_config.GetConnectionString(typeof(DataContext).Name));
                    _logger.LogInformation("MSSQL Connect");
                    break;
                default:
                    break;
            }

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<InformationSchema>().HasNoKey();
            if (Type == DbType.PostgreSQL)
                modelBuilder.HasPostgresExtension("postgis");
            
            

        }

    }
}