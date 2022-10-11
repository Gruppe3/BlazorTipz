using Microsoft.VisualStudio.TestTools.UnitTesting;
using BlazorTipz.ViewModels.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlazorTipzTests.ViewModels.DummyClass;

namespace BlazorTipz.ViewModels.User.Tests
{
    [TestClass()]
    public class UserManagerTests
    {
        private UserManager _userManager { get; set; } = new UserManager( new DummyDBR(), new Components.AuthenticationComponent());

        [TestMethod()]
        public void UserManagerTest()
        {
            _userManager = new UserManager(new DummyDBR(), new Components.AuthenticationComponent());
            Assert.IsNotNull(_userManager);
        }

        [TestMethod()]
        [DataRow("212212", "test1234",true)] // dummy eksisting user
        [DataRow("212212", "test", false)]
        [DataRow("212212", "" , false)]
        [DataRow("710847", "test1234", false)]
        [DataRow("", "test", false)]
        [DataRow("", "", false)]
        public async Task LoginTest(string? id, string? pass, bool good)
        {
            //arrange
            string? err = null;
            string? token = null;
            UserViewmodel user = new UserViewmodel();
            user.employmentId = id;
            user.password = pass;
            //act
            (token, err) = await _userManager.Login(user);
            //assert
            if (good)
            {
                Assert.IsNotNull(token);
                Assert.IsNull(err);
            }
            else
            {
                Assert.IsNull(token);
                Assert.IsNotNull(err);
            }
        }

        [TestMethod()]
        public void registerUserSingelTest()
        {

        }

        [TestMethod()]
        public void registerMultipleTest()
        {

        }

        [TestMethod()]
        public void getRegisterUserListTest()
        {

        }

        [TestMethod()]
        public void stageToRegisterListTest()
        {

        }

        [TestMethod()]
        public void deleteFromRegisterListTest()
        {

        }

        [TestMethod()]
        public void getCurrentUserTest()
        {

        }

        [TestMethod()]
        public void logoutTest()
        {

        }

        [TestMethod()]
        public void getCurrentUserTest1()
        {

        }

        [TestMethod()]
        public void updateCurrentUserTest()
        {

        }

        [TestMethod()]
        public void GetUsersTest()
        {

        }

        [TestMethod()]
        public void updateUsersListTest()
        {

        }

        [TestMethod()]
        public void updateRoleTest()
        {

        }

        [TestMethod()]
        public void getUserTest()
        {

        }

        [TestMethod()]
        public void updateUserTeamTest()
        {

        }

        [TestMethod()]
        public void generatePasswordTest()
        {

        }
    }
}