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
        private List<UserEntity> _Users = new();
        private List<TeamEntity> _Teams = new();
        private List<SuggestionEntity> _Suggestions = new();
        private List<CommentEntity> _Comments = new();
        private List<CategoryEntity> _Categories = new();

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
                UserEntity user = new();
                user.employmentId = i.ToString();
                user.userName = "User" + i.ToString();
                user.password = "password" + i.ToString();
                user.PasswordHashing(user.password);
                user.teamId = "1";
                user.firstTimeLogin = true;
                
                if (i < 10) { user.active = true; }
                else user.active = false;

                _Users.Add(user);
            }
            for (int i = 0; i < 5; i++)
            {
                TeamEntity team = new();
                team.teamId = i.ToString();
                team.teamName = "Team" + i.ToString();
                team.teamLeader = i.ToString();

                if (i < 4) { team.active = true; }
                else team.active = false;

                _Teams.Add(team);
            }
            for (int i = 0; i < 6; i++)
            {
                SuggestionEntity sugg = new();
                sugg.sugId = i.ToString();
                if (i < 3) 
                { 
                    sugg.ownerId = "1";
                    sugg.creatorId = "1";
                    sugg.categoryId = "1";
                    sugg.catName = "HMS";
                }
                else { 
                    sugg.ownerId = "2";
                    sugg.creatorId = "2";
                    sugg.categoryId = "2";
                    sugg.catName = "Kvalitet";
                }
                sugg.sugTitle = "TestTitle" + i.ToString();
                sugg.sugDesc = "DescribingTest" + i.ToString();
                sugg.createdAt = DateTime.Now.ToLocalTime();
                sugg.justDoIt = false;
                _Suggestions.Add(sugg);
            }
            LoadCategoriList();
        }
        private void LoadCategoriList()
        {
            _Categories.Add(new CategoryEntity("1", "HMS"));
            _Categories.Add(new CategoryEntity("2", "Kvalitet"));
            _Categories.Add(new CategoryEntity("3", "Ledetid"));
            _Categories.Add(new CategoryEntity("4", "Kostnader"));
            _Categories.Add(new CategoryEntity("5", "Effektivisering"));
            _Categories.Add(new CategoryEntity("6", "Kompetanse"));
            _Categories.Add(new CategoryEntity("7", "Kommunikasjon"));
            _Categories.Add(new CategoryEntity("8", "5S"));
            _Categories.Add(new CategoryEntity("9", "Standardisering"));
            _Categories.Add(new CategoryEntity("10", "Flyt"));
            _Categories.Add(new CategoryEntity("11", "Visualisering"));
            _Categories.Add(new CategoryEntity("12", "Energi"));
            _Categories.Add(new CategoryEntity("13", "Bærekraft"));
            _Categories.Add(new CategoryEntity("14", "Industri 4.0"));
        }


        public async Task<UserEntity> GetLoginUser(string empId)
        {
            await Task.Delay(0);
            return _Users.Where(x => x.employmentId == empId).FirstOrDefault();
        }
        public async Task<UserEntity> LookUpUser(string empId)
        {
            await Task.Delay(0);
            return _Users.Where(x => x.employmentId == empId).FirstOrDefault();
        }
        public async Task AddUserEntries(List<UserEntity> toSaveUsers)
        {
            await Task.Delay(0);
            foreach (UserEntity user in toSaveUsers)
            {
                _Users.Add(user);
            }
            //return "Successfully added " + x.ToString() + " users.";
            
        }
        public async Task UpdateUserEntry(UserEntity toSaveUser)
        {
            await Task.Delay(0);
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
        public async Task<List<UserEntity>> GetActiveUsers()
        {
            return await GetUsersByActiveStatus(true);
        }
        public async Task<List<UserEntity>> GetInactiveUsers()
        {
            return await GetUsersByActiveStatus(false);
        }
        private async Task<List<UserEntity>> GetUsersByActiveStatus(bool var)
        {
            await Task.Delay(0);
            List<UserEntity> activeUsers = new();

            foreach (UserEntity user in _Users)
            {
                if (user.active == var)
                {
                    activeUsers.Add(user);
                }
            }
            return activeUsers;
        }
        public async Task ChangeUserStateTo(string empid, bool state)
        {
            await Task.Delay(0);
            UserEntity user = _Users.Where(x => x.employmentId == empid).FirstOrDefault();
            if (user != null)
            { 
                user.active = state; 
            }
        }
        public async Task ChangeUsersStateTo(List<UserEntity> users, bool state)
        {
            await Task.Delay(0);
            foreach (UserEntity user in users)
            {
                user.active = state;
            }
        }

        //team/teams
        public async Task<TeamEntity> GetSingleTeamFromDb(string teamId)
        {
            await Task.Delay(0);
            return _Teams.Where(x => x.teamId == teamId).FirstOrDefault();
        }
        public async Task AddTeamEntry(TeamEntity team)
        {
            await Task.Delay(0);
            string numb = _Teams.Count().ToString();
            team.teamId = numb;
            _Teams.Add(team);
        }
        public async Task UpdateTeamEntry(TeamEntity team)
        {
            await Task.Delay(0);
            TeamEntity teamEdit = _Teams.Where(x => x.teamId == team.teamId).FirstOrDefault();
            if (teamEdit != null)
            {
                teamEdit.teamName = team.teamName;
                teamEdit.teamLeader = team.teamLeader;
            }
            
        }
        public async Task<List<TeamEntity>> GetActiveTeams()
        {
            return await GetTeamsByActiveStatus(true);
        }
        public async Task<List<TeamEntity>> GetInactiveTeams()
        {
            return await GetTeamsByActiveStatus(false);
        }
        private async Task<List<TeamEntity>> GetTeamsByActiveStatus(bool var)
        {
            await Task.Delay(0);
            List<TeamEntity> activeTeams = new();

            foreach (TeamEntity team in _Teams)
            {
                if (team.active == var)
                {
                    activeTeams.Add(team);
                }
            }
            return activeTeams;
        }
        public async Task ChangeTeamStateTo(string teamid, bool state)
        {
            await Task.Delay(0);
            TeamEntity team = _Teams.Where(x => x.teamId == teamid).FirstOrDefault();
            if (team != null)
            {
                team.active = state;
            }
        }
        public async Task ChangeTeamsStateTo(List<TeamEntity> teams, bool state)
        {
            await Task.Delay(0);
            foreach (TeamEntity team in teams)
            {
                team.active = state;
            }
        }

        // Suggestions
        public async Task SaveSuggestion(SuggestionEntity suggestion)
        {
            await Task.Delay(0);
            _Suggestions.Add(suggestion);
        }
        public async Task SaveSuggestionList(List<SuggestionEntity> suggestions)
        {
            await Task.Delay(0);
            foreach (SuggestionEntity sugg in suggestions)
            {
                _Suggestions.Add(sugg);
            }
        }

        public async Task<SuggestionEntity> GetSuggestion(string sugId)
        {
            await Task.Delay(0);
            return _Suggestions.Where(x => x.sugId == sugId).FirstOrDefault();
        }

        public async Task<List<SuggestionEntity>> GetSuggestionsOfCreator(string empId)
        {
            await Task.Delay(0);
            return _Suggestions.Where(x => x.creatorId == empId).ToList();
        }
        public async Task<List<SuggestionEntity>> GetAssignedSuggestions(string empId)
        {
            await Task.Delay(0);
            return _Suggestions.Where(x => x.assignedId == empId).ToList();
        }
        public async Task<List<SuggestionEntity>> GetSuggestionOfTeam(string teamId)
        {
            await Task.Delay(0);
            return _Suggestions.Where(x => x.ownerId == teamId).ToList();
        }

        public async Task<List<SuggestionEntity>> GetSuggestionsOfCreator(string empId, SuggStatus status)
        {
            throw new NotImplementedException();
        }
        public async Task<List<SuggestionEntity>> GetAssignedSuggestions(string empId, SuggStatus status)
        {
            throw new NotImplementedException();
        }
        public async Task<List<SuggestionEntity>> GetSuggestionOfTeam(string teamId, SuggStatus status)
        {
            throw new NotImplementedException();
        }

        public async Task<List<SuggestionEntity>?> GetSuggestionsByStatus(SuggStatus status)
        {
            await Task.Delay(0);
            return _Suggestions.Where(x => x.sugStatus == status).ToList();
        }

        public async Task UpdateSuggestion(SuggestionEntity sug)
        {
            await Task.Delay(0);
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

        public Task<List<CategoryEntity>> GetCategoryEntities()
        {
            throw new NotImplementedException();
        }

        public Task UpdateCategories(List<CategoryEntity> catList)
        {
            throw new NotImplementedException();
        }




        public async Task SaveComment(CommentEntity comment)
        {
            await Task.Delay(0);
            _Comments.Add(comment);
        }

        public async Task UpdateComment(CommentEntity comment)
        {
            await Task.Delay(0);
            CommentEntity commentEdit = _Comments.Where(x => x.employmentId == comment.employmentId && x.sugId == comment.sugId && x.createdAt == comment.createdAt).FirstOrDefault();
            if (commentEdit != null)
            {
                commentEdit.content = comment.content;
                commentEdit.active = comment.active;
            }
        }

        public async Task<List<CommentEntity>> GetCommentsOfSuggestion(string sugId)
        {
            await Task.Delay(0);
            return _Comments.Where(x => x.sugId == sugId).ToList();
        }

        public async Task<List<CommentEntity>> GetCommentsOfUser(string empId)
        {
            await Task.Delay(0);
            return _Comments.Where(x => x.employmentId == empId).ToList();
        }
    }
}
