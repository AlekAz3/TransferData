using TransferData.Model.Models;

namespace TransferData.Test
{
    public class DbDataHelperTest
    {
        public DbDataHelperTest()
        {

        }
        [Fact]
        public void CompareColumns_Test()
        {
            SchemaInfo schemaInfo = new SchemaInfo("table1", new List<FieldInfo>() { new FieldInfo("col1", "a"), new FieldInfo("col2", "a"), new FieldInfo("col3", "a") });
            var result = schemaInfo.SetValuesSubQuery();

            string expect = "col1 = T_Source.col1, col2 = T_Source.col2, col3 = T_Source.col3";

            Assert.Equal(expect, result);
        }

        [Fact]
        public void ColumnsWithTableName_Test()
        {
            SchemaInfo schemaInfo = new SchemaInfo("table1", new List<FieldInfo>() { new FieldInfo("col1", "a"), new FieldInfo("col2", "a"), new FieldInfo("col3", "a") });

            var result = schemaInfo.ColumnsWithTableName();

            string expect = "T_Source.col2, T_Source.col3";

            Assert.Equal(expect, result);
        }

        [Fact]
        public void fieldsWithQuotes_Test()
        {
            SchemaInfo schemaInfo = new SchemaInfo("table1", new List<FieldInfo>() { new FieldInfo("col1", "int"), new FieldInfo("col2", "varchar"), new FieldInfo("col3", "double") });
            List<string> data = new List<string>() { "1", "twq", "4.6"};
            var result = schemaInfo.FieldsWithQuotes(data);
            string expect = $"1 as col1, 'twq' as col2, 4.6 as col3";

            Assert.Equal(expect, result);

        }

    }
}
