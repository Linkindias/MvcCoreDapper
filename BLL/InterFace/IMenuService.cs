using Base;
using DAL.DBModel;
using DAL.DTOModel;
using System;
using System.Collections.Generic;

namespace BLL.InterFace
{
    public interface IMenuService
    {
        /// <summary>
        /// 取得選單清單
        /// </summary>
        /// <param name="EmployeeId">帳號</param>
        (Result rtn, List<MenuDTO> menus, List<RoleOfMenuDTO> roles) GetMenusByAccount(string Id);
    }
}
