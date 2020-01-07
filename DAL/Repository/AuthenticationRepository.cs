using Base;
using DAL.DBModel;
using DAL.DTOModel;
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
        public virtual (Result rtn, Authentication auth) GetAuthenticationByParams(int EmployeeId, string CostomerId, string PassWord, int Status)
        {
            string sqlCmd = "SELECT * FROM Authentication ";
            DynamicParameters parameters = new DynamicParameters();

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
                sqlCmd += " and PasswordSha512 = @PassWord";
                parameters.Add("@PassWord", PassWord);
            }

            sqlCmd += " and State = @State";
            parameters.Add("@State", Status);

            var result = this.GetSingleDefault<Authentication>(sqlCmd, parameters);

            return (result.rtn, result.result);
        }

        /// <summary>
        /// 更新權限的認證碼
        /// </summary>
        /// <param name="Id">員工編號</param>
        /// <param name="VerifyCode">認證碼</param>
        public virtual (Result rtn, int exeRows) UpdateAuthCode(int EmpId, string CusId, Guid? VerifyCode, int Statue)
        {
            string sqlCmd = "Update Authentication Set VerifyCode = @Code Where (EmployeesId = @EmpId or CustomersId = @CusId) and State = @State";

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@EmpId", EmpId);
            parameters.Add("@CusId", CusId);
            parameters.Add("@State", Statue);
            parameters.Add("@Code", VerifyCode.HasValue ? VerifyCode.Value : VerifyCode);

            var result = this.GetCUDOfRow(sqlCmd, parameters);

            return (result.rtn, result.Rows);
        }

        /// <summary>
        /// 依帳號取得權限
        /// </summary>
        /// <param name="Id">編號</param>
        public virtual (Result rtn, AuthenticationDTO auth) GetAuthById(string Id, int Statue)
        {
            int EmployeeId = 0;
            int.TryParse(Id, out EmployeeId);
            //實值型別初始化為0
            if (EmployeeId == 0) EmployeeId = -1;

            string SqlCom = @"
select AuthenticId, isnull(Employees.Account,Customers.Account) as 'Account', PasswordSha512 from Authentication
left join Employees on Authentication.EmployeesId = Employees.EmployeeId
left join Customers on Authentication.CustomersId = Customers.CustomerID
where (Authentication.EmployeesId = @EmployeeId or Authentication.CustomersId = @Id) and Authentication.State = @State";

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@EmployeeId", EmployeeId);
            parameters.Add("@Id", Id);
            parameters.Add("@State", Statue);

            var result = this.GetSingleDefault<AuthenticationDTO>(SqlCom, parameters);

            return (result.rtn, result.result);
        }

        /// <summary>
        /// 更新帳密的權限
        /// </summary>
        /// <param name="AuthId">權限編號</param>
        /// <param name="Account">帳號</param>
        /// <param name="Password">密碼</param>
        public virtual (Result rtn, int exeRows) UpdateAuth(int AuthId, string Account, string Password, int Statue)
        {
            string sqlCmd = @"
Update Authentication Set PasswordSha512 = @Password Where AuthenticId = @AuthId and State = @State

if ((select EmployeesId from Authentication where AuthenticId = @AuthId)  <> 0)
Begin
	update Employees
	set Account = @Account
	where EmployeeID = (select EmployeesId from Authentication where AuthenticId = @AuthId)
End

if ((select CustomersId from Authentication where AuthenticId = @AuthId) Is Not Null)
Begin
	update Customers
	set Account = @Account
	where CustomerID = (select CustomersId from Authentication where AuthenticId = @AuthId)
End";

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@AuthId", AuthId);
            parameters.Add("@State", Statue);
            parameters.Add("@Account", Account);
            parameters.Add("@Password", Password);

            var result = this.GetCUDOfRow(sqlCmd, parameters);

            return (result.rtn, result.Rows);
        }

        /// <summary>
        /// 是否驗證
        /// </summary>
        /// <param name="EmployeeId">員工編號</param>
        /// <param name="Guid">驗證碼</param>
        public (Result rtn, bool isVerify)  IsVerify(string Id, string Guid, int Statue)
        {
            if (!string.IsNullOrEmpty(Id) && !string.IsNullOrEmpty(Guid))
            {
                int EmployeeId = 0;
                int.TryParse(Id, out EmployeeId);
                //實值型別初始化為0
                if (EmployeeId == 0) EmployeeId = -1;

                string SqlCom = @"
select * from Authentication
where (EmployeesId = @EmployeeId or CustomersId = @Id) and State = @State and VerifyCode = @VerifyCode";

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@EmployeeId", EmployeeId);
                parameters.Add("@Id", Id);
                parameters.Add("@State", Statue);
                parameters.Add("@VerifyCode", Guid);

                var result = this.GetSingleDefault<Authentication>(SqlCom, parameters);

                return (result.rtn, result.result != null);
            }
            return (new Result() { IsSuccess = false, ErrorMsg = "Id or Guid is null"}, false);
        }
    }
}
