using Microsoft.EntityFrameworkCore;

namespace TransferData.Model
{
    public class DbSchemaExtractor
    {
        private readonly IDataContext _data;
        //.FromSqlRaw($"select table_schema,table_name, column_name, data_type from information_schema.columns where table_name = '{tableName}'")

        public DbSchemaExtractor(IDataContext data)
        {
            _data = data;
        }

        public async Task<SchemaInfo> GetTableSchema(string tableName)
        {
            var informationSchema = await _data.schema
                .FromSqlRaw($"select table_schema, table_name, column_name, data_type from information_schema.columns where table_name = '{tableName}'")
                //.Where(x => x.table_name == tableName)
                .ToListAsync(CancellationToken.None);

            if (informationSchema.Count == 0)
                throw new Exception("Table not Found");
            

            List<FieldInfo> fields = new List<FieldInfo>();

            for (int i = 0; i < informationSchema.Count; i++)
                fields.Add(new FieldInfo(informationSchema[i].column_name, informationSchema[i].data_type));
            

            return new SchemaInfo(tableName, fields);
        }
    }
}
