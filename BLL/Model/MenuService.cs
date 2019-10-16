using Base;
using DAL.DTOModel;
using DAL.Repository;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL.Model
{
    public class MenuService
    {
        MenuRepository MenuRep;
        RoleRepository RoleRep;
        IMemoryCache cache;

        public MenuService(MenuRepository menuRepository, RoleRepository roleRepository, IMemoryCache memoryCache)
        {
            this.MenuRep = menuRepository;
            this.RoleRep = roleRepository;
            this.cache = memoryCache;
        }

        /// <summary>
        /// 取得選單清單
        /// </summary>
        /// <param name="Id">帳號</param>
        public (Result rtn, List<MenuDTO> menus, List<RoleOfMenuDTO> roles) GetMenusByAccount(string Id)
        {
            Result rtn = new Result();
            IEnumerable<MenuDTO> menus = null;
            List<RoleOfMenuDTO> roles = null;

            string keyMenu = $"GetMenus{Id}";
            string keyRole = $"GetRoles{Id}";

            cache.TryGetValue<IEnumerable<MenuDTO>>(keyMenu, out menus);
            cache.TryGetValue<List<RoleOfMenuDTO>>(keyRole, out roles);

            //當無選單或角色，則從後端取得，否則從快取取得
            if (menus == null && roles == null){
                var roleResult = RoleRep.GetRolesByAccount(Id);
                var menuResult = MenuRep.GetMenusByAccount(Id);

                if (roleResult.rtn.IsSuccess && menuResult.rtn.IsSuccess)
                {
                    menus = menuResult.menus.Where(o => o.ParentID == 0);

                    //當角色不為管理者，則無權限選單
                    if (roleResult.roles.Any(o => o.RoleName.ToLower() != "admin")) menus = menus.Where(o => o.MenuCode.ToLower() != "auth");

                    foreach (MenuDTO menu in menus)
                        this.FunGetSubMenus(menu, menuResult.menus);

                    TimeSpan ts = DateTime.Today.AddDays(1) - DateTime.Now; //1天
                    cache.Set<IEnumerable<MenuDTO>>(keyMenu, menus.ToList(),ts); //選單加入快取
                    cache.Set<List<RoleOfMenuDTO>>(keyRole, roleResult.roles,ts); //角色加入快取
                    return (new Result() { IsSuccess = true }, menus.ToList(), roleResult.roles);
                }
                return (!roleResult.rtn.IsSuccess ? roleResult.rtn : menuResult.rtn, new List<MenuDTO>(), new List<RoleOfMenuDTO>());
            }
            else
            {
                rtn.IsSuccess = true;
                return (rtn, menus.ToList(), roles);
            }
        }

        /// <summary>
        /// 取得子選單 遞回
        /// </summary>
        /// <param name="MasterMenu">父選單 </param>
        /// <param name="SubMenus">子選單清單 </param>
        protected virtual void FunGetSubMenus(MenuDTO MasterMenu, List<MenuDTO> SubMenus)
        {
            MasterMenu.SubMenus = SubMenus.Where(o => o.ParentID == MasterMenu.MenuId).ToList(); //依父層編號取得子選單

            //當無子選單，則跳出該層選單
            if (MasterMenu.SubMenus.Count == 0) return;

            foreach (MenuDTO menu in MasterMenu.SubMenus)
                FunGetSubMenus(menu, SubMenus);
        }
    }
}
