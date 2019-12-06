using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BLL.Model;
using BLL.PageModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    public class OrderController : Controller
    {
        ProductService ProductService;
        OrderService OrderService;

        public OrderController(ProductService productService, OrderService orderService)
        {
            this.ProductService = productService;
            this.OrderService = orderService;
        }

        /// <summary>
        /// 依帳號取得訂單資訊
        /// </summary>
        public ActionResult Index()
        {
            ViewBag.Name = HttpContext.Session.GetString("Name");
            ViewBag.Id = HttpContext.Session.GetString("Id");

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