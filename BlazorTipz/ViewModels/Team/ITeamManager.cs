namespace BlazorTipz.ViewModels.Team
{
    public interface ITeamManager
    {
        //These methods are available for anyone implementing this interface
        List<TeamViewmodel>? ActiveTeams { get; set; }
        
        Task<(TeamViewmodel?, string?)> CreateNewTeam(TeamViewmodel team);
        Task<List<TeamViewmodel>> GetInactiveTeams();
        Task<TeamViewmodel> GetTeamById(string teamId);
        Task<List<TeamViewmodel>> GetActiveTeams();
        Task UpdateSingleTeam(TeamViewmodel team);
        Task<List<TeamViewmodel>> UpdateTeamsList();

        //Search in active team, returns a team if found, null if nothing found
        Task<TeamViewmodel?> SearchTeams(string search);
    }
}