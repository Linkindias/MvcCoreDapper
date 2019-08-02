using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;

namespace DAL
{
    public class ConnectionBase
    {
        public string ConnectionString { get; set; }

        public ConnectionBase(string con)
        {
            this.ConnectionString = con;
        }
    }
}
