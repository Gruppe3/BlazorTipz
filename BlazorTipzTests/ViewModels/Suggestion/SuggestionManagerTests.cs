using BlazorTipz.ViewModels.Suggestion;
using BlazorTipzTests.ViewModels.DummyClass;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlazorTipz.ViewModels.Suggestion.Tests
{
    [TestClass()]
    public class SuggestionManagerTests
    {

        [TestMethod()]
        public void SuggestionManagerTest()
        {
            // arrange and act
            DummyDBR dDBR = new DummyDBR();
            SuggestionManager _UnitUnderTest = new SuggestionManager(dDBR, new Models.AppStorage.AppStorage());

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
            SuggestionManager _UnitUnderTest = new SuggestionManager(dDBR, new Models.AppStorage.AppStorage());

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
            testSugg.category = cat;

            if (testCase == 6) { } else { testSugg.StartDate = DateTime.Now.ToLocalTime().ToString("yyyyMMddHHmmss"); }

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
            SuggestionManager _UnitUnderTest = new SuggestionManager(dDBR, new Models.AppStorage.AppStorage());

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
            SuggestionManager _UnitUnderTest = new SuggestionManager(dDBR, new Models.AppStorage.AppStorage());

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
            SuggestionManager _UnitUnderTest = new SuggestionManager(dDBR, new Models.AppStorage.AppStorage());

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
    }
}