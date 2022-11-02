using BlazorTipz.Models;

namespace BlazorTipz.ViewModels.Team
{
    public class TeamViewmodel 
    {
        public string name { get; set; } = string.Empty;
        public string id { get; set; } = string.Empty;
        public string leader { get; set; } = string.Empty;
        public TeamViewmodel()
        {
        }
        public TeamViewmodel(TeamEntity team)
        {
            this.id = team.teamId;
            this.name = team.teamName;
            this.leader = team.teamLeader;
        }
    }
}

