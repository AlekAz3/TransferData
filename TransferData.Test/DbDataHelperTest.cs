using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransferData.Test
{
    public class DbDataHelperTest
    {
        private readonly IConfiguration _config;
        private readonly DbSchemaExtractor _extractor;
        private readonly DataContext _data;
        private readonly DbDataHelper _dataHelper;

        public DbDataHelperTest()
        {
            var builder = new ConfigurationBuilder()
                //.AddInMemoryCollection()
                .AddJsonFile($"appsettings.json", optional: false, reloadOnChange: true);
            _config = builder.Build();
            _data = new DataContext(new NullLogger<DataContext>(), _config);
            _extractor = new DbSchemaExtractor(_data);
            _dataHelper = new DbDataHelper();
        }
        [Fact]
        public void CompareColumns_Test()
        {
            SchemaInfo schemaInfo = new SchemaInfo("table1", new List<FieldInfo>() { new FieldInfo("col1", "a"), new FieldInfo("col2", "a"), new FieldInfo("col3", "a") });
            var result = _dataHelper.CompareColumns(schemaInfo);

            string expect = "col2 = T_Source.col2, col3 = T_Source.col3";

            Assert.Equal(expect, result);
        }

        [Fact]
        public void ColumnsWithTableName_Test()
        {
            SchemaInfo schemaInfo = new SchemaInfo("table1", new List<FieldInfo>() { new FieldInfo("col1", "a"), new FieldInfo("col2", "a"), new FieldInfo("col3", "a") });

            var result = _dataHelper.ColumnsWithTableName("Table", schemaInfo);

            string expect = "Table.col2, Table.col3";

            Assert.Equal(expect, result);
        }

        [Fact]
        public void fieldsWithQuotes_Test()
        {
            SchemaInfo schemaInfo = new SchemaInfo("table1", new List<FieldInfo>() { new FieldInfo("col1", "int"), new FieldInfo("col2", "varchar"), new FieldInfo("col3", "double") });
            List<string> data = new List<string>() { "1", "twq", "4.6"};
            var result = _dataHelper.FieldsWithQuotes(data, schemaInfo, DbType.MSSQL);
            string expect = $"1 as col1, 'twq' as col2, 4.6 as col3";

            Assert.Equal(expect, result);

        }

    }
}
