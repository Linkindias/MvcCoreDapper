using Base;
using BLL.Model;
using DAL.DBModel;
using DAL.DTOModel;
using DAL.Repository;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using System;
using System.Data.SqlTypes;

namespace UnitTestBLL
{
    [TestClass]
    public class authServiceUnitTest
    {
        static AuthService AuthService = null;
        static Mock<EmployeeRepository> mockEmp = null;
        static Mock<CustomerRepository> mockCus = null;
        static Mock<AuthenticationRepository> mockAuth = null;
        static Mock<IMemoryCache> mockCache = null;
        static string connect = string.Empty;
        static int timeout = 0;

        [ClassInitialize]
        public static void authServiceUnitTestInitialize(TestContext testContext)
        {
            mockCache = new Mock<IMemoryCache>();
            mockEmp = new Mock<EmployeeRepository>(new object[] { connect, timeout });
            mockCus = new Mock<CustomerRepository>(new object[] { connect, timeout });
            mockAuth = new Mock<AuthenticationRepository>(new object[] { connect, timeout });
            AuthService = new AuthService(mockAuth.Object, mockEmp.Object, mockCus.Object, mockCache.Object);
        }

        [TestMethod()]
        public void LogIn_當查無帳號_則回傳查無帳號訊息()
        {
            var mockauth = new Mock<AuthService>(mockAuth.Object, mockEmp.Object, mockCus.Object, mockCache.Object);
            mockauth.Protected().Setup<string>("DecryptAES", new object[] { "Test" }).Returns("Test");
            AuthService = mockauth.Object;
            mockEmp.Setup(p => p.GetEmployeeByAccount(It.IsAny<string>(), 10)).Returns(() => (new Result(), null));
            mockCus.Setup(p => p.GetCustomerByAccount(It.IsAny<string>(), 10)).Returns(() => (new Result(), null));

            var result = AuthService.LogIn("Test", "1234");
            Assert.AreEqual("查無此帳號 Test，請確認", result.rtn.ErrorMsg);
        }

        [TestMethod()]
        public void LogIn_當查有帳號且帳密不對_則回傳查錯誤訊息()
        {
            var mockauth = new Mock<AuthService>(mockAuth.Object, mockEmp.Object, mockCus.Object, mockCache.Object);
            mockauth.Protected().Setup<string>("DecryptAES", new object[] { "Test" }).Returns("Test");
            AuthService = mockauth.Object;
            mockEmp.Setup(p => p.GetEmployeeByAccount(It.IsAny<string>(), 10)).Returns(() => (new Result() { IsSuccess = true }, new Employees()
            {
                Account = "Test",
                EmployeeID = 1,
            }));
            mockCus.Setup(p => p.GetCustomerByAccount(It.IsAny<string>(), 10)).Returns(() => (new Result(), null));
            mockAuth.Setup(p => p.GetAuthenticationByParams(It.IsAny<Int32>(), It.IsAny<string>(), It.IsAny<string>(), 10)).Returns((new Result(), null));

            var result = AuthService.LogIn("Test", "1234");
            Assert.AreEqual("帳號 Test或密碼不正確，請確認", result.rtn.ErrorMsg);
        }

        [TestMethod()]
        public void LogIn_當查有帳號且帳密對且鎖定帳號_則回傳查錯誤訊息()
        {
            var mockauth = new Mock<AuthService>(mockAuth.Object, mockEmp.Object, mockCus.Object, mockCache.Object);
            mockauth.Protected().Setup<string>("DecryptAES", new object[] { "Test" }).Returns("Test");
            AuthService = mockauth.Object;
            mockEmp.Setup(p => p.GetEmployeeByAccount(It.IsAny<string>(), 10)).Returns(() => (new Result() { IsSuccess = true }, new Employees()
            {
                Account = "Test",
                EmployeeID = 1,
            }));
            mockCus.Setup(p => p.GetCustomerByAccount(It.IsAny<string>(), 10)).Returns(() => (new Result(), null));
            mockAuth.Setup(p => p.GetAuthenticationByParams(It.IsAny<Int32>(), It.IsAny<string>(), It.IsAny<string>(), 10))
                .Returns(() => (new Result(), new Authentication()
                {
                    State = 30
                }));

            var result = AuthService.LogIn("Test", "1234");

            Assert.AreEqual("帳號 Test已鎖定，無法登入，請連絡客服!", result.rtn.ErrorMsg);
        }

        [TestMethod()]
        public void LogIn_當查有帳號且帳密對且無鎖定帳號且離職_則回傳查錯誤訊息()
        {
            var mockauth = new Mock<AuthService>(mockAuth.Object, mockEmp.Object, mockCus.Object, mockCache.Object);
            mockauth.Protected().Setup<string>("DecryptAES", new object[] { "Test" }).Returns("Test");
            AuthService = mockauth.Object;
            mockEmp.Setup(p => p.GetEmployeeByAccount(It.IsAny<string>(), 10)).Returns(() => (new Result() { IsSuccess = true }, new Employees()
            {
                Account = "Test",
                EmployeeID = 1,
            }));
            mockCus.Setup(p => p.GetCustomerByAccount(It.IsAny<string>(), 10)).Returns(() => (new Result(), null));
            mockAuth.Setup(p => p.GetAuthenticationByParams(It.IsAny<Int32>(), It.IsAny<string>(), It.IsAny<string>(), 10))
                .Returns(() => (new Result(), new Authentication()
                {
                    State = 10,
                    EffectiveDate = DateTime.Now.AddDays(-1)
                }));

            var result = AuthService.LogIn("Test", "1234");
            Assert.AreEqual("帳號 Test已離職，無法登入", result.rtn.ErrorMsg);
        }

        [TestMethod()]
        public void LogIn_當查有帳號且帳密對且無鎖定帳號且離職且已登入_則回傳查錯誤訊息()
        {
            var mockauth = new Mock<AuthService>(mockAuth.Object, mockEmp.Object, mockCus.Object, mockCache.Object);
            mockauth.Protected().Setup<string>("DecryptAES", new object[] { "Test" }).Returns("Test");
            AuthService = mockauth.Object;
            mockEmp.Setup(p => p.GetEmployeeByAccount(It.IsAny<string>(), 10)).Returns(() => (new Result() { IsSuccess = true }, new Employees()
            {
                Account = "Test",
                EmployeeID = 1,
            }));
            mockCus.Setup(p => p.GetCustomerByAccount(It.IsAny<string>(), 10)).Returns(() => (new Result(), null));
            mockAuth.Setup(p => p.GetAuthenticationByParams(It.IsAny<Int32>(), It.IsAny<string>(), It.IsAny<string>(), 10))
                .Returns(() => (new Result(), new Authentication()
                {
                    State = 10,
                    EffectiveDate = DateTime.Now.AddDays(1),
                    VerifyCode = Guid.NewGuid(),
                }));

            var result = AuthService.LogIn("Test", "1234");
            Assert.AreEqual("帳號 Test已登入中，無法登入", result.rtn.ErrorMsg);
        }

        [TestMethod()]
        public void LogIn_當查有帳號且帳密對且無鎖定帳號且離職且未登入_則回傳檢查物件()
        {
            var mockauth = new Mock<AuthService>(mockAuth.Object, mockEmp.Object, mockCus.Object, mockCache.Object);
            mockauth.Protected().Setup<string>("DecryptAES", new object[] { "Test" }).Returns("Test");
            AuthService = mockauth.Object;
            mockEmp.Setup(p => p.GetEmployeeByAccount(It.IsAny<string>(), 10)).Returns(() => (new Result() { IsSuccess = true }, new Employees()
            {
                Account = "Test",
                EmployeeID = 1,
            }));
            mockCus.Setup(p => p.GetCustomerByAccount(It.IsAny<string>(), 10)).Returns(() => (new Result(), null));
            mockAuth.Setup(p => p.GetAuthenticationByParams(It.IsAny<Int32>(), It.IsAny<string>(), It.IsAny<string>(), 10))
                .Returns(() => (new Result(), new Authentication()
                {
                    State = 10,
                    EffectiveDate = DateTime.Now.AddDays(1),
                    VerifyCode = null,
                }));
            mockAuth.Setup(p => p.UpdateAuthCode(It.IsAny<Int32>(), It.IsAny<string>(), It.IsAny<Guid?>(), It.IsAny<Int32>()))
                .Returns(() => (new Result(), 1));

            var result = AuthService.LogIn("Test", "1234");
            Assert.AreEqual(true, result.rtn.IsSuccess);
            Assert.AreEqual(null, result.customer);
            Assert.AreEqual(1, result.employee.EmployeeID);
        }

        [TestMethod()]
        public void LogOut_當查無帳號_則回傳查無帳號訊息()
        {
            mockEmp.Setup(p => p.GetEmployeeById(It.IsAny<Int32>(), It.IsAny<Int32>())).Returns(() => (new Result() { IsSuccess = false }, null));
            mockCus.Setup(p => p.GetCustomerById(It.IsAny<string>(), It.IsAny<Int32>())).Returns(() => (new Result(), null));

            var result = AuthService.LogOut("Test");
            Assert.AreEqual(false, result.IsSuccess);
            Assert.AreEqual("查無此帳號 Test，請確認", result.ErrorMsg);
        }

        [TestMethod()]
        public void LogOut_當查有帳號_則回傳查帳號訊息()
        {
            mockEmp.Setup(p => p.GetEmployeeById(It.IsAny<Int32>(), It.IsAny<Int32>())).Returns(() => (new Result() { IsSuccess = false }, new Employees()
            {
                Account = "Test",
                EmployeeID = 1,
            }));
            mockCus.Setup(p => p.GetCustomerById(It.IsAny<string>(), It.IsAny<Int32>())).Returns(() => (new Result(), null));
            mockAuth.Setup(p => p.UpdateAuthCode(It.IsAny<Int32>(), It.IsAny<string>(), It.IsAny<Guid?>(), It.IsAny<Int32>()))
                .Returns(() => (new Result() { IsSuccess = true }, 1));
            mockCache.Setup(p => p.Remove(It.IsAny<string>()));
            mockCache.Setup(p => p.Remove(It.IsAny<string>()));
            var result = AuthService.LogOut("Test");

            Assert.AreEqual(true, result.IsSuccess);
            Assert.AreEqual("Test 登出成功", result.SuccessMsg);
        }

        [TestMethod()]
        public void GetAuth_當查依帳號查到權限_則回傳查權限訊息()
        {
            var mockauth = new Mock<AuthService>(mockAuth.Object, mockEmp.Object, mockCus.Object, mockCache.Object);
            mockauth.Protected().Setup<string>("Encrypt", new object[] { "Test" }).Returns("Test");
            AuthService = mockauth.Object;

            mockAuth.Setup(p => p.GetAuthById(It.IsAny<string>(), It.IsAny<Int32>()))
                    .Returns(() => (new Result() { IsSuccess = true }, new AuthenticationDTO() { AuthenticId = 1, Account = "Test" }));
            
            var result = AuthService.GetAuth("Test");

            Assert.AreEqual(1, result.AuthenticId);
            Assert.AreEqual("Test", result.Account);
        }

        [TestMethod()]
        public void GetAuth_當查依帳號查無權限_則回傳查權限訊息()
        {
            mockAuth.Setup(p => p.GetAuthById(It.IsAny<string>(), It.IsAny<Int32>()))
                    .Returns(() => (new Result() { IsSuccess = false, ErrorMsg = "error" }, null));

            var result = AuthService.GetAuth("Test");

            Assert.AreEqual(false, result.IsSuccess);
            Assert.AreEqual("error", result.ErrorMsg);
        }

        [TestMethod()]
        public void UpdateAuth_當查依權限更新權限成功_則回傳更新權限成功訊息()
        {
            var mockauth = new Mock<AuthService>(mockAuth.Object, mockEmp.Object, mockCus.Object, mockCache.Object);
            mockauth.Protected().Setup<string>("DecryptAES", new object[] { "Test" }).Returns("Test");
            AuthService = mockauth.Object;
            mockAuth.Setup(p => p.UpdateAuth(It.IsAny<Int32>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Int32>()))
                    .Returns(() => (new Result() { IsSuccess = true }, 0));

            var result = AuthService.UpdateAuth(new BLL.PageModel.AuthModel() {
                AuthenticId = 1 ,Account ="Test", Password = "1234"
            });

            Assert.AreEqual(true, result.IsSuccess);
            Assert.AreEqual("Test 更新成功", result.SuccessMsg);
        }

        [TestMethod()]
        public void UpdateAuth_當查依權限更新權限失敗_則回傳更新權限失敗訊息()
        {
            var mockauth = new Mock<AuthService>(mockAuth.Object, mockEmp.Object, mockCus.Object, mockCache.Object);
            mockauth.Protected().Setup<string>("DecryptAES", new object[] { "Test" }).Returns("Test");
            AuthService = mockauth.Object;
            mockAuth.Setup(p => p.UpdateAuth(It.IsAny<Int32>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Int32>()))
                    .Returns(() => (new Result() { IsSuccess = false ,ErrorMsg = "error"}, 0));

            var result = AuthService.UpdateAuth(new BLL.PageModel.AuthModel()
            {
                AuthenticId = 1,
                Account = "Test",
                Password = "1234"
            });

            Assert.AreEqual(false, result.IsSuccess);
            Assert.AreEqual("error", result.ErrorMsg);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            mockEmp = null;
            mockCus = null;
            mockAuth = null;
            mockCache = null;
            AuthService = null;
        }
    }
}

