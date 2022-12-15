using TransferData.Model.Models;

namespace TransferData.Model.Services.Transfer
{
    /// <summary>
    /// Интерфейс для описания методов для переноса данных
    /// </summary>
    public interface ITransfer
    { 
        /// <summary>
        /// Возвращает список связанных таблиц и запросы для них
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        List<TableQuery> GetTableQueries(string tableName);


        /// <summary>
        /// Создание запроса для создания временной таблицы 
        /// </summary>
        /// <param name="tableName">Название таблицы</param>
        /// <returns>Запрос для генерации временной таблицы</returns>
        List<string> GenerateTempTableQuary(string tableName);

        /// <summary>
        /// Создание запроса для переноса данных из врменной таблицы в нужную 
        /// </summary>
        /// <param name="tableName">Название таблицы</param>
        /// <returns>Запрос для генерации Мёрдж запроса</returns>
        List<string> GenerateMergeQuery(string tableName);
    }
}
