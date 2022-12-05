﻿using System.Text;

namespace TransferData.Model
{
    public class TransferMSSQL : ITransfer
    {
        private readonly DbDataExtractor _dataExtractor;
        private readonly DbSchemaExtractor _schemaExtractor;
        private readonly DbDataHelper _dataHelper;

        public TransferMSSQL(DbDataExtractor dataExtractor, DbSchemaExtractor schemaExtractor, DbDataHelper dataHelper)
        {
            _dataExtractor = dataExtractor;
            _schemaExtractor = schemaExtractor;
            _dataHelper = dataHelper;
        }

        public string GenerateMergeQuary(string tableName)
        {
            var schema = _schemaExtractor.GetTableSchema(tableName).Result;
            var command = new StringBuilder();
            var columns = schema.Fields.Select(x => x.FieldName).ToList();
            string firsColumn = columns[0];
            columns.RemoveAt(0);
            var columsJoin = String.Join(", ", columns);

            command.AppendLine($"merge {schema.TableName} AS T_Base ");
            command.AppendLine($"using #Temp{schema.TableName} AS T_Source ");
            command.AppendLine($"on (T_Base.{schema.Fields[0].FieldName} = T_Source.{schema.Fields[0].FieldName}) ");
            command.AppendLine($"when matched then ");
            command.AppendLine($"update set {_dataHelper.CompareColumns(schema)} ");
            command.AppendLine($"when not matched then ");
            command.AppendLine($"insert ({String.Join(", ", columns)}) ");
            command.AppendLine($"values ({_dataHelper.ColumnsWithTableName("T_Source", schema)}) ");
            command.AppendLine($"--when not matched by source then delete");

            return command.ToString();

        }

        public string GenerateTempTableQuary(string tableName)
        {
            //var schema = _schemaExtractor.GetTableSchema(tableName).Result;
            //var columnsJoin = String.Join(", ", schema.Fields.Select(x => x.FieldName));
            //var tableData = _dataExtractor.ConvertDataTableToList(tableName);

            //var command = new StringBuilder();
            //command.AppendLine($"select {columnsJoin} into #Temp{schema.TableName} from");


            //command.AppendLine("( ");
            //for (int i = 0; i < tableData.Count - 1; i++)
            //    command.AppendLine($"select {JoinWithQuetesForSelect(tableData[i], schema)} union all");
            //command.AppendLine($"select {JoinWithQuetesForSelect(tableData[tableData.Count - 1], schema)}");

            //command.AppendLine(") as dt");

            //return command.ToString();
            return "";
        }
    }
}