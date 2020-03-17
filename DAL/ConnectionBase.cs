﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using Dapper;
using Base;
using System.Linq;
using System.Data;
using static DAL.ConnectionBase;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DAL
{
    public class ConnectionBase
    {
        public string ConnectionString { get; set; }
        public int CommandTimeout { get; set; }

        ILogger log;

        public ConnectionBase(IConfiguration con, ILogger<ConnectionBase> log)
        {
            this.ConnectionString = con["NorthwindConnection"];
            this.CommandTimeout = int.Parse(con["CommandTimeout"]);
            this.log = log;
        }

        public (Result rtn ,T result) GetSingleDefault<T>(string sqlCmd , DynamicParameters Params)
        {
            Result rtn = new Result();
            T result = (T)Convert.ChangeType(null,typeof(T));
            using (SqlConnection connection = new SqlConnection(this.ConnectionString))
            {
                try
                {
                    this.log.LogInformation($"ConnectionBase:{sqlCmd}");
                    this.log.LogInformation($"ConnectionBase:{this.ConnectionString}");
                    connection.Open();
                    result = connection.Query<T>(sqlCmd, Params).SingleOrDefault(); //只允許查出一筆

                    this.log.LogInformation($"ConnectionBase:{(result == null ? string.Empty : result.ToString())}");
                    rtn.IsSuccess = true;
                }
                catch (Exception ex)
                {
                    this.log.LogInformation($"ConnectionBase Exception:{ex.StackTrace}");
                    this.log.LogInformation($"ConnectionBase Exception:{ex.Message}");
                    rtn.IsSuccess = false;
                    rtn.ErrorMsg = ex.Message.ToString();
                }
                finally
                {
                    connection.Close();
                }
            }
            return (rtn, result);
        }

        public (Result rtn, IEnumerable<T> result) GetList<T>(string sqlCmd, DynamicParameters Params)
        {
            Result rtn = new Result();
            List<T> result = null;
            using (SqlConnection connection = new SqlConnection(this.ConnectionString))
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

        public (Result rtn, IEnumerable<T> result) GetList<T>(string sqlCmd, object Params)
        {
            Result rtn = new Result();
            List<T> result = null;
            using (SqlConnection connection = new SqlConnection(this.ConnectionString))
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
            using (SqlConnection connection = new SqlConnection(this.ConnectionString))
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
            using (SqlConnection connection = new SqlConnection(this.ConnectionString))
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
            using (SqlConnection connection = new SqlConnection(this.ConnectionString))
            {
                try
                {
                    Rows = connection.Execute(sqlCmd, Params,
                                                        commandTimeout: this.CommandTimeout,
                                                        commandType: CommandType.Text);
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
