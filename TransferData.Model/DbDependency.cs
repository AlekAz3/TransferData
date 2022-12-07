using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace TransferData.Model
{
    public class DbDependency
    {
        private readonly DataContext _data;

        public DbDependency(DataContext data)
        {
            _data = data;
        }

        public List<string> GetForiegnKeyColumns(string tableName)
        {
            var result = _data.Database.SqlQueryRaw<string>($"select COLUMN_NAME from ( select CONSTRAINT_NAME from INFORMATION_SCHEMA.TABLE_CONSTRAINTS where CONSTRAINT_TYPE = 'FOREIGN KEY') as a, INFORMATION_SCHEMA.KEY_COLUMN_USAGE as b where a.CONSTRAINT_NAME = b.CONSTRAINT_NAME and TABLE_NAME = '{tableName}';").ToList();

            return result;
        }

        public string GetPrimaryKeyColumn(string tableName)
        {
            string column = _data.Database.SqlQueryRaw<string>($"select COLUMN_NAME from ( select CONSTRAINT_NAME from INFORMATION_SCHEMA.TABLE_CONSTRAINTS where CONSTRAINT_TYPE = 'PRIMARY KEY') as a, INFORMATION_SCHEMA.KEY_COLUMN_USAGE as b where a.CONSTRAINT_NAME = b.CONSTRAINT_NAME and TABLE_NAME = '{tableName}';").ToList()[0];
            return column;
        }


        public string GetParentTable(string foreignColumn)
        {
            string result = _data.Database.SqlQueryRaw<string>($"select TABLE_NAME, COLUMN_NAME from (select CONSTRAINT_NAME from INFORMATION_SCHEMA.TABLE_CONSTRAINTS where CONSTRAINT_TYPE = 'PRIMARY KEY') as a, INFORMATION_SCHEMA.KEY_COLUMN_USAGE as b where a.CONSTRAINT_NAME = b.CONSTRAINT_NAME and COLUMN_NAME = '{foreignColumn}';").ToList()[0];


            return result;
        }

        private string GetTableDependency(string tableName)
        {
            
            string result = $"{tableName} ";

            var a = GetForiegnKeyColumns(tableName);

            if (a.IsNullOrEmpty())
            {
                return result.Trim();
            }
            else
            {
                foreach (var item in a)
                {
                    result +=  $"{GetTableDependency(GetParentTable(item))} ";
                }
            }

            return result.Trim();
        }

        public List<string> GetTableDependencyTables(string tableName)
        {
            var list = GetTableDependency(tableName).Split();
            var result = new List<string>();
            foreach (var item in list)
            {
                if (result.Contains(item))
                {
                    result.Remove(result.Find(x => x == item));
                    result.Add(item);
                }
                else
                {
                    result.Add(item);
                }
            }
            return result;
        }
    }
}
