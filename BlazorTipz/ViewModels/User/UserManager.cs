﻿using BlazorTipz.Components;
using BlazorTipz.Data;
using BlazorTipz.Models;
using BlazorTipz.Models.DbRelay;


namespace BlazorTipz.ViewModels.User
{
    public class UserManager : IUserManager
    {
        //
        private readonly IDbRelay _DBR;
        private readonly AuthenticationComponent _Auth;

        //Current user logged in
        public UserViewmodel? CurrentUser { get; set; }

        //A list of all active users
        public List<UserViewmodel>? ActiveUsers { get; set; }

        public List<UserViewmodel>? usersToRegister { get; private set; } = new List<UserViewmodel>();
        //constructor
        public UserManager(IDbRelay DBR, AuthenticationComponent auth)
        {
            _DBR = DBR;
            _Auth = auth;
        }

        //Login function
        public async Task<(string, string)> Login(UserViewmodel user)
        {
            //User entity
            UserDb tryUser = new UserDb(user);
            //Sends emplyment id to userdb through interface relay
            UserDb dbUser = await _DBR.getLoginUser(tryUser.employmentId);
            string token;
            string err;

            //If doesn´t exist
            if (dbUser == null)
            {
                token = null;
                err = "User not found";
                return (token, err);
            }
            //Verifies password typed in
            if (_Auth.VerifyPasswordHash(user.password, dbUser.passwordHash, dbUser.passwordSalt))
            {
                dbUser.CreateToken();

                //setter token
                token = dbUser.AuthToken;
                UserViewmodel userView = new UserViewmodel(dbUser);
                CurrentUser = userView;

                err = null;
                return (token, err);
            }

            //If it does not match it´s wrong
            else
            {
                token = null;
                err = "Wrong password";
                return (token, err);
            }
        }

        //register singel user function
        //first return "string?" = errmsg, second return "string?" = sucsessMsg
        public async Task<(string?,string?)> registerUserSingel(UserViewmodel toRegisterUser)
        {
            string err = null;
            if (toRegisterUser == null) { err = "No user to register"; return (err, null); };
            if (toRegisterUser.employmentId == null) { err = "no emplayment Id"; return (err, null); };
            if (toRegisterUser.name == null) { err = "no name"; return (err, null); };
            if (toRegisterUser.password == null) { err = "no password given"; return (err, null); };

            UserDb userDb = await _DBR.lookUpUser(toRegisterUser.employmentId);
            if (userDb != null) { err = "User alrady exists"; return (err, null); }

            UserDb toSaveUser = new UserDb(toRegisterUser);
            List<UserDb> toSave = new List<UserDb>();
            toSave.Add(toSaveUser);
            if (toSave.Count == 0) { err = "somthing went wrong"; return (err, null); };

            await _DBR.addUserEntries(toSave);
            await getUsers();
            string suc = "succsess";
            return (err, suc);
        }

        //take in a list of users, and registers them
        //first return "string?" = errmsg, second return "string?" = sucsessMsg
        public async Task<(string?,string?)> registerMultiple(List<UserViewmodel>? usersToReg)
        {
            string err = null;
            string retErr= null;
            string filler;
            int itNum = 1;
            if(usersToReg != null) 
            {
                foreach(UserViewmodel user in usersToReg)
                {
                    (retErr, filler) = await registerUserSingel(user);
                    if(retErr != null) { return ("Nr: " + itNum +", Failed with: " + retErr,null); }
                    itNum++;
                }
                return (err, "Succsess");
            } 
            else if (usersToRegister != null)
            {
                foreach (UserViewmodel user in usersToRegister)
                {
                    (retErr, filler) = await registerUserSingel(user);
                    if (retErr != null) { return ("Nr: " + itNum + ", Failed with: " + retErr, null); }
                    itNum++;
                }
                return (err, "Succsess");
            }
            else
            {
                return ("No one to register",null);
            }
        }
        public string stageToRegisterList(UserViewmodel user)
        {
            if (user == null) { return "no user to stage"; }
            if (user.employmentId==null) { return "Not supplied EmploymentID"; }
            if (user.name == string.Empty) { return "Not supplied a name"; }
            //check if user is in list, update instead of add
            bool hit = false;
            foreach(UserViewmodel u in usersToRegister)
            {
                //if(u.employmentId == user.employmentId)
                //{
                //    u.name = user.name;
                //    u.password = user.password;
                //    hit = true; break;
                //}
            }
            if (!hit)
            {
                //adds new user to list
                user.listnum = usersToRegister.Count + 1;
                usersToRegister.Add(user);
                return "User succsessfully added to list of pepole to register";
            } else { return "User in list updated"; }
        }
        //deletes a specified element form usersToRegister list.
        public void deleteFromRegisterList(string emipd)
        {
            //search for user to delete
            foreach(UserViewmodel user in usersToRegister)
            {
                if(user.employmentId == emipd)
                {
                    usersToRegister.RemoveAt(user.listnum - 1);
                }
            }
        }

        //Get the current user with the given token.
        public async Task<(UserViewmodel, string)> getCurrentUser(string token)
        {
            string err = null;
            string empId = _Auth.GetClaimValue(token);
            UserDb user = await _DBR.getLoginUser(empId);
            if (user == null) { err = "User not found"; return (null, err); };
            CurrentUser = new UserViewmodel(user);
            await getUsers();
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

        //Updates current user
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
        
        //Get all active users
        private async Task<List<UserViewmodel>> getUsers()
        {
            if (ActiveUsers == null)
            {
                List<UserDb> dblist = await _DBR.getActiveUsers();
                if (dblist == null) { return null; }
                List<UserViewmodel> ActUsers = new List<UserViewmodel>();
                foreach (UserDb u in dblist)
                {
                    UserViewmodel user = new UserViewmodel(u);
                    ActUsers.Add(user);
                }
                ActiveUsers = ActUsers;
            }
            return ActiveUsers;
        }
        // Returns a list of all active users.
        public async Task<List<UserViewmodel>> GetUsers()
        {
            if (ActiveUsers == null)
            {
                List<UserViewmodel> UList = await getUsers();
                ActiveUsers = UList;
                return UList;

            }
            else
            {
                return ActiveUsers;
            }
        }
        // Updates the list of users.
        public async Task<List<UserViewmodel>> updateUsersList()
        {
            ActiveUsers = null;
            List<UserViewmodel> Users = await GetUsers();
            return Users;
        }

        // Updates a users roles
        public async Task updateRole(UserViewmodel user, RoleE role, bool upgradeRole)
        {
            if(user.employmentId == string.Empty) { return; }
            if(upgradeRole) 
            {
                if (role <= user.role) { return; }
                user.role = role;
                await _DBR.updateUserEntry(new UserDb(user));
                updateUsersList();
            }
            else
            {
                user.role = role;
                await _DBR.updateUserEntry(new UserDb(user));
                updateUsersList();
            }
            
        }
        // Returns the user with the given empid.
        public async Task<UserViewmodel?> getUser(string empid)
        {
            if(ActiveUsers != null)
            {
                foreach(UserViewmodel u in ActiveUsers)
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
        // Updates a users team.
        public async Task updateUserTeam(string empid, string teamId)
        {
            if (ActiveUsers != null)
            {
                foreach (UserViewmodel u in ActiveUsers)
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

        // Generate password for user
        public string generatePassword()
        {
            string password = "";
            Random rnd = new Random();
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
