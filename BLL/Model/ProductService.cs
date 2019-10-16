using Base;
using BLL.PageModel;
using DAL.DBModel;
using DAL.DTOModel;
using DAL.Repository;
using Microsoft.Extensions.Configuration;
using Omu.ValueInjecter;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BLL.Model
{
    public class ProductService
    {
        IConfiguration Configuration;
        ProductRepository ProductRep;
        CategorieRepository CategorieRep;
        OrderDetailRepository OrderDetailRep;
        ProductModel Product;

        public ProductService(IConfiguration configuration, 
            ProductRepository productRepository, CategorieRepository categorieRepository, OrderDetailRepository orderDetailRepository,
            ProductModel productModel)
        {
            this.ProductRep = productRepository;
            this.CategorieRep = categorieRepository;
            this.OrderDetailRep = orderDetailRepository;
            this.Configuration = configuration;
            this.Product = productModel;
        }

        /// <summary>
        /// 取得產品類別、 產品資訊
        /// </summary>
        public (Result rtn, ProductModel product)  GetCategoriesAndProducts(string CategoryId = "", string ProductName = "")
        {
            var resultCategroy = CategorieRep.GetCategorys();
            var resultProduct = ProductRep.GetProductsByParam(string.IsNullOrEmpty(CategoryId) ? 0 : int.Parse(CategoryId), ProductName, false);

            if (resultCategroy.rtn.IsSuccess && resultProduct.rtn.IsSuccess)
            {
                var resultOrder = OrderDetailRep.GetOrderProductQuantitys(resultProduct.products.Select(o => o.ProductID).ToArray());

                if (!resultOrder.rtn.IsSuccess)
                {
                    resultOrder.rtn.ErrorMsg = "查無產品數量";
                    return (resultOrder.rtn, Product);
                }

                List<ProductCountDTO> productCounts = new List<ProductCountDTO>();
                List<ProductCountDTO> orderProductCounts = resultOrder.productCounts;
                resultProduct.products.ToList().ForEach(o => 
                    productCounts.Add((ProductCountDTO)new ProductCountDTO().InjectFrom(o)));

                productCounts.ForEach(o =>
                {
                    o.Sales = orderProductCounts.Where(p => p.ProductID == o.ProductID).FirstOrDefault().Sales; //銷售
                    o.Quantity = o.UnitsInStock.Value - o.Sales; //庫存 - 銷售 = 剩餘
                });

                //過濾最小產品範圍
                int SaleCount = int.Parse(Configuration["ProductSaleCount"]);
                productCounts = productCounts.Where(o => o.Quantity > SaleCount).OrderBy(o => o.ProductID).ToList();

                Product.Categories = resultCategroy.Categorys.Select(o => new SelectListItem() { 
                                            Value = o.CategoryID.ToString(),
                                            Text = o.CategoryName,
                                        });
                Product.Products = productCounts;
                return (resultCategroy.rtn, Product);
            }

            if (!resultCategroy.rtn.IsSuccess) {
                resultCategroy.rtn.ErrorMsg = "查無產品類別";
                return (resultCategroy.rtn, Product);
            }

            if (!resultProduct.rtn.IsSuccess)
            {
                resultProduct.rtn.ErrorMsg = "查無產品資訊";
                return (resultProduct.rtn, Product);
            }
            return (new Result(), Product);
        }
    }
}
