using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransferData.Model
{
    public class DbDependency
    {
        private readonly DataContext _data;

        public DbDependency(DataContext data)
        {
            _data = data;
        }

        public List<string> GetForiegnColumns(string tableName)
        {
            var result = new List<string>();

            var a  = _data.Database.SqlQueryRaw<string>("");

            return result;
        }

        public string GetPrimaryKeyColumn(string tableName)
        {
            string column = _data.Database.SqlQueryRaw<string>($"select COLUMN_NAME from ( select CONSTRAINT_NAME from INFORMATION_SCHEMA.TABLE_CONSTRAINTS where CONSTRAINT_TYPE = 'PRIMARY KEY') as a, INFORMATION_SCHEMA.KEY_COLUMN_USAGE as b where a.CONSTRAINT_NAME = b.CONSTRAINT_NAME and TABLE_NAME = '{tableName}';").ToList()[0];
            return column;
        }

    }
}
