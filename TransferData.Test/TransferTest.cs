﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransferData.Test
{
    public class TransferTest
    {
        private readonly IConfiguration _config;
        private readonly DbSchemaExtractor _extractor;
        private readonly Transfer _transfer;
        private readonly DataContext _data;

        public TransferTest()
        {
            var builder = new ConfigurationBuilder()
                //.AddInMemoryCollection()
                .AddJsonFile($"appsettings.json", optional: false, reloadOnChange: true);
            _config = builder.Build();
            _data = new DataContext(new NullLogger<DataContext>(), _config);
            _extractor = new DbSchemaExtractor(new DataContext(new NullLogger<DataContext>(), _config));
            _transfer = new Transfer(_data, _extractor.GetTableSchema("table1").Result, DbType.MSSQL);
        }

        [Fact (DisplayName = "Проверка генерации селект запроса")]
        public void TransferTest_SelectQuaryString()
        {
            var result = _transfer.GenerateSelectQuary();

            Assert.Equal("select id1, datefield, realnum, id2 from table1", result);
        }

        [Fact (DisplayName = "Проверка генерации запроса для временной таблицы")]
        public void TransferTest_CheckCreateTempTable()
        {
            var result = _transfer.GenerateTempTableQuary();
            var a = result.Split("\r\n");
            Assert.Equal("select id1, datefield, realnum, id2 into #Temptable1 from", a[0]);
            Assert.Equal("select 1 as id1, '04.12.2020 0:00:00' as datefield, 167.87 as realnum, 3 as id2 union all", a[2]);
        }


        [Fact]
        public void TransferTest_CheckCompareFieldInMerge()
        {
            var result = _transfer.CompareColumns();
            Assert.Equal("T_Base.id1 = T_Source.id1, T_Base.datefield = T_Source.datefield, T_Base.realnum = T_Source.realnum, T_Base.id2 = T_Source.id2", result);
        }

        [Fact]
        public void TransferTest_CheckColumnsWithTableName()
        {
            var result = _transfer.ColumnsWithTableName("T_Base");
            Assert.Equal("T_Base.id1, T_Base.datefield, T_Base.realnum, T_Base.id2", result);
        }


    }
}