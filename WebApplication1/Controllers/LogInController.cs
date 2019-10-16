using BLL.Model;
using DAL.DTOModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace WebApplication1.Controllers
{
    public class LogInController : Controller
    {
        AuthService AuthService;
        MenuService MenuService;

        public LogInController(AuthService authService, MenuService menuService)
        {
            this.AuthService = authService;
            this.MenuService = menuService;
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
                HttpContext.Session.SetString("Name", result.employee != null
                                                    ? result.employee.FirstName + result.employee.LastName
                                                    : result.customer.CompanyName);
                HttpContext.Session.SetString("Id", result.employee != null
                                                        ? result.employee.EmployeeID.ToString()
                                                        : result.customer.CustomerID);
                return Ok();
            }
            return BadRequest(result.rtn.ErrorMsg);
        }

        public IActionResult Out(string Id)
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