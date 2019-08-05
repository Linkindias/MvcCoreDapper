using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using Dapper;
using Base;
using System.Linq;

namespace DAL
{
    public class ConnectionBase
    {
        public string ConnectionString { get; set; }

        public ConnectionBase(string con)
        {
            this.ConnectionString = con;
        }

        public (Result rtn ,T result) GetFirstDefault<T>(string sqlCmd , DynamicParameters dynamicParams)
        {
            Result rtn = new Result();
            T result = (T)Convert.ChangeType(null,typeof(T));
            using (var connection = new SqlConnection(this.ConnectionString))
            {
                try
                {
                    result = connection.Query<T>(sqlCmd, dynamicParams).FirstOrDefault();
                    rtn.IsSuccess = true;
                }
                catch (Exception ex)
                {
                    rtn.IsSuccess = false;
                    rtn.ErrorMsg = ex.Message.ToString();
                }

            }
            return (rtn, result);
        }

        public (Result rtn, IEnumerable<T> result) GetList<T>(string sqlCmd, DynamicParameters dynamicParams)
        {
            Result rtn = new Result();
            List<T> result = null;
            using (var connection = new SqlConnection(this.ConnectionString))
            {
                try
                {
                    result = connection.Query<T>(sqlCmd, dynamicParams).ToList();
                    rtn.IsSuccess = true;
                }
                catch (Exception ex)
                {
                    rtn.IsSuccess = false;
                    rtn.ErrorMsg = ex.Message.ToString();
                }
            }
            return (rtn, result);
        }
    }
}
