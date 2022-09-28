using BlazorTipz.Data;
using DataLibrary;
using System.Linq.Expressions;

namespace BlazorTipz.Models.DbManager
{
    public class DbManager : IDbManager
    {
        private readonly IDataAccess _data;
        private readonly IConfiguration _config;

        public DbManager(IDataAccess data, IConfiguration connectionString)
        {
            _data = data;
            _config = connectionString;
        }

        public async Task<UserDb> getSingelUserDbfromDb(string empId)
        {
            string sql = "SELECT * FROM Users WHERE employmentId = " + empId + ";";
            try
            {
                UserDb dbinfo = new UserDb();
                UserDb dbInfo = await _data.LoadData<UserDb, dynamic>(sql, new { }, _config.GetConnectionString("default"), true);
                return dbInfo;
            }
            catch (Exception ex) {
                return null;
            }
        }

        public async Task addUserEntryToDbFromUserDb(UserDb toSUser)
        {
            try
            {
                var sql = "insert into Users (employmentId, passwordHash, passwordSalt, role) values (@employmentId, @passwordHash, @passwordSalt, @role);";

                await _data.SaveData(sql, new { employmentId = toSUser.employmentId, passwordSalt = toSUser.passwordSalt, passwordHash = toSUser.passwordHash, role = toSUser.role }, _config.GetConnectionString("default"));

            }
            catch (Exception ex) { 
            
            }
        }
    }
}
