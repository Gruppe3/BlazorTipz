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
            DummyDBR dDBR = new DummyDBR();
            TeamManager _UnitUnderTest = new TeamManager(dDBR, new UserManager(dDBR, new Components.AuthenticationComponent()));
            Assert.IsNotNull(_UnitUnderTest);
        }

        [TestMethod()]
        public async void getTeamsTest()
        {
            //arrange
            DummyDBR dDBR = new DummyDBR();
            TeamManager _UnitUnderTest = new TeamManager(dDBR, new UserManager(dDBR, new Components.AuthenticationComponent()));
            List<TeamViewmodel> teamList;
            List<TeamViewmodel> teams;

            //act
            teamList = await _UnitUnderTest.getTeams();
            teams = _UnitUnderTest.Teams;

            //assert
            Assert.AreEqual(teamList, teams);
            foreach (TeamViewmodel team in teamList)
            {
                Assert.IsTrue(
                    team.id != "" &&
                    team.name != "" &&
                    team.leader != ""
                    );
            }
        }

        [TestMethod()]
        public async void updateTeamsListTest()
        {
            //arrange
            DummyDBR dDBR = new DummyDBR();
            TeamManager _UnitUnderTest = new TeamManager(dDBR, new UserManager(dDBR, new Components.AuthenticationComponent()));
            List<TeamViewmodel> teamList;
            List<TeamViewmodel> teams;

            //act
            teamList = await _UnitUnderTest.updateTeamsList();
            teams = _UnitUnderTest.Teams;

            //assert
            Assert.AreEqual(teamList, teams);
            foreach (TeamViewmodel team in teamList)
            {
                Assert.IsTrue(
                    team.id != "" &&
                    team.name != "" &&
                    team.leader != ""
                    );
            }
        }

        [TestMethod()]
        [DataRow("0", true)]
        [DataRow("1", true)]
        [DataRow("2", true)]
        [DataRow("556", false)]
        [DataRow("xx", false)]
        public async void getTeamTest(string teamId, bool good)
        {
            //arrange
            DummyDBR dDBR = new DummyDBR();
            TeamManager _UnitUnderTest = new TeamManager(dDBR, new UserManager(dDBR, new Components.AuthenticationComponent()));
            TeamViewmodel result;
            
            //act
            result = await _UnitUnderTest.getTeam(teamId);
            
            //assert
            if (good)
            {
                Assert.AreEqual(result.id, teamId);
            }
            else //if not good
            {
                Assert.IsTrue(result.id == "");
            }
            
        }

        [TestMethod()]
        public async void getInactiveTeamsTest()
        {
            //arrange
            DummyDBR dDBR = new DummyDBR();
            TeamManager _UnitUnderTest = new TeamManager(dDBR, new UserManager(dDBR, new Components.AuthenticationComponent()));
            List<TeamViewmodel> inactiveTeams;
            List<TeamViewmodel> activeTeams;

            //act
            inactiveTeams = await _UnitUnderTest.getInactiveTeams();
            activeTeams = await _UnitUnderTest.getTeams();

            //assert

            Assert.AreNotEqual(inactiveTeams, activeTeams);
            foreach (TeamViewmodel team in inactiveTeams)
            {
                Assert.IsTrue(
                    team.id != "" &&
                    team.name != "" &&
                    team.leader != ""
                    );
            }
        }

        [TestMethod()]
        [DataRow("0", "TestTeam0", "0")]
        [DataRow("1", "TestTeam1", "1")]
        public async void updateTeamTest(string id, string name, string teamLeader)
        {
            //arrange
            DummyDBR dDBR = new DummyDBR();
            TeamManager _UnitUnderTest = new TeamManager(dDBR, new UserManager(dDBR, new Components.AuthenticationComponent()));
            TeamViewmodel teamFromList;
            TeamViewmodel testTeam = new TeamViewmodel();
            testTeam.id = id;
            testTeam.name = name;
            testTeam.leader = teamLeader;

            //act
            await _UnitUnderTest.updateTeam(testTeam);
            teamFromList = _UnitUnderTest.Teams.Where(t => t.id == testTeam.id).FirstOrDefault();
            
            //assert
            Assert.AreEqual(testTeam, teamFromList);
        }

        [TestMethod()]  //BigBoyFunn
        [DataRow("", "", 1)]
        [DataRow("", "", 2)]
        [DataRow("Team11", "", 3)]
        [DataRow("NewTeam11", "", 3)]
        [DataRow("Team12", "-1", 4)]
        [DataRow("Team13", "100", 4)]
        [DataRow("Team14", "2", 5)]
        [DataRow("TestTeam15", "3", 5)]
        [DataRow("GamerTeam", "2", 5)]
        [DataRow("Electrical", "7", 5)]
        public async void createTeamTest(string name, string leader, int testCase)
        {
            //arrange
            DummyDBR dDBR = new DummyDBR();
            TeamManager _UnitUnderTest = new TeamManager(dDBR, new UserManager(dDBR, new Components.AuthenticationComponent()));

            TeamViewmodel? testResult;
            TeamViewmodel testTeam = new TeamViewmodel();
            testTeam.name = name;
            testTeam.leader = leader;

            string error1 = "Team is null";
            string error2 = "Team name is empty";
            string error3 = "No team leader chosen";
            string error4 = "Team leader not found";
            string error5 = "Team creation error";
            string? err;

            //act
            if (testCase == 1) { testTeam = null; }
            (testResult, err) = await _UnitUnderTest.createTeam(testTeam);

            //assert
            if (testCase == 1)
            {
                Assert.AreEqual(testResult, null);
                Assert.AreEqual(err, error1);
            }
            else if (testCase == 2)
            {
                Assert.AreEqual(testResult, null);
                Assert.AreEqual(err, error2);
            }
            else if (testCase == 3)
            {
                Assert.AreEqual(testResult, null);
                Assert.AreEqual(err, error3);
            }
            else if (testCase == 4)
            {
                Assert.AreEqual(testResult, null);
                Assert.AreEqual(err, error4);
            }
            else if (testCase == 5)
            {
                Assert.AreEqual(testResult, testTeam);
                Assert.AreEqual(err, null);
            }
            else
            {
                Assert.Fail("Invalid test method");
            }
            Assert.AreNotEqual(err, error5);

        }
    }
}