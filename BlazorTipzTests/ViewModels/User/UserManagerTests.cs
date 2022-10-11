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
        public async Task registerUserSingelTest()
        {
            //arrange
            string? err;
            string? succ;
            string expected = "succsess";
            UserViewmodel user = new UserViewmodel();
            user.employmentId = "123456";
            user.name = "TestUser";
            user.password = "test1234";

            //act
            (err, succ) = await _userManager.registerUserSingel(user);
            //assert
            Assert.AreEqual(expected, succ);
        }

        [TestMethod()]
        [DataRow("", "","" ,1)]
        [DataRow("123456", "TestUser", "test1234", 2)]
        [DataRow("15","","",3)]
        [DataRow("123556", "TestUser", "test1234", 4)]
        [DataRow("1", "TestUser", "test1234", 5)]
        [DataRow("123557", "", "test1234", 5)]
        [DataRow("123558", "TestUser", "", 5)]
        [DataRow("", "TestUser", "test1234", 5)]
        [DataRow(null, "TestUser", "test1234", 5)]
        [DataRow("123561", null, "test1234", 5)]
        [DataRow("123561", "heyo", null, 5)]
        public async Task registerMultipleTest(string? id,string? pass, string? name, int testcase)
        {
            //arrange
            string? err;
            string? succ;
            string expected = "Succsess";
            List<UserViewmodel> users = prefillGoodUsers();
            UserViewmodel user = new UserViewmodel();
            user.employmentId = id;
            user.name = name;
            user.password = pass;
            users.Add(user);
            if (testcase == 1)
            {
                expected = "No one to register";
                List<UserViewmodel>? userst = null;
                _userManager.getRegisterUserList().Clear();
                //act
                (err, succ) = await _userManager.registerMultiple(userst);
                //assert
                Assert.AreEqual(expected, err);
            }else if(testcase == 2 || testcase == 3)
            {
                foreach (UserViewmodel us in users)
                {
                    _userManager.stageToRegisterList(us);
                    
                }
                if (testcase == 2) {
                    (err, succ) = await _userManager.registerMultiple(null);
                    Assert.AreEqual(expected, succ); }
                if (testcase == 3) {
                    _userManager.getRegisterUserList().Clear();
                    (err, succ) = await _userManager.registerMultiple(null);
                    Assert.AreNotEqual(expected, succ); }
            }
            //act
            
            (err, succ) = await _userManager.registerMultiple(users);

            //assert
            if (testcase == 2) {  }
            else if (testcase == 3) {  }
            else if (testcase == 4) { Assert.AreEqual(expected, succ); }
            else if (testcase == 1) { }
            else
            {
                Assert.IsNotNull(err);
            }
           

        }
        private List<UserViewmodel> prefillGoodUsers()
        {
            {
                List<UserViewmodel> users = new List<UserViewmodel>();
                for (int i = 0; i < 10; i++)
                {
                    UserViewmodel user = new UserViewmodel();
                    user.employmentId ="3"+ i.ToString();
                    user.name = "User3" + i.ToString();
                    user.password = "password3" + i.ToString();
                    users.Add(user);
                }
                return users;
            }
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
        public void getCurrentUserTest()
        {
            //arrenge
            UserViewmodel dummy = new UserViewmodel();
            _userManager.CurrentUser = dummy;
            //act
            UserViewmodel res = _userManager.getCurrentUser();
            //assert
            Assert.IsNotNull(res);
        }

        [TestMethod()]
        public void logoutTest()
        {
            //arrange
            UserViewmodel dummy = new UserViewmodel();
            _userManager.CurrentUser = dummy;
            //act
            _userManager.logout();
            var res = _userManager.CurrentUser;
            Assert.IsNull(res);
        }

        //can be improved
        [TestMethod()]
        public async Task updateCurrentUserTest()
        {
            //arranged
            UserViewmodel user = new UserViewmodel();
            _userManager.CurrentUser = user;
            user.employmentId = "32";
            user.name = "hans";
            user.password = "45";
            user.RepeatPassword = "45";
            //act
            string err = await _userManager.updateCurrentUser(user);
            if(err != null)
            {
                Assert.Fail();
            } 
            
        }

        [TestMethod()]
        public void GetUsersTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void updateUsersListTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void updateRoleTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void getUserTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void updateUserTeamTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void generatePasswordTest()
        {
            Assert.Fail();
        }
    }
}