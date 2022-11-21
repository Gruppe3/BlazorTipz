using Microsoft.VisualStudio.TestTools.UnitTesting;
using BlazorTipz.ViewModels.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlazorTipzTests.ViewModels.DummyClass;
using BlazorTipz.Data;

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
            UserViewmodel user = new()
            {
                EmploymentId = id,
                Password = pass
            };
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
        public async Task RegisterUserSingelTest()
        {
            //arrange
            string? err;
            string? succ;
            string expected = "succsess";
            UserViewmodel user = new()
            {
                EmploymentId = "123456",
                Name = "TestUser",
                Password = "test1234"
            };

            //act
            (err, succ) = await _userManager.RegisterUserSingel(user);
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
        public async Task RegisterMultipleTest(string? id,string? pass, string? name, int testcase)
        {
            //arrange
            string? err;
            string? succ;
            string expected = "Succsess";
            List<UserViewmodel> users = PrefillGoodUsers();
            UserViewmodel user = new()
            {
                EmploymentId = id,
                Name = name,
                Password = pass
            };
            users.Add(user);
            if (testcase == 1)
            {
                expected = "No one to register";
                List<UserViewmodel>? userst = null;
                _userManager.GetRegisterUserList().Clear();
                //act
                (err, succ) = await _userManager.RegisterMultiple(userst);
                //assert
                Assert.AreEqual(expected, err);
            }else if(testcase == 2 || testcase == 3)
            {
                foreach (UserViewmodel us in users)
                {
                    _userManager.StageToRegisterList(us);
                    
                }
                if (testcase == 2) {
                    (err, succ) = await _userManager.RegisterMultiple(null);
                    Assert.AreEqual(expected, succ); }
                if (testcase == 3) {
                    _userManager.GetRegisterUserList().Clear();
                    (err, succ) = await _userManager.RegisterMultiple(null);
                    Assert.AreNotEqual(expected, succ); }
            }
            //act
            
            (err, succ) = await _userManager.RegisterMultiple(users);

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
        private List<UserViewmodel> PrefillGoodUsers()
        {
            {
                List<UserViewmodel> users = new List<UserViewmodel>();
                for (int i = 0; i < 10; i++)
                {
                    UserViewmodel user = new()
                    {
                        EmploymentId = "3" + i.ToString(),
                        Name = "User3" + i.ToString(),
                        Password = "password3" + i.ToString()
                    };
                    users.Add(user);
                }
                return users;
            }
        }

        [TestMethod()]
        public void GetRegisterUserListTest()
        {
            //act
            List<UserViewmodel> list = _userManager.GetRegisterUserList();
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
        public void StageToRegisterListTest(string? id, string? pass,string? name,int testCase)
        {
            
            //arrange
            string expected1 = "User succsessfully added to list of pepole to register";
            string expected2 = "User in list updated";
            string result = "";
            if (testCase == 6)
            {
                //act
                _userManager.StageToRegisterList(null);
                //assert
                Assert.AreNotEqual(expected1, result);
                Assert.AreNotEqual(expected2, result);
            }
            _userManager.GetRegisterUserList().Clear();
            _userManager.GetRegisterUserList();
            if (testCase == 1) {} 
            else if(testCase == 2) {
                UserViewmodel test = new() // dummy user
                {
                    EmploymentId = "212212",
                    Password = "test1234",
                    Name = "test"
                };
                _userManager.StageToRegisterList(test);
            }
            UserViewmodel user = new()
            {
                EmploymentId = id,
                Password = pass,
                Name = name
            };

            //act
            result = _userManager.StageToRegisterList(user);
            
            //Assert
            if (testCase == 1) { Assert.AreEqual(expected1, result); } 
            else if (testCase == 2) { Assert.AreEqual(expected2, result); }
            else{
                Assert.AreNotEqual(expected1, result);
                Assert.AreNotEqual(expected2, result);
            }
        }

        [TestMethod()]
        public void GetCurrentUserTest()
        {
            //arrenge
            UserViewmodel dummy = new();
            _userManager.CurrentUser = dummy;
            //act
            UserViewmodel res = _userManager.GetCurrentUser();
            //assert
            Assert.IsNotNull(res);
        }

        [TestMethod()]
        public void LogoutTest()
        {
            //arrange
            UserViewmodel dummy = new();
            _userManager.CurrentUser = dummy;
            //act
            _userManager.Logout();
            var res = _userManager.CurrentUser;
            Assert.IsNull(res);
        }

        //can be improved
        [TestMethod()]
        public async Task UpdateCurrentUserTest()
        {
            //arranged
            UserViewmodel user = new();
            _userManager.CurrentUser = user;
            user.EmploymentId = "32";
            user.Name = "hans";
            user.Password = "45";
            user.RepeatPassword = "45";
            //act
            string err = await _userManager.UpdateCurrentUser(user);
            if(err != null)
            {
                Assert.Fail();
            } 
            
        }

        [TestMethod()]
        public async Task GetActiveUsersTest()
        {
            //arrange
            //act
            List<UserViewmodel> testlist = await _userManager.GetActiveUsers();
            int count = testlist.Count;
            if (count <= 0)
            {
                //assert
                Assert.Fail();
            }
        }
        
        [TestMethod()]
        [DataRow("2", true)]
        [DataRow("7", true)]
        [DataRow("", false)]
        [DataRow("11", false)]
        [DataRow("Steve", false)]
        public async Task SearchActiveUsersTest(string empId, bool goodCase)
        {
            //arrange
            UserViewmodel? user;

            //act
            await _userManager.GetActiveUsers();
            user = await _userManager.SearchActiveUsers(empId);

            //assert
            if (goodCase) 
            {
                Assert.IsNotNull(user);
                Assert.AreEqual(empId, user.EmploymentId); 
            }
            else if (!goodCase) { Assert.IsNull(user); }
        }
    
        [TestMethod()]
        public async Task UpdateUsersListTest()
        {
            //arrange/act
            List<UserViewmodel> users = await _userManager.UpdateUsersList();
            int count = users.Count;
            if (count <= 0)
            {
                //assert
                Assert.Fail();
            }
        }
        
        [TestMethod()]
        [DataRow("4", 1, true, true)]
        [DataRow("4", 3, true, false)]
        [DataRow("6", 2, false, true)]
        [DataRow("32", 2, false, false)]
        public async Task UpdateRoleTest(string userId, int roleid, bool upgradeRole, bool goodcase)
        {
            //arrange
            RoleE role = RoleE.User;
            RoleE role2 = RoleE.User;
            RoleE role3 = RoleE.TeamLeader;
            if (roleid ==  3){
                role = RoleE.Admin;
            } else if (roleid == 2){
                role = RoleE.TeamLeader;
            }
            UserViewmodel user = new()
            {
                EmploymentId = userId,
                UserRole = role,
                Name = "hans"
            };
            string? err; 
            //act
            if(roleid == 2){
                err = await _userManager.UpdateUserRole(user, role2, upgradeRole);
            }
            else
            {
                err = await _userManager.UpdateUserRole(user, role3, upgradeRole);
            }
            //assert
            if (goodcase && upgradeRole)
            {
                Assert.IsNull(err);
            }
            else if (goodcase && !upgradeRole)
            {
                Assert.IsNull(err);
            }
            else if (!goodcase && upgradeRole)
            {
                Assert.IsNotNull(err);
            }
            else if (!goodcase && !upgradeRole)
            {
                Assert.IsNotNull(err);
            }
            
            
        }
        

        [TestMethod()]
        [DataRow("3",true)]
        [DataRow("47",false)]
        public async Task GetUserTest(string testID,bool good)
        {
            //arrange
            UserViewmodel? result;

            //act
            result = await _userManager.GetUserById(testID);

            //assert
            if (good)
            {
                Assert.IsNotNull(result);
            }
            else
            {
                Assert.IsNull(result);
            }
        }

        [TestMethod()]
        [DataRow("3","2", true)]
        [DataRow("47", "2", false)]
        public async Task UpdateUserTeamTest(string? id, string? team, bool good)
        {
            //arrrenge
            string? err;
            //act
            err = await _userManager.UpdateUserTeam(id, team);
            //assert
            if (good)
            {
                Assert.IsNull(err);
            }
            else
            {
                Assert.IsNotNull(err);
            }
        }
        
        //could be more complex ;)
        [TestMethod()]
        public void GeneratePasswordTest()
        {
            //test if random
            string pass1 = _userManager.GenerateRandomPassword();
            string pass2 = _userManager.GenerateRandomPassword();
            if (pass1 == pass2)
            {
                Assert.Fail();
            }
        }
    }
}