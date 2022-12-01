using Microsoft.EntityFrameworkCore;

namespace TransferData.Model
{
    public class DbSchemaExtractor
    {
        private readonly DataContext _data;
        //.FromSqlRaw($"select table_schema,table_name, column_name, data_type from information_schema.columns where table_name = '{tableName}'")

        public DbSchemaExtractor(DataContext data)
        {
            _data = data;
        }

        public async Task<SchemaInfo> GetTableSchema(string tableName)
        {
            var informationSchema = await _data.Schema
                .FromSqlRaw($"select table_schema, table_name, column_name, data_type from information_schema.columns where table_name = '{tableName}' order by ordinal_position")
                .ToListAsync(CancellationToken.None);

            if (informationSchema.Count == 0)
                throw new Exception("Table not Found");
            

            var fields = new List<FieldInfo>();

            for (int i = 0; i < informationSchema.Count; i++)
                fields.Add(new FieldInfo(informationSchema[i].column_name, informationSchema[i].data_type));
            

            return new SchemaInfo(tableName, fields);
        }
    }
}
