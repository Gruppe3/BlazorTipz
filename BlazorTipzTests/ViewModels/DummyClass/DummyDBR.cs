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
        private List<UserEntity> _Users = new List<UserEntity>();
        private List<TeamEntity> _Teams = new List<TeamEntity>();
        private List<SuggestionEntity> _Suggestions = new List<SuggestionEntity>();

        public string ConnectionString { private get; set; } = "default";

        public DummyDBR()
        {
            fillDummyDB();
        }

        private void fillDummyDB()
        {
            //Fill up the dummy database with some dummy data.

            for (int i = 0; i < 13; i++)
            {
                UserEntity user = new UserEntity();
                user.employmentId = i.ToString();
                user.userName = "User" + i.ToString();
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
                TeamEntity team = new TeamEntity();
                team.teamId = i.ToString();
                team.teamName = "Team" + i.ToString();
                team.teamLeader = i.ToString();

                if (i < 4) { team.active = true; }
                else team.active = false;

                _Teams.Add(team);
            }
            for (int i = 0; i < 6; i++)
            {
                SuggestionEntity sugg = new SuggestionEntity();
                sugg.sugId = i.ToString();
                if (i < 3) 
                { 
                    sugg.ownerId = "1";
                    sugg.creatorId = "1";
                    sugg.categoryId = "HMS";    // HMS
                }
                else { 
                    sugg.ownerId = "2";
                    sugg.creatorId = "2";
                    sugg.categoryId = "2";    // Kvalitet
                }
                sugg.sugTitle = "TestTitle" + i.ToString();
                sugg.sugDesc = "DescribingTest" + i.ToString();
                sugg.createdAt = DateTime.Now.ToString();
                sugg.justDoIt = false;
                _Suggestions.Add(sugg);
            }
        }



        public async Task<UserEntity> getLoginUser(string empId)
        {
            return _Users.Where(x => x.employmentId == empId).FirstOrDefault();
        }
        public async Task<UserEntity> lookUpUser(string empId)
        {
            return _Users.Where(x => x.employmentId == empId).FirstOrDefault();
        }
        public async Task addUserEntries(List<UserEntity> toSaveUsers)
        {
            foreach (UserEntity user in toSaveUsers)
            {
                _Users.Add(user);
            }
            //return "Successfully added " + x.ToString() + " users.";
            
        }
        public async Task updateUserEntry(UserEntity toSaveUser)
        {
            UserEntity user = _Users.Where(x => x.employmentId == toSaveUser.employmentId).FirstOrDefault();
            if (user != null)
            {
                if (toSaveUser.passwordHash != null && toSaveUser.passwordSalt != null)
                {
                    user.userName = toSaveUser.userName;
                    user.passwordSalt = toSaveUser.passwordSalt;
                    user.passwordHash = toSaveUser.passwordHash;
                    user.userRole = toSaveUser.userRole;
                    user.firstTimeLogin = toSaveUser.firstTimeLogin;
                }
                else if (toSaveUser.teamId != null)
                {
                    user.userName = toSaveUser.userName;
                    user.userRole = toSaveUser.userRole;
                    user.teamId = toSaveUser.teamId;
                }
                else
                {
                    user.userName = toSaveUser.userName;
                    user.userRole = toSaveUser.userRole;
                }
            }
        }
        public async Task<List<UserEntity>> getActiveUsers()
        {
            return await getUsersByActiveStatus(true);
        }
        public async Task<List<UserEntity>> getInactiveUsers()
        {
            return await getUsersByActiveStatus(false);
        }
        private async Task<List<UserEntity>> getUsersByActiveStatus(bool var)
        {
            List<UserEntity> activeUsers = new List<UserEntity>();

            foreach (UserEntity user in _Users)
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
            UserEntity user = _Users.Where(x => x.employmentId == empid).FirstOrDefault();
            if (user != null)
            { 
                user.active = state; 
            }
        }
        public async Task changeUsersStateTo(List<UserEntity> users, bool state)
        {
            foreach (UserEntity user in users)
            {
                user.active = state;
            }
        }

        //team/teams
        public async Task<TeamEntity> getSingleTeamDbFromDb(string teamId)
        {
            return _Teams.Where(x => x.teamId == teamId).FirstOrDefault();
        }
        public async Task addTeamEntry(TeamEntity team)
        {
            string numb = _Teams.Count().ToString();
            team.teamId = numb;
            _Teams.Add(team);
        }
        public async Task updateTeamEntry(TeamEntity team)
        {
            TeamEntity teamEdit = _Teams.Where(x => x.teamId == team.teamId).FirstOrDefault();
            if (teamEdit != null)
            {
                teamEdit.teamName = team.teamName;
                teamEdit.teamLeader = team.teamLeader;
            }
            
        }
        public async Task<List<TeamEntity>> getActiveTeams()
        {
            return await getTeamsByActiveStatus(true);
        }
        public async Task<List<TeamEntity>> getInactiveTeams()
        {
            return await getTeamsByActiveStatus(false);
        }
        private async Task<List<TeamEntity>> getTeamsByActiveStatus(bool var)
        {
            List<TeamEntity> activeTeams = new List<TeamEntity>();

            foreach (TeamEntity team in _Teams)
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
            TeamEntity team = _Teams.Where(x => x.teamId == teamid).FirstOrDefault();
            if (team != null)
            {
                team.active = state;
            }
        }
        public async Task changeTeamsStateTo(List<TeamEntity> teams, bool state)
        {
            foreach (TeamEntity team in teams)
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

        public async Task<SuggestionEntity?> GetSuggestion(string sugId)
        {
            return _Suggestions.Where(x => x.sugId == sugId).FirstOrDefault();
        }

        public async Task<List<SuggestionEntity>?> GetSuggestionsOfCreator(string empId)
        {
            return _Suggestions.Where(x => x.creatorId == empId).ToList();
        }

        public async Task<List<SuggestionEntity>?> GetSuggestionOfTeam(string teamId)
        {
            return _Suggestions.Where(x => x.ownerId == teamId).ToList();
        }

        public async Task<List<SuggestionEntity>?> GetSuggestionsByStatus(SuggStatus status)
        {
            return _Suggestions.Where(x => x.sugStatus == status).ToList();
        }

        public async Task updateSuggestion(SuggestionEntity sug)
        {
            SuggestionEntity sugg = _Suggestions.Where(x => x.sugId == sug.sugId).FirstOrDefault();
            if (sugg != null)
            {
                sugg.sugTitle = sug.sugTitle;
                sugg.sugDesc = sug.sugDesc;
                sugg.sugStatus = sug.sugStatus;
                sugg.ownerId = sug.ownerId;
                sugg.assignedId = sug.assignedId;
                sugg.dueDate = sug.dueDate;
                sugg.categoryId = sug.categoryId;
            }
        }
    }
}
