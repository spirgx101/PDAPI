using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PDAPI.DataRepositories
{
    public class DbCSCRepositoryBase
    {
        public IDbConnection DbConnection { get; private set; }
        public DbCSCRepositoryBase(IDbConnectionFactory dbConnectionFactory)
        {
            this.DbConnection = dbConnectionFactory.CreateDbConnection(DbConnectionName.CSC);
        }
    }
}
