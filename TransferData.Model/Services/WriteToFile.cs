using TransferData.Model.Models;
using TransferData.Model.Services.Transfer;

namespace TransferData.Model.Services
{
    /// <summary>
    /// Класс записи полученных запросов в файл
    /// </summary>
    public class WriteToFile
    {
        private readonly ITransfer _transfer;
        private List<TableQuery> tableQueries = new List<TableQuery>();

        public WriteToFile(ITransfer transfer)
        {
            _transfer = transfer;
        }
        /// <summary>
        /// Запись в файл
        /// </summary>
        /// <param name="tableName">Название таблицы</param>
        public void Write(string tableName)
        {
            tableQueries = _transfer.GetTableQueries(tableName);
            WriteOnFile(tableName);
        }

        private void WriteOnFile(string tableName)
        {
            string path = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\{tableName}_Transfer_{DateOnly.FromDateTime(DateTime.Now)}.txt";

            using (var file = new StreamWriter(path, false))
            {
                file.WriteLine($"--Program Output: table {tableName} from {Constants.fromDbType} to {Constants.toDbType} ");

                file.WriteLine("--Temp table query");

                foreach (var item in tableQueries)
                {
                    file.WriteLine($"--======= Table: {item.TableName} =========");
                    file.WriteLine(string.Join("\n", item.TempTableQuery));
                }
                file.WriteLine("");
                file.WriteLine("--Merge query");
                foreach (var item in tableQueries)
                {
                    file.WriteLine($"--======= Table: {item.TableName} =========");
                    file.WriteLine(string.Join("\n", item.MergeQuery));
                }
            }
        }
    }
}