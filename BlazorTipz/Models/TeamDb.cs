using BlazorTipz.ViewModels.Team;

namespace BlazorTipz.Models
{
    public class TeamDb 
    {
        public string teamName { get; set; } = string.Empty;
        public string teamLeader { get; set; } = string.Empty;
        public string teamId { get; set; } = string.Empty;
        public bool active { get; set; } = true;

        public TeamDb()
        {
        }
        public TeamDb(TeamViewmodel team)
        {
            this.teamId = team.id;
            this.teamName = team.name;
            this.teamLeader = team.leader;
        }
    }
}
