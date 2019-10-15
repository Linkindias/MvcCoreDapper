using Base;
using BLL.Model;
using BLL.PageModel;
using DAL.DBModel;
using DAL.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTestBLL
{
    [TestClass]
    public class memberServiceUnitTest
    {
        static MemberService MemberService = null;
        static Mock<EmployeeRepository> mockEmp = null;
        static Mock<CustomerRepository> mockCus = null;
        static Mock<CustomerModel> mockCusM = null;
        static Mock<EmployeeModel> mockEmpM = null;
        static string connect = string.Empty;
        static int timeout = 0;

        [ClassInitialize]
        public static void memberServiceUnitTestInitialize(TestContext testContext)
        {
            mockEmp = new Mock<EmployeeRepository>(new object[] { connect, timeout });
            mockCus = new Mock<CustomerRepository>(new object[] { connect, timeout });
            mockCusM = new Mock<CustomerModel>();
            mockEmpM = new Mock<EmployeeModel>();
            MemberService = new MemberService(mockEmp.Object, mockCus.Object, mockEmpM.Object, mockCusM.Object);
        }

        [TestMethod()]
        public void GetMember_當查依會員編號查詢會員員工錯誤_則回傳員工錯誤訊息()
        {
            mockEmp.Setup(p => p.GetEmployeeById(It.IsAny<Int32>(), It.IsAny<Int32>()))
                        .Returns(() => (new Result() { IsSuccess = false, ErrorMsg = "error" }, null));
            mockCus.Setup(p => p.GetCustomerById(It.IsAny<string>(), It.IsAny<Int32>())).Returns(() => (new Result(), null));

            var result = MemberService.GetMember("Test");

            Assert.AreEqual(false, result.IsSuccess);
            Assert.AreEqual("error", result.ErrorMsg);
        }

        [TestMethod()]
        public void GetMember_當查依會員編號查詢會員客戶錯誤_則回傳客戶錯誤訊息()
        {
            mockEmp.Setup(p => p.GetEmployeeById(It.IsAny<Int32>(), It.IsAny<Int32>())).Returns(() => (new Result() { IsSuccess = true }, null));
            mockCus.Setup(p => p.GetCustomerById(It.IsAny<string>(), It.IsAny<Int32>())).Returns(() => (new Result() { IsSuccess = false, ErrorMsg = "error" }, null));

            var result = MemberService.GetMember("Test");

            Assert.AreEqual(false, result.IsSuccess);
            Assert.AreEqual("error", result.ErrorMsg);
        }

        [TestMethod()]
        public void GetMember_當查依會員編號查詢會員有員工_則回傳員工訊息()
        {
            mockEmp.Setup(p => p.GetEmployeeById(It.IsAny<Int32>(), It.IsAny<Int32>()))
                        .Returns(() => (new Result() { IsSuccess = true }, new Employees()
                        {
                            Account = "Test",
                            EmployeeID = 1,
                        }));
            mockCus.Setup(p => p.GetCustomerById(It.IsAny<string>(), It.IsAny<Int32>())).Returns(() => (new Result() { IsSuccess = true }, null));

            var result = MemberService.GetMember("Test");

            Assert.AreEqual(true, result.IsSuccess);
            Assert.AreEqual("Test", result.Account);
            Assert.AreEqual(1, result.EmployeeID);
        }

        [TestMethod()]
        public void GetMember_當查依會員編號查詢會員有客戶_則回傳客戶訊息()
        {
            mockEmp.Setup(p => p.GetEmployeeById(It.IsAny<Int32>(), It.IsAny<Int32>())).Returns(() => (new Result() { IsSuccess = true }, null));
            mockCus.Setup(p => p.GetCustomerById(It.IsAny<string>(), It.IsAny<Int32>())).Returns(() => (new Result() { IsSuccess = true }, new Customers()
            {
                Account = "Test",
                CustomerID = "1",
            }));

            var result = MemberService.GetMember("Test");

            Assert.AreEqual(true, result.IsSuccess);
            Assert.AreEqual("Test", result.Account);
            Assert.AreEqual("1", result.CustomerID);
        }

        [TestMethod()]
        public void UpdateMember_當查依會員更新客戶成功_則回傳更新客戶成功訊息()
        {
            mockCus.Setup(p => p.UpdateCustomer(It.IsAny<Customers>())).Returns(() => (new Result() { IsSuccess = true }, 1));

            var result = MemberService.UpdateMember(new CustomerModel() { 
                Account = "Test"
            }, null);

            Assert.AreEqual(true, result.IsSuccess);
            Assert.AreEqual("會員更新成功", result.SuccessMsg);
        }

        [TestMethod()]
        public void UpdateMember_當查依會員更新員工成功_則回傳更新員工成功訊息()
        {
            mockEmp.Setup(p => p.UpdateEmployee(It.IsAny<Employees>())).Returns(() => (new Result() { IsSuccess = true }, 1));

            var result = MemberService.UpdateMember(null, new EmployeeModel() { 
                Account = "Test"
            });

            Assert.AreEqual(true, result.IsSuccess);
            Assert.AreEqual("會員更新成功", result.SuccessMsg);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            mockEmp = null;
            mockCus = null;
            MemberService = null;
        }
    }
}
