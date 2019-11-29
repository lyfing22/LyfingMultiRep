using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LyfingMultiRep
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext context;
        private bool _disposed = false;

        public UnitOfWork(DbContext context)
        {
            this.context = context;
        }
        public void Dispose()
        {
            if (!_disposed)
            {
                this.context.Dispose();
            }
            _disposed = true;
            GC.SuppressFinalize(this);
        }

        public IDbConnection GetConnection()
        {
            return this.context.Database.Connection;
        }
    }
}
