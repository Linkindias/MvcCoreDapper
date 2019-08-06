﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using Dapper;
using Base;
using System.Linq;
using System.Data;

namespace DAL
{
    public class ConnectionBase
    {
        public string ConnectionString { get; set; }
        public int CommandTimeout { get; set; }

        public ConnectionBase(string con, int timeout)
        {
            this.ConnectionString = con;
            this.CommandTimeout = timeout;
        }

        public (Result rtn ,T result) GetFirstDefault<T>(string sqlCmd , DynamicParameters[] Params)
        {
            Result rtn = new Result();
            T result = (T)Convert.ChangeType(null,typeof(T));
            using (var connection = new SqlConnection(this.ConnectionString))
            {
                try
                {
                    result = connection.Query<T>(sqlCmd, Params).FirstOrDefault();
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

        public (Result rtn, IEnumerable<T> result) GetList<T>(string sqlCmd, DynamicParameters[] Params)
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

        public (Result rtn, int Rows) GetStoredProcedureOfRow(string sqlCmd, DynamicParameters[] Params)
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

        public (Result rtn, List<DynamicParameters> Params) GetStoredProcedureOfParams(string sqlCmd, DynamicParameters[] Params)
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
            return (rtn, Rowdds);
        }
    }
}