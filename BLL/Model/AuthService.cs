using Base;
using BLL.Commons;
using BLL.InterFace;
using DAL.DBModel;
using DAL.Repository;
using Microsoft.AspNetCore.Http;
using System;
using static Base.Enums;

namespace BLL.Model
{
    public class AuthService : IAuthService
    {
        AuthenticationRepository AuthRep;
        EmployeeRepository EmployeeRep;
        CustomerRepository CustomerRep;

        public AuthService(AuthenticationRepository authenticationRepository,
                EmployeeRepository employeeRepository, CustomerRepository customerRepository)
        {
            this.AuthRep = authenticationRepository;
            this.EmployeeRep = employeeRepository;
            this.CustomerRep = customerRepository;
        }

        /// <summary>
        /// 登入
        /// </summary>
        /// <param name="Account">帳號</param>
        /// <param name="Password">密碼</param>
        public (Result rtn, Employees employee, Customers customer, Guid guid) LogIn(string Account, string Password)
        {
            Result rtn = new Result();
            var myEmployee = EmployeeRep.GetEmployeeByAccount(Account, (int)DataStatus.Enable); //員工
            var myCustomer = CustomerRep.GetCustomerByAccount(Account, (int)DataStatus.Enable); //客戶
            Guid guid = Guid.Empty;

            //當人員不正確，則顯示訊息，否則檢查權限
            if (myEmployee.employee == null && myCustomer.custom == null)
            {
                rtn.IsSuccess = false;
                rtn.ErrorMsg = $"查無此帳號 {Account}，請確認";
            }
            else
            {
                int EmployeeID = myEmployee.employee != null ? myEmployee.employee.EmployeeID : 0; //員工
                string CustomerID = myCustomer.custom != null ? myCustomer.custom.CustomerID : string.Empty; //客戶
                var myAuth = AuthRep.GetAuthenticationByParams(EmployeeID, CustomerID, Password, (int)DataStatus.Enable);

                //當權限不正確，則顯示訊息，否則檢查有效期
                if (myAuth.auth == null)
                {
                    rtn.IsSuccess = false;
                    rtn.ErrorMsg = $"帳號 {Account}或密碼不正確，請確認";
                }
                else
                {
                    rtn.IsSuccess = false;

                    //當權限鎖定，則無法登入
                    if (myAuth.auth.State == (int)DataStatus.Lock)
                        rtn.ErrorMsg = $"帳號 {Account}已鎖定，無法登入，請連絡客服!";

                    //當有效期小於今日，則已離職
                    else if (DateTime.Compare(DateTime.Now, myAuth.auth.EffectiveDate) >= 0)
                        rtn.ErrorMsg = $"帳號 {Account}已離職，無法登入";

                    //當已有認證，則已登入中
                    else if (myAuth.auth.VerifyCode != null)
                        rtn.ErrorMsg = $"帳號 {Account}已登入中，無法登入";

                    else
                    {
                        rtn.IsSuccess = true;
                        guid = Guid.NewGuid();
                        string Id = EmployeeID != 0 ? EmployeeID.ToString() : CustomerID;
                        AuthRep.UpdateAuthCode(Id, guid, (int)DataStatus.Enable);
                    }
                }
            }
            return (rtn, myEmployee.employee, myCustomer.custom, guid);
        }

        /// <summary>
        /// 登出
        /// </summary>
        /// <param name="Id">帳號</param>
        public Result LogOut(string Id)
        {
            Result rtn = new Result();
            (Result rtn, int exeRows) updateAuthCode = (rtn, 0);
            int EmployeeId = 0;
            int.TryParse(Id, out EmployeeId);
            var myEmployee = EmployeeRep.GetEmployeeById(EmployeeId, (int)DataStatus.Enable); //員工
            var myCustomer = CustomerRep.GetCustomerById(EmployeeId, (int)DataStatus.Enable); //客戶

            //當人員不正確，則顯示訊息，否則檢查權限
            if (myEmployee.employee == null && myCustomer.customer == null)
            {
                rtn.IsSuccess = false;
                rtn.ErrorMsg = $"查無此帳號 {Id}，請確認";
            }
            else
                updateAuthCode = AuthRep.UpdateAuthCode(Id, null, (int)DataStatus.Enable);

            if (updateAuthCode.rtn.IsSuccess) rtn.SuccessMsg = $"{Id} 登出成功";
            return rtn;
        }
    }
}
