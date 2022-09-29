using BlazorTipz.Data;
using BlazorTipz.ViewModels.User;

namespace BlazorTipz.ViewModels
{
    public interface IUserManager
    {
        Task<(string,string)> Login(UserViewmodel user);
                
    }
}
