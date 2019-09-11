using Base;
using BLL.InterFace;
using DAL.DTOModel;
using DAL.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Model
{
    public class MenuService : IMenuService
    {
        MenuRepository RoleRep;

        public MenuService(MenuRepository roleRepository)
        {
            this.RoleRep = roleRepository;
        }

        /// <summary>
        /// 取得選單清單
        /// </summary>
        /// <param name="EmployeeId">帳號</param>
        public (Result rtn, MenuDTO menus) GetMenusByAccount(string Id)
        {
            throw new NotImplementedException();
        }
    }
}
