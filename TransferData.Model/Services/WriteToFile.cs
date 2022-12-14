using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransferData.Model.Models;
using TransferData.Model.Services.Transfer;

namespace TransferData.Model.Services
{
    public class WriteToFile
    {
        private readonly ITransfer _transfer;
        private List<TableQuery> tableQueries;

        //jprivate readonly string path = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\{tableName}_Transfer_{DateOnly.FromDateTime(DateTime.Now)}.txt";
        //private readonly string path = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}";

        public WriteToFile(ITransfer transfer)
        {
            _transfer = transfer;
        }

        public void Write(string tableName)
        {
            tableQueries = _transfer.GetTableQueries(tableName);
            WriteAllToOneFile(tableQueries, tableName);
        }

        private void WriteAllToOneFile(List<TableQuery> tableQueries, string tableName)
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
