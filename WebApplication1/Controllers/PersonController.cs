using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BLL.InterFace;
using DAL.DBModel;
using DAL.DTOModel;
using DAL.PageModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    public class PersonController : Controller
    {
        IMemberService MemberService;

        public PersonController(IMemberService memberService)
        {
            this.MemberService = memberService;
        }

        //會員資訊
        public ActionResult Profile(string Id)
        {
            if (!string.IsNullOrEmpty(Id))
            {
                ViewBag.Account = HttpContext.Session.GetString("Account");
                ViewBag.Id = HttpContext.Session.GetString("Id");

                MemberModel memberModel = MemberService.GetMember(Id);

                if (memberModel != null) return View((memberModel));
            }
            return View();
        }

        public ActionResult CreateCustomer(Customers customer)
        {
            return View();
        }
    }
}