using Base;
using BLL.Commons;
using BLL.InterFace;
using DAL.DBModel;
using DAL.DTOModel;
using DAL.PageModel;
using DAL.Repository;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
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
            //當員工錯誤，則回傳錯誤訊息
            else if (!myEmployee.rtn.IsSuccess && myEmployee.employee != null)
                rtn = myEmployee.rtn;
            //當會員錯誤，則回傳錯誤訊息
            else if (!myCustomer.rtn.IsSuccess && myCustomer.custom != null)
                rtn = myCustomer.rtn;
            else
            {
                int EmployeeID = myEmployee.employee != null ? myEmployee.employee.EmployeeID : -1; //員工
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
                        AuthRep.UpdateAuthCode(EmployeeID, CustomerID, guid, (int)DataStatus.Enable);
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
            (Result rtn, int exeRows) updateAuthCode = (new Result(), 0);
            int EmployeeId = 0;
            int.TryParse(Id, out EmployeeId);
            var myEmployee = EmployeeRep.GetEmployeeById(EmployeeId, (int)DataStatus.Enable); //員工
            var myCustomer = CustomerRep.GetCustomerById(Id, (int)DataStatus.Enable); //客戶

            //當人員不正確，則顯示訊息，否則檢查權限
            if (myEmployee.employee == null && myCustomer.customer == null)
            {
                updateAuthCode.rtn.IsSuccess = false;
                updateAuthCode.rtn.ErrorMsg = $"查無此帳號 {Id}，請確認";
            }
            else
                updateAuthCode = AuthRep.UpdateAuthCode(EmployeeId, Id, null, (int)DataStatus.Enable);

            if (updateAuthCode.rtn.IsSuccess) updateAuthCode.rtn.SuccessMsg = $"{Id} 登出成功";

            return updateAuthCode.rtn;
        }

        /// <summary>
        /// 依帳號取得權限
        /// </summary>
        /// <param name="Id">編號</param>
        public AuthModel GetAuth(string Id)
        {
            AuthModel auth = new AuthModel();
            string keyAuth = $"GetAuth{Id}";
            var cacheAuth = CacheHelper.GetCacheObject(keyAuth); //由快取取得選單資訊

            if (cacheAuth.Iskey)
            {
                auth = (AuthModel)cacheAuth.value;
                auth.IsSuccess = true;
            }
            else
            {
                var result = AuthRep.GetAuthById(Id, (int)DataStatus.Enable);
                if (result.rtn.IsSuccess)
                {
                    auth = new AuthModel()
                    {
                        AuthenticId = result.auth.AuthenticId,
                        Account = result.auth.Account,
                        Password = string.Empty,
                    };
                    auth.IsSuccess = result.rtn.IsSuccess;
                    CacheHelper.AddCache(keyAuth, auth, CacheStatus.Absolute, 0, 1); //權限加入快取
                }
                else
                {
                    auth.IsSuccess = result.rtn.IsSuccess;
                    auth.ErrorMsg = result.rtn.ErrorMsg;
                }
            }
            return auth;
        }

        /// <summary>
        /// 更新權限
        /// </summary>
        /// <param name="auth">權限</param>
        public Result UpdateAuth(AuthModel auth)
        {
            (Result rtn, int exeRows) updateAuth = (new Result(), 0);

            updateAuth = AuthRep.UpdateAuth(auth.AuthenticId,auth.Account,auth.Password, (int)DataStatus.Enable);

            if (updateAuth.rtn.IsSuccess) updateAuth.rtn.SuccessMsg = $"{auth.Account} 更新成功";

            return updateAuth.rtn;
        }
    }
}
