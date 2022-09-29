using BlazorTipz.Data;
using DataLibrary;
using System.Linq.Expressions;

namespace BlazorTipz.Models.DbRelay
{
    public class DbRelay
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
        public async Task<UserDb> getSingelUserDbfromDb(string empId)
        {
            string sql = "SELECT * FROM Users WHERE employmentId = " + empId + ";";
            try
            {
                UserDb dbinfo = new UserDb();
                dbinfo = await _data.LoadData<UserDb, dynamic>(sql, new { }, _config.GetConnectionString("default"), true);
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
        
    }
}
