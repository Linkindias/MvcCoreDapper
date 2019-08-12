using Base;
using DAL.DBModel;
using Dapper;
using System;
using System.Collections.Generic;
using System.Text;
using static Base.Enums;

namespace DAL.Repository
{
    public class EmployeeRepository : ConnectionBase
    {
        public EmployeeRepository(string con, int timeout) : base(con, timeout)
        {
        }

        /// <summary>
        /// 依帳號取得人員
        /// </summary>
        /// <param name="Account">帳號</param>
        public (Result rtn ,Employees employee) GetEmployeeByAccount(string Account, int Status)
        {
            string sqlCmd = "SELECT * FROM Employees Where Account = @Account And Status = @Status ";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Account", Account);
            parameters.Add("@Status", Status);

            var result = this.GetSingleDefault<Employees>(sqlCmd, parameters);

            return (result.rtn, result.result);
        }

        /// <summary>
        /// 依會員編號取得人員
        /// </summary>
        /// <param name="EmployeeId">會員編號</param>
        public (Result rtn, Employees employee) GetEmployeeById(int EmployeeId, int Status)
        {
            string sqlCmd = "SELECT * FROM Employees Where EmployeeID = @Id And Status = @Status ";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", EmployeeId);
            parameters.Add("@Status", Status);

            var result = this.GetSingleDefault<Employees>(sqlCmd, parameters);

            return (result.rtn, result.result);
        }
    }
}
