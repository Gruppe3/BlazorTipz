using BlazorTipz.Data;

namespace BlazorTipz.Models.User
{

    public class UserViewmodel : UserA
    {
        public string password { get; set; } = string.Empty;
        public string RepeatPassword { get; set; } = string.Empty;
    }
}
