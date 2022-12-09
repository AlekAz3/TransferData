using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using TransferData.Model.Infrastructure;
using TransferData.Model.Models;
using TransferData.Model.Services;

namespace TransferData.Test
{
    public class DbSchemaExtractorTest
    {
        private readonly IConfiguration _config;
        private readonly MetadataExtractor _extractor;
        private readonly DataContext _data;

        public DbSchemaExtractorTest()
        {
            var builder = new ConfigurationBuilder()
                //.AddInMemoryCollection()
                .AddJsonFile($"appsettings.json", optional: false, reloadOnChange: true);
            _config = builder.Build();
            _data = new DataContext(new NullLogger<DataContext>(), _config);
            _extractor = new MetadataExtractor(_data);

        }

        [Fact]
        public void DbSchemaExtractor_FoundTableName()
        {
            string table = "table1";
            var result = _extractor.GetTableSchema(table);
            Assert.Equal(table, result.TableName);

        }

        [Fact]
        public void DbSchemaExtractor_FoundTableFields()
        {
            string table = "table1";
            var result = _extractor.GetTableSchema(table);
            List<FieldInfo> fields;


            //PostgreSQL
            fields = new List<FieldInfo>() 
            { 
                new FieldInfo("id1", "integer"), 
                new FieldInfo("field1", "real"), 
                new FieldInfo("id4", "integer"),
                new FieldInfo("id2", "integer")
            };

            //MSSQL
            //fields = new List<FieldInfo>() { new FieldInfo("id1", "int"), new FieldInfo("datefield", "date"), new FieldInfo("realnum", "real"), new FieldInfo("id2", "int") };

            Assert.Equal(fields, result.Fields);

        }

        [Fact(DisplayName = "ѕроверка работы при отсутсвующей таблицы")]
        public void DbSchemaExtractor_NotFoundTable()
        {
            Assert.Throws<Exception>(() => _extractor.GetTableSchema("Table111"));
        }

    }
}