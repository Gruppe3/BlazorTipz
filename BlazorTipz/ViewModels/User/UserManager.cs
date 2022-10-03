using BlazorTipz.Components;
using BlazorTipz.Data;
using BlazorTipz.Models;
using BlazorTipz.Models.DbRelay;


namespace BlazorTipz.ViewModels.User
{
    public class UserManager : IUserManager
    {
        private readonly IDbRelay _DBR;
        private readonly AuthenticationComponent _Auth;

        public UserViewmodel? CurrentUser { get; set; }
        public List<UserViewmodel> activeUsers;
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
                UserViewmodel userView = new UserViewmodel(dbUser);
                CurrentUser = userView;

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
            getUsers();
            err = "succsess";
            return err;
        }

        public async Task<(UserViewmodel, string)> getCurrentUser(string token)
        {
            string err = null;
            string empId = _Auth.GetClaimValue(token);
            UserDb user = await _DBR.getUser(empId);
            if (user == null) { err = "User not found"; return (null, err); };
            CurrentUser = new UserViewmodel(user);
            getUsers();
            return (CurrentUser, err);
        }
        public void logout()
        {
            CurrentUser = null;
        }
        //need fix later, might not work in some cases
        public UserViewmodel getCurrentUser()
        {
            return CurrentUser;
        }

        public async Task<string> updateCurrentUser(UserViewmodel user)
        {
            string err = null;
            if (user == null) { err = "No user to update"; return err; };
            if (user.password == null) { err = "no password given"; return err; };
            if (user.password != user.RepeatPassword) { err = "passwords dont match"; return err; };
            if (CurrentUser == null) { err = "not logged in correctly"; return err; }

            CurrentUser.password = user.password;
            CurrentUser.name = user.name;
            if (CurrentUser.employmentId == null) { err = "no emplayment Id"; return err; };

            UserDb toSave = new UserDb(CurrentUser);
            if (toSave == null) { err = "Application err"; return err; };
            await _DBR.updateUserEntry(toSave);

            return err;
        }
        
        //get all active users
        public async Task<List<UserViewmodel>?> getUsers()
        {

            List<UserDb> dblist = await _DBR.getActiveUsers();
            if (dblist == null) { return null; }
            List<UserViewmodel> ActUsers = new List<UserViewmodel>();
            foreach (UserDb u in dblist)
            {
                ActUsers.Add(new UserViewmodel(u));
            }
            activeUsers = ActUsers;
            
            return activeUsers;
        }
        
        public async Task<List<UserViewmodel>> GetUsers()
        {
            if(activeUsers == null)
            {
                await getUsers();
            }
            return activeUsers;
        }
        
        public async Task updateRole(UserViewmodel user, RoleE role, bool upgradeRole)
        {
            if(user.employmentId == string.Empty) { return; }
            if(upgradeRole) 
            {
                if (role <= user.role) { return; }
                user.role = role;
                await _DBR.updateUserEntry(new UserDb(user));
                await getUsers();
            }
            else
            {
                user.role = role;
                await _DBR.updateUserEntry(new UserDb(user));
                await getUsers();
            }
            
        }
        public async Task<UserViewmodel?> getUser(string empid)
        {
            if(activeUsers != null)
            {
                foreach(UserViewmodel u in activeUsers)
                {
                    if(u.employmentId == empid)
                    {
                        return u;
                    }
                }
                return null;
            }
            else
            {
                List<UserViewmodel> searchlist = await getUsers();
                foreach(UserViewmodel u in searchlist)
                {
                    if (u.employmentId == empid)
                    {
                        return u;
                    }
                }
                return null;
            }
        }
        public async Task updateUserTeam(string empid, string teamId)
        {
            if (activeUsers != null)
            {
                foreach (UserViewmodel u in activeUsers)
                {
                    if (u.employmentId == empid)
                    {
                        u.teamId = teamId;
                        await _DBR.updateUserEntry(new UserDb(u));
                        break;
                    }
                }
                
            }
            else
            {
                List<UserViewmodel> searchlist = await getUsers();
                foreach (UserViewmodel u in searchlist)
                {
                    if (u.employmentId == empid)
                    {
                        u.teamId = teamId;
                        await _DBR.updateUserEntry(new UserDb(u));
                        break;
                    }
                }
                
            }

        }

    }
}
