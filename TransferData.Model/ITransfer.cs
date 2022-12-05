using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransferData.Model
{
    public delegate ITransfer TransferResolver();

    public interface ITransfer
    {
        string GenerateTempTableQuary(string tableName);
        string GenerateMergeQuary(string tableName);
    }
}
