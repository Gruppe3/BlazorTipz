using BlazorTipz.Data;
using BlazorTipz.ViewModels.User;

namespace BlazorTipz.ViewModels
{
    public interface IUserManager
    {
        public UserViewmodel getCurrentUser();
        Task<(string,string)> Login(UserViewmodel user);
        Task<string> registerUserSingel(UserViewmodel user);
        Task<(UserViewmodel,string)> getCurrentUser(string token);
        public void logout();
        Task<string> updateCurrentUser(UserViewmodel user);
    }
}
