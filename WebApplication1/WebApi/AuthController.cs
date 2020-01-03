using BLL.Model;
using DAL.DTOModel;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApplication1.WebApi
{
    [Route("api/[controller]/")]
    [ApiController]
    public class AuthController : Controller
    {
        AuthService AuthService;

        public AuthController(AuthService authService)
        {
            this.AuthService = authService;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("In")]
        public IActionResult In([FromBody] LogInDTO LogIn)
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
        [HttpPost]
        [Route("Out")]
        public IActionResult Out([FromQuery] string Id)
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

        [HttpGet]
        [Route("Test")]
        //[ValidateAntiForgeryToken]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }
    }
}
