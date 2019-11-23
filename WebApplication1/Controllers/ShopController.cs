using Base;
using BLL.Model;
using BLL.PageModel;
using DAL.DBModel;
using DAL.DTOModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
            var result = ProductService.GetCategoriesAndProducts(CategoryId, ProductName);

            if (!string.IsNullOrEmpty(CategoryId)) result.product.CategoryId = CategoryId;
            if (!string.IsNullOrEmpty(ProductName)) result.product.ProductName = ProductName;

            if (result.rtn.IsSuccess) return View(result.product);

            return View();
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
            
            var result = ProductService.GetShopCarProducts(ViewBag.Id, shopcars);

            if (result.Item1.IsSuccess)
            {
                ViewBag.ShopCars = result.Item2;
                return Ok(result.Item2);
            }

            return BadRequest(result.Item1.ErrorMsg);
        }

        /// <summary>
        /// 取得產品類別及產品資訊數量
        /// </summary>
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult ShopCar(ShopCarModel shopcars)
        {
            ViewBag.Name = HttpContext.Session.GetString("Name");
            ViewBag.Id = HttpContext.Session.GetString("Id");

            return View("ShopCar", shopcars);
        }
    }
}