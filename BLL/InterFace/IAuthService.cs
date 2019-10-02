using Base;
using DAL.DBModel;
using DAL.DTOModel;
using DAL.PageModel;
using System;


namespace BLL.InterFace
{
    public interface IAuthService
    {
        /// <summary>
        /// 登入
        /// </summary>
        /// <param name="Account">帳號</param>
        /// <param name="Password">密碼</param>
        (Result rtn, Employees employee, Customers customer, Guid guid) LogIn(string Account, string Password);

        /// <summary>
        /// 登出
        /// </summary>
        /// <param name="Id">帳號</param>
        Result LogOut(string Id);

        /// <summary>
        /// 依帳號取得權限
        /// </summary>
        /// <param name="Id">編號</param>
        AuthModel GetAuth(string Id);

        /// <summary>
        /// 更新權限
        /// </summary>
        /// <param name="auth">權限</param>
        Result UpdateAuth(AuthModel auth);
    }
}
