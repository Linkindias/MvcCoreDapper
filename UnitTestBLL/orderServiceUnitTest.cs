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
    public class orderServiceUnitTest
    {
        static Mock<IConfiguration> mockConfig = null;
        static Mock<ILogger<ConnectionBase>> mockLog = null;
        static Mock<IMemberOfOrder> mockMember = null;
        static OrderService OrderService = null;
        static Mock<OrderRepository> mockOrder = null;
        static Mock<OrderDetailRepository> mockOrderDetial = null;
        static Mock<OrderModel> mockOrdetDto = null;

        [ClassInitialize]
        public static void orderServiceUnitTestInitialize(TestContext testContext)
        {
            mockConfig = new Mock<IConfiguration>();
            mockConfig.SetupGet(p => p[It.IsAny<String>()]).Returns("1");
            mockLog = new Mock<ILogger<ConnectionBase>>();
            mockMember = new Mock<IMemberOfOrder>();
            mockOrder = new Mock<OrderRepository>(mockConfig.Object, mockLog.Object);
            mockOrderDetial = new Mock<OrderDetailRepository>(mockConfig.Object, mockLog.Object);
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
            mockLog = null;
            mockMember = null;
            mockOrder = null;
            mockOrderDetial = null;
            mockOrdetDto = null;
            OrderService = null;
        }
    }
}

