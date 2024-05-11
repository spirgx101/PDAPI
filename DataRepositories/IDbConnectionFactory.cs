using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;

namespace PDAPI.DataRepositories
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateDbConnection(DbConnectionName connectionName);
    }
}
