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
        public async Task<UserEntity> getLoginUser(string empId)
        {
            try
            {
                var sql = "SELECT * FROM Users WHERE employmentId = @empid;";
                UserEntity dbinfo = await _data.LoadData<UserEntity, dynamic>(sql, new { empid = empId }, _config.GetConnectionString(ConnectionString), true);
                return dbinfo;
            }
            catch (Exception ex) { return null; }
        }

        public async Task<UserEntity> lookUpUser(string empId)
        {
            try
            {
                var sql = "SELECT name, employmentId FROM Users WHERE employmentId = @empid;";
                UserEntity dbinfo = await _data.LoadData<UserEntity, dynamic>(sql, new { empid = empId }, _config.GetConnectionString(ConnectionString), true);
                return dbinfo;
            }
            catch (Exception ex) { return null; }
        }

        public async Task addUserEntries(List<UserEntity> toSaveUsers)
        {
            try
            {
                foreach (UserEntity tsu in toSaveUsers)
                {
                    var sql = "INSERT IGNORE INTO Users (employmentId, userName, passwordHash, passwordSalt, userRole) VALUES (@EmploymentId, @UserName, @PasswordHash, @PasswordSalt, @UserRole);";

                    await _data.SaveData(sql, new
                    {
                        EmploymentId = tsu.employmentId,
                        UserName = tsu.userName,
                        PasswordSalt = tsu.passwordSalt,
                        PasswordHash = tsu.passwordHash,
                        UserRole = tsu.userRole.ToString()
                    },
                        _config.GetConnectionString(ConnectionString));
                }
            }
            catch (Exception ex) { throw; }
        }
        //update user
        public async Task updateUserEntry(UserEntity toSaveUser)
        {
            string Role = toSaveUser.userRole.ToString();
            try
            {
                if (toSaveUser.passwordHash != null)
                {
                    if (toSaveUser.passwordSalt != null)
                    {
                        var sql = "UPDATE Users SET passwordHash = @PasswordHash, userName = @UserName, passwordSalt = @PasswordSalt, userRole = @UserRole, firstTimeLogin = @FirstTimeLogin WHERE employmentId = @EmploymentId;";

                        await _data.SaveData(sql, new
                        {
                            EmploymentId = toSaveUser.employmentId,
                            UserName = toSaveUser.userName,
                            PasswordSalt = toSaveUser.passwordSalt,
                            PasswordHash = toSaveUser.passwordHash,
                            UserRole = Role,
                            FirstTimeLogin = toSaveUser.firstTimeLogin
                        },
                            _config.GetConnectionString(ConnectionString));
                    }
                }
                else if(toSaveUser.teamId != null)
                {
                    var sql = "UPDATE Users SET userName = @UserName, userRole = @UserRole, teamId = @TeamId WHERE employmentId = @EmploymentId;";

                    await _data.SaveData(sql, new
                    {
                        EmploymentId = toSaveUser.employmentId,
                        UserName = toSaveUser.userName,
                        UserRole = Role,
                        TeamId = toSaveUser.teamId
                    },
                        _config.GetConnectionString(ConnectionString));

                }
                else
                {
                    var sql = "UPDATE Users SET userName = @UserName, userRole = @UserRole WHERE employmentId = @EmploymentId;";

                    await _data.SaveData(sql, new
                    {
                        EmploymentId = toSaveUser.employmentId,
                        UserName = toSaveUser.userName,
                        UserRole = Role
                    },
                        _config.GetConnectionString(ConnectionString));
                }

            }
            catch (Exception ex) { throw; }
        }

        public async Task<List<UserEntity>> getActiveUsers()
        {
            
            try
            {
                var sql = "SELECT * FROM Users WHERE active = true;";

                var dbinfo = await _data.LoadData<UserEntity, dynamic>(sql, new { }, _config.GetConnectionString(ConnectionString));

                return dbinfo;
            }
            catch (Exception ex) { return null; }
            
        }

        public async Task<List<UserEntity>> getInactiveUsers()
        {
            try
            {
                var sql = "SELECT * FROM Users WHERE active = false;";

                var dbinfo = await _data.LoadData<UserEntity, dynamic>(sql, new { }, _config.GetConnectionString(ConnectionString));

                return dbinfo;
            }
            catch (Exception ex) { return null; }
        }

        public async Task changeUserStateTo(string empId, bool state) {
            try
            {
                var sql = "UPDATE Users SET active = @State WHERE employmentId = @Empid;";
                await _data.SaveData(sql, new
                {
                    State = state,
                    Empid = empId
                },
                _config.GetConnectionString(ConnectionString));
            } catch (Exception ex) { throw; }
        }
        public async Task changeUsersStateTo(List<UserEntity> users, bool state)
        {
            try
            {
                foreach (UserEntity user in users)
                {
                    var sql = "UPDATE Users SET active = @State WHERE employmentId = @Empid;";
                    
                    await _data.SaveData(sql, new
                    {
                        State = state,
                        Empid = user.employmentId
                    },
                    _config.GetConnectionString(ConnectionString));
                }
               
            }
            catch (Exception ex) { throw; }
        }
        
        //team/teams

        //get's a single team from database
        public async Task<TeamEntity> getSingleTeamDbFromDb(string teamId)
        {
            try
            {
                var sql = "SELECT * FROM Teams WHERE teamId = @TeamId;";
               
                TeamEntity team = await _data.LoadData<TeamEntity, dynamic>(sql, new { TeamId = teamId }, _config.GetConnectionString(ConnectionString), true);
                return team;
            }
            catch (Exception ex) { return null; }
        }
        
        //add a single team to database
        public async Task addTeamEntry(TeamEntity team)
        {
            try 
            {
                var sql = "INSERT IGNORE INTO Teams (teamName, teamLeader) VALUES (@TeamName, @TeamLeader)";
                await _data.SaveData(sql, new
                {
                    TeamName = team.teamName,
                    TeamLeader = team.teamLeader
                }, _config.GetConnectionString(ConnectionString));
            } 
            catch (Exception ex) { throw; }
        }

        //update a single team in database
        public async Task updateTeamEntry(TeamEntity team)
        {
            try
            {
                var sql = "UPDATE Teams SET teamName = @TeamName, teamLeader = @TeamLeader WHERE teamId = @TeamId";
                await _data.SaveData(sql, new
                {
                    TeamName = team.teamName,
                    TeamLeader = team.teamLeader,
                    TeamId = team.teamId
                }, _config.GetConnectionString(ConnectionString));
            }
            catch (Exception ex) { throw; }
        }
        
        //get all Active teams from database
        public async Task<List<TeamEntity>> getActiveTeams()
        {
            try
            {
                var sql = "SELECT * FROM Teams WHERE active = true;";

                var dbinfo = await _data.LoadData<TeamEntity, dynamic>(sql, new { }, _config.GetConnectionString(ConnectionString));

                return dbinfo;
            }
            catch (Exception ex) { return null; }
        }
        //get all Inactive teams from database
        public async Task<List<TeamEntity>> getInactiveTeams()
        {
            try
            {
                var sql = "SELECT * FROM Teams WHERE active = false;";

                var dbinfo = await _data.LoadData<TeamEntity, dynamic>(sql, new { }, _config.GetConnectionString(ConnectionString));

                return dbinfo;
            }
            catch (Exception ex) { return null; }
        }

        //change a single teams state to active or inactive
        public async Task changeTeamStateTo(string teamId, bool state)
        {
            try
            {
                var sql = "UPDATE Teams SET active = @State WHERE teamId = @TeamId;";
                await _data.SaveData(sql, new
                {
                    State = state,
                    TeamId = teamId
                },
                _config.GetConnectionString(ConnectionString));
            }
            catch (Exception ex) { throw; }
        }
        //change a list of teams state to active or inactive
        public async Task changeTeamsStateTo(List<TeamEntity> teams, bool state)
        {
            try
            {
                foreach (TeamEntity team in teams)
                {
                    var sql = "UPDATE Teams SET active = @State WHERE teamId = @TeamId;";

                    await _data.SaveData(sql, new
                    {
                        State = state,
                        TeamId = team.teamId
                    },
                    _config.GetConnectionString(ConnectionString));
                }

            }
            catch (Exception ex) { throw; }
        }

        // Suggestions
        // Save a single suggestion to database
        public async Task saveSuggestion(SuggestionEntity suggestion)
        {
            try
            {
                var sql = "INSERT INTO Suggestions (ownerId, creatorId, sugTitle, sugDesc, sugStatus, categoryId, justDoIt, assignedId) values (@OwnerId, @CreatorId, @SugTitle, @SugDesc, @SugStatus, @CategoryId, @JustDoIt, @AssignedId);";
                await _data.SaveData(sql, new
                {
                    OwnerId = suggestion.ownerId,
                    CreatorId = suggestion.creatorId,
                    SugTitle = suggestion.sugTitle,
                    SugDesc = suggestion.sugDesc,
                    SugStatus = suggestion.sugStatus.ToString(),
                    CategoryId = suggestion.categoryId,
                    JustDoIt = suggestion.justDoIt,
                    AssignedId = suggestion.assignedId
                },
                _config.GetConnectionString(ConnectionString));
            }
            catch (Exception ex) { throw; }
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
                var sql = "SELECT s.sugId, s.ownerId, s.creatorId, s.assignedId, s.completerId, s.sugStatus, s.sugProgression, s.sugTitle, s.sugDesc, s.createdAt, s.lastChanged, s.dueDate, s.justDoIt, s.beforeImage, s.afterImage, s.categoryId, c.catName, t.teamName, u1.userName AS creatorName, u2.userName AS assignedName FROM Suggestions s LEFT JOIN Categories c ON s.categoryId = c.catId LEFT JOIN Teams t ON s.ownerId = t.teamId LEFT JOIN Users u1 ON s.creatorId = u1.employmentId LEFT JOIN Users u2 ON s.assignedId = u2.employmentId WHERE s.sugId = @SugId;";
                SuggestionEntity sug = await _data.LoadData<SuggestionEntity, dynamic>(sql, new { SugId = sugId }, _config.GetConnectionString(ConnectionString), true);
                return sug;
            }
            catch (Exception ex) { return null; }
        }
        //Get a list of suggestion from database bound to creator id
        //if return = null error
        public async Task<List<SuggestionEntity>?> GetSuggestionsOfCreator(string empId)
        {
            try
            {
                var sql = "SELECT s.sugId, s.ownerId, s.creatorId, s.assignedId, s.completerId, s.sugStatus, s.sugProgression, s.sugTitle, s.sugDesc, s.createdAt, s.lastChanged, s.dueDate, s.justDoIt, s.beforeImage, s.afterImage, s.categoryId, c.catName, t.teamName, u1.userName AS creatorName, u2.userName AS assignedName FROM Suggestions s LEFT JOIN Categories c ON s.categoryId = c.catId LEFT JOIN Teams t ON s.ownerId = t.teamId LEFT JOIN Users u1 ON s.creatorId = u1.employmentId LEFT JOIN Users u2 ON s.assignedId = u2.employmentId WHERE s.creatorId = @CreatorId;";
                List<SuggestionEntity> sug = await _data.LoadData<SuggestionEntity, dynamic>(sql, new { CreatorId = empId }, _config.GetConnectionString(ConnectionString));
                return sug;
            }
            catch (Exception ex) { return null; }   
        }
        //Get a list of suggestion from database bound to creator id filtered by status
        public async Task<List<SuggestionEntity>?> GetSuggestionsOfCreator(string empId, SuggStatus status)
        {
            try
            {
                var sql = "SELECT s.sugId, s.ownerId, s.creatorId, s.assignedId, s.completerId, s.sugStatus, s.sugProgression, s.sugTitle, s.sugDesc, s.createdAt, s.lastChanged, s.dueDate, s.justDoIt, s.beforeImage, s.afterImage, s.categoryId, c.catName, t.teamName, u1.userName AS creatorName, u2.userName AS assignedName FROM Suggestions s LEFT JOIN Categories c ON s.categoryId = c.catId LEFT JOIN Teams t ON s.ownerId = t.teamId LEFT JOIN Users u1 ON s.creatorId = u1.employmentId LEFT JOIN Users u2 ON s.assignedId = u2.employmentId WHERE s.creatorId = @CreatorId AND s.sugStatus = @SugStatus;";
                List<SuggestionEntity> sug = await _data.LoadData<SuggestionEntity, dynamic>(sql, new { CreatorId = empId, SugStatus = status.ToString() }, _config.GetConnectionString(ConnectionString));
                return sug;
            }
            catch (Exception ex) { return null; }
        }

        //Get a list of suggestions from database bound to assigned id 
        public async Task<List<SuggestionEntity>?> GetAssignedSuggestions(string empId)
        {
            try
            {
                var sql = "SELECT s.sugId, s.ownerId, s.creatorId, s.assignedId, s.completerId, s.sugStatus, s.sugProgression, s.sugTitle, s.sugDesc, s.createdAt, s.lastChanged, s.dueDate, s.justDoIt, s.beforeImage, s.afterImage, s.categoryId, c.catName, t.teamName, u1.userName AS creatorName, u2.userName AS assignedName FROM Suggestions s LEFT JOIN Categories c ON s.categoryId = c.catId LEFT JOIN Teams t ON s.ownerId = t.teamId LEFT JOIN Users u1 ON s.creatorId = u1.employmentId LEFT JOIN Users u2 ON s.assignedId = u2.employmentId WHERE s.assignedId = @EmpId;";
                List<SuggestionEntity> sug = await _data.LoadData<SuggestionEntity, dynamic>(sql, new { EmpId = empId }, _config.GetConnectionString(ConnectionString));
                return sug;
            }
            catch (Exception ex) { return null; }
        }
        //Get a list of suggestions from database bound to assigned id filtered by status
        public async Task<List<SuggestionEntity>?> GetAssignedSuggestions(string empId, SuggStatus status) 
        {
            try
            {
                var sql = "SELECT s.sugId, s.ownerId, s.creatorId, s.assignedId, s.completerId, s.sugStatus, s.sugProgression, s.sugTitle, s.sugDesc, s.createdAt, s.lastChanged, s.dueDate, s.justDoIt, s.beforeImage, s.afterImage, s.categoryId, c.catName, t.teamName, u1.userName AS creatorName, u2.userName AS assignedName FROM Suggestions s LEFT JOIN Categories c ON s.categoryId = c.catId LEFT JOIN Teams t ON s.ownerId = t.teamId LEFT JOIN Users u1 ON s.creatorId = u1.employmentId LEFT JOIN Users u2 ON s.assignedId = u2.employmentId WHERE s.assignedId = @EmpId AND sugStatus = @Status;";
                List<SuggestionEntity> sug = await _data.LoadData<SuggestionEntity, dynamic>(sql, new { EmpId = empId, Status = status.ToString() }, _config.GetConnectionString(ConnectionString));
                return sug;
            }
            catch (Exception ex) { return null; }
        }

        //Get a list of suggestion from database bound to team id
        //if return = null error
        public async Task<List<SuggestionEntity>?> GetSuggestionOfTeam(string teamId)
        {
            try
            {
                var sql = "SELECT s.sugId, s.ownerId, s.creatorId, s.assignedId, s.completerId, s.sugStatus, s.sugProgression, s.sugTitle, s.sugDesc, s.createdAt, s.lastChanged, s.dueDate, s.justDoIt, s.beforeImage, s.afterImage, s.categoryId, c.catName, t.teamName, u1.userName AS creatorName, u2.userName AS assignedName FROM Suggestions s LEFT JOIN Categories c ON s.categoryId = c.catId LEFT JOIN Teams t ON s.ownerId = t.teamId LEFT JOIN Users u1 ON s.creatorId = u1.employmentId LEFT JOIN Users u2 ON s.assignedId = u2.employmentId WHERE s.ownerId = @OwnerId;";
                List<SuggestionEntity> sug = await _data.LoadData<SuggestionEntity, dynamic>(sql, new { OwnerId = teamId }, _config.GetConnectionString(ConnectionString));
                return sug;
            }
            catch (Exception ex) { return null; }
        }
        //Get a list of suggestion from database bound to team id filtered by status
        public async Task<List<SuggestionEntity>?> GetSuggestionOfTeam(string teamId, SuggStatus status)
        {
            try
            {
                var sql = "SELECT s.sugId, s.ownerId, s.creatorId, s.assignedId, s.completerId, s.sugStatus, s.sugProgression, s.sugTitle, s.sugDesc, s.createdAt, s.lastChanged, s.dueDate, s.justDoIt, s.beforeImage, s.afterImage, s.categoryId, c.catName, t.teamName, u1.userName AS creatorName, u2.userName AS assignedName FROM Suggestions s LEFT JOIN Categories c ON s.categoryId = c.catId LEFT JOIN Teams t ON s.ownerId = t.teamId LEFT JOIN Users u1 ON s.creatorId = u1.employmentId LEFT JOIN Users u2 ON s.assignedId = u2.employmentId WHERE s.ownerId = @OwnerId AND s.sugStatus = @Status;";
                List<SuggestionEntity> sug = await _data.LoadData<SuggestionEntity, dynamic>(sql, new { OwnerId = teamId, Status = status.ToString() }, _config.GetConnectionString(ConnectionString));
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
                var sql = "SELECT s.sugId, s.ownerId, s.creatorId, s.assignedId, s.completerId, s.sugStatus, s.sugProgression, s.sugTitle, s.sugDesc, s.createdAt, s.lastChanged, s.dueDate, s.justDoIt, s.beforeImage, s.afterImage, s.categoryId, c.catName, t.teamName, u1.userName AS creatorName, u2.userName AS assignedName FROM Suggestions s LEFT JOIN Categories c ON s.categoryId = c.catId LEFT JOIN Teams t ON s.ownerId = t.teamId LEFT JOIN Users u1 ON s.creatorId = u1.employmentId LEFT JOIN Users u2 ON s.assignedId = u2.employmentId WHERE s.sugStatus = @SugStatus;";
                List<SuggestionEntity> sug = await _data.LoadData<SuggestionEntity, dynamic>(sql, new { SugStatus = status.ToString() }, _config.GetConnectionString(ConnectionString));
                return sug;
            }
            catch (Exception ex) { return null; }
        }
        //update a suggestion entry
        public async Task updateSuggestion(SuggestionEntity sug)
        {
            try
            {
                var sql = "UPDATE Suggestions SET ownerId = @OwnerId, sugTitle = @SugTitle, sugDesc = @SugDesc, sugStatus = @SugStatus, categoryId = @CategoryId, assignedId = @AssignedId, dueDate = @DueDate WHERE sugId = @SugId;";
                await _data.SaveData(sql, new
                {
                    OwnerId = sug.ownerId,
                    SugTitle = sug.sugTitle,
                    SugDesc = sug.sugDesc,
                    SugStatus = sug.sugStatus.ToString(),
                    CategoryId = sug.categoryId,
                    AssignedId = sug.assignedId,
                    DueDate = sug.dueDate,
                    SugId = sug.sugId
                },
                _config.GetConnectionString(ConnectionString));
            }
            catch (Exception ex) { throw; }
        }

        public Task SaveComment(CommentEntity comment)
        {
            try
            {
                var sql = "INSERT INTO Comments (employmentId, sugId, content) values (@EmpId, @SugId, @Content);";
                return _data.SaveData(sql, new
                {
                    EmpId = comment.employmentId,
                    SugId = comment.sugId,
                    Content = comment.content
                },
                _config.GetConnectionString(ConnectionString));
            }
            catch (Exception ex) { throw; }
        }

        public Task UpdateComment(CommentEntity comment)
        {
            try
            {
                var sql = "UPDATE Comments SET content = @Content, active = @Active WHERE employmentId = @EmpId AND sugId = @SugId AND createdAt = @CreatedAt;";
                return _data.SaveData(sql, new
                {
                    EmpId = comment.employmentId,
                    SugId = comment.sugId,
                    CreatedAt = comment.createdAt,
                    Content = comment.content,
                    Active = comment.active
                },
                _config.GetConnectionString(ConnectionString));
            }
            catch (Exception ex) { throw; }
        }

        public Task<List<CommentEntity>> GetCommentsOfSuggestion(string sugId)
        {
            try
            {
                var sql = "SELECT c.employmentId, c.sugId, c.createdAt, c.content, c.active, Users.userName FROM Comments c INNER JOIN Users ON c.employmentId = Users.employmentId WHERE c.sugId = @SugId ORDER BY createdAt DESC;";
                return _data.LoadData<CommentEntity, dynamic>(sql, new { SugId = sugId }, _config.GetConnectionString(ConnectionString));
            }
            catch (Exception ex) { throw; }
        }

        public Task<List<CommentEntity>> GetCommentsOfUser(string empId)
        {
            try
            {
                var sql = "SELECT c.employmentId, c.sugId, c.createdAt, c.content, c.active, Users.userName FROM Comments c INNER JOIN Users ON c.employmentId = Users.employmentId WHERE c.employmentId = @EmpId ORDER BY createdAt DESC;";
                return _data.LoadData<CommentEntity, dynamic>(sql, new { EmpId = empId }, _config.GetConnectionString(ConnectionString));
            }
            catch (Exception ex) { throw; }
        }
    }
}
