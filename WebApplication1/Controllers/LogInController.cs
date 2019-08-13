using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BLL.InterFace;
using DAL.DTOModel;
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

        public IActionResult In(LogInDTO LogIn)
        {
            var result = AuthService.LogIn(LogIn.account, LogIn.password);
            return View();
        }
    }
}