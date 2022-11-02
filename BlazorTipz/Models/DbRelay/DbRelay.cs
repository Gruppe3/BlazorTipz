﻿using BlazorTipz.Data;
using BlazorTipz.Components.DataAccess;
using System.Linq.Expressions;

namespace BlazorTipz.Models.DbRelay
{
    public class DbRelay : IDbRelay
    {
        private readonly IDataAccess _data;
        private readonly IConfiguration _config;

        public string ConnectionString { private get; set; } = "default";

        public DbRelay(IDataAccess data, IConfiguration connectionString)
        {
            _data = data;
            _config = connectionString;
        }

        //methodes for getting, setting and updating the database
        
        //User/Users
        public async Task<UserDb> getLoginUser(string empId)
        {
            try
            {
                var sql = "SELECT * FROM Users WHERE employmentId = @empid;";
                UserDb dbinfo = await _data.LoadData<UserDb, dynamic>(sql, new { empid = empId }, _config.GetConnectionString(ConnectionString), true);
                return dbinfo;
            }
            catch (Exception ex) { return null; }
        }

        public async Task<UserDb> lookUpUser(string empId)
        {
            try
            {
                var sql = "SELECT name, employmentId FROM Users WHERE employmentId = @empid;";
                UserDb dbinfo = await _data.LoadData<UserDb, dynamic>(sql, new { empid = empId }, _config.GetConnectionString(ConnectionString), true);
                return dbinfo;
            }
            catch (Exception ex) { return null; }
        }

        public async Task addUserEntries(List<UserDb> toSaveUsers)
        {
            try
            {
                foreach (UserDb tsu in toSaveUsers)
                {
                    var sql = "INSERT IGNORE INTO Users (employmentId, name, passwordHash, passwordSalt, role) VALUES (@employmentId, @name, @passwordHash, @passwordSalt, @role);";

                    await _data.SaveData(sql, new
                    {
                        employmentId = tsu.employmentId,
                        name = tsu.name,
                        passwordSalt = tsu.passwordSalt,
                        passwordHash = tsu.passwordHash,
                        role = tsu.role.ToString()
                    },
                        _config.GetConnectionString(ConnectionString));
                }
            }
            catch (Exception ex) { }
        }
        //update user
        public async Task updateUserEntry(UserDb toSaveUser)
        {
            string Role = toSaveUser.role.ToString();
            try
            {
                if (toSaveUser.passwordHash != null)
                {
                    if (toSaveUser.passwordSalt != null)
                    {
                        var sql = "UPDATE Users SET passwordHash = @passwordHash, name = @name, passwordSalt = @passwordSalt, role = @role, firstTimeLogin = @firstTimeLogin WHERE employmentId = @employmentId;";

                        await _data.SaveData(sql, new
                        {
                            employmentId = toSaveUser.employmentId,
                            name = toSaveUser.name,
                            passwordSalt = toSaveUser.passwordSalt,
                            passwordHash = toSaveUser.passwordHash,
                            role = Role,
                            firstTimeLogin = toSaveUser.firstTimeLogin
                        },
                            _config.GetConnectionString(ConnectionString));
                    }
                }
                else if(toSaveUser.teamId != null)
                {
                    var sql = "UPDATE Users SET name = @name, role = @role, teamId = @teamId WHERE employmentId = @employmentId;";

                    await _data.SaveData(sql, new
                    {
                        employmentId = toSaveUser.employmentId,
                        name = toSaveUser.name,
                        role = Role,
                        teamId = toSaveUser.teamId
                    },
                        _config.GetConnectionString(ConnectionString));

                }
                else
                {
                    var sql = "UPDATE Users SET name = @name, role = @role WHERE employmentId = @employmentId;";

                    await _data.SaveData(sql, new
                    {
                        employmentId = toSaveUser.employmentId,
                        name = toSaveUser.name,
                        role = Role
                    },
                        _config.GetConnectionString(ConnectionString));
                }

            }
            catch (Exception ex) { }
        }

        public async Task<List<UserDb>> getActiveUsers()
        {
            
            try
            {
                var sql = "SELECT name, employmentId, active FROM Users WHERE active = true;";

                var dbinfo = await _data.LoadData<UserDb, dynamic>(sql, new { }, _config.GetConnectionString(ConnectionString));

                return dbinfo;
            }
            catch (Exception ex) { return null; }
            
        }

        public async Task<List<UserDb>> getInactiveUsers()
        {
            try
            {
                var sql = "SELECT name, employmentId, active FROM Users WHERE active = false;";

                var dbinfo = await _data.LoadData<UserDb, dynamic>(sql, new { }, _config.GetConnectionString(ConnectionString));

                return dbinfo;
            }
            catch (Exception ex) { return null; }
        }

        public async Task changeUserStateTo(string empId, bool state) {
            try
            {
                var sql = "UPDATE Users SET active = @state WHERE employmentId = @empid;";
                await _data.SaveData(sql, new
                {
                    state = state,
                    empid = empId
                },
                _config.GetConnectionString(ConnectionString));
            } catch (Exception ex) { }
        }
        public async Task changeUsersStateTo(List<UserDb> users, bool state)
        {
            try
            {
                foreach (UserDb user in users)
                {
                    var sql = "UPDATE Users SET active = @state WHERE employmentId = @empid;";
                    
                    await _data.SaveData(sql, new
                    {
                        state = state,
                        empid = user.employmentId
                    },
                    _config.GetConnectionString(ConnectionString));
                }
               
            }
            catch (Exception ex) { }
        }
        
        //team/teams

        //get's a single team from database
        public async Task<TeamDb> getSingleTeamDbFromDb(string teamId)
        {
            try
            {
                var sql = "SELECT * FROM Teams WHERE teamId = @teamid;";
               
                TeamDb team = await _data.LoadData<TeamDb, dynamic>(sql, new { teamid = teamId }, _config.GetConnectionString(ConnectionString), true);
                return team;
            }
            catch (Exception ex) { return null; }
        }
        
        //add a single team to database
        public async Task addTeamEntry(TeamDb team)
        {
            try 
            {
                var sql = "INSERT IGNORE INTO Teams (teamName, teamLeader) VALUES (@teamName, @teamLeader)";
                await _data.SaveData(sql, new
                {
                    teamName = team.teamName,
                    teamLeader = team.teamLeader
                }, _config.GetConnectionString(ConnectionString));
            } 
            catch (Exception ex) { }
        }

        //update a single team in database
        public async Task updateTeamEntry(TeamDb team)
        {
            try
            {
                var sql = "UPDATE Teams SET teamName = @teamName, teamLeader = @teamLeader WHERE teamId = @teamId";
                await _data.SaveData(sql, new
                {
                    teamName = team.teamName,
                    teamLeader = team.teamLeader,
                    teamId = team.teamId
                }, _config.GetConnectionString(ConnectionString));
            }
            catch (Exception ex) { }
        }

        //get all Active teams from database
        public async Task<List<TeamDb>> getActiveTeams()
        {
            try
            {
                var sql = "SELECT * FROM Teams WHERE active = true;";

                var dbinfo = await _data.LoadData<TeamDb, dynamic>(sql, new { }, _config.GetConnectionString(ConnectionString));

                return dbinfo;
            }
            catch (Exception ex) { return null; }
        }
        //get all Inactive teams from database
        public async Task<List<TeamDb>> getInactiveTeams()
        {
            try
            {
                var sql = "SELECT * FROM Teams WHERE active = false;";

                var dbinfo = await _data.LoadData<TeamDb, dynamic>(sql, new { }, _config.GetConnectionString(ConnectionString));

                return dbinfo;
            }
            catch (Exception ex) { return null; }
        }

        //change a single teams state to active or inactive
        public async Task changeTeamStateTo(string teamId, bool state)
        {
            try
            {
                var sql = "UPDATE Teams SET active = @state WHERE teamId = @teamid;";
                await _data.SaveData(sql, new
                {
                    state = state,
                    teamid = teamId
                },
                _config.GetConnectionString(ConnectionString));
            }
            catch (Exception ex) { }
        }
        //change a list of teams state to active or inactive
        public async Task changeTeamsStateTo(List<TeamDb> teams, bool state)
        {
            try
            {
                foreach (TeamDb team in teams)
                {
                    var sql = "UPDATE Teams SET active = @state WHERE teamId = @teamid;";

                    await _data.SaveData(sql, new
                    {
                        state = state,
                        teamid = team.teamId
                    },
                    _config.GetConnectionString(ConnectionString));
                }

            }
            catch (Exception ex) { }
        }

        // Suggestions
        // Save a single suggestion to database
        public async Task saveSuggestion(SuggestionEntity suggestion)
        {
            try
            {
                var sql = "INSERT INTO Suggestions (owner, creator, sugTitle, sugDesc, status, Category, JustDoIt) values (@owner, @creator, @sugTitle, @sugDesc, @status, @Category, @JustDoIt);";
                await _data.SaveData(sql, new
                {
                    owner = suggestion.owner,
                    creator = suggestion.creator,
                    sugTitle = suggestion.sugTitle,
                    sugDesc = suggestion.sugDesc,
                    status = suggestion.status.ToString(),
                    Category = suggestion.Category,
                    JustDoIt = suggestion.JustDoIt
                },
                _config.GetConnectionString(ConnectionString));
            }
            catch (Exception ex)
            {
     
            }
        }

        // Saves a list of suggestions
        public async Task saveSuggestionList(List<SuggestionEntity> suggestions)
        {
           foreach (SuggestionEntity sug in suggestions)
            {
                await saveSuggestion(sug);
            }
        }

        //get all coulums of a suggestion from database with suggestion id
        //if return = null error
        public async Task<SuggestionEntity?> GetSuggestion(string sugId)
        {
            try
            {
                var sql = "SELECT * FROM Suggestions WHERE sugId = @sugId;";
                SuggestionEntity sug = await _data.LoadData<SuggestionEntity, dynamic>(sql, new { sugId = sugId }, _config.GetConnectionString(ConnectionString), true);
                return sug;
            }
            catch (Exception ex) { return null; }
        }

        //get a list suggestion from database bound to creator id
        //if return = null error
        public async Task<List<SuggestionEntity>?> GetSuggestionsOfCreator(string empId)
        {
            try
            {
                var sql = "SELECT * FROM Suggestions WHERE creator = @creator;";
                List<SuggestionEntity> sug = await _data.LoadData<SuggestionEntity, dynamic>(sql, new { creator = empId }, _config.GetConnectionString(ConnectionString));
                return sug;
            }
            catch (Exception ex) { return null; }   
        }

        //get a list suggestion from database bound to owner id
        //if return = null error
        public async Task<List<SuggestionEntity>?> GetSuggestionOfTeam(string teamId)
        {
            try
            {
                var sql = "SELECT * FROM Suggestions WHERE owner = @owner;";
                List<SuggestionEntity> sug = await _data.LoadData<SuggestionEntity, dynamic>(sql, new { owner = teamId }, _config.GetConnectionString(ConnectionString));
                return sug;
            }
            catch (Exception ex) { return null; }
        }

        //get a list suggestion from database bound to status
        //@param status = a type of SuggStatus (enum class)
        //if return null = error
        public async Task<List<SuggestionEntity>?> GetSuggestionsByStatus(SuggStatus status)
        {
            try
            {
                var sql = "SELECT * FROM Suggestions WHERE status = @status;";
                List<SuggestionEntity> sug = await _data.LoadData<SuggestionEntity, dynamic>(sql, new { status = status.ToString() }, _config.GetConnectionString(ConnectionString));
                return sug;
            }
            catch (Exception ex) { return null; }
        }
        //update a suggestion entry
        public async Task updateSuggestion(SuggestionEntity sug)
        {
            try
            {
                var sql = "UPDATE Suggestions SET owner = @Owner, sugTitle = @SugTitle, sugDesc = @SugDesc, status = @Status, Category = @Category, assigned = @Assigned, deadline = @Deadline WHERE sugId = @SugId;";
                await _data.SaveData(sql, new
                {
                    Owner = sug.owner,
                    SugTitle = sug.sugTitle,
                    SugDesc = sug.sugDesc,
                    Status = sug.status.ToString(),
                    Category = sug.Category,
                    Assigned = sug.assigned,
                    Deadline = sug.deadline,
                    SugId = sug.sugId
                },
                _config.GetConnectionString(ConnectionString));
            }
            catch (Exception ex) { }
        }
    }
}
