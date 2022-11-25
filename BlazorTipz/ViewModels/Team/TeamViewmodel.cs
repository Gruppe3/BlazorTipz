using BlazorTipz.Models;

namespace BlazorTipz.ViewModels.Team
{
    public class TeamViewmodel 
    {
        public string TeamName { get; set; } = string.Empty;
        public string TeamId { get; set; } = string.Empty;
        public string TeamLeaderId { get; set; } = string.Empty;
        public TeamViewmodel()
        {
        }
        public TeamViewmodel(TeamEntity team)
        {
            this.TeamId = team.teamId;
            this.TeamName = team.teamName;
            this.TeamLeaderId = team.teamLeader;
        }
    }
}

