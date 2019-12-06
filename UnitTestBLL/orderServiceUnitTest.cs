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
    public class orderServiceUnitTest
    {
        static Mock<IMemoryCache> mockCache = null;
        static Mock<IMemberOfOrder> mockMember = null;
        static OrderService OrderService = null;
        static Mock<OrderRepository> mockOrder = null;
        static Mock<OrderDetailRepository> mockOrderDetial = null;
        static string connect = string.Empty;
        static int timeout = 0;

        [ClassInitialize]
        public static void orderServiceUnitTestInitialize(TestContext testContext)
        {
            mockCache = new Mock<IMemoryCache>();
            mockMember = new Mock<IMemberOfOrder>();
            mockOrder = new Mock<OrderRepository>(new object[] { connect, timeout });
            mockOrderDetial = new Mock<OrderDetailRepository>(new object[] { connect, timeout });
            OrderService = new OrderService(mockOrder.Object, mockOrderDetial.Object, mockMember.Object);
        }

        [TestMethod()]
        public void CreateOrder_當建立訂單正常_則回傳訊息()
        {
            
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            mockCache = null;
            mockMember = null;
            mockOrder = null;
            mockOrderDetial = null;
            OrderService = null;
        }
    }
}

