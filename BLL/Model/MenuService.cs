using Base;
using BLL.Commons;
using BLL.InterFace;
using DAL.DTOModel;
using DAL.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Base.Enums;

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
        /// <param name="Id">帳號</param>
        public (Result rtn, List<MenuDTO> menus, List<RoleOfMenuDTO> roles) GetMenusByAccount(string Id)
        {
            Result rtn = new Result();
            IEnumerable<MenuDTO> menus = null;
            List<RoleOfMenuDTO> roles = null;

            string keyMenu = $"GetMenus{Id}";
            string keyRole = $"GetRoles{Id}";
            var cacheMenu = CacheHelper.GetCacheObject(keyMenu); //由快取取得選單資訊
            var cacheRole = CacheHelper.GetCacheObject(keyRole); //由快取取得角色資訊

            if (cacheMenu.Iskey) menus = (IEnumerable<MenuDTO>)cacheMenu.value;
            if (cacheRole.Iskey) roles = (List<RoleOfMenuDTO>)cacheRole.value;

            //當無選單或角色，則從後端取得，否則從快取取得
            if (menus == null && roles == null){
                var roleResult = RoleRep.GetRolesByAccount(Id);
                var menuResult = MenuRep.GetMenusByAccount(Id);

                if (roleResult.rtn.IsSuccess && menuResult.rtn.IsSuccess)
                {
                    menus = menuResult.menus.Where(o => o.ParentID == 0);

                    //當角色不為管理者，則無個人設定選單
                    if (roleResult.roles.Any(o => o.RoleName.ToLower() != "admin"))
                        menus = menuResult.menus.Where(o => o.MenuCode != "Person");

                    foreach (MenuDTO menu in menus)
                        FunGetSubMenus(menu, menuResult.menus);

                    CacheHelper.AddCacheCollection(keyMenu, menus.ToList(), CacheStatus.Absolute, 0, 1); //選單加入快取
                    CacheHelper.AddCacheCollection(keyRole, roleResult.roles, CacheStatus.Absolute, 0, 1); //角色加入快取
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
        private void FunGetSubMenus(MenuDTO MasterMenu, List<MenuDTO> SubMenus)
        {
            MasterMenu.SubMenus = SubMenus.Where(o => o.ParentID == MasterMenu.MenuId).ToList(); //依父層編號取得子選單

            //當無子選單，則跳出該層選單
            if (MasterMenu.SubMenus.Count == 0) return;

            foreach (MenuDTO menu in MasterMenu.SubMenus)
                FunGetSubMenus(menu, SubMenus);
        }
    }
}
