using BlazorTipz.ViewModels.Team;

namespace BlazorTipz.Models
{
    public class TeamEntity 
    {
        public string teamName { get; set; } = string.Empty;
        public string teamLeader { get; set; } = string.Empty;
        public string teamId { get; set; } = string.Empty;
        public bool active { get; set; } = true;
        public DateTime? createdAt { get; set; }
        public string? department { get; set; } = string.Empty;

        public TeamEntity()
        {
        }
        public TeamEntity(TeamViewmodel team)
        {
            this.teamId = team.TeamId;
            this.teamName = team.TeamName;
            this.teamLeader = team.TeamLeaderId;
        }
    }
}
