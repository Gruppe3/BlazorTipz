using BlazorTipz.Data;
using BlazorTipz.Models;
using BlazorTipz.ViewModels.User;

namespace BlazorTipz.ViewModels.Team
{
    public class TeamMemberViewmodel
    {
        public string AnsattNavn { get; set; } = string.Empty;
        public string TeamNavn { get; set; } = string.Empty;
        public string AnsattId { get; set; } = string.Empty;
        public string TeamId { get; set; } = string.Empty;
        public string TeamRolle { get; set; } = "Medlem";
        public bool AktivStatus { get; set; } = true;


        public TeamMemberViewmodel()
        {

        }

        public TeamMemberViewmodel(TeamMemberEntity entity)
        {
            AnsattNavn = entity.EmpName;
            TeamNavn = entity.TeamName;
            AnsattId = entity.UserId;
            TeamId = entity.TeamId;
            TeamRolle = entity.Role;
            AktivStatus = entity.Active;
        }

        public TeamMemberViewmodel(UserViewmodel viewM)
        {
            AnsattId = viewM.employmentId;
            TeamId = viewM.teamId;
            if (viewM.role == RoleE.TeamLeader)
            {
                TeamRolle = "TeamLeder";
            }
        }
    }
}
