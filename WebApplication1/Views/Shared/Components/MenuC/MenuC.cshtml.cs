using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BLL.InterFace;
using DAL.DTOModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication1.Component
{
    public class MenuCViewComponent : ViewComponent
    {
        IMenuService MenuService;

        public MenuCViewComponent(IMenuService menuService)
        {
            this.MenuService = menuService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string Id)
        {
            if (!string.IsNullOrEmpty(Id))
            {
                var result = await GetItemsAsync(Id);

                return View("MenuC", result);
            }
            return View("MenuC");
        }

        private Task<List<MenuDTO>> GetItemsAsync(string Id)
        {
            var result = MenuService.GetMenusByAccount(Id);
           
            if (result.rtn.IsSuccess)
                return Task.FromResult(result.menus);

            return Task.FromResult(new List<MenuDTO>());
        }
    }
}
