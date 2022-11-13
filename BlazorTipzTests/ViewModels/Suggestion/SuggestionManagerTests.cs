using BlazorTipz.ViewModels.Suggestion;
using BlazorTipz.ViewModels.Team;
using BlazorTipz.ViewModels.User;
using BlazorTipzTests.ViewModels.DummyClass;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlazorTipz.ViewModels.Suggestion.Tests
{
    [TestClass()]
    public class SuggestionManagerTests
    {
        private IUserManager _UM { get; set; } = new UserManager(new DummyDBR(), new Components.AuthenticationComponent());
        private ITeamManager _TM = new TeamManager(new DummyDBR(), new UserManager(new DummyDBR(), new Components.AuthenticationComponent()));

        [TestMethod()]
        public void SuggestionManagerTest()
        {
            // arrange and act
            DummyDBR dDBR = new DummyDBR();
            SuggestionManager _UnitUnderTest = new SuggestionManager(dDBR, new Models.AppStorage.AppStorage(), _UM, _TM);

            // assert
            Assert.IsNotNull(_UnitUnderTest);
        }

        [TestMethod()]
        [DataRow("", "", "", "", 1)]    // Setter alt til null
        [DataRow("", "", "", "", 2)]
        [DataRow("", "Bla bla bla", "1", "user2", 2)]
        [DataRow("Bakery explosion", "", "1", "user2", 3)]
        [DataRow("Something wrong", "Oh no no", "", "user2", 4)]
        [DataRow("Missing files", "Where are they?", "1", "", 5)]
        [DataRow("Door gone", "It do be gone", "1", "user2", 6)]
        [DataRow("I'm sad", "Lost my teddy in the door", "1", "user2", 7)]
        public async Task SaveSuggestionTest(string Title, string Description, string OwnerTeam, string Creator, int testCase)
        {
            // arrange
            DummyDBR dDBR = new DummyDBR();
            SuggestionManager _UnitUnderTest = new SuggestionManager(dDBR, new Models.AppStorage.AppStorage(), _UM, _TM);

            string error1 = "No supplied suggestion";
            string error2 = "No supplied title";
            string error3 = "No supplied description";
            string error4 = "No supplied owner";
            string error5 = "No supplied creator";
            string error6 = "No supplied start date";
            string error7 = "Program failure";

            SuggViewmodel testSugg = new SuggViewmodel();
            Category cat = new Category();
            cat.Name = "HMS";

            testSugg.Title = Title;
            testSugg.Description = Description;
            testSugg.OwnerTeam = OwnerTeam;
            testSugg.Creator = Creator;
            testSugg.Category = cat;

            if (testCase == 6) { } else { testSugg.StartDate = DateTime.Now.ToLocalTime(); }

            string? testResult;

            // act
            if (testCase == 1) { testSugg = null; }
            testResult = await _UnitUnderTest.saveSuggestion(testSugg);

            // assert
            if (testCase == 1)
            {
                Assert.AreEqual(error1, testResult);
            }
            else if (testCase == 2)
            {
                Assert.AreEqual(error2, testResult);
            }
            else if (testCase == 3)
            {
                Assert.AreEqual(error3, testResult);
            }
            else if (testCase == 4)
            {
                Assert.AreEqual(error4, testResult);
            }
            else if (testCase == 5)
            {
                Assert.AreEqual(error5, testResult);
            }
            else if (testCase == 6)
            {
                Assert.AreEqual(error6, testResult);
            }
            else if (testCase == 7)
            {
                Assert.IsNull(testResult);
                Assert.AreNotEqual(error7, testResult);
            }
        }

        [TestMethod()]
        public void GetCategoriesTest()
        {
            // arrange
            DummyDBR dDBR = new DummyDBR();
            SuggestionManager _UnitUnderTest = new SuggestionManager(dDBR, new Models.AppStorage.AppStorage(), _UM, _TM);

            // act
            List<Category> testResult = _UnitUnderTest.GetCategories();

            // assert
            Assert.IsNotNull(testResult);
            if (testResult.Count <= 0)
            {
                Assert.Fail("No categories found");
            }
        }

        [TestMethod()]
        [DataRow("1", true)]
        [DataRow("2", true)]
        [DataRow("3", false)]
        [DataRow("4", false)]
        [DataRow("", false)]
        [DataRow("-44", false)]
        public async Task GetSuggestionsOfTeamTest(string teamId, bool goodCase)
        {
            // arrange
            DummyDBR dDBR = new DummyDBR();
            SuggestionManager _UnitUnderTest = new SuggestionManager(dDBR, new Models.AppStorage.AppStorage(), _UM, _TM);

            // act
            List<SuggViewmodel> testResult = await _UnitUnderTest.GetSuggestionsOfTeam(teamId);

            // assert
            if (goodCase)
            {
                if (testResult.Count <= 0) { Assert.Fail("Something went wrong"); }
                foreach (SuggViewmodel sugg in testResult)
                {
                    Assert.AreEqual(teamId, sugg.OwnerTeam);
                }
            }
            else if (!goodCase)
            {
                Assert.AreEqual(0, testResult.Count);
            }

        }

        [TestMethod()]
        [DataRow("1", true)]
        [DataRow("2", true)]
        [DataRow("", false)]
        public async Task GetSuggestionsOfUserTest(string userId, bool goodCase)
        {
            // arrange
            DummyDBR dDBR = new DummyDBR();
            SuggestionManager _UnitUnderTest = new SuggestionManager(dDBR, new Models.AppStorage.AppStorage(), _UM, _TM);

            // act
            List<SuggViewmodel> testResult = await _UnitUnderTest.GetSuggestionsOfUser(userId);

            // assert
            if (goodCase)
            {
                if (testResult.Count <= 0) { Assert.Fail("Something went wrong"); }
                foreach (SuggViewmodel sugg in testResult)
                {
                    Assert.AreEqual(userId, sugg.Creator);
                }
            }
            else if (!goodCase)
            {
                Assert.AreEqual(0, testResult.Count);
            }
        }

        [TestMethod()]
        [DataRow("1", true)]
        [DataRow("35", false)]
        public async Task GetSuggestionTest(string testId,bool goodCase)
        {
            // arrange
            DummyDBR dDBR = new DummyDBR();
            SuggestionManager _UnitUnderTest = new SuggestionManager(dDBR, new Models.AppStorage.AppStorage(), _UM, _TM);
            
            // act
            SuggViewmodel testResult = await _UnitUnderTest.GetSuggestion(testId);

            // assert
            if (goodCase)
            {
                Assert.IsNotNull(testResult);
            }
            else if (!goodCase)
            {
                Assert.IsNull(testResult);
            }
        }

        [TestMethod()]
        [DataRow("1",true)]
        [DataRow("39", false)]
        public async Task UpdateSuggestionTest(string testID, bool goodcase)
        {
            // arrange
            DummyDBR dDBR = new DummyDBR();
            SuggestionManager _UnitUnderTest = new SuggestionManager(dDBR, new Models.AppStorage.AppStorage(), _UM, _TM);
            string? err = null;
            SuggViewmodel testSugg = new()
            {
                Id = testID,
                Title = "Test",
                Description = "Test",
                OwnerTeam = "1",
                Creator = "1",
                StartDate = DateTime.Now.ToLocalTime()
            };
            Category cat = new Category();
            cat.Name = "HMS";
            testSugg.Category = cat;
            UserViewmodel user = new UserViewmodel();
            user.employmentId = "1";
            
            //act
            err = await _UnitUnderTest.UpdateSuggestion(testSugg,user);
            
            //Assert
            if (goodcase)
            {
                Assert.IsNull(err);
            }
            else
            {
                Assert.IsNotNull(err);
            }
        }

        [TestMethod()]
        [DataRow("000000", "1", "AbcdefghijKLmnoPqRSTuvWxyZæøå.,-?!1234567890[](){}++", true)]
        [DataRow("000000", "1", "¬ Q ã ÎÌâTØ   ¬Ð   Û&#E    ¤  #   é 5n¶eó y ó @¦  §¹ = XÕ{Ù9ôkÀ°", true)]
        [DataRow("000000", "1", "ㄴ ㄷ ㄹ ㅁ ㅂ ㅅ ㅇ ㅈ ", false)]
        public async Task SaveCommentTest(string empId, string sugId, string comment, bool goodcase)
        {
            // arrange
            DummyDBR dDBR = new DummyDBR();
            SuggestionManager _UnitUnderTest = new SuggestionManager(dDBR, new Models.AppStorage.AppStorage(), _UM, _TM);
            string goodres = "Kommentar lagret";
            string badres = "Kommentar inneholder ukjente tegn";
            
            CommentViewmodel testComment = new()
            {
                EmpId = empId,
                SugId = sugId,
                Comment = comment,
                TimeStamp = DateTime.Now.ToLocalTime()
            };

            //act
            string result = await _UnitUnderTest.SaveComment(testComment);
            
            //Assert
            if (goodcase)
            {
                Assert.AreEqual(goodres, result);
            }
            else
            {
                Assert.AreEqual(badres, result);
            }
        }

        //[TestMethod()]
        //[DataRow(1, "AbcdefghijKLmnoPqRSTuvWxyZæøå.,-?!1234567890[](){}++", true)]
        //[DataRow(0, "¬ Q ã ÎÌâTØ   ¬Ð   Û&#E    ¤  #   é 5n¶eó y ó @¦  §¹ = XÕ{Ù9ôkÀ°", true)]
        //[DataRow(1, "ㄴ ㄷ ㄹ ㅁ ㅂ ㅅ ㅇ ㅈ ", false)]
        //public async Task UpdateCommentTest(bool active, string comment, bool goodcase)
        //{
        //    // arrange
        //    DummyDBR dDBR = new DummyDBR();
        //    SuggestionManager _UnitUnderTest = new SuggestionManager(dDBR, new Models.AppStorage.AppStorage(), _UM, _TM);
        //    string goodres = "Kommentar oppdatert";
        //    string badres = "Kommentar inneholder ukjente tegn";

        //    CommentViewmodel testComment = new()
        //    {
        //        EmpId = "000000",
        //        SugId = "1",
        //        Comment = comment,
        //        IsActive = active,
        //        TimeStamp = DateTime.Now.ToLocalTime()
        //    };

        //    //act
        //    await _UnitUnderTest.SaveComment(testComment);
        //    string result = await _UnitUnderTest.UpdateComment(testComment);

        //    //Assert
        //    if (goodcase)
        //    {
        //        Assert.AreEqual(goodres, result);
        //    }
        //    else
        //    {
        //        Assert.AreEqual(badres, result);
        //    }
        //}
    }
}