using System;
using System.Collections.Generic;
using System.Diagnostics;
using Base;
using BLL.InterFace;
using DAL.DTOModel;
using DAL.PageModel;
using DAL.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        IMenuService MenuService;

        public HomeController(IMenuService menuService)
        {
            this.MenuService = menuService;
        }

        public IActionResult Index()
        {
            ViewBag.Account = HttpContext.Session.GetString("Account");
            ViewBag.Id = HttpContext.Session.GetString("Id");

            if (ViewBag.Id != null)
            {
                (Result rtn, List<MenuDTO> menus, List<RoleOfMenuDTO> roles) result = result = MenuService.GetMenusByAccount(ViewBag.Id);
                if (result.rtn.IsSuccess)
                    return View(new MenuModel() { Menus = result.menus });
            }

            return View(new MenuModel() { Menus = new List<MenuDTO>() });
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
