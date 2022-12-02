using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Data.Common;
using System.Text;

namespace TransferData.Model
{
    public class Transfer
    {
        private readonly DataContext _data;
        private readonly DbSchemaExtractor _extractor;

        //private string colums;
        private List<string> typesForPosgreSQL = new List<string>() { "character varying", "date", "varchar" };//Типы данных где нужны ковычки  
        private List<string> typesForMSSQL = new List<string>() { "varchar", "date" };//Типы данных где нужны ковычки  


        public Transfer(DataContext data, DbSchemaExtractor extractor)
        {
            _data = data;
            _extractor = extractor;
        }

        public string GenerateSelectQuary(SchemaInfo schema) => $"select {String.Join(", ", schema.Fields.Select(x => x.FieldName))} from {schema.TableName}";


        public string GenerateTempTableQuary(string tableName, DbType type)
        {
            var schema = _extractor.GetTableSchema(tableName).Result;
            var columnsJoin = String.Join(", ", schema.Fields.Select(x => x.FieldName));
            var tableData = ConvertDataTableToList(GetDataTable(GenerateSelectQuary(schema)));

            var command = new StringBuilder();
            switch (type)
            {
                case DbType.PostgreSQL:
                    command.AppendLine($"select {columnsJoin} into temp table Temp{schema.TableName} from");
                    break;
                case DbType.MSSQL:
                    command.AppendLine($"select {columnsJoin} into #Temp{schema.TableName} from");
                    break;
            }
            command.AppendLine("( ");
            for (int i = 0; i < tableData.Count - 1; i++)
                command.AppendLine($"select {JoinWithQuetesForSelect(tableData[i], schema)} union all");
            command.AppendLine($"select {JoinWithQuetesForSelect(tableData[tableData.Count - 1], schema)}");

            command.AppendLine(") as dt");

            return command.ToString();
        }



        public string GenerateMergeQuary(string tableName, DbType type)
        {
            var schema = _extractor.GetTableSchema(tableName).Result;
            var command = new StringBuilder();
            var columns = schema.Fields.Select(x => x.FieldName).ToList(); 
            string firsColumn = columns[0];
            columns.RemoveAt(0);
            var columsJoin = String.Join(", ", columns);
            switch (type)
            {
                case DbType.PostgreSQL:
                    command.AppendLine($"with upsert({firsColumn}) as");
                    command.AppendLine($"(");
                    command.AppendLine($"insert into {schema.TableName} ({columsJoin})");
                    command.AppendLine($"select {columsJoin} from Temp{schema.TableName}");
                    command.AppendLine($"on conflict ({firsColumn}) do update set ");
                    for (int i = 0; i < columns.Count() - 1 ; i++)
                    {
                        command.AppendLine($"{columns[i]} = excluded.{columns[i]},");
                    }

                    command.AppendLine($"{columns[columns.Count() - 1]} = excluded.{columns[columns.Count() - 1]}");

                    command.AppendLine($"returning {firsColumn} )");
                    command.AppendLine($"");
                    command.AppendLine($"--delete from {schema.TableName} where {firsColumn} not in (select {firsColumn} from upsert);");

                    break;
                case DbType.MSSQL:
                    command.AppendLine($"merge {schema.TableName} AS T_Base ");
                    command.AppendLine($"using #Temp{schema.TableName} AS T_Source ");
                    command.AppendLine($"on (T_Base.{schema.Fields[0].FieldName} = T_Source.{schema.Fields[0].FieldName}) ");
                    command.AppendLine($"when matched then ");
                    command.AppendLine($"update set {this.CompareColumns(columns)} ");
                    command.AppendLine($"when not matched then ");
                    command.AppendLine($"insert ({String.Join(", ", columns)}) ");
                    command.AppendLine($"values ({this.ColumnsWithTableName("T_Source", columns)}) ");
                    command.AppendLine($"--when not matched by source then delete");
                    break;
            }
     
            return command.ToString();
        }

        public string CompareColumns(List<string> fieldName)
        {
            string line = String.Empty;
            foreach (string item in fieldName)
                line += $"{item} = T_Source.{item}, ";
            
            return line.Remove(line.Length - 2, 2);
        }


        public string ColumnsWithTableName(string tableName, List<string> columns)
        {
            string line = String.Empty;

            foreach(string column in columns)
                line += $"{tableName}.{column}, ";
            
            return line.Remove(line.Length - 2, 2);
        }

        public string JoinWithQuetesForSelect(List<string> input, SchemaInfo schema)
        {

            var data_type = schema.Fields.Select(x => x.FieldType).ToList();
            var columnName = schema.Fields;

            List<string> types = new List<string>();

            switch (_data.type)
            {
                case DbType.PostgreSQL:
                    types = typesForPosgreSQL;
                    break;
                case DbType.MSSQL:
                    types = typesForMSSQL;
                    break;
            }

            string result = String.Empty;

            for (int i = 0; i < input.Count; i++)
            {
                if (types.Contains(data_type[i]) && input[i] != "null")
                    result += $"'{input[i]}' as {columnName[i].FieldName}, ";
                else
                    result += $"{input[i]} as {columnName[i].FieldName}, ";
            }

            return result.Remove(result.Length - 2, 2);
        }


        public DataTable GetDataTable(string sqlQuery)
        {
            var dbFactory = DbProviderFactories.GetFactory(_data.Database.GetDbConnection());

            using (var cmd = dbFactory.CreateCommand())
            {
                cmd.Connection = _data.Database.GetDbConnection();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sqlQuery;
                using (var adapter = dbFactory.CreateDataAdapter())
                {
                    adapter.SelectCommand = cmd;
                    var dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    return dataTable;
                }
            }
        }

        public List<List<string>> ConvertDataTableToList(DataTable dataTable)
        {
            var resultList = new List<List<string>>();
            foreach (DataRow row in dataTable.Rows)
            {
                object[] cells = row.ItemArray;
                var cellsList = new List<string>();
                foreach (object cell in cells)
                {
                    if (cell is null)
                        cellsList.Add("null");
                    else
                        cellsList.Add(cell.ToString().Replace(',', '.'));
                }
                resultList.Add(cellsList);
            }
            return resultList;
        }
    }
}
