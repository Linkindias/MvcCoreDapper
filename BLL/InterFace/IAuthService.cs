﻿using Base;
using DAL.DBModel;
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
    }
}