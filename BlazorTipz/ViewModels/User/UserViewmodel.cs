using BlazorTipz.Data;
using BlazorTipz.Models;
using BlazorTipz.ViewModels.Team;

namespace BlazorTipz.ViewModels.User
{

    public class UserViewmodel 
    {
        public string Name { get; set; } = string.Empty;
        public string EmploymentId { get; set; } = string.Empty; 
        public RoleE UserRole { get; set; } = RoleE.User;
        public string TeamId { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string RepeatPassword { get; set; } = string.Empty;
        public int ListNum { get; set; } = 0;
        public bool FirstTimeLogin{ get; set; } = false;
        public string TeamName {private get; set; } = string.Empty;
        
        public UserViewmodel()
        {
            
        }
        
        // passerer inn data fra UserDb og setter lokale verdier 
        public UserViewmodel(UserEntity user)
        {
            this.EmploymentId = user.employmentId;
            this.Name = user.userName;
            this.Password = user.password;
            this.UserRole = user.userRole;
            this.FirstTimeLogin = user.firstTimeLogin;
            if (user.teamId != null)
                this.TeamId = user.teamId;
        }

        public async Task<string?> GetTeamName(ITeamManager _TM)
        {
            if (TeamId == string.Empty || _TM == null) { return null; }
            if (TeamName == string.Empty) 
            { 
                TeamViewmodel team = await _TM.GetTeamById(TeamId);
                if (team != null) { TeamName = team.TeamName; } 
            }
            return TeamName;
        }
    }

}