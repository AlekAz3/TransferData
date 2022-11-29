using Microsoft.EntityFrameworkCore;

namespace TransferData.Model
{
    public interface IDataContext
    {
        DbSet<InformationSchema> schema { get; }
        DbType type { get; }
    }
}