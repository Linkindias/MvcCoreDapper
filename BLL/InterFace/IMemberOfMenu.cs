using Base;
using DAL.DTOModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.InterFace
{
    public interface IMemberOfMenu
    {
        /// <summary>
        /// 依帳號取得角色資訊
        /// </summary>
        /// <param name="EmployeeId">帳號</param>
        (Result rtn, List<RoleOfMenuDTO> roles) GetRolesByAccount(string Id);
    }
}
