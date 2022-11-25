using BlazorTipz.Data;

namespace BlazorTipz.ViewModels.User
{
    public interface IUserManager
    {
        UserViewmodel? CurrentUser { get; set; }

        //These methods are available for anyone implementing this interface
        UserViewmodel GetCurrentUser();

        //first return "string?" = "token", second return "string?" = errorMsg
        Task<(string?, string?)> Login(UserViewmodel user);

        List<UserViewmodel> GetRegisterUserList();

        //first return "string?" = errmsg, second return "string?" = sucsessMsg
        Task<(string?, string?)> RegisterUserSingel(UserViewmodel user);
        Task<(string?, string?)> RegisterMultiple(List<UserViewmodel>? usersToReg);

        //add to list usersToregister, returns either a error or succsess msg
        string StageToRegisterList(UserViewmodel user);

        //deletes a specified element form usersToRegister list.
        void DeleteFromRegisterList(string emipd);

        Task<(UserViewmodel, string)> GetCurrentUser(string token);
        void Logout();
        Task<string> UpdateCurrentUser(UserViewmodel user);
        Task<List<UserViewmodel>> GetActiveUsers();

        //serach active users by id or name
        //returns a user if found
        //null if nothing found
        Task<UserViewmodel?> SearchActiveUsers(string search);
        Task<string?> UpdateUserRole(UserViewmodel user, RoleE role, bool upgradeRole);
        Task<UserViewmodel?> GetUserById(string empid);
        Task<string?> UpdateUserTeam(string empid, string teamId);
        Task<List<UserViewmodel>> UpdateUsersList();
        string GenerateRandomPassword();
        
    }
}
