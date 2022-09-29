using BlazorTipz.Data;
using DataLibrary;
using System.Linq.Expressions;

namespace BlazorTipz.Models.DbRelay
{
    public class DbRelay : IDbRelay
    {
        private readonly IDataAccess _data;
        private readonly IConfiguration _config;

        public DbRelay(IDataAccess data, IConfiguration connectionString)
        {
            _data = data;
            _config = connectionString;
        }

        //methodes for getting, setting and updating the database
        
        //User/Users
        public async Task<UserDb> getUser(string empId)
        {
            
            try
            {
                var sql = "SELECT * FROM Users WHERE employmentId = @empid;";
                
                UserDb dbinfo = await _data.LoadData<UserDb, dynamic>(sql, new { empid = empId }, _config.GetConnectionString("default"), true);
                return dbinfo;
            }
            catch (Exception ex) {
                return null;
            }
        }

        public async Task addUserEntryToDbFromUserDb(UserDb toSaveUser)
        {
            try
            {
                var sql = "insert into Users (employmentId, passwordHash, passwordSalt, role) values (@employmentId, @passwordHash, @passwordSalt, @role);";

                await _data.SaveData(sql, new { 
                    employmentId = toSaveUser.employmentId,
                    passwordSalt = toSaveUser.passwordSalt, 
                    passwordHash = toSaveUser.passwordHash, 
                    role = toSaveUser.role },
                    _config.GetConnectionString("default"));

            }
            catch (Exception ex) { 
            
            }
        }
        //update user
        public async Task updateUserEntryToDbFromUserDb(UserDb toSaveUser)
        {
            try
            {
                var sql = "update Users set passwordHash = @passwordHash, passwordSalt = @passwordSalt, role = @role where employmentId = @employmentId;";

                await _data.SaveData(sql, new
                {
                    employmentId = toSaveUser.employmentId,
                    passwordSalt = toSaveUser.passwordSalt,
                    passwordHash = toSaveUser.passwordHash,
                    role = toSaveUser.role
                },
                    _config.GetConnectionString("default"));

            }
            catch (Exception ex)
            {

            }
        }

        public async Task<List<UserDb>> getActiveUsers()
        {
            
            try
            {
                var sql = "SELECT * FROM Users WHERE active = true;";

                var dbinfo = await _data.LoadData<UserDb, dynamic>(sql, new { }, _config.GetConnectionString("default"));

                return dbinfo;
            }
            catch (Exception ex) { return null; }
            
        }

        public async Task<List<UserDb>> getInactiveUsers()
        {
            try
            {
                var sql = "SELECT * FROM Users WHERE active = false;";

                var dbinfo = await _data.LoadData<UserDb, dynamic>(sql, new { }, _config.GetConnectionString("default"));

                return dbinfo;
            }
            catch (Exception ex) { return null; }
        }

        public async Task changeUserStateTo(string empid, bool state) {
            try
            {
                var sql = "UPDATE Users SET active = @state WHERE employmentId = @empid;";
                await _data.SaveData(sql, new
                {
                    active = state,
                    employmentId = empid
                },
                _config.GetConnectionString("default"));
            } catch (Exception ex) { }
        }
        public async Task changeUsersStateTo(List<UserA> users, bool state)
        {
            try
            {
                foreach (UserA user in users)
                {
                    var sql = "UPDATE Users SET active = @state WHERE employmentId = @empid;";
                    
                    await _data.SaveData(sql, new
                    {
                        active = state,
                        employmentId = user.employmentId
                    },
                    _config.GetConnectionString("default"));
                }
               
            }
            catch (Exception ex) { }
        }
        
        //team/teams

        //get's a single team from database
        public async Task<TeamA> getSingleTeamDbFromDb(string teamId)
        {
            try
            {
                var sql = "SELECT * FROM Teams WHERE teamId = @teamid;";
               
                TeamDb team = await _data.LoadData<TeamDb, dynamic>(sql, new { teamid = teamId }, _config.GetConnectionString("default"), true);
                return team;
            }
            catch (Exception ex) { return null; }
        }
        
        //add a single team to database
        public async Task addTeamEntryToDbFromTeamDb(TeamDb team)
        {
            try 
            {
                var sql = "INSERT INTO Team (teamName, teamLeader) VALUES (@teamName, @teamLeader)";
                await _data.SaveData(sql, new
                {
                    teamName = team.teamName,
                    teamLeader = team.teamLeader
                }, _config.GetConnectionString("default"));
            } 
            catch (Exception ex) { }
        }

        //update a single team in database
        public async Task updateTeamEntryToDbFromTeamDb(TeamDb team)
        {
            try
            {
                var sql = "UPDATE Team SET teamName = @teamName, teamLeader = @teamLeader WHERE teamId = @teamId";
                await _data.SaveData(sql, new
                {
                    teamName = team.teamName,
                    teamLeader = team.teamLeader,
                    teamId = team.teamid
                }, _config.GetConnectionString("default"));
            }
            catch (Exception ex) { }
        }

        //get all Active teams from database
        public async Task<List<TeamDb>> getActiveTeams()
        {
            try
            {
                var sql = "SELECT * FROM Teams WHERE active = true;";

                var dbinfo = await _data.LoadData<TeamDb, dynamic>(sql, new { }, _config.GetConnectionString("default"));

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

                var dbinfo = await _data.LoadData<TeamDb, dynamic>(sql, new { }, _config.GetConnectionString("default"));

                return dbinfo;
            }
            catch (Exception ex) { return null; }
        }

        //change a single teams state to active or inactive
        public async Task changeTeamStateTo(string teamid, bool state)
        {
            try
            {
                var sql = "UPDATE Teams SET active = @state WHERE teamId = @teamid;";
                await _data.SaveData(sql, new
                {
                    active = state,
                    teamId = teamid
                },
                _config.GetConnectionString("default"));
            }
            catch (Exception ex) { }
        }
        //change a list of teams state to active or inactive
        public async Task changeTeamsStateTo(List<TeamA> teams, bool state)
        {
            try
            {
                foreach (TeamA team in teams)
                {
                    var sql = "UPDATE Teams SET active = @state WHERE teamId = @teamid;";

                    await _data.SaveData(sql, new
                    {
                        active = state,
                        teamId = team.teamid
                    },
                    _config.GetConnectionString("default"));
                }

            }
            catch (Exception ex) { }
        }


    }
}
