using Base;
using BLL.InterFace;
using BLL.Model;
using BLL.PageModel;
using DAL;
using DAL.DBModel;
using DAL.DTOModel;
using DAL.Repository;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
        static Mock<ILogger<ConnectionBase>> mockLog = null;
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
            mockConfig.SetupGet(p => p[It.IsAny<String>()]).Returns("1");
            mockLog = new Mock<ILogger<ConnectionBase>>();
            mockCache = new Mock<IMemoryCache>();
            mockMember = new Mock<IMemberOfProduct>();
            mockProduct = new Mock<ProductRepository>(mockConfig.Object, mockLog.Object);
            mockCategory = new Mock<CategorieRepository>(mockConfig.Object, mockLog.Object);
            mockOrderDetail = new Mock<OrderDetailRepository>(mockConfig.Object, mockLog.Object);
            mockSupplier = new Mock<SupplierRepository>(mockConfig.Object, mockLog.Object);
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
            object supplier = null;
            mockCache.Setup(p => p.TryGetValue("GetSupplier", out supplier)).Returns(true);
            var mockproduct = new Mock<ProductService>(mockConfig.Object, mockCache.Object, mockMember.Object,
                                    mockProduct.Object, mockCategory.Object, mockOrderDetail.Object, mockSupplier.Object,
                                    mockProductModel.Object, mockShopCarModel.Object);
            mockproduct.Protected().Setup("GetOptions", new object[] { 0, 10 }).Verifiable();
            ProductService = mockproduct.Object;
            mockCategory.Setup(p => p.GetCategorys()).Returns(() => (new Result() { IsSuccess = true }, new List<Categories>() {
                new Categories() {
                    CategoryID = 1, CategoryName = "test"
                }}));
            mockProduct.Setup(p => p.GetProductsByParam(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int[]>(), It.IsAny<bool>())).Returns(() => (new Result() { IsSuccess = true }, new List<Products>() {
                new Products() {
                    CategoryID = 1, ProductID = 2, ProductName = "producttest",UnitsInStock = 10,SupplierID =1
                }}));
            mockSupplier.Setup(p => p.GetSuppliers()).Returns(() => (new Result() { IsSuccess = true }, new List<Suppliers>() {
                new Suppliers() {
                    SupplierID = 1, CompanyName = "Test",
                }}));
            var entryMock = new Mock<ICacheEntry>();
            mockCache.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(entryMock.Object);
            mockOrderDetail.Setup(p => p.GetOrderProductQuantitys(It.IsAny<int[]>())).Returns(() => (new Result() { IsSuccess = true }, new List<ProductCountDTO>() {
                new ProductCountDTO() {
                    CategoryID = 1, ProductID = 2, ProductName = "producttest",UnitsInStock = 10, Sales = 1,
                }}));
            mockConfig.SetupGet(p => p[It.IsAny<String>()]).Returns("1");

            var result = ProductService.GetCategoriesAndProducts("", "");

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
            object supplier = null;
            mockCache.Setup(p => p.TryGetValue("GetSupplier", out supplier)).Returns(true);
            var mockproduct = new Mock<ProductService>(mockConfig.Object, mockCache.Object, mockMember.Object,
                                    mockProduct.Object, mockCategory.Object, mockOrderDetail.Object, mockSupplier.Object,
                                    mockProductModel.Object, mockShopCarModel.Object);
            mockproduct.Protected().Setup("GetOptions", new object[] { 0, 10 }).Verifiable();
            ProductService = mockproduct.Object;

            mockCategory.Setup(p => p.GetCategorys()).Returns(() => (new Result() { IsSuccess = false }, new List<Categories>() {
                new Categories() {
                    CategoryID = 1, CategoryName = "test"
                }}));
            mockProduct.Setup(p => p.GetProductsByParam(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int[]>(), It.IsAny<bool>())).Returns(() => (new Result() { IsSuccess = true }, new List<Products>() {
                new Products() {
                    CategoryID = 1, ProductID = 2, ProductName = "producttest",UnitsInStock = 10,SupplierID=1
                }}));
            mockSupplier.Setup(p => p.GetSuppliers()).Returns(() => (new Result() { IsSuccess = true }, new List<Suppliers>() {
                new Suppliers() {
                    SupplierID = 1, CompanyName = "Test",
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
            object supplier = null;
            mockCache.Setup(p => p.TryGetValue("GetSupplier", out supplier)).Returns(true);
            var mockproduct = new Mock<ProductService>(mockConfig.Object, mockCache.Object, mockMember.Object,
                                    mockProduct.Object, mockCategory.Object, mockOrderDetail.Object, mockSupplier.Object,
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
            mockSupplier.Setup(p => p.GetSuppliers()).Returns(() => (new Result() { IsSuccess = true }, new List<Suppliers>() {
                new Suppliers() {
                    SupplierID = 1, CompanyName = "Test",
                }}));

            var result = ProductService.GetCategoriesAndProducts("", "");

            Assert.AreEqual(false, result.rtn.IsSuccess);
            Assert.AreEqual("查無產品資訊", result.rtn.ErrorMsg);
        }

        [TestMethod()]
        public void GetCategoriesAndProducts_當取得供應商資訊錯誤_則回傳供應商資訊錯誤()
        {
            object categorys = null;
            mockCache.Setup(p => p.TryGetValue("GetCategory", out categorys)).Returns(true);
            object products = null;
            mockCache.Setup(p => p.TryGetValue("GetProduct", out products)).Returns(true);
            object supplier = null;
            mockCache.Setup(p => p.TryGetValue("GetSupplier", out supplier)).Returns(true);
            var mockproduct = new Mock<ProductService>(mockConfig.Object, mockCache.Object, mockMember.Object,
                                    mockProduct.Object, mockCategory.Object, mockOrderDetail.Object, mockSupplier.Object,
                                    mockProductModel.Object, mockShopCarModel.Object);
            mockproduct.Protected().Setup("GetOptions", new object[] { 0, 10 }).Verifiable();
            ProductService = mockproduct.Object;

            mockCategory.Setup(p => p.GetCategorys()).Returns(() => (new Result() { IsSuccess = true }, new List<Categories>() {
                new Categories() {
                    CategoryID = 1, CategoryName = "test"
                }}));
            mockProduct.Setup(p => p.GetProductsByParam(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int[]>(), It.IsAny<bool>())).Returns(() => (
                new Result() { IsSuccess = true },
                new List<Products>() {
                    new Products() {
                        CategoryID = 1, ProductID = 2, ProductName = "producttest",UnitsInStock = 10,SupplierID=1
                    }}));
            mockSupplier.Setup(p => p.GetSuppliers()).Returns(() => (new Result() { IsSuccess = false }, new List<Suppliers>() {
                new Suppliers() {
                    SupplierID = 1, CompanyName = "Test",
                }}));

            var result = ProductService.GetCategoriesAndProducts("", "");

            Assert.AreEqual(false, result.rtn.IsSuccess);
            Assert.AreEqual("查無供應商", result.rtn.ErrorMsg);
        }

        [TestMethod()]
        public void GetCategoriesAndProducts_當取得產品數量錯誤_則回傳產品數量錯誤()
        {
            string Id = "1";
            object categorys = null;
            mockCache.Setup(p => p.TryGetValue("GetCategory", out categorys)).Returns(true);
            object products = null;
            mockCache.Setup(p => p.TryGetValue($"GetProduct{Id}", out products)).Returns(true);
            object supplier = null;
            mockCache.Setup(p => p.TryGetValue("GetSupplier", out supplier)).Returns(true);
            var mockproduct = new Mock<ProductService>(mockConfig.Object, mockCache.Object, mockMember.Object,
                                    mockProduct.Object, mockCategory.Object, mockOrderDetail.Object, mockSupplier.Object,
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
            mockSupplier.Setup(p => p.GetSuppliers()).Returns(() => (new Result() { IsSuccess = true }, new List<Suppliers>() {
                new Suppliers() {
                    SupplierID = 1, CompanyName = "Test",
                }}));
            var entryMock = new Mock<ICacheEntry>();
            mockCache.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(entryMock.Object);
            mockOrderDetail.Setup(p => p.GetOrderProductQuantitys(It.IsAny<int[]>())).Returns(() => (new Result() { IsSuccess = false }, new List<ProductCountDTO>() {
                new ProductCountDTO() {
                    CategoryID = 1, ProductID = 2, ProductName = "producttest",UnitsInStock = 10, Sales = 1,
                }}));

            var result = ProductService.GetCategoriesAndProducts(Id, "", "");

            Assert.AreEqual(false, result.rtn.IsSuccess);
            Assert.AreEqual("查無產品數量", result.rtn.ErrorMsg);
        }

        [TestMethod()]
        public void GetShopCarAmount_當取得購物車產品資訊正確_則回傳購物車產品資訊()
        {
            mockProduct.Setup(p => p.GetProductsByParam(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int[]>(), It.IsAny<bool>())).Returns(() => (new Result() { IsSuccess = true }, new List<Products>() {
                new Products() {
                    CategoryID = 1, ProductID = 1, ProductName = "test1",UnitsInStock = 10, UnitPrice = 100m, SupplierID = 1, UnitsOnOrder = 0
                },
                new Products() {
                    CategoryID = 1, ProductID = 2, ProductName = "test2",UnitsInStock = 10, UnitPrice = 900m, SupplierID = 2, UnitsOnOrder = 0
                }}));
            object categorys = new List<Categories>() { new Categories() { CategoryID =1, CategoryName = "CN", Description = "D"} };
            mockCache.Setup(p => p.TryGetValue("GetCategory", out categorys)).Returns(true);
            object supplier = new List<Suppliers>() {
                new Suppliers() { SupplierID =1, CompanyName="CN1",ContactName ="Cn1",ContactTitle="CT1",Phone="123",Address="ad1"},
                new Suppliers() { SupplierID =2, CompanyName="CN2",ContactName ="Cn2",ContactTitle="CT2",Phone="123",Address="ad2"},
            };
            mockCache.Setup(p => p.TryGetValue("GetSupplier", out supplier)).Returns(true);
            mockMember.Setup(p => p.GetCalculateAmounts(It.IsAny<string>(), It.IsAny<int>())).Returns(() => (1000, 0.1));
            var entryMock = new Mock<ICacheEntry>();
            mockCache.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(entryMock.Object);

            var result = ProductService.GetShopCarAmount("1", new List<ShopCarProductModel>() { 
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
            Assert.AreEqual(1900, result.shopCar.totalAmount);
            Assert.AreEqual(0.1, result.shopCar.discount);
            Assert.AreEqual(1000, result.shopCar.disAmount);
        }

        [TestMethod()]
        public void GetProductsById_當依登入者取得購物車產品資訊正確_則回傳購物車產品資訊()
        {
            string Id = "1";
            object products = new List<ShopCarProductModel>() { new ShopCarProductModel() { } };
            mockCache.Setup(p => p.TryGetValue($"ShopCar{Id}", out products)).Returns(true);

            var result = ProductService.GetProductsById(Id).ToList();

            Assert.AreEqual(1, result.Count());
        }

        [TestMethod()]
        public void GetShopCarInfoById_當依登入者取得購物車資訊正確_則回傳購物車資訊()
        {
            string Id = "1";
            object products = new List<ShopCarProductModel>() { new ShopCarProductModel() { Id = 1,ProductID = 1, Count = 1, UnitPrice = 100, CategoryID = 1, SupplierID = 1 } };
            mockCache.Setup(p => p.TryGetValue($"ShopCar{Id}", out products)).Returns(true);

            mockProduct.Setup(p => p.GetProductsByParam(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int[]>(), It.IsAny<bool>())).Returns(() => (new Result() { IsSuccess = true }, new List<Products>() {
                new Products() {
                    CategoryID = 1, ProductID = 1, ProductName = "test1",UnitsInStock = 10, UnitPrice = 100m, SupplierID = 1, UnitsOnOrder = 0
                },
                new Products() {
                    CategoryID = 1, ProductID = 2, ProductName = "test2",UnitsInStock = 10, UnitPrice = 900m, SupplierID = 2, UnitsOnOrder = 0
                }}));
            object categorys = new List<Categories>() { new Categories() { CategoryID = 1, CategoryName = "CN", Description = "D" } };
            mockCache.Setup(p => p.TryGetValue("GetCategory", out categorys)).Returns(true);
            object supplier = new List<Suppliers>() {
                new Suppliers() { SupplierID =1, CompanyName="CN1",ContactName ="Cn1",ContactTitle="CT1",Phone="123",Address="ad1"},
                new Suppliers() { SupplierID =2, CompanyName="CN2",ContactName ="Cn2",ContactTitle="CT2",Phone="123",Address="ad2"},
            };
            mockCache.Setup(p => p.TryGetValue("GetSupplier", out supplier)).Returns(true);
            mockMember.Setup(p => p.GetCalculateAmounts(It.IsAny<string>(), It.IsAny<int>())).Returns(() => (1000, 0.1));
            var entryMock = new Mock<ICacheEntry>();
            mockCache.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(entryMock.Object);

            var result = ProductService.GetShopCarInfoById("1");

            Assert.AreEqual(1000, result.disAmount);
            Assert.AreEqual(0.1, result.discount);
            Assert.AreEqual(100, result.totalAmount);
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

