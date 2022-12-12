using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TransferData.Model.Infrastructure;
using TransferData.Model.Models;

namespace TransferData.Model.Services
{
    public class MetadataExtractor
    {
        private readonly DataContext _dataContext;

        public MetadataExtractor(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        internal List<string> GetForiegnKeyColumns(string tableName)
        {
            var result = _dataContext.Database.SqlQueryRaw<string>($"select COLUMN_NAME from ( select CONSTRAINT_NAME from INFORMATION_SCHEMA.TABLE_CONSTRAINTS where CONSTRAINT_TYPE = 'FOREIGN KEY') as a, INFORMATION_SCHEMA.KEY_COLUMN_USAGE as b where a.CONSTRAINT_NAME = b.CONSTRAINT_NAME and TABLE_NAME = '{tableName}';").ToList();

            return result;
        }

        internal string GetPrimaryKeyColumn(string tableName)
        {
            string column = _dataContext.Database.SqlQueryRaw<string>($"select COLUMN_NAME from ( select CONSTRAINT_NAME from INFORMATION_SCHEMA.TABLE_CONSTRAINTS where CONSTRAINT_TYPE = 'PRIMARY KEY') as a, INFORMATION_SCHEMA.KEY_COLUMN_USAGE as b where a.CONSTRAINT_NAME = b.CONSTRAINT_NAME and TABLE_NAME = '{tableName}';").ToList()[0];
            return column;
        }


        internal List<string> GetParentTables(string tableName)
        {
            List<string> result = _dataContext.Database.SqlQueryRaw<string>($"select \r\n  c.TABLE_NAME\r\nfrom \r\n  (\r\n    select \r\n      c.CONSTRAINT_NAME,\r\n\t  d.COLUMN_NAME\r\n    from \r\n      INFORMATION_SCHEMA.TABLE_CONSTRAINTS as c, INFORMATION_SCHEMA.KEY_COLUMN_USAGE as d\r\n    where \r\n      CONSTRAINT_TYPE = 'FOREIGN KEY'  and\r\n\t  c.CONSTRAINT_NAME = d.CONSTRAINT_NAME and c.TABLE_NAME = '{tableName}'\r\n  ) as a, \r\n  INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS as b,\r\n  (select \r\n      * \r\n    from \r\n      INFORMATION_SCHEMA.TABLE_CONSTRAINTS \r\n    where \r\n      CONSTRAINT_TYPE = 'PRIMARY KEY') as c\r\nwhere \r\n  a.CONSTRAINT_NAME = b.CONSTRAINT_NAME and\r\n  b.UNIQUE_CONSTRAINT_NAME = c.CONSTRAINT_NAME and TABLE_NAME != '{tableName}';").ToList();


            return result;
        }

        public string GetTableDependency(string tableName)
        {

            string result = $"{tableName} {string.Join(" ", GetParentTables(tableName))} ";

            var a = GetParentTables(tableName);

            if (a.IsNullOrEmpty())
            {
                return result.Trim();
            }
            else
            {
                foreach (var item in a)
                {
                    result += $"{string.Join(" ", GetParentTables(item))} ";
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
                    //result.Remove(result.Find(x => x == item));
                    //result.Add(item);
                }
                else
                {
                    result.Add(item);
                }
            }
            return result;
        }

        internal SchemaInfo GetTableSchema(string tableName)
        {
            var informationSchema = _dataContext.Schema
                .FromSqlRaw($"select table_schema, table_name, column_name, data_type from information_schema.columns where table_name = '{tableName}' order by ORDINAL_POSITION")
                .ToList();

            if (informationSchema.Count == 0)
                throw new Exception("Table not Found");


            var fields = new List<FieldInfo>();

            for (int i = 0; i < informationSchema.Count; i++)
                fields.Add(new FieldInfo(informationSchema[i].column_name, informationSchema[i].data_type));

            return new SchemaInfo(tableName, fields);
        }

    }
}
