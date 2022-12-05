using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransferData.Model
{
    public class TransferPostgreSQL : ITransfer
    {
        public string GenerateMergeQuary(string tableName)
        {
            throw new NotImplementedException();
        }

        public string GenerateTempTableQuary(string tableName)
        {
            throw new NotImplementedException();
        }
    }
}
