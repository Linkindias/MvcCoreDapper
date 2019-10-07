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
        public virtual (Result rtn ,Employees employee) GetEmployeeByAccount(string Account, int Status)
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
        public virtual (Result rtn, Employees employee) GetEmployeeById(int EmployeeId, int Status)
        {
            string sqlCmd = "SELECT * FROM Employees Where EmployeeID = @Id And Status = @Status ";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", EmployeeId);
            parameters.Add("@Status", Status);

            var result = this.GetSingleDefault<Employees>(sqlCmd, parameters);

            return (result.rtn, result.result);
        }

        /// <summary>
        /// 更新員工
        /// </summary>
        /// <param name="employee">員工</param>
        public (Result rtn, int exeRows) UpdateEmployee(Employees employee)
        {
            string sqlCmd = @"Update Employees Set FirstName = @FirstName, LastName = @LastName, BirthDate = @BirthDate, HomePhone = @HomePhone, Address = @Address
                                Where EmployeeId = @EmpId";

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@FirstName", employee.FirstName);
            parameters.Add("@LastName", employee.LastName);
            parameters.Add("@BirthDate", employee.BirthDate);
            parameters.Add("@HomePhone", employee.HomePhone);
            parameters.Add("@Address", employee.Address);
            parameters.Add("@EmpId", employee.EmployeeID);

            var result = this.GetCUDOfRow(sqlCmd, parameters);

            return (result.rtn, result.Rows);
        }
    }
}
