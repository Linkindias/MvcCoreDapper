using Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.InterFace
{
    public interface IMemberOfAuth
    {
        /// <summary>
        /// 取得是否有會員
        /// </summary>
        /// <param name="account">帳號</param>
        (Result rtn, int EmpId, string CusId, string Id, string Name) IsExistMemberByAccount(string account);

        /// <summary>
        /// 取得是否有會員
        /// </summary>
        /// <param name="Id">會員編號</param>
        (Result rtn, int EmpId, string CusId) IsExistMemberById(string Id);
    }
}
