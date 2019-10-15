using Base;
using BLL.Model;
using DAL.DTOModel;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace WebApplication1.Controllers
{
    public class ProductController : Controller
    {
        ProductService ProductService;

        public ProductController(ProductService productService)
        {
            this.ProductService = productService;
        }

        /// <summary>
        /// 取得產品資訊數量
        /// </summary>
        [HttpGet]
        public ActionResult GetProductQuantitys()
        {
            List<ProductCountDTO> productCountDTOs = ProductService.GetProductQuantitys();

            if (productCountDTOs.Count > 0) return Ok(productCountDTOs);

            var rtn = new Result() { ErrorMsg = $"查無產品數量" };
            return BadRequest(rtn.ErrorMsg);
        }
    }
}