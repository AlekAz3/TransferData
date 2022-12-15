using Microsoft.Extensions.Logging;
using TransferData.Model.Infrastructure;
using TransferData.Model.Models;

namespace TransferData.Model.Services
{
    /// <summary>
    /// Класс вход в программу
    /// </summary>
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

        /// <summary>
        /// Метод который делает настройки и вызывает метод для записи данных в файл
        /// </summary>
        /// <param name="tableName">Название таблицы</param>
        /// <param name="dbType">Тип СУБД в который надо перевести данные</param>
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
