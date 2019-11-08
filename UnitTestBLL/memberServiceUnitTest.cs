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
        static Mock<RoleRepository> mockRole = null;
        static Mock<CustomerModel> mockCusM = null;
        static Mock<EmployeeModel> mockEmpM = null;
        static string connect = string.Empty;
        static int timeout = 0;

        [ClassInitialize]
        public static void memberServiceUnitTestInitialize(TestContext testContext)
        {
            mockEmp = new Mock<EmployeeRepository>(new object[] { connect, timeout });
            mockCus = new Mock<CustomerRepository>(new object[] { connect, timeout });
            mockRole = new Mock<RoleRepository>(new object[] { connect, timeout });
            mockCusM = new Mock<CustomerModel>();
            mockEmpM = new Mock<EmployeeModel>();
            MemberService = new MemberService(mockEmp.Object, mockCus.Object, mockRole.Object, mockEmpM.Object, mockCusM.Object);
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

        [TestMethod()]
        public void IsExistMemberByAccount_當是否有會員的員工及會員不存在_則回傳無帳號訊息()
        {
            mockEmp.Setup(p => p.GetEmployeeByAccount(It.IsAny<string>(), It.IsAny<int>())).Returns(() => (new Result(), null));
            mockCus.Setup(p => p.GetCustomerByAccount(It.IsAny<string>(), It.IsAny<int>())).Returns(() => (new Result(), null));

            var result = MemberService.IsExistMemberByAccount("A");

            Assert.AreEqual(false, result.rtn.IsSuccess);
            Assert.AreEqual("查無此帳號 A，請確認", result.rtn.ErrorMsg);
        }

        [TestMethod()]
        public void IsExistMemberByAccount_當是否有會員的員工錯誤_則回傳錯誤訊息()
        {
            mockEmp.Setup(p => p.GetEmployeeByAccount(It.IsAny<string>(), It.IsAny<int>())).Returns(() => (new Result() { IsSuccess = false, ErrorMsg = "Error" }, null));
            mockCus.Setup(p => p.GetCustomerByAccount(It.IsAny<string>(), It.IsAny<int>())).Returns(() => (new Result(), new Customers()));

            var result = MemberService.IsExistMemberByAccount("A");

            Assert.AreEqual(false, result.rtn.IsSuccess);
            Assert.AreEqual("Error", result.rtn.ErrorMsg);
        }

        [TestMethod()]
        public void IsExistMemberByAccount_當是否有會員的客戶錯誤_則回傳錯誤訊息()
        {
            mockEmp.Setup(p => p.GetEmployeeByAccount(It.IsAny<string>(), It.IsAny<int>())).Returns(() => (new Result() { IsSuccess = true}, new Employees()));
            mockCus.Setup(p => p.GetCustomerByAccount(It.IsAny<string>(), It.IsAny<int>())).Returns(() => (new Result() { IsSuccess = false, ErrorMsg = "Error" },null));

            var result = MemberService.IsExistMemberByAccount("A");

            Assert.AreEqual(false, result.rtn.IsSuccess);
            Assert.AreEqual("Error", result.rtn.ErrorMsg);
        }

        [TestMethod()]
        public void IsExistMemberByAccount_當是否有會員的員工正確_則回傳員工訊息()
        {
            mockEmp.Setup(p => p.GetEmployeeByAccount(It.IsAny<string>(), It.IsAny<int>())).Returns(() => (new Result() { IsSuccess = true }, 
            new Employees() { 
                EmployeeID = 1,
                FirstName = "T",
                LastName = "est"
            }));
            mockCus.Setup(p => p.GetCustomerByAccount(It.IsAny<string>(), It.IsAny<int>())).Returns(() => (new Result() { IsSuccess = true } , null));

            var result = MemberService.IsExistMemberByAccount("A");

            Assert.AreEqual(true, result.rtn.IsSuccess);
            Assert.AreEqual(1, result.EmpId);
            Assert.AreEqual("1", result.Id);
            Assert.AreEqual("Test", result.Name);
        }

        [TestMethod()]
        public void IsExistMemberByAccount_當是否有會員的客戶正確_則回傳客戶訊息()
        {
            mockEmp.Setup(p => p.GetEmployeeByAccount(It.IsAny<string>(), It.IsAny<int>())).Returns(() => (new Result() { IsSuccess = true }, null));
            mockCus.Setup(p => p.GetCustomerByAccount(It.IsAny<string>(), It.IsAny<int>())).Returns(() => (new Result() { IsSuccess = true }, 
            new Customers() { 
                CustomerID = "1",
                CompanyName = "Test"
            }));

            var result = MemberService.IsExistMemberByAccount("A");

            Assert.AreEqual(true, result.rtn.IsSuccess);
            Assert.AreEqual("1", result.CusId);
            Assert.AreEqual("1", result.Id);
            Assert.AreEqual("Test", result.Name);
        }

        [TestMethod()]
        public void IsExistMemberById_當是否有會員的員工及會員不存在_則回傳無帳號訊息()
        {
            mockEmp.Setup(p => p.GetEmployeeById(It.IsAny<int>(), It.IsAny<int>())).Returns(() => (new Result(), null));
            mockCus.Setup(p => p.GetCustomerById(It.IsAny<string>(), It.IsAny<int>())).Returns(() => (new Result(), null));

            var result = MemberService.IsExistMemberById("A");

            Assert.AreEqual(false, result.rtn.IsSuccess);
            Assert.AreEqual("查無此帳號 A，請確認", result.rtn.ErrorMsg);
        }

        [TestMethod()]
        public void IsExistMemberById_當是否有會員的員工錯誤_則回傳錯誤訊息()
        {
            mockEmp.Setup(p => p.GetEmployeeById(It.IsAny<int>(), It.IsAny<int>())).Returns(() => (new Result() { IsSuccess = false, ErrorMsg = "Error" }, null));
            mockCus.Setup(p => p.GetCustomerById(It.IsAny<string>(), It.IsAny<int>())).Returns(() => (new Result(), new Customers()));

            var result = MemberService.IsExistMemberById("A");

            Assert.AreEqual(false, result.rtn.IsSuccess);
            Assert.AreEqual("Error", result.rtn.ErrorMsg);
        }

        [TestMethod()]
        public void IsExistMemberById_當是否有會員的客戶錯誤_則回傳錯誤訊息()
        {
            mockEmp.Setup(p => p.GetEmployeeById(It.IsAny<int>(), It.IsAny<int>())).Returns(() => (new Result() { IsSuccess = true }, new Employees()));
            mockCus.Setup(p => p.GetCustomerById(It.IsAny<string>(), It.IsAny<int>())).Returns(() => (new Result() { IsSuccess = false, ErrorMsg = "Error" }, null));

            var result = MemberService.IsExistMemberById("A");

            Assert.AreEqual(false, result.rtn.IsSuccess);
            Assert.AreEqual("Error", result.rtn.ErrorMsg);
        }

        [TestMethod()]
        public void IsExistMemberById_當是否有會員的員工正確_則回傳員工訊息()
        {
            mockEmp.Setup(p => p.GetEmployeeById(It.IsAny<int>(), It.IsAny<int>())).Returns(() => (new Result() { IsSuccess = true },
            new Employees()
            {
                EmployeeID = 1,
                FirstName = "T",
                LastName = "est"
            }));
            mockCus.Setup(p => p.GetCustomerById(It.IsAny<string>(), It.IsAny<int>())).Returns(() => (new Result() { IsSuccess = true }, null));

            var result = MemberService.IsExistMemberById("A");

            Assert.AreEqual(true, result.rtn.IsSuccess);
            Assert.AreEqual(1, result.EmpId);
            Assert.AreEqual(string.Empty, result.CusId);
        }

        [TestMethod()]
        public void IsExistMemberById_當是否有會員的客戶正確_則回傳客戶訊息()
        {
            mockEmp.Setup(p => p.GetEmployeeById(It.IsAny<int>(), It.IsAny<int>())).Returns(() => (new Result() { IsSuccess = true }, null));
            mockCus.Setup(p => p.GetCustomerById(It.IsAny<string>(), It.IsAny<int>())).Returns(() => (new Result() { IsSuccess = true },
            new Customers()
            {
                CustomerID = "1",
                CompanyName = "Test"
            }));

            var result = MemberService.IsExistMemberById("A");

            Assert.AreEqual(true, result.rtn.IsSuccess);
            Assert.AreEqual(-1, result.EmpId);
            Assert.AreEqual("1", result.CusId);
        }

        [TestMethod()]
        public void GetRolesByAccount_當依帳號取得角色資訊正確_則回傳角色訊息()
        {
            mockRole.Setup(p => p.GetRolesByAccount(It.IsAny<string>())).Returns(() => (new Result() { IsSuccess = true }, null));

            var result = MemberService.GetRolesByAccount("A");

            Assert.AreEqual(true, result.rtn.IsSuccess);
        }

        [TestMethod()]
        public void GetCalculateAmounts_當依帳號為員工_則回傳員工金額及折扣()
        {
            mockEmpM.Setup(p => p.CalculateAmounts(It.IsAny<int>())).Returns(() => (800,0.8));

            var result = MemberService.GetCalculateAmounts("1",1000);

            Assert.AreEqual(800, result.TotalAmount);
            Assert.AreEqual(0.8, result.Discount);
        }

        [TestMethod()]
        public void GetCalculateAmounts_當依帳號為客戶_則回傳客戶金額及折扣()
        {
            mockCusM.Setup(p => p.CalculateAmounts(It.IsAny<int>())).Returns(() => (950, 0.95));
            var result = MemberService.GetCalculateAmounts("A", 1000);

            Assert.AreEqual(950, result.TotalAmount);
            Assert.AreEqual(0.95, result.Discount);
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
