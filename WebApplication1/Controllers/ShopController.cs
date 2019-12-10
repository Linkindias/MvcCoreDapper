using Base;
using BLL.Model;
using BLL.PageModel;
using DAL.DBModel;
using DAL.DTOModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Omu.ValueInjecter;
using System.Collections.Generic;

namespace WebApplication1.Controllers
{
    public class ShopController : Controller
    {
        ProductService ProductService;

        public ShopController(ProductService productService)
        {
            this.ProductService = productService;
        }

        /// <summary>
        /// 取得產品類別及產品資訊數量
        /// </summary>
        [HttpGet]
        public ActionResult Product(string CategoryId = "", string ProductName = "")
        {
            ViewBag.Name = HttpContext.Session.GetString("Name");
            ViewBag.Id = HttpContext.Session.GetString("Id");
            var result = ProductService.GetCategoriesAndProducts(ViewBag.Id, CategoryId, ProductName);

            if (!string.IsNullOrEmpty(CategoryId)) result.Item2.CategoryId = CategoryId;
            if (!string.IsNullOrEmpty(ProductName)) result.Item2.ProductName = ProductName;

            return View(result.Item2);
        }

        /// <summary>
        /// 取得購物車
        /// </summary>
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult GetProducts(List<ShopCarProductModel> shopcars)
        {
            ViewBag.Name = HttpContext.Session.GetString("Name");
            ViewBag.Id = HttpContext.Session.GetString("Id");
            
            var result = ProductService.GetShopCarAmount(ViewBag.Id, shopcars);

            if (result.Item1.IsSuccess) return Ok(result.Item2);

            return BadRequest(result.Item1.ErrorMsg);
        }

        /// <summary>
        /// 取得產品類別及產品資訊數量
        /// </summary>
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult ShopCar(ShopCarModel shopcar)
        {
            ViewBag.Name = HttpContext.Session.GetString("Name");
            ViewBag.Id = HttpContext.Session.GetString("Id");

            shopcar.products = ProductService.GetProductsById(ViewBag.Id);

            return View("ShopCar", shopcar);
        }

        /// <summary>
        /// 取得產品類別及產品資訊數量
        /// </summary>
        public ActionResult ShopCar(string Id)
        {
            ViewBag.Name = HttpContext.Session.GetString("Name");
            ViewBag.Id = HttpContext.Session.GetString("Id");

            ShopCarModel shopcar = ProductService.GetShopCarInfoById(ViewBag.Id);

            return View("ShopCar", shopcar);
        }

        /// <summary>
        /// 依帳號取得訂單
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult Order(string Id)
        {
            return RedirectToAction("Index", "Order");
        }
    }
}