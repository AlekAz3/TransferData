namespace TransferData.Model
{
    public class DbDataHelper
    {

        public string CompareColumns(SchemaInfo schema)
        {
            string line = String.Empty;
            for (int i = 1; i < schema.Fields.Count; i++)
            {
                line += $"{schema.Fields[i].FieldName} = T_Source.{schema.Fields[i].FieldName}, ";
            }

            return line.Remove(line.Length - 2, 2);
        }

        internal object ColumnsWithTableName(string tableName, SchemaInfo schema)
        {
            string line = String.Empty;

            for (int i = 1; i < schema.Fields.Count; i++)
            {
                line += $"{tableName}.{schema.Fields[i].FieldName}, ";
            }

            return line.Remove(line.Length - 2, 2);
        }
    }
}