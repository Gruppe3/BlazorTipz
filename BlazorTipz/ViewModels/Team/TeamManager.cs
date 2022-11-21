using BlazorTipz.Data;
using BlazorTipz.Models;
using BlazorTipz.Models.DbRelay;
using BlazorTipz.ViewModels.User;

namespace BlazorTipz.ViewModels.Team
{
    public class TeamManager : ITeamManager
    {
        private readonly IDbRelay _DBR;
        private readonly IUserManager _UM;
        public List<TeamViewmodel>? ActiveTeams { get; set; }
        
        public TeamManager(IDbRelay DBR, IUserManager UM)
        {
            _DBR = DBR;
            _UM = UM;
        }
        // Returns a list of all active teams.
        public async Task<List<TeamViewmodel>> GetActiveTeams()
        {
            if (ActiveTeams == null)
            {
                List<TeamEntity> dbTeams = await _DBR.GetActiveTeams();
                List<TeamViewmodel> teams = new();
                foreach (TeamEntity dbTeam in dbTeams)
                {
                    TeamViewmodel team = new(dbTeam);
                    teams.Add(team);
                }
                ActiveTeams = teams;
            }
            return ActiveTeams;
        }
        // Updates the list of available teams.
        public async Task<List<TeamViewmodel>> UpdateTeamsList()
        {
            ActiveTeams = null;
            List<TeamViewmodel> teams = await GetActiveTeams();
            return teams;
        }
        // Returns the team with the given ID.
        public async Task<TeamViewmodel> GetTeamById(string teamId)
        {

            TeamViewmodel team = new();
            if (ActiveTeams == null)
            {
                await GetActiveTeams();
            }
            if (ActiveTeams != null)
            {
                foreach (TeamViewmodel t in ActiveTeams)
                {
                    if (t.TeamId == teamId)
                    {
                        team = t;
                        break;
                    }
                }
            }
            
            return team;
        }
        // Returns a list of all inactive teams.
        public async Task<List<TeamViewmodel>> GetInactiveTeams()
        {
            List<TeamViewmodel> teams = new();
            List<TeamEntity> dbTeams = await _DBR.GetInactiveTeams();
            foreach (TeamEntity dbTeam in dbTeams)
            {
                TeamViewmodel team = new(dbTeam);
                teams.Add(team);
            }
            return teams;
        }
        // Update team
        public async Task UpdateSingleTeam(TeamViewmodel team)
        {
            TeamEntity dbTeam = new(team);
            if (UpdateTeams(team))
            {
                await _DBR.UpdateTeamEntry(dbTeam);
            }
            else
            {
                await _DBR.UpdateTeamEntry(dbTeam);
                await GetActiveTeams();
            }
        }
        // Gets the team id for a given team name and leader.
        private string GetTeamId(string teamName, string teamLeader)
        {
            string teamId = string.Empty;
            
            if (ActiveTeams != null)
            {
                foreach (TeamViewmodel team in ActiveTeams)
                {
                    if (team.TeamName == teamName && 
                        team.TeamLeaderId == teamLeader)
                    {
                        teamId = team.TeamId;
                        break;
                    }
                }
            }
            return teamId;
        }
        // Updates the teams of the given viewmodel.
        private bool UpdateTeams(TeamViewmodel team)
        {
            if (ActiveTeams == null) { return false; }
            foreach (TeamViewmodel t in ActiveTeams)
            {
                if (t.TeamId == team.TeamId)
                {
                    t.TeamName = team.TeamName;
                    t.TeamLeaderId = team.TeamLeaderId;
                    return true;
                }
            }
            return false;
        }
        //add team
        public async Task<(TeamViewmodel?, string?)> CreateNewTeam(TeamViewmodel team)
        {

            if (team == null) { return (null, "Team is null"); }
            if (team.TeamName == string.Empty) { return (null, "Team name is empty"); }
            if (team.TeamLeaderId == string.Empty) { return (null, "No team leader chosen"); }

            UserViewmodel? teamLeader = await _UM.GetUserById(team.TeamLeaderId);
            if (teamLeader == null) { return (null, "Team leader not found"); }
            if (teamLeader.UserRole != RoleE.TeamLeader)
            {
                await _UM.UpdateUserRole(teamLeader, RoleE.TeamLeader, true);
            }

            TeamEntity dbTeam = new(team);
            await _DBR.AddTeamEntry(dbTeam);
            await UpdateTeamsList();
            string teamid = GetTeamId(team.TeamName, team.TeamLeaderId);
            if (teamid == string.Empty) { return (null, "Team creation error"); }
            TeamViewmodel rTeam = await GetTeamById(teamid);

            await _UM.UpdateUserTeam(teamLeader.EmploymentId, teamid);

            return (rTeam, null);
        }
        public async Task<TeamViewmodel?> SearchTeams(string search) 
        { 
            if (ActiveTeams == null)
            {
                await GetActiveTeams();
            }
            if (ActiveTeams != null)
            {
                foreach (TeamViewmodel team in ActiveTeams)
                {
                    if (team.TeamName.ToLower().Contains(search.ToLower()))
                    {
                        return team;
                    }
                    else if (team.TeamLeaderId.ToLower().Contains(search.ToLower()))
                    {
                        return team;
                    }
                    else if (team.TeamId.ToLower().Contains(search.ToLower()))
                    {
                        return team;
                    }
                }
            }
            return null;
        }
    }
}
