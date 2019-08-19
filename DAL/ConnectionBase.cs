using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using Dapper;
using Base;
using System.Linq;
using System.Data;
using static DAL.ConnectionBase;

namespace DAL
{
    public class ConnectionBase
    {
        public string ConnectionString { get; set; }
        public int CommandTimeout { get; set; }

        public ConnectionBase()
        {
        }

        public ConnectionBase(string con, int timeout)
        {
            this.ConnectionString = con;
            this.CommandTimeout = timeout;
        }

        public (Result rtn ,T result) GetSingleDefault<T>(string sqlCmd , DynamicParameters Params)
        {
            Result rtn = new Result();
            T result = (T)Convert.ChangeType(null,typeof(T));
            using (var connection = new SqlConnection(this.ConnectionString))
            {
                try
                {
                    result = connection.Query<T>(sqlCmd, Params).SingleOrDefault(); //只允許查出一筆
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

        public (Result rtn, IEnumerable<T> result) GetList<T>(string sqlCmd, DynamicParameters Params)
        {
            Result rtn = new Result();
            List<T> result = null;
            using (var connection = new SqlConnection(this.ConnectionString))
            {
                try
                {
                    result = connection.Query<T>(sqlCmd, Params).ToList();
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

        public (Result rtn, int Rows) GetStoredProcedureOfRow(string sqlCmd, DynamicParameters Params)
        {
            Result rtn = new Result();
            int Rows = 0;
            using (var connection = new SqlConnection(this.ConnectionString))
            {
                try
                {
                    Rows = connection.Execute(sqlCmd, Params,
                                                        commandTimeout : this.CommandTimeout,
                                                        commandType: CommandType.StoredProcedure);
                    rtn.IsSuccess = true;
                }
                catch (Exception ex)
                {
                    rtn.IsSuccess = false;
                    rtn.ErrorMsg = ex.Message.ToString();
                }
            }
            return (rtn, Rows);
        }

        public (Result rtn, DynamicParameters Params) GetStoredProcedureOfParams(string sqlCmd, DynamicParameters Params)
        {
            Result rtn = new Result();
            using (var connection = new SqlConnection(this.ConnectionString))
            {
                try
                {
                    int rows = connection.Execute(sqlCmd, Params,
                                                        commandTimeout: this.CommandTimeout,
                                                        commandType: CommandType.StoredProcedure);
                    if (rows > 0) rtn.IsSuccess = true;
                }
                catch (Exception ex)
                {
                    rtn.IsSuccess = false;
                    rtn.ErrorMsg = ex.Message.ToString();
                }
            }
            return (rtn, Params);
        }

        public (Result rtn, int Rows) GetCUDOfRow(string sqlCmd, DynamicParameters Params)
        {
            Result rtn = new Result();
            int Rows = 0;
            using (var connection = new SqlConnection(this.ConnectionString))
            {
                try
                {
                    Rows = connection.Execute(sqlCmd, Params,
                                                        commandTimeout: this.CommandTimeout,
                                                        commandType: CommandType.StoredProcedure);
                    rtn.IsSuccess = true;
                }
                catch (Exception ex)
                {
                    rtn.IsSuccess = false;
                    rtn.ErrorMsg = ex.Message.ToString();
                }
            }
            return (rtn, Rows);
        }
    }
}
