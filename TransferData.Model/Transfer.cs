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
        private readonly DbDataExtractor _dataExtractor;
        private readonly SchemaInfo _schema;
        private readonly DbType _type;
        private string colums;
        private List<string> typesForPosgreSQL = new List<string>() { "character varying", "date", "varchar" };//Типы данных где нужны ковычки  
        private List<string> typesForMSSQL = new List<string>() { "varchar", "date" };//Типы данных где нужны ковычки  

        public Transfer(DataContext data, SchemaInfo schema, DbDataExtractor dataExtractor ,DbType dbType)
        {
            _data = data;
            _dataExtractor = dataExtractor;
            _schema = schema;
            colums = String.Join(", ", _schema.Fields.Select(x => x.FieldName));
            _type = dbType;
        }

        public string GenerateSelectQuary() => $"select {colums} from {_schema.TableName}";


        public string GenerateTempTableQuary()
        {
            var data = _dataExtractor.ConvertDataTableToList(_dataExtractor.GetDataTable(GenerateSelectQuary()));
            var command = new StringBuilder();
            switch (_type)
            {
                case DbType.PostgreSQL:
                    command.AppendLine($"select {colums} into temp table Temp{_schema.TableName} from");
                    break;
                case DbType.MSSQL:
                    command.AppendLine($"select {colums} into #Temp{_schema.TableName} from");
                    break;
            }
            command.AppendLine("( ");
            for (int i = 0; i < data.Count - 1; i++)
                command.AppendLine($"select {JoinWithQuetesForSelect(data[i])} union all");
            command.AppendLine($"select {JoinWithQuetesForSelect(data[data.Count - 1])}");

            command.AppendLine(") as dt");

            return command.ToString();
        }



        public string GenerateMergeQuary()
        {
            var command = new StringBuilder();
            var columns = _schema.Fields.Select(x => x.FieldName).ToList(); 
            string firsColumn = _schema.Fields.Select(x => x.FieldName).ToList()[0];
            switch (_type)
            {
                case DbType.PostgreSQL:
                    command.AppendLine($"with upsert({firsColumn}) as");
                    command.AppendLine($"(");
                    command.AppendLine($"insert into {_schema.TableName} ({colums})");
                    command.AppendLine($"select {colums} from Temp{_schema.TableName}");
                    command.AppendLine($"on conflict ({firsColumn}) do update set ");
                    for (int i = 0; i < columns.Count() - 1; i++)
                    {
                        command.AppendLine($"{columns[i]} = excluded.{columns[i]},");
                    }
                    command.AppendLine($"{columns[columns.Count() - 1]} = excluded.{columns[columns.Count() - 1]}");

                    command.AppendLine($"returning {firsColumn} )");
                    command.AppendLine($"");
                    command.AppendLine($"--delete from {_schema.TableName} where {firsColumn} not in (select {firsColumn} from upsert);");

                    break;
                case DbType.MSSQL:
                    command.AppendLine($"merge {_schema.TableName} AS T_Base ");
                    command.AppendLine($"using #Temp{_schema.TableName} AS T_Source ");
                    command.AppendLine($"on (T_Base.{_schema.Fields[0].FieldName} = T_Source.{_schema.Fields[0].FieldName}) ");
                    command.AppendLine($"when matched then ");
                    command.AppendLine($"update set {this.CompareColumns()} ");
                    command.AppendLine($"when not matched then ");
                    command.AppendLine($"insert ({String.Join(", ", columns)}) ");
                    command.AppendLine($"values ({this.ColumnsWithTableName("T_Source")}) ");
                    command.AppendLine($"--when not matched by source then delete");
                    break;
            }
     
            return command.ToString();
        }

        public string CompareColumns()
        {
            string line = String.Empty;
            for (int i = 1; i < _schema.Fields.Count; i++)
            {
                line += $"{_schema.Fields[i].FieldName} = T_Source.{_schema.Fields[i].FieldName}, ";
            }

            return line.Remove(line.Length - 2, 2);
        }


        public string ColumnsWithTableName(string tableName)
        {
            string line = String.Empty;

            for (int i = 1; i < _schema.Fields.Count; i++)
            {
                line += $"{tableName}.{_schema.Fields[i].FieldName}, ";
            }

            return line.Remove(line.Length - 2, 2);
        }

        public string JoinWithQuetesForSelect(List<string> input)
        {

            var data_type = _schema.Fields.Select(x => x.FieldType).ToList();
            var columnName = _schema.Fields;

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
                if (types.Contains(data_type[i]) && input[i] != "null")
                    result += $"'{input[i]}' as {columnName[i].FieldName}, ";
                else
                    result += $"{input[i]} as {columnName[i].FieldName}, ";

            return result.Remove(result.Length - 2, 2);
        }
    }
}
