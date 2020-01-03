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
        static Mock<IConfiguration> mockConfig = null;
        static Mock<IMemberOfOrder> mockMember = null;
        static OrderService OrderService = null;
        static Mock<OrderRepository> mockOrder = null;
        static Mock<OrderDetailRepository> mockOrderDetial = null;
        static Mock<OrderModel> mockOrdetDto = null;
        static string connect = string.Empty;
        static int timeout = 0;

        [ClassInitialize]
        public static void orderServiceUnitTestInitialize(TestContext testContext)
        {
            mockConfig = new Mock<IConfiguration>();
            mockMember = new Mock<IMemberOfOrder>();
            mockOrder = new Mock<OrderRepository>(new object[] { connect, timeout });
            mockOrderDetial = new Mock<OrderDetailRepository>(new object[] { connect, timeout });
            mockOrdetDto = new Mock<OrderModel>();
            OrderService = new OrderService(mockMember.Object, mockConfig.Object,
                mockOrder.Object, mockOrderDetial.Object, mockOrdetDto.Object);
        }

        [TestMethod()]
        public void CreateOrder_當建立訂單正常_則回傳訊息()
        {
            
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            mockConfig = null;
            mockMember = null;
            mockOrder = null;
            mockOrderDetial = null;
            mockOrdetDto = null;
            OrderService = null;
        }
    }
}

