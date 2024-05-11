using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PDAPI.DataRepositories
{
    public abstract class DbPOPRepositoryBase
    {
        public IDbConnection DbConnection { get; private set; }

        public DbPOPRepositoryBase(IDbConnectionFactory dbConnectionFactory)
        {
            this.DbConnection = dbConnectionFactory.CreateDbConnection(DbConnectionName.POP);
        }
    }
}
