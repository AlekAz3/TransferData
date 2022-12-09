﻿using System.Text;
using TransferData.Model.Infrastructure;

namespace TransferData.Model.Services.Transfer
{
    public class TransferMSSQL : ITransfer
    {
        private readonly DbDataExtractor _dataExtractor;
        private readonly MetadataExtractor _metadataExtractor;
        private readonly DataContext _data;

        public TransferMSSQL(DbDataExtractor dataExtractor, MetadataExtractor metadataExtractor, DataContext data)
        {
            _dataExtractor = dataExtractor;
            _metadataExtractor = metadataExtractor;
            _data = data;
        }

        public string GenerateMergeQuery(string tableName)
        {
            var schema = _metadataExtractor.GetTableSchema(tableName);
            var sqlQueryString = new StringBuilder();
            var columns = schema.Fields.Select(x => x.FieldName).ToList();
            string firsColumn = columns[0];
            columns.RemoveAt(0);
            string columsJoin = string.Join(", ", columns);

            sqlQueryString.AppendLine($"merge {schema.TableName} AS T_Base ");
            sqlQueryString.AppendLine($"using #Temp{schema.TableName} AS T_Source ");
            sqlQueryString.AppendLine($"on (T_Base.{schema.Fields[0].FieldName} = T_Source.{schema.Fields[0].FieldName}) ");
            sqlQueryString.AppendLine($"when matched then ");
            sqlQueryString.AppendLine($"update set {schema.SetValuesSubQuery()} ");
            sqlQueryString.AppendLine($"when not matched then ");
            sqlQueryString.AppendLine($"insert ({string.Join(", ", columns)}) ");
            sqlQueryString.AppendLine($"values ({schema.ColumnsWithTableName()}) ");
            sqlQueryString.AppendLine($";");
            sqlQueryString.AppendLine($"--when not matched by source then delete");

            return sqlQueryString.ToString();

        }


        public string GenerateTempTableQuary(string tableName)
        {
            var schema = _metadataExtractor.GetTableSchema(tableName);
            string columnsJoin = string.Join(", ", schema.Fields.Select(x => x.FieldName));
            var tableData = _dataExtractor.ConvertDataTableToList(tableName);

            var sqlQueryString = new StringBuilder();
            sqlQueryString.AppendLine($"select {columnsJoin} into #Temp{schema.TableName} from");
            sqlQueryString.AppendLine("( ");
            for (int i = 0; i < tableData.Count - 1; i++)
                sqlQueryString.AppendLine($"select {schema.FieldsWithQuotes(tableData[i], _data.type)} union all");
            sqlQueryString.AppendLine($"select {schema.FieldsWithQuotes(tableData[tableData.Count - 1], _data.type)}");

            sqlQueryString.AppendLine(") as dt;");

            return sqlQueryString.ToString();
        }
    }
}