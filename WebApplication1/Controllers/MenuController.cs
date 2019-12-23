using BLL.Model;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication1.Controllers
{
    public class MenuController : Controller
    {
        MenuService MenuService;

        public MenuController(MenuService menuService)
        {
            this.MenuService = menuService;
        }

        [Authorize]
        public IActionResult Index(string Id)
        {
            if (!string.IsNullOrEmpty(Id))
            {
                var result = MenuService.GetMenusByAccount(Id);

                if (result.rtn.IsSuccess)
                {
                    string htmlStatement = string.Empty;
                    foreach (var menu in result.menus)
                    {
                        htmlStatement += "<li class='dropdown'>";
                        htmlStatement += $"<a class='font dropdown-toggle' data-toggle='dropdown' asp-area=''>{menu.MenuName}</a>";
                        htmlStatement += GetSubMenus(menu.SubMenus, menu.MenuCode,Id);
                        htmlStatement += "</li>";
                    }
                    return Ok(htmlStatement);
                }
                return BadRequest(result.rtn.ErrorMsg);
            }

            return BadRequest("Not LogIn Account!");
        }

        [Authorize]
        private string GetSubMenus(List<DAL.DTOModel.MenuDTO> subMenus, string controller, string Id)
        {
            if (subMenus.Count > 0)
            {
                string htmlStatement = "<ul class='dropdown-menu'>";
                foreach (var menuSub in subMenus)
                {
                    htmlStatement += $"<li><a class='dropdown-item' href='{@Url.Action(menuSub.MenuCode, controller, new { Id = Id })}'>{menuSub.MenuName}</a></li>";
                    htmlStatement += GetSubMenus(menuSub.SubMenus, menuSub.MenuCode, Id);
                }
                htmlStatement += "</ul>";
                return htmlStatement;
            }
            return string.Empty;
        }
    }
}