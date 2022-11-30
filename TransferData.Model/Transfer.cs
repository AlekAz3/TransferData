using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using System.ComponentModel.DataAnnotations;
using Microsoft.IdentityModel.Tokens;

namespace TransferData.Model
{
    public class Transfer
    {
        private readonly DataContext _data;
        private readonly SchemaInfo _schema;
        private string colums;



        private List<string> typesPosgreSQL = new List<string>() { "character varying", "date", "varchar"};//Типы данных где нужны ковычки  

        private List<string> typesMSSQL = new List<string>() { "varchar", "date" };//Типы данных где нужны ковычки  


        public Transfer(DataContext data, SchemaInfo schema)
        {
            _data = data;
            _schema = schema;
            colums = String.Join(", ", _schema.Fields.Select(x => x.FieldName));

        }

        public string GenerateSelectQuary() => $"select {colums} from {_schema.TableName}";
        

        public string GenerateTempTableQuaty(DbType dbType)
        {
            var values = ConvertDataTableToList(GetDataTable(GenerateSelectQuary()));
            StringBuilder command = new StringBuilder() ;
            switch (dbType)
            {
                case DbType.Postgres:
                    command.AppendLine($"select {colums} into temp table Temp{_schema.TableName} from");
                    command.AppendLine("( ");
                    for (int i = 0; i < values.Count - 1; i++)
                        command.AppendLine($"select {JoinWithQuetes(values[i], typesPosgreSQL)} union all");

                    command.AppendLine($"select {JoinWithQuetes(values[values.Count - 1], typesPosgreSQL)}");
                    break;
                case DbType.MSSQL:
                    command.AppendLine($"select {colums} into #Temp{_schema.TableName} from");
                    command.AppendLine("( ");
                    for (int i = 0; i < values.Count - 1; i++)
                        command.AppendLine($"select {JoinWithQuetes(values[i], typesMSSQL)} union all");

                    command.AppendLine($"select {JoinWithQuetes(values[values.Count - 1], typesMSSQL)}");

                    break;
            }
            command.AppendLine(") as dt");

            return command.ToString();
        }
        
        public string JoinWithQuetes(List<string> input, List<string> types)
        {

            List<string> data_type = _schema.Fields.Select(x => x.FieldType).ToList();
            var columnName = _schema.Fields;

            string result = "";

            for (int i = 0; i < input.Count - 1; i++)
                if (types.Contains(data_type[i]) && input[i] != "null")
                    result += $"'{input[i]}' as {columnName[i].FieldName}, ";
                else 
                    result += $"{input[i]} as {columnName[i].FieldName}, ";
                
            if (types.Contains(data_type[input.Count - 1]))
                result += $"'{input[input.Count - 1]}' as {columnName[input.Count - 1].FieldName}";            
            else
                result += $"{input[input.Count - 1]} as {columnName[input.Count - 1].FieldName}";
            

            return result;
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
