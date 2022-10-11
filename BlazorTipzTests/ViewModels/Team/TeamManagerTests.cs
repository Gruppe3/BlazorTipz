using Microsoft.VisualStudio.TestTools.UnitTesting;
using BlazorTipz.ViewModels.Team;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlazorTipz.Models;
using BlazorTipz.Models.DbRelay;
using BlazorTipz.ViewModels.User;
using Microsoft.AspNetCore.Routing;
using BlazorTipzTests.ViewModels.DummyClass;

namespace BlazorTipz.ViewModels.Team.Tests
{
    [TestClass()]
    public class TeamManagerTests
    {
        [TestMethod()]
        public void TeamManagerTest()
        {

        }

        [TestMethod()]
        public async void getTeamsTest()
        {
            //arrange
            TeamManager UnitUnderTest = new TeamManager(new DummyDBR(), new DummyUserManager());


            //act

            //assert

            Assert.Fail();
        }

        [TestMethod()]
        public void getTeamsWithNoTeamTest()
        {

        }

        [TestMethod()]
        public void updateTeamsListTest()
        {

        }

        [TestMethod()]
        public void getTeamTest()
        {

        }

        [TestMethod()]
        public void getInactiveTeamsTest()
        {

        }

        [TestMethod()]
        public void updateTeamTest()
        {

        }

        [TestMethod()]
        public void createTeamTest()
        {
            
            
        }
    }
}