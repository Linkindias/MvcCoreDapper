using BLL.Model;
using DAL.DTOModel;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
    [IgnoreAntiforgeryToken]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
                string JwtToken = AuthService.CreateToken(result.Name, result.Id, result.guid);

                if (string.IsNullOrEmpty(JwtToken)) return BadRequest("Not Generate Token!");
                    
                return Ok(JwtToken);
            }
            return BadRequest(result.rtn.ErrorMsg);
        }

        [HttpPost]
        [Route("Out")]
        public IActionResult Out([FromQuery] string Id)
        {
            var result = AuthService.LogOut(Id);
            if (result.IsSuccess) return Ok(result.SuccessMsg);

            return BadRequest(result.ErrorMsg);
        }
    }
}
