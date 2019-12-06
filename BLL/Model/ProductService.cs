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
using Microsoft.AspNetCore.Http;

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
        SupplierRepository SupplierRep;
        ProductModel Product;
        ShopCarModel ShopCar;

        public ProductService(IConfiguration configuration, IMemoryCache memoryCache, IMemberOfProduct memberOfProduct,
            ProductRepository productRepository, CategorieRepository categorieRepository, OrderDetailRepository orderDetailRepository,
            SupplierRepository supplierRepository,
            ProductModel productModel, ShopCarModel shopCarModel)
        {
            this.configuration = configuration;
            this.cache = memoryCache;
            this.MemberService = memberOfProduct;
            this.ProductRep = productRepository;
            this.CategorieRep = categorieRepository;
            this.OrderDetailRep = orderDetailRepository;
            this.SupplierRep = supplierRepository;
            this.Product = productModel;
            this.ShopCar = shopCarModel;
        }

        /// <summary>
        /// 取得產品類別、 產品資訊
        /// </summary>
        public (Result rtn, ProductModel product) GetCategoriesAndProducts(string Id, string CategoryId = "", string ProductName = "")
        {
            IEnumerable<Categories> categories = null;
            IEnumerable<Products> products = null;
            IEnumerable<Suppliers> suppliers = null;

            string keyCategory = $"GetCategory";
            string keyProduct = $"GetProduct{Id}";
            string keySupplier = $"GetSupplier";

            cache.TryGetValue<IEnumerable<Categories>>(keyCategory, out categories);
            cache.TryGetValue<IEnumerable<Products>>(keyProduct, out products);
            cache.TryGetValue<IEnumerable<Suppliers>>(keySupplier, out suppliers);

            if (categories == null && products == null && suppliers == null)
            {
                var resultCategroy = CategorieRep.GetCategorys();
                var resultProduct = ProductRep.GetProductsByParam(string.IsNullOrEmpty(CategoryId) ? 0 : int.Parse(CategoryId), ProductName,new int[0], false);
                var resultSupplier = SupplierRep.GetSuppliers();

                if (resultCategroy.rtn.IsSuccess && resultProduct.rtn.IsSuccess && resultSupplier.rtn.IsSuccess)
                {
                    categories = resultCategroy.Categorys;
                    products = resultProduct.products;
                    suppliers = resultSupplier.Suppliers;

                    TimeSpan ts = DateTime.Today.AddDays(1) - DateTime.Now; //1天
                    cache.Set<IEnumerable<Categories>>(keyCategory, categories, ts); //產品類別加入快取
                    cache.Set<IEnumerable<Products>>(keyProduct, products, ts); //產品資訊加入快取
                    cache.Set<IEnumerable<Suppliers>>(keySupplier, suppliers, ts); //供應商資訊加入快取
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

                if (!resultSupplier.rtn.IsSuccess)
                {
                    resultSupplier.rtn.ErrorMsg = "查無供應商";
                    return (resultSupplier.rtn, Product);
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
        /// 依購物產單清單 取得購物車金額
        /// </summary>
        public (Result rtn, ShopCarModel shopCar) GetShopCarAmount(string Id, List<ShopCarProductModel> products)
        {
            var result = ProductRep.GetProductsByParam(0, "", products.Select(o => o.Id).Distinct().ToArray(), false);
            if (result.rtn.IsSuccess)
            {
                string keyCategory = $"GetCategory";
                string keySupplier = $"GetSupplier";
                IEnumerable<Categories> categories = null;
                IEnumerable<Suppliers> suppliers = null;

                cache.TryGetValue<IEnumerable<Categories>>(keyCategory, out categories);
                cache.TryGetValue<IEnumerable<Suppliers>>(keySupplier, out suppliers);

                int TotalAmount = 0;
                products.ForEach(o =>
                {
                    var product = result.products.Where(p => p.ProductID == o.Id).FirstOrDefault();
                    o.CategoryID = product.CategoryID;
                    o.ProductID = product.ProductID;
                    o.ProductName = product.ProductName;
                    o.QuantityPerUnit = product.QuantityPerUnit;
                    o.ReorderLevel = product.ReorderLevel;
                    o.SupplierID = product.SupplierID.Value;
                    o.UnitPrice = product.UnitPrice;
                    o.UnitsInStock = product.UnitsInStock.Value;
                    o.UnitsOnOrder = product.UnitsOnOrder;
                    o.Amount = o.Count * (int)o.UnitPrice; //數量 * 單價
                    TotalAmount += o.Amount; //總價

                    if (categories.Count() > 0) //產品類別
                    {
                        var Category = categories.Where(p => p.CategoryID == o.CategoryID).FirstOrDefault();
                        o.CategoryDescription = Category.Description;
                        o.CategoryName = Category.CategoryName;
                    }

                    if (suppliers.Count() > 0) //供應商
                    {
                        var Supplier = suppliers.Where(p => p.SupplierID == o.SupplierID).FirstOrDefault();
                        o.SupplierID = Supplier.SupplierID;
                        o.SupplierCompanyName = Supplier.CompanyName;
                        o.SupplierContactName = Supplier.ContactName;
                        o.SupplierContactTitle = Supplier.ContactTitle;
                        o.SupplierPhone = Supplier.Phone;
                        o.SupplierAddress = Supplier.Address;
                    }
                });

                var rtnAmount = MemberService.GetCalculateAmounts(Id, TotalAmount); //依登入號計算折扣

                ShopCar.totalAmount = TotalAmount;
                ShopCar.disAmount = rtnAmount.TotalAmount;
                ShopCar.discount = rtnAmount.Discount;
                ShopCar.products = products;

                string keyShopCar = $"ShopCar{Id}";
                TimeSpan ts = DateTime.Now.AddHours(3) - DateTime.Now; //1天
                cache.Set<IEnumerable<ShopCarProductModel>>(keyShopCar, products, ts); //購物車產品清單加入快取

                return (result.rtn, ShopCar);
            }
            return (result.rtn, new ShopCarModel());
        }

        /// <summary>
        /// 依登入者 取得購物車產品資訊
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public IEnumerable<ShopCarProductModel> GetProductsById(string Id)
        {
            IEnumerable<ShopCarProductModel> products = null;

            string keyShopCar = $"ShopCar{Id}";

            cache.TryGetValue<IEnumerable<ShopCarProductModel>>(keyShopCar, out products);

            return products ?? new List<ShopCarProductModel>();
        }

        /// <summary>
        ///  依登入者 取得購物車資訊
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ShopCarModel GetShopCarInfoById(string Id)
        {
            ShopCarModel shopcar = new ShopCarModel();
            shopcar.products = this.GetProductsById(Id).ToList();
            if (shopcar.products != null)
            {
                var result = this.GetShopCarAmount(Id, shopcar.products);

                if (result.rtn.IsSuccess)
                {
                    shopcar.totalAmount = result.shopCar.totalAmount;
                    shopcar.disAmount = result.shopCar.disAmount;
                    shopcar.discount = result.shopCar.discount;
                }
            }
            return shopcar;
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
