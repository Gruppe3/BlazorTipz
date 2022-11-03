namespace BlazorTipz.ViewModels.Team
{
    public interface ITeamManager
    {
        //These methods are available for anyone implementing this interface
        List<TeamViewmodel>? Teams { get; set; }
        
        Task<(TeamViewmodel?, string?)> createTeam(TeamViewmodel team);
        Task<List<TeamViewmodel>> getInactiveTeams();
        Task<TeamViewmodel> getTeam(string teamId);
        Task<List<TeamViewmodel>> getTeams();
        Task updateTeam(TeamViewmodel team);
        Task<List<TeamViewmodel>> updateTeamsList();

        //Search in active team, returns a team if found, null if nothing found
        Task<TeamViewmodel?> SearchTeams(string search);

        Task<string?> AddTeamMembers(List<TeamMemberViewmodel> teamMemberList);
        Task<(List<TeamMemberViewmodel>, string?)> GetTeamMembersByUser(string empId);
        Task<(List<TeamMemberViewmodel>, string?)> GetTeamMembersByTeam(string teamId);
        Task<(List<TeamMemberViewmodel>, string?)> GetAllTeamMembers();
    }
}