using BlazorTipz.Models;
using BlazorTipz.Models.DbRelay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorTipzTests.ViewModels.DummyClass
{
    public class DummyDBR : IDbRelay
    {
        public Task addTeamEntry(TeamDb team)
        {
            throw new NotImplementedException();
        }

        public Task addUserEntries(List<UserDb> toSaveUsers)
        {
            throw new NotImplementedException();
        }

        public Task changeTeamsStateTo(List<TeamDb> users, bool state)
        {
            throw new NotImplementedException();
        }

        public Task changeTeamStateTo(string teamid, bool state)
        {
            throw new NotImplementedException();
        }

        public Task changeUsersStateTo(List<UserDb> users, bool state)
        {
            throw new NotImplementedException();
        }

        public Task changeUserStateTo(string empid, bool state)
        {
            throw new NotImplementedException();
        }

        public Task<List<TeamDb>> getActiveTeams()
        {
            throw new NotImplementedException();
        }

        public Task<List<UserDb>> getActiveUsers()
        {
            throw new NotImplementedException();
        }

        public Task<List<TeamDb>> getInactiveTeams()
        {
            throw new NotImplementedException();
        }

        public Task<List<UserDb>> getInactiveUsers()
        {
            throw new NotImplementedException();
        }

        public Task<UserDb> getLoginUser(string empId)
        {
            throw new NotImplementedException();
        }

        public Task<TeamDb> getSingleTeamDbFromDb(string teamId)
        {
            throw new NotImplementedException();
        }

        public Task<UserDb> lookUpUser(string empId)
        {
            throw new NotImplementedException();
        }

        public Task saveSuggestion(SuggestionEntity suggestion)
        {
            throw new NotImplementedException();
        }

        public Task saveSuggestionList(List<SuggestionEntity> suggestions)
        {
            throw new NotImplementedException();
        }

        public Task updateTeamEntry(TeamDb team)
        {
            throw new NotImplementedException();
        }

        public Task updateUserEntry(UserDb toSaveUser)
        {
            throw new NotImplementedException();
        }
    }
}
