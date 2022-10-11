using Microsoft.VisualStudio.TestTools.UnitTesting;
using BlazorTipz.ViewModels.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorTipz.ViewModels.User.Tests
{
    [TestClass()]
    public class UserManagerTests
    {
        private UserManager _userManager { get; set; } = new UserManager();

        [TestMethod()]
        public void UserManagerTest()
        {
           
        }

        [TestMethod()]
        public async Task LoginTest()
        {
            //arrange
            string err;
            string token;
            string expected;
            UserViewmodel user = new UserViewmodel();
            user.employmentId = "";
            user.password = "";
            //act
            (err, token) = await _userManager.Login(user);
            //assert


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