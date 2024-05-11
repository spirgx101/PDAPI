using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace PDAPI.DataRepositories
{
    public class xxxxxCSCContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connection;

        public xxxxxCSCContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connection = _configuration.GetConnectionString("CSC");
        }

        public IDbConnection CreateConnection()
            => new SqlConnection(_connection);
    }
}
