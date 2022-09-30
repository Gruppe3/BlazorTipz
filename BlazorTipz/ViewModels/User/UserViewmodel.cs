using BlazorTipz.Data;

namespace BlazorTipz.ViewModels.User
{

    public class UserViewmodel : UserA
    {
        public string RepeatPassword { get; set; } = string.Empty;
        public UserViewmodel()
        {

        }
        public UserViewmodel(UserA user)
        {
            this.employmentId = user.employmentId;
            this.name = user.name;
            this.password = user.password;
            this.role = user.role;
            this.teamId = user.teamId;
        }
    }

}
