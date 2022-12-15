namespace TransferData.Model.Models
{
    /// <summary>
    /// Класс для описания схемы таблицы 
    /// </summary>
    public record SchemaInfo
    {
        public string TableName { get; init; }
        public List<FieldInfo> Fields { get; init; }

        public SchemaInfo(string tableName, List<FieldInfo> fields)
        {
            TableName = tableName;
            Fields = fields;
        }

        /// <summary>
        /// Возвращает строку сравнение полей 
        /// </summary>
        /// <returns></returns>
        internal string SetValuesSubQuery()
        {
            return string.Join(", ",
                Fields.Select(x => $"{x.FieldNameWithEscape()} = {Constants.TableSourceName}.{x.FieldNameWithEscape()}"));
        }
        /// <summary>
        /// возвращает столбцы с названием таблицы
        /// </summary>
        /// <returns></returns>
        internal string ColumnsWithTableName()
        {
            return string.Join(", ",
                Fields.Select(x => $"{Constants.TableSourceName}.{x.FieldNameWithEscape()}"));
        }
        /// <summary>
        /// обращение данных столбцов в Апострофы если надо 
        /// </summary>
        /// <param name="input">Данные таблицы</param>
        /// <returns></returns>
        internal string FieldsWithQuotes(List<string> input)
        {
            List<string> result = new List<string>();

            for (int i = 0; i < input.Count; i++)
            {
                result.Add($"{Fields[i].DataCheckQuotes(input[i])} as {Fields[i].FieldNameWithEscape()}");
            }

            return string.Join(", ", result);

        }

    }
}
