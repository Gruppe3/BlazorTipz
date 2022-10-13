using BlazorTipz.Data;
using BlazorTipz.Models;
using BlazorTipz.Models.DbRelay;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BlazorTipzTests.ViewModels.DummyClass
{
    public class DummyDBR : IDbRelay
    {
        //DummyDatabaseTables
        private List<UserDb> _Users = new List<UserDb>();
        private List<TeamDb> _Teams = new List<TeamDb>();
        private List<SuggestionEntity> _Suggestions = new List<SuggestionEntity>();
        

    public DummyDBR()
        {
            fillDummyDB();
        }

        private void fillDummyDB()
        {
            //Fill up the dummy database with some dummy data.

            for (int i = 0; i < 13; i++)
            {
                UserDb user = new UserDb();
                user.employmentId = i.ToString();
                user.name = "User" + i.ToString();
                user.password = "password" + i.ToString();
                user.passwordHashing(user.password);
                user.teamId = "1";
                user.firstTimeLogin = true;
                
                if (i < 10) { user.active = true; }
                else user.active = false;

                _Users.Add(user);
            }
            for (int i = 0; i < 5; i++)
            {
                TeamDb team = new TeamDb();
                team.teamId = i.ToString();
                team.teamName = "Team" + i.ToString();
                team.teamLeader = i.ToString();

                if (i < 4) { team.active = true; }
                else team.active = false;

                _Teams.Add(team);
            }
            for (int i = 0; i < 3; i++)
            {
                SuggestionEntity sugg = new SuggestionEntity();
                sugg.sugId = i.ToString();
                sugg.owner = "Team2";
                sugg.sugTitle = "TestTitle" + i.ToString();
                sugg.sugDesc = "DescribingTest" + i.ToString();
                sugg.createdAt = DateTime.Now.ToString();
                sugg.JustDoIt = false;
                _Suggestions.Add(sugg);
            }
        }



        public async Task<UserDb> getLoginUser(string empId)
        {
            return _Users.Where(x => x.employmentId == empId).FirstOrDefault();
        }
        public async Task<UserDb> lookUpUser(string empId)
        {
            return _Users.Where(x => x.employmentId == empId).FirstOrDefault();
        }
        public async Task addUserEntries(List<UserDb> toSaveUsers)
        {
            foreach (UserDb user in toSaveUsers)
            {
                _Users.Add(user);
            }
            //return "Successfully added " + x.ToString() + " users.";
            
        }
        public async Task updateUserEntry(UserDb toSaveUser)
        {
            UserDb user = _Users.Where(x => x.employmentId == toSaveUser.employmentId).FirstOrDefault();
            if (user != null)
            {
                if (toSaveUser.passwordHash != null && toSaveUser.passwordSalt != null)
                {
                    user.name = toSaveUser.name;
                    user.passwordSalt = toSaveUser.passwordSalt;
                    user.passwordHash = toSaveUser.passwordHash;
                    user.role = toSaveUser.role;
                    user.firstTimeLogin = toSaveUser.firstTimeLogin;
                }
                else if (toSaveUser.teamId != null)
                {
                    user.name = toSaveUser.name;
                    user.role = toSaveUser.role;
                    user.teamId = toSaveUser.teamId;
                }
                else
                {
                    user.name = toSaveUser.name;
                    user.role = toSaveUser.role;
                }
            }
        }
        public async Task<List<UserDb>> getActiveUsers()
        {
            return await getUsersByActiveStatus(true);
        }
        public async Task<List<UserDb>> getInactiveUsers()
        {
            return await getUsersByActiveStatus(false);
        }
        private async Task<List<UserDb>> getUsersByActiveStatus(bool var)
        {
            List<UserDb> activeUsers = new List<UserDb>();

            foreach (UserDb user in _Users)
            {
                if (user.active == var)
                {
                    activeUsers.Add(user);
                }
            }
            return activeUsers;
        }
        public async Task changeUserStateTo(string empid, bool state)
        {
            UserDb user = _Users.Where(x => x.employmentId == empid).FirstOrDefault();
            if (user != null)
            { 
                user.active = state; 
            }
        }
        public async Task changeUsersStateTo(List<UserDb> users, bool state)
        {
            foreach (UserDb user in users)
            {
                user.active = state;
            }
        }

        //team/teams
        public async Task<TeamDb> getSingleTeamDbFromDb(string teamId)
        {
            return _Teams.Where(x => x.teamId == teamId).FirstOrDefault();
        }
        public async Task addTeamEntry(TeamDb team)
        {
            string numb = _Teams.Count().ToString();
            team.teamId = numb;
            _Teams.Add(team);
        }
        public async Task updateTeamEntry(TeamDb team)
        {
            TeamDb teamEdit = _Teams.Where(x => x.teamId == team.teamId).FirstOrDefault();
            if (teamEdit != null)
            {
                teamEdit.teamName = team.teamName;
                teamEdit.teamLeader = team.teamLeader;
            }
            
        }
        public async Task<List<TeamDb>> getActiveTeams()
        {
            return await getTeamsByActiveStatus(true);
        }
        public async Task<List<TeamDb>> getInactiveTeams()
        {
            return await getTeamsByActiveStatus(false);
        }
        private async Task<List<TeamDb>> getTeamsByActiveStatus(bool var)
        {
            List<TeamDb> activeTeams = new List<TeamDb>();

            foreach (TeamDb team in _Teams)
            {
                if (team.active == var)
                {
                    activeTeams.Add(team);
                }
            }
            return activeTeams;
        }
        public async Task changeTeamStateTo(string teamid, bool state)
        {
            TeamDb team = _Teams.Where(x => x.teamId == teamid).FirstOrDefault();
            if (team != null)
            {
                team.active = state;
            }
        }
        public async Task changeTeamsStateTo(List<TeamDb> teams, bool state)
        {
            foreach (TeamDb team in teams)
            {
                team.active = state;
            }
        }

        // Suggestions
        public async Task saveSuggestion(SuggestionEntity suggestion)
        {
            _Suggestions.Add(suggestion);
        }
        public async Task saveSuggestionList(List<SuggestionEntity> suggestions)
        {
            foreach (SuggestionEntity sugg in suggestions)
            {
                _Suggestions.Add(sugg);
            }
        }

        //må pusses litt på

        public Task<SuggestionEntity?> GetSuggestion(string sugId)
        {
            throw new NotImplementedException();
        }

        public Task<List<SuggestionEntity>?> GetSuggestionsOfCreator(string empId)
        {
            throw new NotImplementedException();
        }

        public Task<List<SuggestionEntity>?> GetSuggestionOfTeam(string teamId)
        {
            throw new NotImplementedException();
        }

        public Task<List<SuggestionEntity>?> GetSuggestionsByStatus(SuggStatus status)
        {
            throw new NotImplementedException();
        }
    }
}
