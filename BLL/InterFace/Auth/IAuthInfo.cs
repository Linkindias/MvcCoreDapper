using Base;
using BLL.PageModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.InterFace.Auth
{
    interface IAuthInfo
    {
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
