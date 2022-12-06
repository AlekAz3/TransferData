﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransferData.Test
{
    public class DbDependencyTest
    {

        private readonly IConfiguration _config;
        private readonly DataContext _data;
        private readonly DbDependency _dependency;

        public DbDependencyTest()
        {
            var builder = new ConfigurationBuilder()
                //.AddInMemoryCollection()
                .AddJsonFile($"appsettings.json", optional: false, reloadOnChange: true);
            _config = builder.Build();
            _data = new DataContext(new NullLogger<DataContext>(), _config);
            _dependency = new DbDependency(_data);
        }

        [Fact]
        public void GetPrimaryKeyColumn_Test()
        {
            string expected = "code";
            string result = _dependency.GetPrimaryKeyColumn("pc");
           
            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetForiegnKeyColumns_Test()
        {
            List<string> expected = new List<string>() { "model"};
            List<string> result = _dependency.GetForiegnKeyColumns("pc");

            Assert.Equal(expected, result);
        }

    }
}
