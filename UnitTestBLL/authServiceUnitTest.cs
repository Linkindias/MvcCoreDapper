using Base;
using BLL.Model;
using DAL.DBModel;
using DAL.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using System;

namespace UnitTestBLL
{
    [TestClass]
    public class authServiceUnitTest
    {
        static AuthService AuthService = null;
        static Mock<EmployeeRepository> mockEmp = null;
        static Mock<CustomerRepository> mockCus = null;
        static Mock<AuthenticationRepository> mockAuth = null;
        static string connect = string.Empty;
        static int timeout = 0;

        [ClassInitialize]
        public static void authServiceUnitTestInitialize(TestContext testContext)
        {
            mockEmp = new Mock<EmployeeRepository>(new object[] { connect, timeout });
            mockCus = new Mock<CustomerRepository>(new object[] { connect, timeout });
            mockAuth = new Mock<AuthenticationRepository>(new object[] { connect , timeout });
            AuthService = new AuthService(mockAuth.Object, mockEmp.Object, mockCus.Object,  null);
        }

        [TestMethod()]
        public void LogIn_當查無帳號_則回傳查無帳號訊息()
        {
            var mockauth = new Mock<AuthService>(mockAuth.Object, mockEmp.Object, mockCus.Object, null);
            mockauth.Protected().Setup<string>("DecryptAES", new object[] { "Test" }).Returns("Test");
            AuthService = mockauth.Object;
            mockEmp.Setup(p => p.GetEmployeeByAccount("test", 10)).Returns((new Result(), null));
            mockCus.Setup(p => p.GetCustomerByAccount("test", 10)).Returns((new Result(), null));

            var result = AuthService.LogIn("Test", "1234");
            Assert.AreEqual("查無此帳號 Test，請確認", result.rtn.ErrorMsg);
        }

        [TestMethod()]
        public void LogIn_當查有帳號且帳密不對_則回傳查錯誤訊息()
        {
            var mockauth = new Mock<AuthService>(mockAuth.Object, mockEmp.Object, mockCus.Object, null);
            mockauth.Protected().Setup<string>("DecryptAES", new object[] { "Test" }).Returns("Test");
            AuthService = mockauth.Object;
            mockEmp.Setup(p => p.GetEmployeeByAccount("test", 10)).Returns<(Result, Employees)>(new Result() { IsSuccess  = true }, new Employees()
            {
                Account = "Test",
                EmployeeID = 1,
            });
            mockCus.Setup(p => p.GetCustomerByAccount("test", 10)).Returns((new Result(), null));
            mockAuth.Setup(p => p.GetAuthenticationByParams(0,string.Empty,string.Empty, 10)).Returns((new Result(), null));

            var result = AuthService.LogIn("Test", "1234");
            Assert.AreEqual("帳號 Test或密碼不正確，請確認", result.rtn.ErrorMsg);
        }



        [ClassCleanup]
        public static void ClassCleanup()
        {
            AuthService = null;
        }
    }
}

