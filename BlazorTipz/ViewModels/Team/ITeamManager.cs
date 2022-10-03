namespace BlazorTipz.ViewModels.Team
{
    public interface ITeamManager
    {
        List<TeamViewmodel>? Teams { get; set; }

        Task<(TeamViewmodel?, string?)> createTeam(TeamViewmodel team);
        Task<List<TeamViewmodel>> getInactiveTeams();
        Task<TeamViewmodel> getTeam(string teamId);
        Task<List<TeamViewmodel>> getTeams();
        Task updateTeam(TeamViewmodel team);
        Task<List<TeamViewmodel>> updateTeamsList();
    }
}