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
        private readonly SchemaInfo _schema;
        private readonly DbType _type;

        private string colums;



        private List<string> typesPosgreSQL = new List<string>() { "character varying", "date", "varchar" };//Типы данных где нужны ковычки  

        private List<string> typesMSSQL = new List<string>() { "varchar", "date" };//Типы данных где нужны ковычки  


        public Transfer(DataContext data, SchemaInfo schema, DbType dbType)
        {
            _data = data;
            _schema = schema;
            colums = String.Join(", ", _schema.Fields.Select(x => x.FieldName));
            _type = dbType;
        }

        public string GenerateSelectQuary() => $"select {colums} from {_schema.TableName}";


        public string GenerateTempTableQuary()
        {
            var values = ConvertDataTableToList(GetDataTable(GenerateSelectQuary()));
            StringBuilder command = new StringBuilder();
            switch (_type)
            {
                case DbType.Postgres:
                    command.AppendLine($"select {colums} into temp table Temp{_schema.TableName} from");
                    break;
                case DbType.MSSQL:
                    command.AppendLine($"select {colums} into #Temp{_schema.TableName} from");
                    break;
            }
            command.AppendLine("( ");
            for (int i = 0; i < values.Count - 1; i++)
                command.AppendLine($"select {JoinWithQuetesForSelect(values[i])} union all");
            command.AppendLine($"select {JoinWithQuetesForSelect(values[values.Count - 1])}");

            command.AppendLine(") as dt");

            return command.ToString();
        }



        public string GenerateMergeQuary()
        {
            StringBuilder command = new StringBuilder();
            var columns = _schema.Fields.Select(x => x.FieldName).ToList(); 
            var firs_col = _schema.Fields.Select(x => x.FieldName).ToList()[0];
            switch (_type)
            {
                case DbType.Postgres:
                    command.AppendLine($"WITH upsert({firs_col}) AS");
                    command.AppendLine($"(");
                    command.AppendLine($"INSERT INTO {_schema.TableName} ({colums})");
                    command.AppendLine($"SELECT {colums} FROM Temp{_schema.TableName}");
                    command.AppendLine($"ON CONFLICT ({firs_col}) DO UPDATE SET ");


                    for (int i = 0; i < columns.Count() - 1; i++)
                    {
                        command.AppendLine($"{columns[i]} = excluded.{columns[i]},");
                    }
                    command.AppendLine($"{columns[columns.Count() - 1]} = excluded.{columns[columns.Count() - 1]}");

                    command.AppendLine($"RETURNING {firs_col} )");
                    command.AppendLine($"");
                    command.AppendLine($"--DELETE FROM {_schema.TableName} WHERE {firs_col} NOT IN (SELECT {firs_col} FROM upsert);");

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
            string line = "";
            for (int i = 1; i < _schema.Fields.Count; i++)
            {
                line += $"{_schema.Fields[i].FieldName} = T_Source.{_schema.Fields[i].FieldName}, ";
            }

            return line.Remove(line.Length - 2, 2);
        }


        public string ColumnsWithTableName(string table_Name)
        {
            string line = "";

            for (int i = 1; i < _schema.Fields.Count; i++)
            {
                line += $"{table_Name}.{_schema.Fields[i].FieldName}, ";
            }

            return line.Remove(line.Length - 2, 2);
        }

        public string JoinWithQuetesForSelect(List<string> input)
        {

            List<string> data_type = _schema.Fields.Select(x => x.FieldType).ToList();
            var columnName = _schema.Fields;

            List<string> types = new List<string>();

            switch (_data.type)
            {
                case DbType.Postgres:
                    types = typesPosgreSQL;
                    break;
                case DbType.MSSQL:
                    types = typesMSSQL;
                    break;
            }

            string result = "";

            for (int i = 0; i < input.Count; i++)
                if (types.Contains(data_type[i]) && input[i] != "null")
                    result += $"'{input[i]}' as {columnName[i].FieldName}, ";
                else
                    result += $"{input[i]} as {columnName[i].FieldName}, ";

            return result.Remove(result.Length - 2, 2);
        }


        public DataTable GetDataTable(string sqlQuery)
        {
            DbProviderFactory dbFactory = DbProviderFactories.GetFactory(_data.Database.GetDbConnection());

            using (var cmd = dbFactory.CreateCommand())
            {
                cmd.Connection = _data.Database.GetDbConnection();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sqlQuery;
                using (DbDataAdapter adapter = dbFactory.CreateDataAdapter())
                {
                    adapter.SelectCommand = cmd;
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }

        public List<List<string>> ConvertDataTableToList(DataTable dt)
        {
            //Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");// Вместо 32,23 теперь 32.23
            List<List<string>> a = new List<List<string>>();
            foreach (DataRow row in dt.Rows)
            {
                object[] cells = row.ItemArray;
                List<string> cellstr = new List<string>();
                foreach (object cell in cells)
                {
                    if (cell.ToString().IsNullOrEmpty())
                        cellstr.Add("null");
                    else
                        cellstr.Add(cell.ToString().Replace(',', '.'));
                }
                a.Add(cellstr);
            }
            return a;
        }

    }
}
