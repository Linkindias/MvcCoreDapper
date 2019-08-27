using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BLL.InterFace;
using DAL.DTOModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    [Authorize]
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
        public async Task<IActionResult> LoginAsync(LogInDTO LogIn)
        {
            var result = AuthService.LogIn(LogIn.account, LogIn.password);
            if (result.rtn.IsSuccess)
            {
                HttpContext.Session.SetString(LogIn.account, LogIn.account);
                //創建一個身份認證
                var claims = new List<Claim>() {
                    new Claim(ClaimTypes.Sid, LogIn.account), //用戶ID
                    new Claim(ClaimTypes.Name, result.guid.ToString())  //用戶名稱
                    };

                var identity = new ClaimsIdentity(claims, "Login");
                var userPrincipal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync("LogInScheme", userPrincipal, new AuthenticationProperties
                {
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(20),
                    IsPersistent = false,
                    AllowRefresh = false
                });
            }
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