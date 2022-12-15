using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TransferData.Model.Infrastructure;
using TransferData.Model.Models;

namespace TransferData.Model.Services
{
    /// <summary>
    /// Класс для получения метаданных из таблицы в СУБД
    /// </summary>
    public class MetadataExtractor
    {
        private readonly DataContext _dataContext;

        public MetadataExtractor(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        /// <summary>
        /// Получение списка внешних ключей 
        /// </summary>
        /// <param name="tableName">Название таблицы</param>
        /// <returns>Список столбцов внешних ключей</returns>
        internal List<string> GetForiegnKeyColumns(string tableName)
        {
            var result = _dataContext.Database.SqlQueryRaw<string>($"select COLUMN_NAME from ( select CONSTRAINT_NAME from INFORMATION_SCHEMA.TABLE_CONSTRAINTS where CONSTRAINT_TYPE = 'FOREIGN KEY') as a, INFORMATION_SCHEMA.KEY_COLUMN_USAGE as b where a.CONSTRAINT_NAME = b.CONSTRAINT_NAME and TABLE_NAME = '{tableName}';").ToList();

            return result;
        }

        /// <summary>
        /// Получение столбца первичного ключа
        /// </summary>
        /// <param name="tableName">Название таблицы</param>
        /// <returns>Название столба первичного ключа</returns>
        internal string GetPrimaryKeyColumn(string tableName)
        {
            string column = _dataContext.Database.SqlQueryRaw<string>($"select COLUMN_NAME from ( select CONSTRAINT_NAME from INFORMATION_SCHEMA.TABLE_CONSTRAINTS where CONSTRAINT_TYPE = 'PRIMARY KEY') as a, INFORMATION_SCHEMA.KEY_COLUMN_USAGE as b where a.CONSTRAINT_NAME = b.CONSTRAINT_NAME and TABLE_NAME = '{tableName}';").ToList()[0];
            return column;
        }

        /// <summary>
        /// Получение списка родительских таблиц данной таблицы 
        /// </summary>
        /// <param name="tableName">Название таблиц</param>
        /// <returns>Список родительских таблиц</returns>
        internal List<string> GetParentTables(string tableName)
        {
            List<string> result = _dataContext.Database.SqlQueryRaw<string>($"select c.TABLE_NAME from ( select c.CONSTRAINT_NAME, d.COLUMN_NAME from INFORMATION_SCHEMA.TABLE_CONSTRAINTS as c, INFORMATION_SCHEMA.KEY_COLUMN_USAGE as d where CONSTRAINT_TYPE = 'FOREIGN KEY'  and c.CONSTRAINT_NAME = d.CONSTRAINT_NAME and c.TABLE_NAME = '{tableName}') as a, INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS as b,(select * from INFORMATION_SCHEMA.TABLE_CONSTRAINTS where CONSTRAINT_TYPE = 'PRIMARY KEY') as c where a.CONSTRAINT_NAME = b.CONSTRAINT_NAME and b.UNIQUE_CONSTRAINT_NAME = c.CONSTRAINT_NAME and TABLE_NAME != '{tableName}';").ToList();
            return result;
        }
        
        /// <summary>
        /// Получение сырового списка таблиц от которых зависит исходная таблица 
        /// </summary>
        /// <param name="tableName">Название таблицы</param>
        /// <returns></returns>
        private string GetTableDependency(string tableName)
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

        /// <summary>
        /// Получение списка таблиц от которых зависит исходная таблицы
        /// </summary>
        /// <param name="tableName">Название таблицы</param>
        /// <returns></returns>
        public List<string> GetTableDependencyTables(string tableName)
        {
            var list = GetTableDependency(tableName).Split();
            var result = new List<string>();
            foreach (var item in list)
                if (!result.Contains(item) && !item.IsNullOrEmpty())
                    result.Add(item);
            return result;
        }

        /// <summary>
        /// Получение схемы таблицы
        /// </summary>
        /// <param name="tableName">Название таблицы</param>
        /// <returns></returns>
        /// <exception cref="Exception">Таблца не найдена</exception>
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
