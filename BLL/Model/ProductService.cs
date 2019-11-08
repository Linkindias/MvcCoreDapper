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
using Microsoft.Extensions.Caching.Memory;
using System;
using BLL.InterFace;

namespace BLL.Model
{
    public class ProductService
    {
        IConfiguration configuration;
        IMemoryCache cache;
        IMemberOfProduct MemberService;
        ProductRepository ProductRep;
        CategorieRepository CategorieRep;
        OrderDetailRepository OrderDetailRep;
        ProductModel Product;
        ShopCarModel ShopCar;

        public ProductService(IConfiguration configuration, IMemoryCache memoryCache, IMemberOfProduct memberOfProduct,
            ProductRepository productRepository, CategorieRepository categorieRepository, OrderDetailRepository orderDetailRepository,
            ProductModel productModel, ShopCarModel shopCarModel)
        {
            this.configuration = configuration;
            this.cache = memoryCache;
            this.MemberService = memberOfProduct;
            this.ProductRep = productRepository;
            this.CategorieRep = categorieRepository;
            this.OrderDetailRep = orderDetailRepository;
            this.Product = productModel;
            this.ShopCar = shopCarModel;
        }

        /// <summary>
        /// 取得產品類別、 產品資訊
        /// </summary>
        public (Result rtn, ProductModel product) GetCategoriesAndProducts(string CategoryId = "", string ProductName = "")
        {
            IEnumerable<Categories> categories = null;
            IEnumerable<Products> products = null;

            string keyCategory = $"GetCategory{CategoryId}";
            string keyProduct = $"GetProduct{CategoryId}{ProductName}";

            cache.TryGetValue<IEnumerable<Categories>>(keyCategory, out categories);
            cache.TryGetValue<IEnumerable<Products>>(keyProduct, out products);

            if (categories == null && products == null)
            {
                var resultCategroy = CategorieRep.GetCategorys();
                var resultProduct = ProductRep.GetProductsByParam(string.IsNullOrEmpty(CategoryId) ? 0 : int.Parse(CategoryId), ProductName,new int[0], false);

                if (resultCategroy.rtn.IsSuccess && resultProduct.rtn.IsSuccess)
                {
                    categories = resultCategroy.Categorys;
                    products = resultProduct.products;

                    TimeSpan ts = DateTime.Today.AddDays(1) - DateTime.Now; //1天
                    cache.Set<IEnumerable<Categories>>(keyCategory, categories, ts); //產品類別加入快取
                    cache.Set<IEnumerable<Products>>(keyProduct, products, ts); //產品資訊加入快取
                }

                if (!resultCategroy.rtn.IsSuccess)
                {
                    resultCategroy.rtn.ErrorMsg = "查無產品類別";
                    return (resultCategroy.rtn, Product);
                }

                if (!resultProduct.rtn.IsSuccess)
                {
                    resultProduct.rtn.ErrorMsg = "查無產品資訊";
                    return (resultProduct.rtn, Product);
                }
            }

            var resultOrder = OrderDetailRep.GetOrderProductQuantitys(products.Select(o => o.ProductID).ToArray());

            if (!resultOrder.rtn.IsSuccess)
            {
                resultOrder.rtn.ErrorMsg = "查無產品數量";
                return (resultOrder.rtn, Product);
            }

            List<ProductCountDTO> productCounts = new List<ProductCountDTO>();
            List<ProductCountDTO> orderProductCounts = resultOrder.productCounts;
            products.ToList().ForEach(o =>
                productCounts.Add((ProductCountDTO)new ProductCountDTO().InjectFrom(o))
            );

            //過濾最小產品範圍
            int SaleCount = int.Parse(configuration["ProductSaleCount"]);

            productCounts.ForEach(o =>
            {
                o.Sales = orderProductCounts.Where(p => p.ProductID == o.ProductID).FirstOrDefault().Sales; //銷售
                o.Quantity = o.UnitsInStock.Value - o.Sales; //庫存 - 銷售 = 剩餘
                o.QuantityOptions = this.GetOptions(SaleCount, o.Quantity);
            });

            productCounts = productCounts.Where(o => o.Quantity > SaleCount).OrderBy(o => o.ProductID).ToList();

            //產品類別
            Product.Categories = categories.Select(o => new SelectListItem()
            {
                Value = o.CategoryID.ToString(),
                Text = o.CategoryName,
            });
            Product.Products = productCounts;

            return (new Result() { IsSuccess = true }, Product);
        }

        /// <summary>
        /// 取得購物車產品資訊
        /// </summary>
        public (Result rtn, ShopCarModel shopCar) GetShopCarProducts(string Id, List<ShopCarProductModel> shopcars)
        {
            var result = ProductRep.GetProductsByParam(0, "", shopcars.Select(o => o.Id).Distinct().ToArray(), false);
            if (result.rtn.IsSuccess)
            {
                int TotalAmount = 0;
                shopcars.ForEach(o =>
                {
                    o.InjectFrom(result.products.Where(p => p.ProductID == o.Id).FirstOrDefault());
                    o.Amount = o.Count * (int)o.UnitPrice; //數量 * 單價
                    TotalAmount += o.Amount; //總價
                });

                var rtnAmount = MemberService.GetCalculateAmounts(Id, TotalAmount);

                ShopCar.shopcarProducts = shopcars;
                ShopCar.TotalAmount = rtnAmount.TotalAmount;
                ShopCar.Discount = rtnAmount.Discount;

                return (result.rtn, ShopCar);
            }
            return (result.rtn, new ShopCarModel());
        }

        protected virtual IEnumerable<SelectListItem> GetOptions(int mix, int max)
        {
            List<int> options = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            for (int i = mix; i < max; i += 5)
            {
                options.Add(i);
            }
            return options.Distinct().Select(o => new SelectListItem()
            {
                Value = o.ToString(),
                Text = o.ToString(),
            }); ;
        }
    }
}
