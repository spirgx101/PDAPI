using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;


namespace PDAPI.DataRepositories
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly IDictionary<DbConnectionName, string> _connectionDict;

        public DbConnectionFactory(IDictionary<DbConnectionName, string> connectionDic)
        {
            _connectionDict = connectionDic;
        }

        public IDbConnection CreateDbConnection(DbConnectionName connectionName)
        {
            string connectionString = null;

            if (_connectionDict.TryGetValue(connectionName, out connectionString))
            {
                return new SqlConnection(connectionString);
            }

            throw new ArgumentNullException();
        }

    }
}
