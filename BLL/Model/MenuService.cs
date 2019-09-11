using Base;
using BLL.InterFace;
using DAL.DTOModel;
using DAL.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.Model
{
    public class MenuService : IMenuService
    {
        MenuRepository MenuRep;
        RoleRepository RoleRep;

        public MenuService(MenuRepository menuRepository, RoleRepository roleRepository)
        {
            this.MenuRep = menuRepository;
            this.RoleRep = roleRepository;
        }

        /// <summary>
        /// 取得選單清單
        /// </summary>
        /// <param name="EmployeeId">帳號</param>
        public (Result rtn, List<MenuDTO> menus, List<RoleOfMenuDTO> roles) GetMenusByAccount(string Id)
        {
            var roleResult = RoleRep.GetRolesByAccount(Id);
            var menuResult = MenuRep.GetMenusByAccount(Id);

            if (roleResult.rtn.IsSuccess && menuResult.rtn.IsSuccess)
            {
                IEnumerable<MenuDTO> menuMaster = menuResult.menus;

                if (roleResult.roles.Any(o => o.RoleName.ToLower() != "admin"))
                    menuMaster = menuResult.menus.Where(o => o.MenuCode != "Person");


            }

            return (!roleResult.rtn.IsSuccess ? roleResult.rtn : menuResult.rtn, new List<MenuDTO>(), new List<RoleOfMenuDTO>());
        }
    }
}
