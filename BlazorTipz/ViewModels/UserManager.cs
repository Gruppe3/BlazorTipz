using BlazorTipz.Models;
using BlazorTipz.ViewModels.User;
using DataLibrary;

namespace BlazorTipz.ViewModels
{
    public class UserManager : IUserManager
    {
        private readonly IDataAccess _data;
        private readonly IConfiguration _config;

        public UserManager(IDataAccess data, IConfiguration connectionString)
        {
            _data = data;
            _config = connectionString;
        }
        public async Task Login(UserViewmodel user, out string token, out string err)
        {
            token = "";
            err = "";
            try
            {
                var sql = "SELECT * FROM Users WHERE employmentId = @empid;";

                UserDb dbinfo = await _data.LoadData<UserDb, dynamic>(sql, new { empid = user.employmentId }, _config.GetConnectionString("default"), true);
                if (dbinfo == null)
                {
                    err = "User not found";
                    return;
                }
                if (!VerifyPasswordHash(user.password, dbinfo.passwordHash, dbinfo.passwordSalt))
                {
                    err = "Wrong password";
                    return;
                }
                token = GenerateToken(dbinfo);
            }
            catch (Exception ex)
            {
                err = ex.Message;
            }
        }
        {

        }
    }
}
