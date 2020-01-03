using BLL.Model;
using DAL.DTOModel;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebApplication1.Controllers
{
    public class LogInController : Controller
    {
        AuthService AuthService;

        public LogInController(AuthService authService)
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
                HttpContext.Session.SetString("Name", result.Name);
                HttpContext.Session.SetString("Id", result.Id);

                return Ok();
            }
            return BadRequest(result.rtn.ErrorMsg);
        }

        [Authorize]
        public async Task<IActionResult> Out(string Id)
        {
            var result = AuthService.LogOut(Id);
            if (result.IsSuccess)
            {
                HttpContext.Session.Remove("Id");
                HttpContext.Session.Remove("Name");
                return RedirectToAction("Index", "LogIn");
            }

            return BadRequest(result.ErrorMsg);
        }
    }
}