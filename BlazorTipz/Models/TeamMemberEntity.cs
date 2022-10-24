using BlazorTipz.ViewModels.Team;

namespace BlazorTipz.Models
{
    public class TeamMemberEntity
    {
        public string EmpName { get; set; } = string.Empty;
        public string TeamName { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string TeamId { get; set; } = string.Empty;
        public string Role { get; set; } = "Medlem";
        public string JoinedAt { get; set; } = string.Empty;
        public bool Active { get; set; } = true;

        
        public TeamMemberEntity() {

        } 
        
        public TeamMemberEntity(TeamMemberViewmodel viewM)
        {
            this.UserId = viewM.AnsattId;
            this.TeamId = viewM.TeamId;
            this.Role = viewM.TeamRolle;
            this.Active = viewM.AktivStatus;
        }
    
    }
}
