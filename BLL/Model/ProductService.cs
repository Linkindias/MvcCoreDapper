using DAL.DTOModel;
using DAL.Repository;
using Microsoft.Extensions.Configuration;
using Omu.ValueInjecter;
using System.Collections.Generic;
using System.Linq;

namespace BLL.Model
{
    public class ProductService
    {
        IConfiguration Configuration;
        ProductRepository ProductRep;
        CategorieRepository CategorieRep;
        OrderDetailRepository OrderDetailRep;

        public ProductService(IConfiguration configuration, 
            ProductRepository productRepository, CategorieRepository categorieRepository, OrderDetailRepository orderDetailRepository)
        {
            this.ProductRep = productRepository;
            this.CategorieRep = categorieRepository;
            this.OrderDetailRep = orderDetailRepository;
            this.Configuration = configuration;
        }

        /// <summary>
        /// 取得產品數量資訊
        /// </summary>
        public List<ProductCountDTO> GetProductQuantitys()
        {
            var resultProduct = ProductRep.GetProductsByDiscontinued(false);
            var resultOrder = OrderDetailRep.GetOrderProductQuantitys(0);
            if (resultProduct.rtn.IsSuccess && resultOrder.rtn.IsSuccess)
            {
                List<ProductCountDTO> productCounts = new List<ProductCountDTO>();
                productCounts.InjectFrom(resultProduct.products);
                List<ProductCountDTO> orderProductCounts = resultOrder.productCounts;

                productCounts.ForEach(o =>
                {
                    o.Sales = orderProductCounts.Where(p => p.ProductID == o.ProductID).FirstOrDefault().Sales; //銷售
                    o.Quantity = o.Inventory - o.Sales; //庫存 - 銷售 = 剩餘
});
                int SaleCount = int.Parse(Configuration["ProductSaleCount"]);
                return productCounts.Where(o=> o.Quantity > SaleCount)
                                    .OrderBy(o => o.ProductID).ToList();
            }
            return new List<ProductCountDTO>();
        }
    }
}
