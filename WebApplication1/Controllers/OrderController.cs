using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BLL.InterFace;
using BLL.Model;
using BLL.PageModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace WebApplication1.Controllers
{
    public class OrderController : Controller
    {
        ProductService ProductService;
        OrderService OrderService;
        IMemberOfOrder memberService;
        IConfiguration configuration;

        public OrderController(ProductService productService, OrderService orderService,
            IMemberOfOrder memberOfOrder, IConfiguration configuration)
        {
            this.ProductService = productService;
            this.OrderService = orderService;
            this.memberService = memberOfOrder;
            this.configuration = configuration;
        }

        /// <summary>
        /// 依帳號取得訂單資訊
        /// </summary>
        public ActionResult Index()
        {
            ViewBag.Name = HttpContext.Session.GetString("Name");
            ViewBag.Id = HttpContext.Session.GetString("Id");

            var Member = memberService.GetMember(ViewBag.Id);

            if (Member != null)
            {
                int Day = int.Parse(configuration["OrderScopeDay"]);
                DateTime dtNow = DateTime.Today.AddDays(1);
                return View(OrderService.GetOrderById(ViewBag.Id, Member, dtNow.AddDays(-1 * Day), dtNow));
            }
            return View();
        }

        /// <summary>
        /// 訂單查詢
        /// </summary>
        /// <param name="dtStart"></param>
        /// <param name="dtEnd"></param>
        /// <returns></returns>
        public ActionResult OrderQuery(DateTime dtStart, DateTime dtEnd)
        {
            ViewBag.Name = HttpContext.Session.GetString("Name");
            ViewBag.Id = HttpContext.Session.GetString("Id");

            var Member = memberService.GetMember(ViewBag.Id);

            if (Member != null)
            {
                int Day = int.Parse(configuration["OrderScopeDay"]);
                DateTime dtNow = DateTime.Today.AddDays(1);
                return View(OrderService.GetOrderById(ViewBag.Id, Member, dtStart, dtEnd));
            }
            return View();
        }

        /// <summary>
        /// 設定訂單
        /// </summary>
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult SetOrder(List<ShopCarProductModel> shopcars)
        {
            ViewBag.Name = HttpContext.Session.GetString("Name");
            ViewBag.Id = HttpContext.Session.GetString("Id");

            var result = ProductService.GetShopCarAmount(ViewBag.Id, shopcars);

            if (result.Item1.IsSuccess)
            {
                var rtn = OrderService.CreateOrder(ViewBag.Id, result.Item2);

                if (rtn.IsSuccess) return Ok(this.Url.Action("Index", "Order"));

                return BadRequest(rtn.ErrorMsg);
            }
            return BadRequest(result.Item1.ErrorMsg);
        }
    }
}