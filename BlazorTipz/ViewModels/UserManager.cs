using BlazorTipz.Components;
using BlazorTipz.Models;
using BlazorTipz.Models.DbRelay;
using BlazorTipz.ViewModels.User;


namespace BlazorTipz.ViewModels
{
    public class UserManager : IUserManager
    {
        private readonly IDbRelay _DBR;
        private readonly AuthenticationComponent _Auth;
        public UserManager(IDbRelay DBR, AuthenticationComponent auth)
        {
            _DBR = DBR;
            _Auth = auth;
        }
        public async Task<(string, string)> Login(UserViewmodel user)
        {
            UserDb tryUser = new UserDb(user);
            UserDb dbUser = await _DBR.getUser(tryUser.employmentId);
            string token;
            string err;
            if (dbUser == null)
            {
                token = null;
                err = "User not found";
                return (token, err);
            }
            if (_Auth.VerifyPasswordHash(user.password, dbUser.passwordHash, dbUser.passwordSalt))
            {
                dbUser.CreateToken();
                token = dbUser.AuthToken;
                err = null;
                return (token, err);
            }
            else
            {
                token = null;
                err = "Wrong password";
                return (token, err);
            }
        }
        public async Task<string> registerUserSingel(UserViewmodel toRegisterUser)
        {
            string err = null;
            if (toRegisterUser == null) { err = "No user to register"; return err; };
            if (toRegisterUser.employmentId == null) { err = "no emplayment Id"; return err; };
            if (toRegisterUser.password == null) { err = "no password given"; return err; };
            
            UserDb userDb = await _DBR.getUser(toRegisterUser.employmentId);
            if (userDb != null) { err = "User alrady exists"; return err; }
            
            UserDb toSaveUser = new UserDb(toRegisterUser);
            List<UserDb> toSave = new List<UserDb>();
            toSave.Add(toSaveUser);
            if (toSave.Count == 0) { err = "somthing went wrong"; return err; };
            
            await _DBR.addUserEntries(toSave);
            err = "succsess";
            return err;
        }
    }        
}
