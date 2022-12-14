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

        //Current user logged in
        public UserViewmodel? CurrentUser { get; set; }

        //A list of all active users
        public List<UserViewmodel>? ActiveUsers { get; set; }
        
        //A list of new users to register
        private List<UserViewmodel>? UsersToRegister {  get; set; } = new();

        //A list of all eksisting users
        private List<UserEntity> EksistingUsers { get; set; } = new();
        //constructor
        
        public UserManager(IDbRelay DBR, AuthenticationComponent auth)
        {
            _DBR = DBR;
            _Auth = auth;
        }

        //Login function
        public async Task<(string?, string?)> Login(UserViewmodel user)
        {
            string? token;
            string? err;
            
            //User entity
            UserEntity tryUser = new(user);
            UserEntity? dbUser;
            //Sends emplyment id to userdb through interface relay
            try
            {
                dbUser = await _DBR.GetLoginUser(tryUser.employmentId);
            }
            catch (Exception e)
            {
                err = "Noe gikk galt, prøv igjen.";
                Console.WriteLine(e);
                return (null, err);
            }
            

            //If doesn´t exist
            if (dbUser == null)
            {
                token = null;
                err = "Bruker ikke funnet";
                return (token, err);
            }
            
            //Verifies password typed in
            if (_Auth.VerifyPasswordHash(user.Password, dbUser.passwordHash, dbUser.passwordSalt))
            {
                if (!dbUser.active)
                {
                    token = null;
                    err = "Bruker er deaktivert";
                    return (token, err);
                }
                dbUser.CreateToken();

                //setter token
                token = dbUser.AuthToken;
                CurrentUser = new(dbUser);
                
                err = null;
                return (token, err);
            }
            //If it does not match it´s wrong
            else
            {
                token = null;
                err = "Feil passord";
                return (token, err);
            }
        }
        private (bool,string?) CheckUserDataBeforeReg(UserViewmodel user)
        {
            
            string? err = null;
            if (user == null) { err = "Ingen bruker å registrere"; return (false, err); };
            if (user.EmploymentId == null || user.EmploymentId == "") { err = "Ingen AnsattNr"; return (false, err); };
            if (user.Name == null || user.Name == "") { err = "Ingen navn"; return (false, err); };
            if (user.Password == null || user.Password == "") { err = "Ingen passord gitt"; return (false, err); };
            return (true, null);
        }
        private async Task UpdateEksistingUsers()
        {
            EksistingUsers = await _DBR.GetAllUsers();
        }
        private bool SearchIfMatchEksisitingUsers(string userToCheckID)
        {
            bool match = false;
            foreach(UserEntity e in EksistingUsers)
            {
                if(e.employmentId == userToCheckID)
                {
                    match = true;
                    break;
                }
            }
            return match;
        }
    
        //register singel user function
        //first return "string?" = errmsg, second return "string?" = sucsessMsg
        public async Task<(string?,string?)> RegisterUserSingel(UserViewmodel toRegisterUser)
        {
            (bool passed, string? err)=CheckUserDataBeforeReg(toRegisterUser);
            if (!passed) { return (err, null); }

            UserEntity userDb = await _DBR.LookUpUser(toRegisterUser.EmploymentId);
            if (userDb != null) { err = "User alrady exists"; return (err, null); }

            UserEntity toSaveUser = new(toRegisterUser);
            List<UserEntity> toSave = new();
            toSave.Add(toSaveUser);
            if (toSave.Count == 0) { err = "noe gikk galt"; return (err, null); };

            await _DBR.AddUserEntries(toSave);
            await GetActiveUsersFromDBR();
            string suc = "succsess";
            return (err, suc);
        }

        //take in a list of users, and registers them
        //first return "string?" = errmsg, second return "string?" = sucsessMsg
        public async Task<(string?,string?)> RegisterMultiple(List<UserViewmodel>? usersToReg)
        {
            await UpdateEksistingUsers();
            int itNum = 1;
            List<UserEntity> eksistingUsers = await _DBR.GetAllUsers();
            List<UserEntity> toSave = new();
            if (usersToReg != null) 
            {
                foreach(UserViewmodel user in usersToReg)
                {

                    (bool check1, string? err1) = CheckUserDataBeforeReg(user);
                    bool check2 = SearchIfMatchEksisitingUsers(user.EmploymentId);
                    if (!check1) { 
                        return ("Nr: " + itNum + ", Feilet med: " + err1, null); 
                    }else if (check2)
                    {
                        return ("Nr: " + itNum + ", Feilet med: Bruker eksistrer allerede", null);
                    }
                    UserEntity entity = new(user);
                    toSave.Add(entity);
                    itNum++;
                }                
            } 
            else if (UsersToRegister != null && UsersToRegister.Count > 0)
            {
                foreach (UserViewmodel user in UsersToRegister)
                {
                    (bool check1, string? err1) = CheckUserDataBeforeReg(user);
                    bool check2 = SearchIfMatchEksisitingUsers(user.EmploymentId);
                    if (!check1)
                    {
                        return ("Nr: " + itNum + ", Feilet med: " + err1, null);
                    }
                    else if (check2)
                    {
                        return ("Nr: " + itNum + ", Feilet med: Bruker eksistrer allerede", null);
                    }
                    UserEntity entity = new(user);
                    toSave.Add(entity);
                    itNum++;
                }
            }
            else
            {
                return ("No one to register",null);
            }
            if (toSave.Count == 0) { string? err = "noe gikk galt"; return (err, null); };

            await _DBR.AddUserEntries(toSave);
            await GetActiveUsersFromDBR();
            string suc = "Succsess";
            return (null, suc);
        }
        public List<UserViewmodel> GetRegisterUserList()
        {
            if (UsersToRegister == null)
            {
                List<UserViewmodel> list = new();
                UsersToRegister = list;
            }
            return UsersToRegister;
        }
        public string StageToRegisterList(UserViewmodel user)
        {
            if (user == null) { return "no user to stage"; }
            if (user.EmploymentId==null|| user.EmploymentId == string.Empty) { return "Not supplied EmploymentID"; }
            if (user.Name == string.Empty) { return "Not supplied a name"; }
            if (user.Password == string.Empty || user.Password == "" ) { return "Not supplied a password"; }
            //check if user is in list, update instead of add
            bool hit = false;
            foreach(UserViewmodel u in UsersToRegister)
            {
                if (u.EmploymentId == user.EmploymentId)
                {
                    u.Name = user.Name;
                    u.Password = user.Password;
                    u.UserRole = user.UserRole;
                    hit = true; break;
                }
            }
            if (!hit)
            {
                //adds new user to list
                user.ListNum = UsersToRegister.Count + 1;
                UsersToRegister.Add(user);
                return "User succsessfully added to list of pepole to register";
            } else { return "User in list updated"; }
        }
        private void UpdateListnum()
        {
            foreach (UserViewmodel u in UsersToRegister)
            {
                u.ListNum = UsersToRegister.IndexOf(u) + 1;
            }
        }
        //deletes a specified element form usersToRegister list.
        public void DeleteFromRegisterList(string emipd)
        {
            int i = 0;
            //search for user to delete
            foreach(UserViewmodel user in UsersToRegister)
            {
                if(user.EmploymentId == emipd)
                {
                    UsersToRegister.RemoveAt(i);
                    UpdateListnum();
                    break;
                }
                i++;
            }
            
        }

        //Get the current user with the given token.
        public async Task<(UserViewmodel, string)> GetCurrentUser(string token)
        {
            string err = null;
            string empId = _Auth.GetClaimValue(token);
            UserEntity user = await _DBR.GetLoginUser(empId);
            if (user == null) { err = "User not found"; return (null, err); };
            CurrentUser = new(user);
            await GetActiveUsersFromDBR();
            return (CurrentUser, err);
        }
        public void Logout()
        {
            CurrentUser = null;
        }
        //need fix later, might not work in some cases
        public UserViewmodel GetCurrentUser()
        {
            return CurrentUser;
        }
            
        
        //Updates current user
        public async Task<string> UpdateCurrentUser(UserViewmodel user)
        {
            string err = null;
            if (user == null) { err = "No user to update"; return err; };
            if (user.Password == null) { err = "no password given"; return err; };
            if (user.Password != user.RepeatPassword) { err = "passwords dont match"; return err; };
            if (CurrentUser == null) { err = "not logged in correctly"; return err; }

            CurrentUser.Password = user.Password;
            CurrentUser.Name = user.Name;
            CurrentUser.FirstTimeLogin = user.FirstTimeLogin;
            if (CurrentUser.EmploymentId == null) { err = "no emplayment Id"; return err; };

            UserEntity toSave = new(CurrentUser);
            if (toSave == null) { err = "Application err"; return err; };
            await _DBR.UpdateUserEntry(toSave);

            return err;
        }
        
        //Get all active users
        private async Task<List<UserViewmodel>> GetActiveUsersFromDBR()
        {
            if (ActiveUsers == null)
            {
                List<UserEntity> dblist = await _DBR.GetActiveUsers();
                if (dblist == null) { return null; }
                List<UserViewmodel> ActUsers = new();
                foreach (UserEntity u in dblist)
                {
                    UserViewmodel user = new(u);
                    ActUsers.Add(user);
                }
                ActiveUsers = ActUsers;
            }
            return ActiveUsers;
        }
        // Returns a list of all active users.
        public async Task<List<UserViewmodel>> GetActiveUsers()
        {
            if (ActiveUsers == null)
            {
                List<UserViewmodel> UList = await GetActiveUsersFromDBR();
                ActiveUsers = UList;
                return UList;

            }
            else
            {
                return ActiveUsers;
            }
        }

        //serach active users by id or name
        //returns a user if found
        //if null nothing found
        public async Task<UserViewmodel?> SearchActiveUsers(string search)
        {
            if (search == null || search == string.Empty) { return null; }

            List<UserViewmodel> Ausers = await GetActiveUsers();
            UserViewmodel? target = null;
            foreach (UserViewmodel u in Ausers)
            {
                if (u.Name == search)
                {
                    target = u;
                    break;
                }
                else if (u.EmploymentId == search)
                {
                    target = u;
                    break;
                }
            }
            return target;
        } 
        

        // Updates the list of users.
        public async Task<List<UserViewmodel>> UpdateUsersList()
        {
            ActiveUsers = null;
            List<UserViewmodel> Users = await GetActiveUsers();
            return Users;
        }

        // Updates a users roles
        public async Task<string?> UpdateUserRole(UserViewmodel user, RoleE role, bool upgradeRole)
        {
            if(user.EmploymentId == string.Empty) { return "No id on user"; }
            if(upgradeRole) 
            {
                if (role < user.UserRole|| role==user.UserRole) { return "Alrady at needed role or higher"; }
                user.UserRole = role;
                await _DBR.UpdateUserEntry(new UserEntity(user));
                await UpdateUsersList();
            }
            else
            {
                user.UserRole = role;
                await _DBR.UpdateUserEntry(new UserEntity(user));
                await UpdateUsersList();
            }
            //check if Active list updated correctly
            UserViewmodel? checkUser;
            checkUser = await SearchActiveUsers(user.EmploymentId);
            if(checkUser != null)
            {
                if(checkUser.UserRole != role)
                {
                    checkUser.UserRole = role;
                }
            }
            else
            {
                return "Noe gikk galt";
            }
            return null;
        }
        // Returns the user with the given empid.
        public async Task<UserViewmodel?> GetUserById(string empid)
        {
            if(ActiveUsers != null)
            {
                foreach(UserViewmodel u in ActiveUsers)
                {
                    if(u.EmploymentId == empid)
                    {
                        return u;
                    }
                }
                return null;
            }
            else
            {
                List<UserViewmodel> searchlist = await GetActiveUsersFromDBR();
                foreach(UserViewmodel u in searchlist)
                {
                    if (u.EmploymentId == empid)
                    {
                        return u;
                    }
                }
                return null;
            }
        }
        // Updates a users team.
        public async Task<string?> UpdateUserTeam(string empid, string teamId)
        {
            if (ActiveUsers != null)
            {
                foreach (UserViewmodel u in ActiveUsers)
                {
                    if (u.EmploymentId == empid)
                    {
                        u.TeamId = teamId;
                        await _DBR.UpdateUserEntry(new UserEntity(u));
                        return null;
                    }
                }
                return "User not found";
            }
            else
            {
                List<UserViewmodel> searchlist = await GetActiveUsersFromDBR();
                foreach (UserViewmodel u in searchlist)
                {
                    if (u.EmploymentId == empid)
                    {
                        u.TeamId = teamId;
                        await _DBR.UpdateUserEntry(new UserEntity(u));
                        return null;
                    }
                }
                return "User not found";
            }

        }

        // Generate password for user
        public string GenerateRandomPassword()
        {
            string password = "";
            Random rnd = new();
            for (int i = 0; i < 8; i++)
            {
                int num = rnd.Next(0, 3);
                switch (num)
                {
                    case 0:
                        password += rnd.Next(0, 10);
                        break;
                    case 1:
                        password += (char)rnd.Next(65, 91);
                        break;
                    case 2:
                        password += (char)rnd.Next(97, 123);
                        break;
                }
            }
            return password;
        }

    }

}
