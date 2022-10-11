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
        [DataRow("1", "password1",true)] // dummy eksisting user
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
            //begin
        }

        [TestMethod()]
        public void registerMultipleTest()
        {

        }

        [TestMethod()]
        public void getRegisterUserListTest()
        {
            //act
            List<UserViewmodel> list = _userManager.getRegisterUserList();
            //assert
            Assert.IsNotNull(list);
        }

        [TestMethod()]
        [DataRow("212212", "tesss","tehjfa", 1)] 
        [DataRow("212212", "tesss", "tehjfa", 2)] //should update dummy
        [DataRow(null, "tesss", "tehjfa", 3)]
        [DataRow("212212", "", "tehjfa", 4)]
        [DataRow("212212", "tesss", "", 5)]
        [DataRow( "","","", 6)]
        public void stageToRegisterListTest(string? id, string? pass,string? name,int testCase)
        {
            
            //arrange
            string expected1 = "User succsessfully added to list of pepole to register";
            string expected2 = "User in list updated";
            string result = "";
            if (testCase == 6)
            {
                //act
                _userManager.stageToRegisterList(null);
                //assert
                Assert.AreNotEqual(expected1, result);
                Assert.AreNotEqual(expected2, result);
            }
            _userManager.getRegisterUserList().Clear();
            _userManager.getRegisterUserList();
            if (testCase == 1) {} 
            else if(testCase == 2) {
                UserViewmodel test = new UserViewmodel(); // dummy user
                test.employmentId = "212212";
                test.password = "test1234";
                test.name = "test";
                _userManager.stageToRegisterList(test);
            }
            UserViewmodel user = new UserViewmodel();
            user.employmentId = id;
            user.password = pass;
            user.name = name;

            //act
            result = _userManager.stageToRegisterList(user);
            
            //Assert
            if (testCase == 1) { Assert.AreEqual(expected1, result); } 
            else if (testCase == 2) { Assert.AreEqual(expected2, result); }
            else{
                Assert.AreNotEqual(expected1, result);
                Assert.AreNotEqual(expected2, result);
            }
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