using Base;
using BLL.Model;
using DAL.DTOModel;
using DAL.Repository;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTestBLL
{
    [TestClass]
    public class menuServiceUnitTest
    {
        static MenuService MenuService = null;
        static Mock<MenuRepository> mockMenu = null;
        static Mock<RoleRepository> mockRole = null;
        static Mock<IMemoryCache> mockCache = null;
        static string connect = string.Empty;
        static int timeout = 0;

        [ClassInitialize]
        public static void memberServiceUnitTestInitialize(TestContext testContext)
        {
            mockCache = new Mock<IMemoryCache>();
            mockMenu = new Mock<MenuRepository>(new object[] { connect, timeout });
            mockRole = new Mock<RoleRepository>(new object[] { connect, timeout });
            MenuService = new MenuService(mockMenu.Object, mockRole.Object, mockCache.Object);
        }

        [TestMethod()]
        public void GetMenusByAccount_當依帳號查選單_則回傳查選單訊息()
        {
            var mockmenu = new Mock<MenuService>(mockMenu.Object, mockRole.Object, mockCache.Object);
            mockmenu.Protected().Setup<string>("FunGetSubMenus", new object[] { new MenuDTO(), new List<MenuDTO>()}).Returns("Test");
            MenuService = mockmenu.Object;

            //mockEmp.Setup(p => p.GetEmployeeByAccount(It.IsAny<string>(), 10)).Returns(() => (new Result(), null));
            //mockCus.Setup(p => p.GetCustomerByAccount(It.IsAny<string>(), 10)).Returns(() => (new Result(), null));

            //var result = AuthService.LogIn("Test", "1234");
            //Assert.AreEqual("查無此帳號 Test，請確認", result.rtn.ErrorMsg);
        }

        [TestMethod()]
        public void GetMenusByAccount_當依帳號查選單查無_則回傳查無訊息()
        {
            IEnumerable<MenuDTO> menus = new List<MenuDTO>() { new MenuDTO() {
                MenuId = 1
            }};
            mockCache.Setup(p => p.TryGetValue<IEnumerable<MenuDTO>>(It.IsAny<object>(), out menus)).Verifiable();
            List<RoleOfMenuDTO> roles = new List<RoleOfMenuDTO>() { new RoleOfMenuDTO() {
                RoleId = 1
            }};
            mockCache.Setup(p => p.TryGetValue<List<RoleOfMenuDTO>>(It.IsAny<bool>(), out roles)).Returns(true);
            var mockmenu = new Mock<MenuService>(mockMenu.Object, mockRole.Object, mockCache.Object);
            mockmenu.Protected().Setup("FunGetSubMenus", new object[] { new MenuDTO(), new List<MenuDTO>() }).Verifiable();
            MenuService = mockmenu.Object;

            mockRole.Setup(p => p.GetRolesByAccount(It.IsAny<string>())).Returns(() => (new Result() { IsSuccess = true }, new List<RoleOfMenuDTO>() {
                new RoleOfMenuDTO() {
                    RoleName = "general"
                }
            }));
            mockMenu.Setup(p => p.GetMenusByAccount(It.IsAny<string>())).Returns(() => (new Result() { IsSuccess = true }, new List<MenuDTO>() {
                new MenuDTO() {
                    ParentID = 0,
                    MenuCode = "Person"
                }
            }));
            mockCache.Setup(p => p.Set(It.IsAny<bool>(), It.IsAny<IEnumerable<MenuDTO>>(), It.IsAny<TimeSpan>())).Verifiable();

            var result = MenuService.GetMenusByAccount("Test");
            Assert.AreEqual(true, result.rtn.IsSuccess);
            Assert.AreEqual(true, result.rtn.IsSuccess);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            mockMenu = null;
            mockRole = null;
            mockCache = null;
            MenuService = null;
        }
    }
}
