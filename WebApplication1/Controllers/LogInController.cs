using BLL.Model;
using DAL.DTOModel;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

                //var claims = new List<Claim>() {
                //    new Claim(ClaimTypes.NameIdentifier, result.Id),
                //    new Claim(ClaimTypes.Name, result.Id),
                //};
                //ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "BasicAuthentication");
                //var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                //HttpContext.User = claimsPrincipal;
                //await HttpContext.Authentication.SignInAsync("BasicAuthentication", claimsPrincipal);
                return Ok();
            }
            return BadRequest(result.rtn.ErrorMsg);
        }

        [Authorize]
        public IActionResult Out(string Id)
        {
            var result = AuthService.LogOut(Id);
            if (result.IsSuccess)
            {
                HttpContext.Authentication.SignOutAsync(DefaultAuthenticationTypes.ApplicationCookie);
                HttpContext.Session.Remove("Id");
                HttpContext.Session.Remove("Name");
                return RedirectToAction("Index", "LogIn");
            }

            return BadRequest(result.ErrorMsg);
        }
    }
}