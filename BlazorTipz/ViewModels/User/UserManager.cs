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

        private List<UserViewmodel>? UsersToRegister {  get; set; } = new List<UserViewmodel>();
        //constructor
        public UserManager(IDbRelay DBR, AuthenticationComponent auth)
        {
            _DBR = DBR;
            _Auth = auth;
        }
        public UserManager()
        {
            //for testing
        }

        //Login function
        public async Task<(string, string)> Login(UserViewmodel user)
        {
            //User entity
            UserEntity tryUser = new UserEntity(user);
            //Sends emplyment id to userdb through interface relay
            UserEntity dbUser = await _DBR.getLoginUser(tryUser.employmentId);
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
                SetCurrentUser(new UserViewmodel(dbUser));

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
            if (toRegisterUser.employmentId == null|| toRegisterUser.employmentId == "") { err = "no emplayment Id"; return (err, null); };
            if (toRegisterUser.name == null|| toRegisterUser.name =="") { err = "no name"; return (err, null); };
            if (toRegisterUser.password == null|| toRegisterUser.password == "") { err = "no password given"; return (err, null); };

            UserEntity userDb = await _DBR.lookUpUser(toRegisterUser.employmentId);
            if (userDb != null) { err = "User alrady exists"; return (err, null); }

            UserEntity toSaveUser = new UserEntity(toRegisterUser);
            List<UserEntity> toSave = new List<UserEntity>();
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
            else if (UsersToRegister != null && UsersToRegister.Count() >0)
            {
                foreach (UserViewmodel user in UsersToRegister)
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
        public List<UserViewmodel> getRegisterUserList()
        {
            if (UsersToRegister == null)
            {
                List<UserViewmodel> list = new List<UserViewmodel>();
                UsersToRegister = list;
            }
            return UsersToRegister;
        }
        public string stageToRegisterList(UserViewmodel user)
        {
            if (user == null) { return "no user to stage"; }
            if (user.employmentId==null) { return "Not supplied EmploymentID"; }
            if (user.name == string.Empty) { return "Not supplied a name"; }
            if (user.password == string.Empty || user.password == "" ) { return "Not supplied a password"; }
            //check if user is in list, update instead of add
            bool hit = false;
            foreach(UserViewmodel u in UsersToRegister)
            {
                if (u.employmentId == user.employmentId)
                {
                    u.name = user.name;
                    u.password = user.password;
                    u.role = user.role;
                    hit = true; break;
                }
            }
            if (!hit)
            {
                //adds new user to list
                user.listnum = UsersToRegister.Count + 1;
                UsersToRegister.Add(user);
                return "User succsessfully added to list of pepole to register";
            } else { return "User in list updated"; }
        }
        private void updateListnum()
        {
            foreach (UserViewmodel u in UsersToRegister)
            {
                u.listnum = UsersToRegister.IndexOf(u) + 1;
            }
        }
        //deletes a specified element form usersToRegister list.
        public void deleteFromRegisterList(string emipd)
        {
            int i = 0;
            //search for user to delete
            foreach(UserViewmodel user in UsersToRegister)
            {
                if(user.employmentId == emipd)
                {
                    UsersToRegister.RemoveAt(i);
                    updateListnum();
                    break;
                }
                i++;
            }
            
        }

        //Get the current user with the given token.
        public async Task<(UserViewmodel, string)> getCurrentUser(string token)
        {
            string err = null;
            string empId = _Auth.GetClaimValue(token);
            UserEntity user = await _DBR.getLoginUser(empId);
            if (user == null) { err = "User not found"; return (null, err); };
            SetCurrentUser(new UserViewmodel(user));
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
        public void SetCurrentUser(UserViewmodel user)
        {
            CurrentUser = user;
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
            CurrentUser.firstTimeLogin = user.firstTimeLogin;
            if (CurrentUser.employmentId == null) { err = "no emplayment Id"; return err; };

            UserEntity toSave = new UserEntity(CurrentUser);
            if (toSave == null) { err = "Application err"; return err; };
            await _DBR.updateUserEntry(toSave);

            return err;
        }
        
        //Get all active users
        private async Task<List<UserViewmodel>> getUsers()
        {
            if (ActiveUsers == null)
            {
                List<UserEntity> dblist = await _DBR.getActiveUsers();
                if (dblist == null) { return null; }
                List<UserViewmodel> ActUsers = new List<UserViewmodel>();
                foreach (UserEntity u in dblist)
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

        //serach active users by id or name
        //returns a user if found
        //if null nothing found
        public async Task<UserViewmodel?> SearchActiveUsers(string search)
        {
            if (search == null || search == string.Empty) { return null; }

            List<UserViewmodel> Ausers = await GetUsers();
            UserViewmodel? target = null;
            foreach (UserViewmodel u in Ausers)
            {
                if (u.name == search)
                {
                    target = u;
                    break;
                }
                else if (u.employmentId == search)
                {
                    target = u;
                    break;
                }
            }
            return target;
        } 
        

        // Updates the list of users.
        public async Task<List<UserViewmodel>> updateUsersList()
        {
            ActiveUsers = null;
            List<UserViewmodel> Users = await GetUsers();
            return Users;
        }

        // Updates a users roles
        public async Task<string?> updateRole(UserViewmodel user, RoleE role, bool upgradeRole)
        {
            if(user.employmentId == string.Empty) { return "No id on user"; }
            if(upgradeRole) 
            {
                if (role < user.role|| role==user.role) { return "Alrady at needed role or higher"; }
                user.role = role;
                await _DBR.updateUserEntry(new UserEntity(user));
                await updateUsersList();
            }
            else
            {
                user.role = role;
                await _DBR.updateUserEntry(new UserEntity(user));
                await updateUsersList();
            }
            //check if Active list updated correctly
            UserViewmodel? checkUser;
            checkUser = await SearchActiveUsers(user.employmentId);
            if(checkUser != null)
            {
                if(checkUser.role != role)
                {
                    checkUser.role = role;
                }
            }
            else
            {
                return "Noe gikk galt";
            }
            return null;
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
        public async Task<string?> updateUserTeam(string empid, string teamId)
        {
            if (ActiveUsers != null)
            {
                foreach (UserViewmodel u in ActiveUsers)
                {
                    if (u.employmentId == empid)
                    {
                        u.teamId = teamId;
                        await _DBR.updateUserEntry(new UserEntity(u));
                        return null;
                        break;
                    }
                }
                return "User not found";
            }
            else
            {
                List<UserViewmodel> searchlist = await getUsers();
                foreach (UserViewmodel u in searchlist)
                {
                    if (u.employmentId == empid)
                    {
                        u.teamId = teamId;
                        await _DBR.updateUserEntry(new UserEntity(u));
                        return null;
                        break;
                    }
                }
                return "User not found";
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
