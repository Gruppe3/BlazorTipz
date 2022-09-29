using BlazorTipz.Data;
using BlazorTipz.ViewModels.User;

namespace BlazorTipz.ViewModels
{
    public interface IUserManager
    {
        Task Login(UserViewmodel user, out string token, out string err);
        
    }
}
