using BlazorTipz.Data;

namespace BlazorTipz.ViewModels.User
{
    public interface IUserManager
    {
        //These methods are available for anyone implementing this interface
        UserViewmodel getCurrentUser();
        Task<(string, string)> Login(UserViewmodel user);
        Task<string> registerUserSingel(UserViewmodel user);
        Task<(UserViewmodel, string)> getCurrentUser(string token);
        void logout();
        Task<string> updateCurrentUser(UserViewmodel user);
        Task<List<UserViewmodel>> GetUsers();
        Task updateRole(UserViewmodel user, RoleE role, bool upgradeRole);
        Task<UserViewmodel?> getUser(string empid);
        Task updateUserTeam(string empid, string teamId);
        Task<List<UserViewmodel>> updateUsersList();
        String generatePassword();
    }
}
