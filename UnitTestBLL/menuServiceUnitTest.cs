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
        public void GetMenusByAccount_當依帳號查選單有選單和角色_則回傳查選單和角色訊息()
        {
            object menus = null;
            mockCache.Setup(p => p.TryGetValue("GetMenusTest", out menus)).Returns(true);
            object roles = null;
            mockCache.Setup(p => p.TryGetValue("GetRolesTest", out roles)).Returns(true);
            var mockmenu = new Mock<MenuService>(mockMenu.Object, mockRole.Object, mockCache.Object);
            mockmenu.Protected().Setup("GetSubMenus", new object[] { new MenuDTO(), new List<MenuDTO>()}).Verifiable();
            MenuService = mockmenu.Object;
            mockRole.Setup(p => p.GetRolesByAccount(It.IsAny<string>())).Returns(() => (new Result() { IsSuccess = true }, new List<RoleOfMenuDTO>() {
                new RoleOfMenuDTO() {
                    RoleId = 1,
                    RoleName = "general"
                }}));
            mockMenu.Setup(p => p.GetMenusByAccount(It.IsAny<string>())).Returns(() => (new Result() { IsSuccess = true }, new List<MenuDTO>() {
                new MenuDTO() {
                    ParentID = 0,
                    MenuId = 2,
                    MenuCode = "Person"
                }}));
            List<MenuDTO> menusSet = new List<MenuDTO>() { new MenuDTO() {
                MenuId = 1
            }};
            var entryMock = new Mock<ICacheEntry>();
            mockCache.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(entryMock.Object);
            var result = MenuService.GetMenusByAccount("Test");

            Assert.AreEqual(true, result.rtn.IsSuccess);
            Assert.AreEqual(2, result.menus[0].MenuId);
            Assert.AreEqual(1, result.roles[0].RoleId);
        }

        [TestMethod()]
        public void GetMenusByAccount_當依帳號查選單無選單和角色_則回傳查選單和角色訊息()
        {
            object menus = null;
            mockCache.Setup(p => p.TryGetValue("GetMenusTest", out menus)).Returns(true);
            object roles = null;
            mockCache.Setup(p => p.TryGetValue("GetRolesTest", out roles)).Returns(true);

            mockRole.Setup(p => p.GetRolesByAccount(It.IsAny<string>())).Returns(() => (new Result() { IsSuccess = false }, new List<RoleOfMenuDTO>()));
            mockMenu.Setup(p => p.GetMenusByAccount(It.IsAny<string>())).Returns(() => (new Result() { IsSuccess = true }, new List<MenuDTO>()));

            var result = MenuService.GetMenusByAccount("Test");

            Assert.AreEqual(false, result.rtn.IsSuccess);
            Assert.AreEqual(0, result.menus.Count);
            Assert.AreEqual(0, result.roles.Count);
        }

        [TestMethod()]
        public void GetMenusByAccount_當依帳號查選單查無_則回傳查無訊息()
        {
            object menus = new List<MenuDTO>() { new MenuDTO() {
                MenuId = 1
            }};
            mockCache.Setup(p => p.TryGetValue("GetMenusTest", out menus)).Returns(true);
            object roles = new List<RoleOfMenuDTO>() { new RoleOfMenuDTO() {
                RoleId = 1
            }};
            mockCache.Setup(p => p.TryGetValue("GetRolesTest", out roles)).Returns(true);

            var result = MenuService.GetMenusByAccount("Test");

            Assert.AreEqual(true, result.rtn.IsSuccess);
            Assert.AreEqual(1, result.menus[0].MenuId);
            Assert.AreEqual(1, result.roles[0].RoleId);
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
