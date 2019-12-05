using Base;
using BLL.InterFace;
using BLL.Model;
using BLL.PageModel;
using DAL.DBModel;
using DAL.DTOModel;
using DAL.Repository;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnitTestBLL
{
    [TestClass]
    public class productServiceUnitTest
    {
        static Mock<IConfiguration> mockConfig = null;
        static Mock<IMemoryCache> mockCache = null;
        static Mock<IMemberOfProduct> mockMember = null;
        static ProductService ProductService = null;
        static Mock<ProductRepository> mockProduct = null;
        static Mock<CategorieRepository> mockCategory = null;
        static Mock<OrderDetailRepository> mockOrderDetail = null;
        static Mock<SupplierRepository> mockSupplier = null;
        static Mock<ProductModel> mockProductModel = null;
        static Mock<ShopCarModel> mockShopCarModel = null;
        static string connect = string.Empty;
        static int timeout = 0;

        [ClassInitialize]
        public static void memberServiceUnitTestInitialize(TestContext testContext)
        {
            mockConfig = new Mock<IConfiguration>();
            mockCache = new Mock<IMemoryCache>();
            mockMember = new Mock<IMemberOfProduct>();
            mockProduct = new Mock<ProductRepository>(new object[] { connect, timeout });
            mockCategory = new Mock<CategorieRepository>(new object[] { connect, timeout });
            mockOrderDetail = new Mock<OrderDetailRepository>(new object[] { connect, timeout });
            mockSupplier = new Mock<SupplierRepository>(new object[] { connect, timeout });
            mockProductModel = new Mock<ProductModel>();
            mockShopCarModel = new Mock<ShopCarModel>();
            ProductService = new ProductService(mockConfig.Object, mockCache.Object, mockMember.Object,
                                    mockProduct.Object, mockCategory.Object, mockOrderDetail.Object, mockSupplier.Object,
                                    mockProductModel.Object, mockShopCarModel.Object);
        }

        [TestMethod()]
        public void GetCategoriesAndProducts_當取得產品類別及資訊正常_則回傳查產品類別及資訊訊息()
        {
            object categorys = null;
            mockCache.Setup(p => p.TryGetValue("GetCategory", out categorys)).Returns(true);
            object products = null;
            mockCache.Setup(p => p.TryGetValue("GetProduct", out products)).Returns(true);
            var mockproduct = new Mock<ProductService>(mockConfig.Object, mockCache.Object, mockMember.Object,
                                    mockProduct.Object, mockCategory.Object, mockOrderDetail.Object, 
                                    mockProductModel.Object, mockShopCarModel.Object);
            mockproduct.Protected().Setup("GetOptions", new object[] { 0, 10 }).Verifiable();
            ProductService = mockproduct.Object;
            mockCategory.Setup(p => p.GetCategorys()).Returns(() => (new Result() { IsSuccess = true }, new List<Categories>() { 
                new Categories() { 
                    CategoryID = 1, CategoryName = "test"
                }}));
            mockProduct.Setup(p => p.GetProductsByParam(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int[]>(), It.IsAny<bool>())).Returns(() => (new Result() { IsSuccess = true }, new List<Products>() {
                new Products() {
                    CategoryID = 1, ProductID = 2, ProductName = "producttest",UnitsInStock = 10
                }}));
            var entryMock = new Mock<ICacheEntry>();
            mockCache.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(entryMock.Object);
            mockOrderDetail.Setup(p => p.GetOrderProductQuantitys(It.IsAny<int[]>())).Returns(() => (new Result() { IsSuccess = true }, new List<ProductCountDTO>() {
                new ProductCountDTO() {
                    CategoryID = 1, ProductID = 2, ProductName = "producttest",UnitsInStock = 10, Sales = 1,
                }}));
            mockConfig.SetupGet(p => p[It.IsAny<String>()]).Returns("1");

            var result = ProductService.GetCategoriesAndProducts("","");

            Assert.AreEqual(true, result.rtn.IsSuccess);
            Assert.AreEqual(2, result.product.Products[0].ProductID);
            Assert.AreEqual("producttest", result.product.Products[0].ProductName);
            Assert.AreEqual(9, result.product.Products[0].Quantity);
            Assert.AreEqual("test", result.product.Categories.ToList()[0].Text);
            Assert.AreEqual("1", result.product.Categories.ToList()[0].Value);
        }

        [TestMethod()]
        public void GetCategoriesAndProducts_當取得產品類別錯誤_則回傳產品類別錯誤()
        {
            object categorys = null;
            mockCache.Setup(p => p.TryGetValue("GetCategory", out categorys)).Returns(true);
            object products = null;
            mockCache.Setup(p => p.TryGetValue("GetProduct", out products)).Returns(true);
            var mockproduct = new Mock<ProductService>(mockConfig.Object, mockCache.Object, mockMember.Object,
                                    mockProduct.Object, mockCategory.Object, mockOrderDetail.Object,
                                    mockProductModel.Object, mockShopCarModel.Object);
            mockproduct.Protected().Setup("GetOptions", new object[] { 0, 10 }).Verifiable();
            ProductService = mockproduct.Object;

            mockCategory.Setup(p => p.GetCategorys()).Returns(() => (new Result() { IsSuccess = false }, new List<Categories>() {
                new Categories() {
                    CategoryID = 1, CategoryName = "test"
                }}));
            mockProduct.Setup(p => p.GetProductsByParam(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int[]>(), It.IsAny<bool>())).Returns(() => (new Result() { IsSuccess = true }, new List<Products>() {
                new Products() {
                    CategoryID = 1, ProductID = 2, ProductName = "producttest",UnitsInStock = 10
                }}));

            var result = ProductService.GetCategoriesAndProducts("", "");

            Assert.AreEqual(false, result.rtn.IsSuccess);
            Assert.AreEqual("查無產品類別", result.rtn.ErrorMsg);
        }

        [TestMethod()]
        public void GetCategoriesAndProducts_當取得產品資訊錯誤_則回傳產品資訊錯誤()
        {
            object categorys = null;
            mockCache.Setup(p => p.TryGetValue("GetCategory", out categorys)).Returns(true);
            object products = null;
            mockCache.Setup(p => p.TryGetValue("GetProduct", out products)).Returns(true);
            var mockproduct = new Mock<ProductService>(mockConfig.Object, mockCache.Object, mockMember.Object,
                                    mockProduct.Object, mockCategory.Object, mockOrderDetail.Object,
                                    mockProductModel.Object, mockShopCarModel.Object);
            mockproduct.Protected().Setup("GetOptions", new object[] { 0, 10 }).Verifiable();
            ProductService = mockproduct.Object;

            mockCategory.Setup(p => p.GetCategorys()).Returns(() => (new Result() { IsSuccess = true }, new List<Categories>() {
                new Categories() {
                    CategoryID = 1, CategoryName = "test"
                }}));
            mockProduct.Setup(p => p.GetProductsByParam(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int[]>(), It.IsAny<bool>())).Returns(() => (
                new Result() { IsSuccess = false }, 
                new List<Products>() {
                    new Products() {
                        CategoryID = 1, ProductID = 2, ProductName = "producttest",UnitsInStock = 10
                    }}));

            var result = ProductService.GetCategoriesAndProducts("", "");

            Assert.AreEqual(false, result.rtn.IsSuccess);
            Assert.AreEqual("查無產品資訊", result.rtn.ErrorMsg);
        }

        [TestMethod()]
        public void GetCategoriesAndProducts_當取得產品數量錯誤_則回傳產品數量錯誤()
        {
            object categorys = null;
            mockCache.Setup(p => p.TryGetValue("GetCategory", out categorys)).Returns(true);
            object products = null;
            mockCache.Setup(p => p.TryGetValue("GetProduct", out products)).Returns(true);
            var mockproduct = new Mock<ProductService>(mockConfig.Object, mockCache.Object, mockMember.Object,
                                    mockProduct.Object, mockCategory.Object, mockOrderDetail.Object,
                                    mockProductModel.Object, mockShopCarModel.Object);
            mockproduct.Protected().Setup("GetOptions", new object[] { 0, 10 }).Verifiable();
            ProductService = mockproduct.Object;
            mockCategory.Setup(p => p.GetCategorys()).Returns(() => (new Result() { IsSuccess = true }, new List<Categories>() {
                new Categories() {
                    CategoryID = 1, CategoryName = "test"
                }}));
            mockProduct.Setup(p => p.GetProductsByParam(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int[]>(), It.IsAny<bool>())).Returns(() => (new Result() { IsSuccess = true }, new List<Products>() {
                new Products() {
                    CategoryID = 1, ProductID = 2, ProductName = "producttest",UnitsInStock = 10
                }}));
            var entryMock = new Mock<ICacheEntry>();
            mockCache.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(entryMock.Object);
            mockOrderDetail.Setup(p => p.GetOrderProductQuantitys(It.IsAny<int[]>())).Returns(() => (new Result() { IsSuccess = false }, new List<ProductCountDTO>() {
                new ProductCountDTO() {
                    CategoryID = 1, ProductID = 2, ProductName = "producttest",UnitsInStock = 10, Sales = 1,
                }}));

            var result = ProductService.GetCategoriesAndProducts("", "");

            Assert.AreEqual(false, result.rtn.IsSuccess);
            Assert.AreEqual("查無產品數量", result.rtn.ErrorMsg);
        }

        [TestMethod()]
        public void GetShopCarProducts_當取得購物車產品資訊正確_則回傳購物車產品資訊()
        {
            mockProduct.Setup(p => p.GetProductsByParam(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int[]>(), It.IsAny<bool>())).Returns(() => (new Result() { IsSuccess = true }, new List<Products>() {
                new Products() {
                    CategoryID = 1, ProductID = 1, ProductName = "test1",UnitsInStock = 10, UnitPrice = 100m,
                },
                new Products() {
                    CategoryID = 1, ProductID = 2, ProductName = "test2",UnitsInStock = 10, UnitPrice = 900m,
                }}));
            mockMember.Setup(p => p.GetCalculateAmounts(It.IsAny<string>(), It.IsAny<int>())).Returns(() => (1100, 0.0));
            var entryMock = new Mock<ICacheEntry>();
            mockCache.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(entryMock.Object);

            var result = ProductService.GetShopCarProducts("1", new List<ShopCarProductModel>() { 
                new ShopCarProductModel()
                {
                    Id = 2,
                    Count = 2,
                    UnitPrice = 0m,
                    Amount = 0,
                },
                new ShopCarProductModel()
                {
                    Id = 1,
                    Count = 1,
                    UnitPrice = 0m,
                    Amount = 0,
                }
            });

            Assert.AreEqual(true, result.rtn.IsSuccess);
            Assert.AreEqual(1100, result.shopCar.totalAmount);
            Assert.AreEqual(0, result.shopCar.discount);
        }

        [TestMethod()]
        public void GetProductsById_當依登入者取得購物車產品資訊正確_則回傳購物車產品資訊()
        {
            string Id = "1";
            var entryMock = new Mock<ICacheEntry>();
            mockCache.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(entryMock.Object);
            
            var result = ProductService.GetProductsById(Id).ToList();

            Assert.AreEqual(null, result);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            mockConfig = null;
            mockCache = null;
            mockMember = null;
            mockProduct = null;
            mockCategory = null;
            mockOrderDetail = null;
            mockProductModel = null;
            mockShopCarModel = null;
            ProductService = null;
        }
    }
}

