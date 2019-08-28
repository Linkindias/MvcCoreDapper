﻿using BLL.InterFace;
using DAL.DTOModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> In(LogInDTO LogIn)
        {
            var result = AuthService.LogIn(LogIn.account, LogIn.password);
            if (result.rtn.IsSuccess)
            {
                HttpContext.Session.SetString("Id", LogIn.account);
                return Ok();
            }
            return BadRequest(result.rtn.ErrorMsg);
        }

        
        public IActionResult Out(string Id)
        {
            var result = AuthService.LogOut(Id);
            if (result.IsSuccess)
                HttpContext.Session.Remove(Id);
            return View();
        }
    }
}