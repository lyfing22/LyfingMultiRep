using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LyfingMultiRep
{
    public interface IUnitOfWork : IDisposable
    {
        IDbConnection GetConnection();
    }
}
