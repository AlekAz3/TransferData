using System.ComponentModel.DataAnnotations.Schema;

namespace TransferData.Model
{
    [Table("INFORMATION_SCHEMA.COLUMNS")]
    public class InformationSchema
    {
        [Column("TABLE_SCHEMA")]
        public string table_schema { get; set; }

        [Column("TABLE_NAME")]
        public string table_name { get; set; }

        [Column("COLUMN_NAME")]
        public string column_name { get; set; }

        [Column("DATA_TYPE")]
        public string data_type { get; set; }

    }
}
