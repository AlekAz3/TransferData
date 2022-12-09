using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransferData.Model.Infrastructure;
using TransferData.Model.Services;

namespace TransferData.Test
{
    public class DbDependencyTest
    {

        private readonly IConfiguration _config;
        private readonly DataContext _data;
        private readonly MetadataExtractor _dependency;

        public DbDependencyTest()
        {
            var builder = new ConfigurationBuilder()
                //.AddInMemoryCollection()
                .AddJsonFile($"appsettings.json", optional: false, reloadOnChange: true);
            _config = builder.Build();
            _data = new DataContext(new NullLogger<DataContext>(), _config);
            _dependency = new MetadataExtractor(_data);
        }

        [Fact]
        public void GetPrimaryKeyColumn_Test()
        {
            string expected = "id1";
            string result = _dependency.GetPrimaryKeyColumn("table1");
           
            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetForiegnKeyColumns_Test()
        {
            List<string> expected = new List<string>() { "id2", "id4"};
            List<string> result = _dependency.GetForiegnKeyColumns("table1");

            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetParentTable_Test()
        {
            List<string> expected = new List<string>() { "table1" };
            List<string> result = _dependency.GetParentTables("table1");

            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetTableDependencyTables_Test()
        {
            var expected = new List<string>() {"table1", "table2",  "table4", "table3", "table5" };
            //var expected = "";
            var result = _dependency.GetTableDependencyTables("Layers");


            Assert.Equal(expected, result);
        }

    }
}