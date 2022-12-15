using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransferData.Model.Infrastructure;
using TransferData.Model.Models;
using TransferData.Model.Services;

namespace TransferData.Test
{
    public class MetadataExtractorTest
    {
        private readonly IConfiguration _config;
        private readonly MetadataExtractor _metadata;
        private readonly DataContext _data;

        public MetadataExtractorTest()
        {
            var builder = new ConfigurationBuilder()
                //.AddInMemoryCollection()
                .AddJsonFile($"appsettings.json", optional: false, reloadOnChange: true);
            _config = builder.Build();
            _data = new DataContext(new NullLogger<DataContext>(), _config);
            _metadata = new MetadataExtractor(_data);

        }

        [Fact(DisplayName = "Проверка на наличие таблицы в бд")]
        public void DbSchemaExtractor_FoundTableName()
        {
            string table = "table1";
            var result = _metadata.GetTableSchema(table);
            Assert.Equal(table, result.TableName);

        }

        [Fact(DisplayName = "Проверка на правильное чтение столбцов в таблице")]
        public void DbSchemaExtractor_FoundTableFields()
        {
            string table = "table1";
            var result = _metadata.GetTableSchema(table);
            List<FieldInfo> fields;

            fields = new List<FieldInfo>()
            {
                new FieldInfo("id1", "int"),
                new FieldInfo("field1", "varchar"),
                new FieldInfo("id2", "int"),
                new FieldInfo("id4", "int"),
                new FieldInfo("boolf", "bit"),
            };

            Assert.Equal(fields, result.Fields);

        }

        [Fact(DisplayName = "Проверка работы при отсутсвующей таблицы")]
        public void DbSchemaExtractor_NotFoundTable()
        {
            Assert.Throws<Exception>(() => _metadata.GetTableSchema("Table111"));
        }

        [Fact(DisplayName = "Проверка на получение первичного ключа таблицы")]
        public void GetPrimaryKeyColumn_Test()
        {
            string expected = "id1";
            string result = _metadata.GetPrimaryKeyColumn("table1");

            Assert.Equal(expected, result);
        }

        [Fact(DisplayName = "Проверка на поиск внешних ключей в таблице")]
        public void GetForiegnKeyColumns_Test()
        {
            List<string> expected = new List<string>() { "id2", "id4" };
            List<string> result = _metadata.GetForiegnKeyColumns("table1");

            Assert.Equal(expected, result);
        }

        [Fact(DisplayName = "Провверка на поиск таблиц от которых зависит данная таблица")]
        public void GetParentTable_Test()
        {
            List<string> expected = new List<string>() { "table2", "table4"};
            List<string> result = _metadata.GetParentTables("table1");

            Assert.Equal(expected, result);
        }

        [Fact(DisplayName = "Проверка на поиск ВСЕХ зависимостей")]
        public void GetTableDependencyTables_Test()
        {
            var expected = new List<string>() { "table1", "table2", "table4", "table3", "table5" };

            var result = _metadata.GetTableDependencyTables("table1");

            Assert.Equal(expected, result);
        }
    }
}
