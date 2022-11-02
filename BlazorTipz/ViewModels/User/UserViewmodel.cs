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
        public int listnum { get; set; } = 0;
        public bool firstTimeLogin{ get; set; }

        public string TeamName { get; set; } = string.Empty;
        public UserViewmodel()
        {

        }
        
        // passerer inn data fra UserDb og setter lokale verdier 
        public UserViewmodel(UserEntity user)
        {
            this.employmentId = user.employmentId;
            this.name = user.userName;
            this.password = user.password;
            this.role = user.userRole;
            this.teamId = user.teamId;
            this.firstTimeLogin = user.firstTimeLogin;
        }
    }

}
