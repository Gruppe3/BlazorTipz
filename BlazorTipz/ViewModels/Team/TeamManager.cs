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
        public List<TeamViewmodel>? Teams { get; set; }
        
        public TeamManager(IDbRelay DBR, IUserManager UM)
        {
            _DBR = DBR;
            _UM = UM;
        }
        // Returns a list of all active teams.
        public async Task<List<TeamViewmodel>> getTeams()
        {
            if (Teams == null)
            {
                List<TeamEntity> dbTeams = await _DBR.getActiveTeams();
                List<TeamViewmodel> teams = new List<TeamViewmodel>();
                foreach (TeamEntity dbTeam in dbTeams)
                {
                    TeamViewmodel team = new TeamViewmodel(dbTeam);
                    teams.Add(team);
                }
                Teams = teams;
            }
            return Teams;
        }
        // Updates the list of available teams.
        public async Task<List<TeamViewmodel>> updateTeamsList()
        {
            Teams = null;
            List<TeamViewmodel> teams = await getTeams();
            return teams;
        }
        // Returns the team with the given ID.
        public async Task<TeamViewmodel> getTeam(string teamId)
        {

            TeamViewmodel team = new TeamViewmodel();
            if (Teams == null)
            {
                await getTeams();
            }
            foreach (TeamViewmodel t in Teams)
            {
                if (t.id == teamId)
                {
                    team = t;
                    break;
                }
            }
            return team;
        }
        // Returns a list of all inactive teams.
        public async Task<List<TeamViewmodel>> getInactiveTeams()
        {
            List<TeamViewmodel> teams = new List<TeamViewmodel>();
            List<TeamEntity> dbTeams = await _DBR.getInactiveTeams();
            foreach (TeamEntity dbTeam in dbTeams)
            {
                TeamViewmodel team = new TeamViewmodel(dbTeam);
                teams.Add(team);
            }
            return teams;
        }
        // Update team
        public async Task updateTeam(TeamViewmodel team)
        {
            TeamEntity dbTeam = new TeamEntity(team);
            if (updateTeams(team))
            {
                await _DBR.updateTeamEntry(dbTeam);
            }
            else
            {
                await _DBR.updateTeamEntry(dbTeam);
                await getTeams();
            }
        }
        // Gets the team id for a given team name and leader.
        private string getTeamId(string teamName, string teamLeader)
        {
            string teamId = string.Empty;
            foreach (TeamViewmodel team in Teams)
            {
                if (team.name == teamName)
                {
                    if (team.leader == teamLeader)
                    {
                        teamId = team.id;
                        break;
                    }
                }
            }
            return teamId;
        }
        // Updates the teams of the given viewmodel.
        private bool updateTeams(TeamViewmodel team)
        {
            if (Teams == null) { return false; }
            foreach (TeamViewmodel t in Teams)
            {
                if (t.id == team.id)
                {
                    t.name = team.name;
                    t.leader = team.leader;
                    return true;
                }
            }
            return false;
        }
        //add team
        public async Task<(TeamViewmodel?, string?)> createTeam(TeamViewmodel team)
        {

            if (team == null) { return (null, "Team is null"); }
            if (team.name == string.Empty) { return (null, "Team name is empty"); }
            if (team.leader == string.Empty) { return (null, "No team leader chosen"); }

            UserViewmodel teamLeader = await _UM.getUser(team.leader);
            if (teamLeader == null) { return (null, "Team leader not found"); }
            if (teamLeader.role != RoleE.TeamLeader)
            {
                await _UM.updateRole(teamLeader, RoleE.TeamLeader, true);
            }

            TeamEntity dbTeam = new TeamEntity(team);
            await _DBR.addTeamEntry(dbTeam);
            await updateTeamsList();
            string teamid = getTeamId(team.name, team.leader);
            if (teamid == string.Empty) { return (null, "Team creation error"); }
            TeamViewmodel rTeam = await getTeam(teamid);

            await _UM.updateUserTeam(teamLeader.employmentId, teamid);

            return (rTeam, null);
        }
        public async Task<TeamViewmodel?> SearchTeams(string search) 
        { 
            if (Teams == null)
            {
                await getTeams();
            }
            foreach (TeamViewmodel team in Teams)
            {
                if (team.name.ToLower().Contains(search.ToLower()))
                {
                    return team;
                }
                else if (team.leader.ToLower().Contains(search.ToLower()))
                {
                    return team;
                }
                else if (team.id.ToLower().Contains(search.ToLower()))
                {
                    return team;
                }
            }
            return null;
        }
    }
}
