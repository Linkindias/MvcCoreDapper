using Base;
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
        static ProductService ProductService = null;
        static Mock<ProductRepository> mockProduct = null;
        static Mock<CategorieRepository> mockCategory = null;
        static Mock<OrderDetailRepository> mockOrderDetail = null;
        static Mock<ProductModel> mockProductModel = null;
        static Mock<EmployeeModel> mockEmployeeModel = null;
        static Mock<CustomerModel> mockCustomerModel = null;
        static string connect = string.Empty;
        static int timeout = 0;

        [ClassInitialize]
        public static void memberServiceUnitTestInitialize(TestContext testContext)
        {
            mockConfig = new Mock<IConfiguration>();
            mockCache = new Mock<IMemoryCache>();
            mockProduct = new Mock<ProductRepository>(new object[] { connect, timeout });
            mockCategory = new Mock<CategorieRepository>(new object[] { connect, timeout });
            mockOrderDetail = new Mock<OrderDetailRepository>(new object[] { connect, timeout });
            mockProductModel = new Mock<ProductModel>();
            mockEmployeeModel = new Mock<EmployeeModel>();
            mockCustomerModel = new Mock<CustomerModel>();
            ProductService = new ProductService(mockConfig.Object, mockCache.Object,
                                    mockProduct.Object, mockCategory.Object, mockOrderDetail.Object, 
                                    mockProductModel.Object, mockEmployeeModel.Object, mockCustomerModel.Object);
        }

        [TestMethod()]
        public void GetCategoriesAndProducts_當取得產品類別及資訊正常_則回傳查產品類別及資訊訊息()
        {
            object categorys = null;
            mockCache.Setup(p => p.TryGetValue("GetCategory", out categorys)).Returns(true);
            object products = null;
            mockCache.Setup(p => p.TryGetValue("GetProduct", out products)).Returns(true);
            var mockproduct = new Mock<ProductService>(mockConfig.Object, mockCache.Object,
                                    mockProduct.Object, mockCategory.Object, mockOrderDetail.Object, 
                                    mockProductModel.Object, mockEmployeeModel.Object, mockCustomerModel.Object);
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
            var mockproduct = new Mock<ProductService>(mockConfig.Object, mockCache.Object,
                                    mockProduct.Object, mockCategory.Object, mockOrderDetail.Object,
                                    mockProductModel.Object, mockEmployeeModel.Object, mockCustomerModel.Object);
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
            var mockproduct = new Mock<ProductService>(mockConfig.Object, mockCache.Object,
                                    mockProduct.Object, mockCategory.Object, mockOrderDetail.Object,
                                    mockProductModel.Object, mockEmployeeModel.Object, mockCustomerModel.Object);
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
            var mockproduct = new Mock<ProductService>(mockConfig.Object, mockCache.Object,
                                    mockProduct.Object, mockCategory.Object, mockOrderDetail.Object,
                                    mockProductModel.Object, mockEmployeeModel.Object, mockCustomerModel.Object);
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

        [ClassCleanup]
        public static void ClassCleanup()
        {
            mockConfig = null;
            mockCache = null;
            mockProduct = null;
            mockCategory = null;
            mockOrderDetail = null;
            mockProductModel = null;
            ProductService = null;
        }
    }
}

