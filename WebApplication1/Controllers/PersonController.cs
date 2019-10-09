using Base;
using BLL.Model;
using BLL.PageModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    public class PersonController : Controller
    {
        MemberService MemberService;
        AuthService AuthService;

        public PersonController(MemberService memberService, AuthService authService)
        {
            this.MemberService = memberService;
            this.AuthService = authService;
        }

        //會員資訊
        public ActionResult Profile(string Id)
        {
            if (!string.IsNullOrEmpty(Id))
            {
                ViewBag.Name = HttpContext.Session.GetString("Name");
                ViewBag.Id = HttpContext.Session.GetString("Id");

                MemberModel memberModel = MemberService.GetMember(Id);

                if (memberModel != null) return View(memberModel);
            }
            return View();
        }

        //會員帳密
        public ActionResult AP(string Id)
        {
            if (!string.IsNullOrEmpty(Id))
            {
                ViewBag.Name = HttpContext.Session.GetString("Name");
                ViewBag.Id = HttpContext.Session.GetString("Id");

                AuthModel auth = AuthService.GetAuth(Id);

                if (auth != null) return View(auth);
            }
            return View();
        }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult UpdateMember(MemberModel member)
        {
            Result result = MemberService.UpdateMember(member);

            if (result.IsSuccess) return Ok(result.SuccessMsg);

            return BadRequest(result.ErrorMsg);
        }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult UpdateAuth(AuthModel auth)
        {
            Result result = AuthService.UpdateAuth(auth);

            if (result.IsSuccess) return Ok(result.SuccessMsg);

            return BadRequest(result.ErrorMsg);
        }
    }
}