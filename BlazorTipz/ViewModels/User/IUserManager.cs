using BlazorTipz.Data;

namespace BlazorTipz.ViewModels.User
{
    public interface IUserManager
    {
        UserViewmodel? CurrentUser { get; set; }

        //These methods are available for anyone implementing this interface
        UserViewmodel getCurrentUser();

        //first return "string?" = "token", second return "string?" = errorMsg
        Task<(string, string)> Login(UserViewmodel user);

        List<UserViewmodel> getRegisterUserList();

        //first return "string?" = errmsg, second return "string?" = sucsessMsg
        Task<(string?, string?)> registerUserSingel(UserViewmodel user);
        Task<(string?, string?)> registerMultiple(List<UserViewmodel>? usersToReg);

        //add to list usersToregister, returns either a error or succsess msg
        string stageToRegisterList(UserViewmodel user);

        //deletes a specified element form usersToRegister list.
        void deleteFromRegisterList(string emipd);

        Task<(UserViewmodel, string)> getCurrentUser(string token);
        void logout();
        Task<string> updateCurrentUser(UserViewmodel user);
        Task<List<UserViewmodel>> GetUsers();
        Task<string?> updateRole(UserViewmodel user, RoleE role, bool upgradeRole);
        Task<UserViewmodel?> getUser(string empid);
        Task<string?> updateUserTeam(string empid, string teamId);
        Task<List<UserViewmodel>> updateUsersList();
        string generatePassword();
    }
}
