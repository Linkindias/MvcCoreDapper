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

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult ShopCar(List<ShopCarModel> Shopcars)
        {
            ViewBag.Name = HttpContext.Session.GetString("Name");
            ViewBag.Id = HttpContext.Session.GetString("Id");
            ViewBag.Role = HttpContext.Session.GetString("Role");
            return View();
        }
    }
}