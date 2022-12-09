﻿namespace TransferData.Model.Models
{
    public record SchemaInfo
    {
        public string TableName { get; init; }
        public List<FieldInfo> Fields { get; init; }

        public SchemaInfo(string tableName, List<FieldInfo> fields)
        {
            TableName = tableName;
            Fields = fields;
        }

        internal string SetValuesSubQuery()
        {
            return string.Join(", ",
                Fields.Select(x => $"{x.FieldName} = {Constants.TableSourceName}.{x.FieldName}"));
        }

        internal string ColumnsWithTableName()
        {
            return string.Join(", ",
                Fields.Select(x => $"{Constants.TableSourceName}.{x.FieldName}"));
        }

        internal string FieldsWithQuotes(List<string> input, DbType dbType)
        {
            List<string> result = new List<string>();

            for (int i = 0; i < input.Count; i++)
            {
                result.Add($"{Fields[i].DataCheckQuotes(input[i], dbType)} as {Fields[i].FieldName}");
            }

            return string.Join(", ", result);

        }

    }
}
