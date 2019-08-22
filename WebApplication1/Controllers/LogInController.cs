using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BLL.InterFace;
using DAL.DTOModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    public class LogInController : Controller
    {
        IAuthService AuthService;

        public LogInController(IAuthService authService)
        {
            this.AuthService = authService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login(LogInDTO LogIn)
        {
            var result = AuthService.LogIn(LogIn.account, LogIn.password);
            if (result.rtn.IsSuccess)
                HttpContext.Session.SetString(LogIn.account, LogIn.account);
            return View();
        }

        public IActionResult Logout(string Id)
        {
            var result = AuthService.LogOut(Id);
            if (result.IsSuccess)
                HttpContext.Session.Remove(Id);
            return View();
        }
    }
}