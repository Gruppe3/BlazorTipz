using BlazorTipz.Models;
using BlazorTipz.Data;

namespace BlazorTipz.ViewModels.User
{

    public class UserViewmodel 
    {
        public string name { get; set; } = string.Empty;
        public string employmentId { get; set; }
        public RoleE role { get; set; } = RoleE.User;
        public string teamId { get; set; }
        public string password { get; set; }
        public string RepeatPassword { get; set; } = string.Empty;
        
        
        public UserViewmodel()
        {

        }
        public UserViewmodel(UserDb user)
        {
            this.employmentId = user.employmentId;
            this.name = user.name;
            this.password = user.password;
            this.role = user.role;
            this.teamId = user.teamId;
        }
    }

}
