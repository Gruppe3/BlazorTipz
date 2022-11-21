using BlazorTipz.ViewModels.Team;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BlazorTipz.ViewModels.User;
using BlazorTipzTests.ViewModels.DummyClass;

namespace BlazorTipz.ViewModels.Team.Tests
{
    [TestClass()]
    public class TeamManagerTests
    {
        [TestMethod()]
        public void TeamManagerTest()
        {
            DummyDBR dDBR = new();
            TeamManager _UnitUnderTest = new(dDBR, new UserManager(dDBR, new Components.AuthenticationComponent()));
            Assert.IsNotNull(_UnitUnderTest);
        }

        [TestMethod()]
        public async Task GetTeamsTest()
        {
            //arrange
            DummyDBR dDBR = new();
            TeamManager _UnitUnderTest = new(dDBR, new UserManager(dDBR, new Components.AuthenticationComponent()));
            List<TeamViewmodel> teamList;
            List<TeamViewmodel>? teams;

            //act
            teamList = await _UnitUnderTest.GetActiveTeams();
            teams = _UnitUnderTest.ActiveTeams;

            //assert
            Assert.AreEqual(teamList, teams);
            foreach (TeamViewmodel team in teamList)
            {
                Assert.IsTrue(
                    team.TeamId != "" &&
                    team.TeamName != "" &&
                    team.TeamLeaderId != ""
                    );
            }
        }

        [TestMethod()]
        public async Task UpdateTeamsListTest()
        {
            //arrange
            DummyDBR dDBR = new();
            TeamManager _UnitUnderTest = new(dDBR, new UserManager(dDBR, new Components.AuthenticationComponent()));
            List<TeamViewmodel> teamList;
            List<TeamViewmodel>? teams;

            //act
            teamList = await _UnitUnderTest.UpdateTeamsList();
            teams = _UnitUnderTest.ActiveTeams;

            //assert
            Assert.AreEqual(teamList, teams);
            foreach (TeamViewmodel team in teamList)
            {
                Assert.IsTrue(
                    team.TeamId != "" &&
                    team.TeamName != "" &&
                    team.TeamLeaderId != ""
                    );
            }
        }

        [TestMethod()]
        [DataRow("0", true)]
        [DataRow("1", true)]
        [DataRow("2", true)]
        [DataRow("556", false)]
        [DataRow("xx", false)]
        public async Task GetTeamTest(string teamId, bool good)
        {
            //arrange
            DummyDBR dDBR = new();
            TeamManager _UnitUnderTest = new(dDBR, new UserManager(dDBR, new Components.AuthenticationComponent()));
            TeamViewmodel result;

            //act
            result = await _UnitUnderTest.GetTeamById(teamId);

            //assert
            if (good)
            {
                Assert.AreEqual(result.TeamId, teamId);
            }
            else //if not good
            {
                Assert.IsTrue(result.TeamId == "");
            }

        }

        [TestMethod()]
        public async Task GetInactiveTeamsTest()
        {
            //arrange
            DummyDBR dDBR = new();
            TeamManager _UnitUnderTest = new(dDBR, new UserManager(dDBR, new Components.AuthenticationComponent()));
            List<TeamViewmodel> inactiveTeams;
            List<TeamViewmodel> activeTeams;

            //act
            inactiveTeams = await _UnitUnderTest.GetInactiveTeams();
            activeTeams = await _UnitUnderTest.GetActiveTeams();

            //assert

            Assert.AreNotEqual(inactiveTeams, activeTeams);
            foreach (TeamViewmodel team in inactiveTeams)
            {
                Assert.IsTrue(
                    team.TeamId != "" &&
                    team.TeamName != "" &&
                    team.TeamLeaderId != ""
                    );
            }
        }

        [TestMethod()]
        [DataRow("0", "TestTeam0", "0", true)]
        [DataRow("1", "TestTeam1", "1", true)]
        [DataRow("7", "TestTeam17", "8", false)]
        [DataRow("15", "TestTeam66", "3", false)]
        public async Task UpdateTeamTest(string id, string name, string teamLeader, bool good)
        {
            //arrange
            DummyDBR dDBR = new();
            TeamManager _UnitUnderTest = new(dDBR, new UserManager(dDBR, new Components.AuthenticationComponent()));
            TeamViewmodel? teamFromList;
            TeamViewmodel testTeam = new();
            testTeam.TeamId = id;
            testTeam.TeamName = name;
            testTeam.TeamLeaderId = teamLeader;

            //act
            await _UnitUnderTest.UpdateSingleTeam(testTeam);
            teamFromList = _UnitUnderTest.ActiveTeams.Where(t => t.TeamId == testTeam.TeamId).FirstOrDefault();

            //assert
            if (good)
            {
                Assert.AreEqual(testTeam.TeamId, teamFromList?.TeamId);
                Assert.AreEqual(testTeam.TeamName, teamFromList?.TeamName);
                Assert.AreEqual(testTeam.TeamLeaderId, teamFromList?.TeamLeaderId);
            }
            else
            {
                Assert.AreNotEqual(testTeam.TeamId, teamFromList?.TeamId);
                Assert.AreNotEqual(testTeam.TeamName, teamFromList?.TeamName);
                Assert.AreNotEqual(testTeam.TeamLeaderId, teamFromList?.TeamLeaderId);
            }
        }

        [TestMethod()]  //BigBoyFunn
        [DataRow("", "", 1)]
        [DataRow("", "", 2)]
        [DataRow("Team11", "", 3)]
        [DataRow("NewTeam11", "", 3)]
        [DataRow("Team12", "-1", 4)]
        [DataRow("Team13", "100", 4)]
        [DataRow("Team11", "4", 5)]
        [DataRow("TestTeam1", "3", 5)]
        [DataRow("GamerTeam", "2", 5)]
        [DataRow("Electrical", "7", 5)]
        public async Task CreateTeamTest(string name, string leader, int testCase)
        {
            //arrange
            DummyDBR dDBR = new();
            TeamManager _UnitUnderTest = new(dDBR, new UserManager(dDBR, new Components.AuthenticationComponent()));

            TeamViewmodel? testResult;
            TeamViewmodel? testTeam = new()
            {
                TeamName = name,
                TeamLeaderId = leader
            };

            string error1 = "Team is null";
            string error2 = "Team name is empty";
            string error3 = "No team leader chosen";
            string error4 = "Team leader not found";
            string error5 = "Team creation error";
            string? err;

            //act
            if (testCase == 1) { testTeam = null; }
            (testResult, err) = await _UnitUnderTest.CreateNewTeam(testTeam);

            //assert
            if (testCase == 1)
            {
                Assert.AreEqual(testResult, null);
                Assert.AreEqual(error1, err);
            }
            else if (testCase == 2)
            {
                Assert.AreEqual(testResult, null);
                Assert.AreEqual(error2, err);
            }
            else if (testCase == 3)
            {
                Assert.AreEqual(testResult, null);
                Assert.AreEqual(error3, err);
            }
            else if (testCase == 4)
            {
                Assert.AreEqual(testResult, null);
                Assert.AreEqual(error4, err);
            }
            else if (testCase == 5)
            {
                Assert.IsNotNull(testResult);
                Assert.IsNull(err);
                //Assert.IsNotNull(testTeam);
                //Assert.AreNotEqual(error5, err);
            }
            else
            {
                Assert.Fail("Invalid test method");
                Assert.AreNotEqual(error5, err);
            }
        }

        [TestMethod()]
        [DataRow("0", true)]
        [DataRow("Team1", true)]
        [DataRow("2", true)]
        [DataRow("5", false)]
        [DataRow("Team5", false)]
        [DataRow("Fail", false)]
        public async Task SearchTeamsTest(string input,bool goodcase)
        {
            //arrange
            DummyDBR dDBR = new();
            TeamManager _UnitUnderTest = new(dDBR, new UserManager(dDBR, new Components.AuthenticationComponent()));

            //act
            TeamViewmodel result = await _UnitUnderTest.SearchTeams(input);
            //assert

            if (goodcase)
            {
                Assert.IsNotNull(result);
            }
            else
            {
                Assert.IsNull(result);
            }
        }
    }
}