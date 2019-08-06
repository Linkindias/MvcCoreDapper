using Base;
using DAL.DBModel;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using static Base.Enums;

namespace DAL.Repository
{
    public class AuthenticationRepository : ConnectionBase
    {
        public AuthenticationRepository(string con, int timeout) : base(con, timeout)
        {
        }

        /// <summary>
        /// 依員工編號和密碼取得權限
        /// </summary>
        /// <param name="EmployeeId">員工編號</param>
        /// <param name="CostomerId">客戶編號</param>
        /// <param name="PassWord">密碼</param>
        public (Result rtn, Authentication auth) GetAuthenticationByParams(int EmployeeId, string CostomerId, string PassWord)
        {
            Result rtn = new Result();
            string sqlCmd = "SELECT * FROM Authentication ";
            DynamicParameters parameters = new DynamicParameters();
            Authentication authentication = null;

            using (var connection = new SqlConnection(this.ConnectionString))
            {
                if (EmployeeId > 0)
                {
                    sqlCmd += "WHERE EmployeesId = @EmpId";
                    parameters.Add("@EmpId", EmployeeId);
                }

                if (!string.IsNullOrEmpty(CostomerId))
                {
                    sqlCmd += sqlCmd.IndexOf("WHERE") == -1 ? "Where " : "and ";
                    sqlCmd += "CustomersId = @CusId";
                    parameters.Add("@CusId", CostomerId);
                }

                if (!string.IsNullOrEmpty(PassWord))
                {
                    sqlCmd += " and Password = @PassWord";
                    parameters.Add("@PassWord", PassWord);
                }

                sqlCmd += " and State = @State";
                parameters.Add("@State", (int)DataStatus.Enable);

                var result = this.GetFirstDefault<Authentication>(sqlCmd, parameters);
                rtn = result.rtn;
                authentication = result.result;
            }
            return (rtn, authentication);
        }
    }
}
