using TransferData.Model.Models;

namespace TransferData.Model.Services.Transfer
{
    /// <summary>
    /// Интерфейс для описания методов для переноса данных
    /// </summary>
    public interface ITransfer
    { 
        /// <summary>
        /// Метод который возвращает список связанных таблиц и запросы для них
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        List<TableQuery> GetTableQueries(string tableName);


        /// <summary>
        /// Метод для создания запроса для создания временной таблицы 
        /// </summary>
        /// <param name="tableName">Название таблицы</param>
        /// <returns>Запрос для генерации временной таблицы</returns>
        List<string> GenerateTempTableQuary(string tableName);

        /// <summary>
        /// Метод для создания запроса для переноса данных из врменной таблицы в нужную 
        /// </summary>
        /// <param name="tableName">Название таблицы</param>
        /// <returns>Запрос для генерации Мёрдж запроса</returns>
        List<string> GenerateMergeQuery(string tableName);
    }
}
