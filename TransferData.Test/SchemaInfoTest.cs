using TransferData.Model.Models;

namespace TransferData.Test
{
    public class SchemaInfoTest
    {
        [Fact(DisplayName = "Проверка сравнения столбцов исходной и временной таблицы")]
        public void SetValuesSubQuery_Test()
        {
            SchemaInfo schemaInfo = new SchemaInfo("table1", new List<FieldInfo>() { new FieldInfo("col1", "a"), new FieldInfo("col2", "a"), new FieldInfo("col3", "a") });
            string result = schemaInfo.SetValuesSubQuery();

            string expect = "\"col1\" = T_Source.\"col1\", \"col2\" = T_Source.\"col2\", \"col3\" = T_Source.\"col3\"";

            Assert.Equal(expect, result);
        }

        [Fact(DisplayName = "Проверка того что столбцы выводятся с псевдонимом временной таблицы")]
        public void ColumnsWithTableName_Test()
        {
            SchemaInfo schemaInfo = new SchemaInfo("table1", new List<FieldInfo>() { new FieldInfo("col1", "a"), new FieldInfo("col2", "a"), new FieldInfo("col3", "a") });

            string result = schemaInfo.ColumnsWithTableName();

            string expect = "T_Source.\"col1\", T_Source.\"col2\", T_Source.\"col3\"";

            Assert.Equal(expect, result);
        }

        [Fact(DisplayName = "Проверка сверки данных каждой строчки с названием столбца")]
        public void fieldsWithQuotes_Test()
        {
            SchemaInfo schemaInfo = new SchemaInfo("table1", new List<FieldInfo>() { new FieldInfo("col1", "int"), new FieldInfo("col2", "varchar"), new FieldInfo("col3", "double") });
            List<string> data = new List<string>() { "1", "twq", "4.6"};
            string result = schemaInfo.FieldsWithQuotes(data);
            string expect = $"1 as \"col1\", 'twq' as \"col2\", 4.6 as \"col3\"";

            Assert.Equal(expect, result);

        }

    }
}
